using ITRepairService.Models;
using ITRepairService.Services;
using ITRepairService.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ITRepairService.Controllers;

public class AccountController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ILdapAuthenticationService ldapAuthService) : Controller
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly ILdapAuthenticationService _ldapAuthService = ldapAuthService;

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // 1. Try AD/LDAP authentication first
        var ldapResult = _ldapAuthService.Authenticate(model.UserName, model.Password);
        if (ldapResult is not null && ldapResult.IsSuccess)
        {
            // Check if user exists locally; if not, auto-create
            var user = await _userManager.FindByNameAsync(ldapResult.Username);
            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = ldapResult.Username,
                    Email = ldapResult.Email,
                    FullName = ldapResult.DisplayName,
                    EmailConfirmed = true,
                    CreatedByName = ldapResult.DisplayName,
                    UpdatedByName = ldapResult.DisplayName,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Create with a random password (user will authenticate via AD)
                var createResult = await _userManager.CreateAsync(user, Guid.NewGuid().ToString("N") + "Aa1!");
                if (createResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, AppRoles.User);
                }
            }

            if (user is not null)
            {
                // Bypass password check and sign in using the external/AD identity
                await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);

                if (user.MustChangePassword)
                {
                    return RedirectToAction("ChangePassword", "Account");
                }

                return RedirectToLocal(model.ReturnUrl);
            }
        }

        // 2. Fallback: try local Identity authentication (for locally registered users)
        var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user is not null && user.MustChangePassword)
            {
                return RedirectToAction("ChangePassword", "Account");
            }
            return RedirectToLocal(model.ReturnUrl);
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Account locked. Please try again later.");
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register(string? returnUrl = null)
    {
        return View(new RegisterViewModel { ReturnUrl = returnUrl });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            EmailConfirmed = true,
            CreatedByName = model.FullName,
            UpdatedByName = model.FullName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user, model.Password);
        if (createResult.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, AppRoles.User);
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToLocal(model.ReturnUrl);
        }

        foreach (var error in createResult.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }

    [Authorize]
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordViewModel());
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return RedirectToAction("Login");
        }

        var changeResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!changeResult.Succeeded)
        {
            foreach (var error in changeResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        user.MustChangePassword = false;
        await _userManager.UpdateAsync(user);

        await _signInManager.RefreshSignInAsync(user);

        TempData["SuccessMessage"] = "เปลี่ยนรหัสผ่านเรียบร้อยแล้ว";
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }
}
