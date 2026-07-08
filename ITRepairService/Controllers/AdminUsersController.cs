using ITRepairService.Models;
using ITRepairService.ViewModels.AdminUsers;
using ITRepairService.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITRepairService.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class AdminUsersController(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    AppDbContext dbContext,
    IWebHostEnvironment webHostEnvironment) : Controller
{
    private const string DepartmentSectionStoreRelativePath = "App_Data/department-section-master.json";

    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly AppDbContext _dbContext = dbContext;
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users
            .OrderBy(user => user.Email)
            .ToListAsync();

        var viewModel = new List<AdminUserListItemViewModel>(users.Count);

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            viewModel.Add(new AdminUserListItemViewModel
            {
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                Roles = roles
            });
        }

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);

        var viewModel = new UserDetailsViewModel
        {
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            Department = user.Department,
            Section = user.Section,
            IsActive = user.IsActive,
            Roles = roles,
            CreatedByName = user.CreatedByName,
            UpdatedByName = user.UpdatedByName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> EditRoles(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        var availableRoles = await _roleManager.Roles
            .OrderBy(role => role.Name)
            .Select(role => role.Name!)
            .ToListAsync();

        var (availableDepartments, availableSections) = await GetActiveDepartmentSectionOptionsAsync();
        var sectionDepartmentOptions = await GetActiveSectionDepartmentOptionsAsync();

        var currentDepartment = NormalizeMasterValue(user.Department);
        var currentSection = NormalizeMasterValue(user.Section);

        if (!string.IsNullOrWhiteSpace(currentDepartment) && !availableDepartments.Any(item => string.Equals(item, currentDepartment, StringComparison.OrdinalIgnoreCase)))
        {
            availableDepartments.Add(currentDepartment);
            availableDepartments = availableDepartments
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name)
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(currentSection) && !availableSections.Any(item => string.Equals(item, currentSection, StringComparison.OrdinalIgnoreCase)))
        {
            availableSections.Add(currentSection);
            availableSections = availableSections
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name)
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(currentDepartment) && !string.IsNullOrWhiteSpace(currentSection))
        {
            var hasCurrentPair = sectionDepartmentOptions.Any(item =>
                string.Equals(item.Department, currentDepartment, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(item.Section, currentSection, StringComparison.OrdinalIgnoreCase));

            if (!hasCurrentPair)
            {
                sectionDepartmentOptions.Add(new DepartmentSectionOptionViewModel
                {
                    Department = currentDepartment,
                    Section = currentSection
                });
            }
        }

        sectionDepartmentOptions = sectionDepartmentOptions
            .Where(item => !string.IsNullOrWhiteSpace(item.Department) && !string.IsNullOrWhiteSpace(item.Section))
            .GroupBy(item => $"{item.Department}|||{item.Section}", StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .OrderBy(item => item.Department)
            .ThenBy(item => item.Section)
            .ToList();

        var selectedRoles = await _userManager.GetRolesAsync(user);

        var viewModel = new EditUserRolesViewModel
        {
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            Department = user.Department,
            Section = user.Section,
            IsActive = user.IsActive,
            MustChangePassword = user.MustChangePassword,
            AvailableRoles = availableRoles,
            SelectedRoles = selectedRoles,
            AvailableDepartments = availableDepartments,
            AvailableSections = availableSections,
            SectionDepartmentOptions = sectionDepartmentOptions
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRoles(EditUserRolesViewModel model)
    {
        var actor = await _userManager.GetUserAsync(User);
        var actorName = GetActorName(actor);

        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user is null)
        {
            return NotFound();
        }

        var availableRoles = await _roleManager.Roles
            .OrderBy(role => role.Name)
            .Select(role => role.Name!)
            .ToListAsync();

        var (availableDepartments, availableSections) = await GetActiveDepartmentSectionOptionsAsync();
        var sectionDepartmentOptions = await GetActiveSectionDepartmentOptionsAsync();

        var normalizedDepartment = NormalizeMasterValue(model.Department);
        var normalizedSection = NormalizeMasterValue(model.Section);

        if (!string.IsNullOrWhiteSpace(normalizedDepartment) && !availableDepartments.Any(item => string.Equals(item, normalizedDepartment, StringComparison.OrdinalIgnoreCase)))
        {
            availableDepartments.Add(normalizedDepartment);
            availableDepartments = availableDepartments
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name)
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(normalizedSection) && !availableSections.Any(item => string.Equals(item, normalizedSection, StringComparison.OrdinalIgnoreCase)))
        {
            availableSections.Add(normalizedSection);
            availableSections = availableSections
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name)
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(normalizedDepartment) && !string.IsNullOrWhiteSpace(normalizedSection))
        {
            var hasCurrentPair = sectionDepartmentOptions.Any(item =>
                string.Equals(item.Department, normalizedDepartment, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(item.Section, normalizedSection, StringComparison.OrdinalIgnoreCase));

            if (!hasCurrentPair)
            {
                sectionDepartmentOptions.Add(new DepartmentSectionOptionViewModel
                {
                    Department = normalizedDepartment,
                    Section = normalizedSection
                });
            }
        }

        sectionDepartmentOptions = sectionDepartmentOptions
            .Where(item => !string.IsNullOrWhiteSpace(item.Department) && !string.IsNullOrWhiteSpace(item.Section))
            .GroupBy(item => $"{item.Department}|||{item.Section}", StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .OrderBy(item => item.Department)
            .ThenBy(item => item.Section)
            .ToList();

        var normalizedEmail = model.Email.Trim().ToUpperInvariant();
        var existingUser = await _userManager.Users
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail && u.Id != model.UserId);

        if (existingUser is not null)
        {
            ModelState.AddModelError(nameof(model.Email), "A user with this email already exists.");
        }

        var validDepartment = availableDepartments
            .Any(item => string.Equals(item, normalizedDepartment, StringComparison.OrdinalIgnoreCase));

        if (!validDepartment)
        {
            ModelState.AddModelError(nameof(model.Department), "Department ที่เลือกไม่ถูกต้องหรือไม่ได้เปิดใช้งาน");
        }

        var validSection = sectionDepartmentOptions
            .Any(item =>
                string.Equals(item.Department, normalizedDepartment, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(item.Section, normalizedSection, StringComparison.OrdinalIgnoreCase));

        if (!validSection)
        {
            ModelState.AddModelError(nameof(model.Section), "Section ที่เลือกไม่ตรงกับ Department ที่เลือก");
        }

        if (!ModelState.IsValid)
        {
            model.AvailableRoles = availableRoles;
            model.AvailableDepartments = availableDepartments;
            model.AvailableSections = availableSections;
            model.SectionDepartmentOptions = sectionDepartmentOptions;
            return View(model);
        }

        user.FullName = model.FullName.Trim();
        user.Department = model.Department.Trim();
        user.Section = model.Section.Trim();
        user.IsActive = model.IsActive;
        user.MustChangePassword = model.MustChangePassword;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedByName = actorName;

        if (!string.Equals(user.UserName, model.UserName.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            var usernameResult = await _userManager.SetUserNameAsync(user, model.UserName.Trim());
            if (!usernameResult.Succeeded)
            {
                foreach (var error in usernameResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }

        if (!string.Equals(user.Email, model.Email.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            var emailResult = await _userManager.SetEmailAsync(user, model.Email.Trim());
            if (!emailResult.Succeeded)
            {
                foreach (var error in emailResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            var userNameResult = await _userManager.SetUserNameAsync(user, model.Email.Trim());
            if (!userNameResult.Succeeded)
            {
                foreach (var error in userNameResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResult = await _userManager.ResetPasswordAsync(user, resetToken, model.Password);
            if (!passwordResult.Succeeded)
            {
                foreach (var error in passwordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else if (!model.MustChangePassword)
            {
                // Admin set a new password but did not check force-change; keep current flag unchanged
                // (flag was already assigned from model.MustChangePassword above)
            }
        }

        var selectedRoles = model.SelectedRoles
            .Where(role => availableRoles.Contains(role))
            .Distinct()
            .ToList();

        var currentRoles = await _userManager.GetRolesAsync(user);

        var rolesToAdd = selectedRoles.Except(currentRoles).ToList();
        var rolesToRemove = currentRoles.Except(selectedRoles).ToList();

        if (rolesToAdd.Count > 0)
        {
            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!addResult.Succeeded)
            {
                foreach (var error in addResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }

        if (rolesToRemove.Count > 0)
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded)
            {
                foreach (var error in removeResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }

        if (!ModelState.IsValid)
        {
            model.AvailableRoles = availableRoles;
            model.AvailableDepartments = availableDepartments;
            model.AvailableSections = availableSections;
            model.SectionDepartmentOptions = sectionDepartmentOptions;
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var availableRoles = await _roleManager.Roles
            .OrderBy(role => role.Name)
            .Select(role => role.Name!)
            .ToListAsync();

        var (availableDepartments, availableSections) = await GetActiveDepartmentSectionOptionsAsync();
        var sectionDepartmentOptions = await GetActiveSectionDepartmentOptionsAsync();

        var viewModel = new CreateUserViewModel
        {
            AvailableRoles = availableRoles,
            AvailableDepartments = availableDepartments,
            AvailableSections = availableSections,
            SectionDepartmentOptions = sectionDepartmentOptions,
            IsActive = true,
            MustChangePassword = false
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        var actor = await _userManager.GetUserAsync(User);
        var actorName = GetActorName(actor);
        var nowUtc = DateTime.UtcNow;

        var availableRoles = await _roleManager.Roles
            .OrderBy(role => role.Name)
            .Select(role => role.Name!)
            .ToListAsync();

        var (availableDepartments, availableSections) = await GetActiveDepartmentSectionOptionsAsync();
        var sectionDepartmentOptions = await GetActiveSectionDepartmentOptionsAsync();

        if (!ModelState.IsValid)
        {
            model.AvailableRoles = availableRoles;
            model.AvailableDepartments = availableDepartments;
            model.AvailableSections = availableSections;
            model.SectionDepartmentOptions = sectionDepartmentOptions;
            return View(model);
        }

        var normalizedDepartment = NormalizeMasterValue(model.Department);
        var normalizedSection = NormalizeMasterValue(model.Section);

        var validDepartment = availableDepartments
            .Any(item => string.Equals(item, normalizedDepartment, StringComparison.OrdinalIgnoreCase));

        if (!validDepartment)
        {
            ModelState.AddModelError(nameof(model.Department), "Department ที่เลือกไม่ถูกต้องหรือไม่ได้เปิดใช้งาน");
        }

        var allowedSectionsForDepartment = sectionDepartmentOptions
            .Where(item => string.Equals(item.Department, normalizedDepartment, StringComparison.OrdinalIgnoreCase))
            .Select(item => item.Section)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var validSection = allowedSectionsForDepartment
            .Any(item => string.Equals(item, normalizedSection, StringComparison.OrdinalIgnoreCase));

        if (!validSection)
        {
            ModelState.AddModelError(nameof(model.Section), "Section ที่เลือกไม่ตรงกับ Department ที่เลือก");
        }

        if (!ModelState.IsValid)
        {
            model.AvailableRoles = availableRoles;
            model.AvailableDepartments = availableDepartments;
            model.AvailableSections = availableSections;
            model.SectionDepartmentOptions = sectionDepartmentOptions;
            return View(model);
        }

        // Check if username already exists
        var existingUserName = await _userManager.FindByNameAsync(model.UserName);
        if (existingUserName is not null)
        {
            ModelState.AddModelError(nameof(model.UserName), "A user with this username already exists.");
            model.AvailableRoles = availableRoles;
            model.AvailableDepartments = availableDepartments;
            model.AvailableSections = availableSections;
            model.SectionDepartmentOptions = sectionDepartmentOptions;
            return View(model);
        }

        // Check if email already exists
        var existingEmail = await _userManager.FindByEmailAsync(model.Email);
        if (existingEmail is not null)
        {
            ModelState.AddModelError(nameof(model.Email), "A user with this email already exists.");
            model.AvailableRoles = availableRoles;
            model.AvailableDepartments = availableDepartments;
            model.AvailableSections = availableSections;
            model.SectionDepartmentOptions = sectionDepartmentOptions;
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.UserName.Trim(),
            Email = model.Email.Trim(),
            FullName = model.FullName.Trim(),
            Department = model.Department.Trim(),
            Section = model.Section.Trim(),
            IsActive = model.IsActive,
            MustChangePassword = model.MustChangePassword,
            EmailConfirmed = true,
            CreatedByName = actorName,
            UpdatedByName = actorName,
            CreatedAt = nowUtc,
            UpdatedAt = nowUtc
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            model.AvailableRoles = availableRoles;
            model.AvailableDepartments = availableDepartments;
            model.AvailableSections = availableSections;
            model.SectionDepartmentOptions = sectionDepartmentOptions;
            return View(model);
        }

        // Assign roles if any are selected
        if (model.SelectedRoles.Count > 0)
        {
            var validRoles = model.SelectedRoles
                .Where(role => availableRoles.Contains(role))
                .Distinct()
                .ToList();

            if (validRoles.Count > 0)
            {
                var roleResult = await _userManager.AddToRolesAsync(user, validRoles);
                if (!roleResult.Succeeded)
                {
                    foreach (var error in roleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser is not null && currentUser.Id == id)
        {
            TempData["Error"] = "You cannot delete your own account.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            TempData["Error"] = "Failed to delete user: " + string.Join(", ", result.Errors.Select(e => e.Description));
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> DepartmentSections()
    {
        var model = await BuildDepartmentSectionManagementViewModelAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateDepartment(DepartmentSectionManagementViewModel model)
    {
        var newDepartment = NormalizeMasterValue(model.CreateDepartment.Name);

        if (string.IsNullOrWhiteSpace(newDepartment))
        {
            TempData["Error"] = "กรุณาระบุชื่อ Department ใหม่";
            return RedirectToAction(nameof(DepartmentSections));
        }

        var masterData = await LoadDepartmentSectionStoreAsync();
        var existsInMaster = masterData.Departments
            .Any(name => string.Equals(name, newDepartment, StringComparison.OrdinalIgnoreCase));

        if (!existsInMaster)
        {
            var usedDepartmentNames = await _dbContext.Users
                .AsNoTracking()
                .Select(user => user.Department)
                .ToListAsync();

            var existsInUsage = usedDepartmentNames
                .Select(NormalizeMasterValue)
                .Any(name => string.Equals(name, newDepartment, StringComparison.OrdinalIgnoreCase));

            if (existsInUsage)
            {
                TempData["Warning"] = "Department นี้มีอยู่แล้วในระบบ";
                return RedirectToAction(nameof(DepartmentSections));
            }

            masterData.Departments.Add(newDepartment);
            masterData.InactiveDepartments.RemoveAll(name => string.Equals(name, newDepartment, StringComparison.OrdinalIgnoreCase));
            await SaveDepartmentSectionStoreAsync(masterData);
        }

        TempData["Success"] = $"เพิ่ม Department ใหม่สำเร็จ: {newDepartment}";
        return RedirectToAction(nameof(DepartmentSections));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSection(DepartmentSectionManagementViewModel model)
    {
        var sectionDepartment = NormalizeMasterValue(model.CreateSection.Department);
        var newSection = NormalizeMasterValue(model.CreateSection.Name);

        if (string.IsNullOrWhiteSpace(sectionDepartment))
        {
            TempData["Error"] = "กรุณาเลือก Department สำหรับ Section";
            return RedirectToAction(nameof(DepartmentSections));
        }

        if (string.IsNullOrWhiteSpace(newSection))
        {
            TempData["Error"] = "กรุณาระบุชื่อ Section ใหม่";
            return RedirectToAction(nameof(DepartmentSections));
        }

        var (availableDepartments, _) = await GetActiveDepartmentSectionOptionsAsync();
        var departmentExists = availableDepartments
            .Any(name => string.Equals(name, sectionDepartment, StringComparison.OrdinalIgnoreCase));

        if (!departmentExists)
        {
            TempData["Error"] = "Department ที่เลือกไม่พร้อมใช้งาน กรุณาตรวจสอบอีกครั้ง";
            return RedirectToAction(nameof(DepartmentSections));
        }

        var masterData = await LoadDepartmentSectionStoreAsync();
        var existsInMaster = masterData.Sections
            .Any(name => string.Equals(name, newSection, StringComparison.OrdinalIgnoreCase));

        var usedSectionNames = await _dbContext.Users
            .AsNoTracking()
            .Select(user => user.Section)
            .ToListAsync();

        var existsInUsage = usedSectionNames
            .Select(NormalizeMasterValue)
            .Any(name => string.Equals(name, newSection, StringComparison.OrdinalIgnoreCase));

        if (existsInMaster || existsInUsage)
        {
            if (!masterData.Sections.Any(name => string.Equals(name, newSection, StringComparison.OrdinalIgnoreCase)))
            {
                masterData.Sections.Add(newSection);
            }

            masterData.InactiveSections.RemoveAll(name => string.Equals(name, newSection, StringComparison.OrdinalIgnoreCase));
            UpsertSectionDepartmentMapping(masterData, newSection, sectionDepartment);
            await SaveDepartmentSectionStoreAsync(masterData);

            TempData["Success"] = $"อัปเดต Department ของ Section สำเร็จ: {newSection} -> {sectionDepartment}";
            return RedirectToAction(nameof(DepartmentSections));
        }

        if (!existsInMaster)
        {
            masterData.Sections.Add(newSection);
            masterData.InactiveSections.RemoveAll(name => string.Equals(name, newSection, StringComparison.OrdinalIgnoreCase));
            UpsertSectionDepartmentMapping(masterData, newSection, sectionDepartment);
            await SaveDepartmentSectionStoreAsync(masterData);
        }

        TempData["Success"] = $"เพิ่ม Section ใหม่สำเร็จ: {newSection} ({sectionDepartment})";
        return RedirectToAction(nameof(DepartmentSections));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RenameDepartment(DepartmentSectionManagementViewModel model)
    {
        var oldDepartment = NormalizeMasterValue(model.RenameDepartment.OldDepartment);
        var newDepartment = NormalizeMasterValue(model.RenameDepartment.NewDepartment);

        if (string.IsNullOrWhiteSpace(oldDepartment) || string.IsNullOrWhiteSpace(newDepartment))
        {
            TempData["Error"] = "กรุณาระบุ Department เดิมและ Department ใหม่ให้ครบถ้วน";
            return RedirectToAction(nameof(DepartmentSections));
        }

        if (string.Equals(oldDepartment, newDepartment, StringComparison.OrdinalIgnoreCase))
        {
            TempData["Warning"] = "Department เดิมและใหม่เหมือนกัน จึงไม่มีการเปลี่ยนแปลง";
            return RedirectToAction(nameof(DepartmentSections));
        }

        var actor = await _userManager.GetUserAsync(User);
        var actorName = GetActorName(actor);
        var nowUtc = DateTime.UtcNow;

        var users = await _dbContext.Users.ToListAsync();
        var requesterTickets = await _dbContext.RepairTickets.ToListAsync();

        var updatedUserCount = 0;
        var updatedRequesterTicketCount = 0;
        var updatedApproverTicketCount = 0;

        foreach (var user in users)
        {
            if (!string.Equals(NormalizeMasterValue(user.Department), oldDepartment, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            user.Department = newDepartment;
            user.UpdatedAt = nowUtc;
            user.UpdatedByName = actorName;
            updatedUserCount++;
        }

        foreach (var ticket in requesterTickets)
        {
            if (string.Equals(NormalizeMasterValue(ticket.Department), oldDepartment, StringComparison.OrdinalIgnoreCase))
            {
                ticket.Department = newDepartment;
                ticket.UpdatedAt = nowUtc;
                ticket.UpdatedByName = actorName;
                updatedRequesterTicketCount++;
            }

            if (!string.Equals(NormalizeMasterValue(ticket.ApproverDepartment), oldDepartment, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            ticket.ApproverDepartment = newDepartment;
            ticket.UpdatedAt = nowUtc;
            ticket.UpdatedByName = actorName;
            updatedApproverTicketCount++;
        }

        if (updatedUserCount == 0 && updatedRequesterTicketCount == 0 && updatedApproverTicketCount == 0)
        {
            TempData["Warning"] = "ไม่พบข้อมูล Department ที่ต้องการเปลี่ยนชื่อ";
            return RedirectToAction(nameof(DepartmentSections));
        }

        await _dbContext.SaveChangesAsync();

        var masterData = await LoadDepartmentSectionStoreAsync();
        for (var i = 0; i < masterData.Departments.Count; i++)
        {
            if (string.Equals(masterData.Departments[i], oldDepartment, StringComparison.OrdinalIgnoreCase))
            {
                masterData.Departments[i] = newDepartment;
            }
        }

        for (var i = 0; i < masterData.InactiveDepartments.Count; i++)
        {
            if (string.Equals(masterData.InactiveDepartments[i], oldDepartment, StringComparison.OrdinalIgnoreCase))
            {
                masterData.InactiveDepartments[i] = newDepartment;
            }
        }

        masterData.Departments = masterData.Departments
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        masterData.InactiveDepartments = masterData.InactiveDepartments
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        for (var i = 0; i < masterData.SectionDepartmentMappings.Count; i++)
        {
            if (string.Equals(masterData.SectionDepartmentMappings[i].Department, oldDepartment, StringComparison.OrdinalIgnoreCase))
            {
                masterData.SectionDepartmentMappings[i].Department = newDepartment;
            }
        }

        await SaveDepartmentSectionStoreAsync(masterData);

        TempData["Success"] = $"เปลี่ยนชื่อ Department สำเร็จ: ผู้ใช้ {updatedUserCount} รายการ, ผู้แจ้งซ่อม {updatedRequesterTicketCount} รายการ, ฝ่ายอนุมัติ {updatedApproverTicketCount} รายการ";
        return RedirectToAction(nameof(DepartmentSections));
    }

    [HttpGet]
    public IActionResult EditDepartment(string name)
    {
        var currentName = NormalizeMasterValue(name);
        if (string.IsNullOrWhiteSpace(currentName))
        {
            TempData["Error"] = "ไม่พบ Department ที่ต้องการแก้ไข";
            return RedirectToAction(nameof(DepartmentSections));
        }

        var model = new RenameDepartmentInputModel
        {
            OldDepartment = currentName,
            NewDepartment = currentName
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditDepartment(RenameDepartmentInputModel model)
    {
        var wrapper = new DepartmentSectionManagementViewModel
        {
            RenameDepartment = model
        };

        return await RenameDepartment(wrapper);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleDepartmentActive(string name)
    {
        var departmentName = NormalizeMasterValue(name);
        if (string.IsNullOrWhiteSpace(departmentName))
        {
            TempData["Error"] = "ไม่พบ Department ที่ต้องการเปลี่ยนสถานะ";
            return RedirectToAction(nameof(DepartmentSections));
        }

        var masterData = await LoadDepartmentSectionStoreAsync();
        if (!masterData.Departments.Any(x => string.Equals(x, departmentName, StringComparison.OrdinalIgnoreCase)))
        {
            masterData.Departments.Add(departmentName);
        }

        var wasInactive = masterData.InactiveDepartments
            .Any(x => string.Equals(x, departmentName, StringComparison.OrdinalIgnoreCase));

        if (wasInactive)
        {
            masterData.InactiveDepartments.RemoveAll(x => string.Equals(x, departmentName, StringComparison.OrdinalIgnoreCase));
            TempData["Success"] = $"เปิดใช้งาน Department สำเร็จ: {departmentName}";
        }
        else
        {
            masterData.InactiveDepartments.Add(departmentName);
            TempData["Success"] = $"ปิดใช้งาน Department สำเร็จ: {departmentName}";
        }

        await SaveDepartmentSectionStoreAsync(masterData);
        return RedirectToAction(nameof(DepartmentSections));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RenameSection(DepartmentSectionManagementViewModel model)
    {
        var oldSection = NormalizeMasterValue(model.RenameSection.OldSection);
        var newSection = NormalizeMasterValue(model.RenameSection.NewSection);
        var selectedDepartment = NormalizeMasterValue(model.RenameSection.Department);

        if (string.IsNullOrWhiteSpace(oldSection) || string.IsNullOrWhiteSpace(newSection))
        {
            TempData["Error"] = "กรุณาระบุ Section เดิมและ Section ใหม่ให้ครบถ้วน";
            return RedirectToAction(nameof(DepartmentSections));
        }

        if (string.IsNullOrWhiteSpace(selectedDepartment))
        {
            TempData["Error"] = "กรุณาเลือก Department";
            return RedirectToAction(nameof(DepartmentSections));
        }

        var (availableDepartments, _) = await GetActiveDepartmentSectionOptionsAsync();
        var departmentExists = availableDepartments
            .Any(name => string.Equals(name, selectedDepartment, StringComparison.OrdinalIgnoreCase));

        if (!departmentExists)
        {
            TempData["Error"] = "Department ที่เลือกไม่พร้อมใช้งาน กรุณาตรวจสอบอีกครั้ง";
            return RedirectToAction(nameof(DepartmentSections));
        }

        var masterData = await LoadDepartmentSectionStoreAsync();
        var currentDepartment = masterData.SectionDepartmentMappings
            .Where(item => string.Equals(item.Section, oldSection, StringComparison.OrdinalIgnoreCase))
            .Select(item => NormalizeMasterValue(item.Department))
            .FirstOrDefault() ?? string.Empty;

        var sectionNameChanged = !string.Equals(oldSection, newSection, StringComparison.OrdinalIgnoreCase);
        var departmentChanged = !string.Equals(currentDepartment, selectedDepartment, StringComparison.OrdinalIgnoreCase);

        if (!sectionNameChanged && !departmentChanged)
        {
            TempData["Warning"] = "Section และ Department เดิมและใหม่เหมือนกัน จึงไม่มีการเปลี่ยนแปลง";
            return RedirectToAction(nameof(DepartmentSections));
        }

        var actor = await _userManager.GetUserAsync(User);
        var actorName = GetActorName(actor);
        var nowUtc = DateTime.UtcNow;

        var updatedUserCount = 0;

        if (sectionNameChanged)
        {
            var users = await _dbContext.Users.ToListAsync();

            foreach (var user in users)
            {
                if (!string.Equals(NormalizeMasterValue(user.Section), oldSection, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                user.Section = newSection;
                user.UpdatedAt = nowUtc;
                user.UpdatedByName = actorName;
                updatedUserCount++;
            }

            if (updatedUserCount == 0)
            {
                var sectionExistsInMaster = masterData.Sections
                    .Any(item => string.Equals(item, oldSection, StringComparison.OrdinalIgnoreCase));

                if (!sectionExistsInMaster)
                {
                    TempData["Warning"] = "ไม่พบข้อมูล Section ที่ต้องการเปลี่ยนชื่อ";
                    return RedirectToAction(nameof(DepartmentSections));
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        for (var i = 0; i < masterData.Sections.Count; i++)
        {
            if (string.Equals(masterData.Sections[i], oldSection, StringComparison.OrdinalIgnoreCase))
            {
                masterData.Sections[i] = newSection;
            }
        }

        for (var i = 0; i < masterData.InactiveSections.Count; i++)
        {
            if (string.Equals(masterData.InactiveSections[i], oldSection, StringComparison.OrdinalIgnoreCase))
            {
                masterData.InactiveSections[i] = newSection;
            }
        }

        masterData.Sections = masterData.Sections
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        masterData.InactiveSections = masterData.InactiveSections
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        for (var i = 0; i < masterData.SectionDepartmentMappings.Count; i++)
        {
            if (string.Equals(masterData.SectionDepartmentMappings[i].Section, oldSection, StringComparison.OrdinalIgnoreCase))
            {
                masterData.SectionDepartmentMappings[i].Section = newSection;
            }
        }

        UpsertSectionDepartmentMapping(masterData, newSection, selectedDepartment);

        await SaveDepartmentSectionStoreAsync(masterData);
        TempData["Success"] = sectionNameChanged
            ? $"อัปเดต Section สำเร็จ: ผู้ใช้ {updatedUserCount} รายการ, Department {selectedDepartment}"
            : $"อัปเดต Department ของ Section สำเร็จ: {newSection} -> {selectedDepartment}";
        return RedirectToAction(nameof(DepartmentSections));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleSectionActive(string name)
    {
        var sectionName = NormalizeMasterValue(name);
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            TempData["Error"] = "ไม่พบ Section ที่ต้องการเปลี่ยนสถานะ";
            return RedirectToAction(nameof(DepartmentSections));
        }

        var masterData = await LoadDepartmentSectionStoreAsync();
        if (!masterData.Sections.Any(x => string.Equals(x, sectionName, StringComparison.OrdinalIgnoreCase)))
        {
            masterData.Sections.Add(sectionName);
        }

        var wasInactive = masterData.InactiveSections
            .Any(x => string.Equals(x, sectionName, StringComparison.OrdinalIgnoreCase));

        if (wasInactive)
        {
            masterData.InactiveSections.RemoveAll(x => string.Equals(x, sectionName, StringComparison.OrdinalIgnoreCase));
            TempData["Success"] = $"เปิดใช้งาน Section สำเร็จ: {sectionName}";
        }
        else
        {
            masterData.InactiveSections.Add(sectionName);
            TempData["Success"] = $"ปิดใช้งาน Section สำเร็จ: {sectionName}";
        }

        await SaveDepartmentSectionStoreAsync(masterData);
        return RedirectToAction(nameof(DepartmentSections));
    }

    [HttpGet]
    public async Task<IActionResult> EditSection(string name)
    {
        var currentName = NormalizeMasterValue(name);
        if (string.IsNullOrWhiteSpace(currentName))
        {
            TempData["Error"] = "ไม่พบ Section ที่ต้องการแก้ไข";
            return RedirectToAction(nameof(DepartmentSections));
        }

        var managementModel = await BuildDepartmentSectionManagementViewModelAsync();
        var currentDepartment = managementModel.Sections
            .Where(item => string.Equals(item.Name, currentName, StringComparison.OrdinalIgnoreCase))
            .Select(item => item.Department)
            .FirstOrDefault() ?? string.Empty;

        var model = new RenameSectionInputModel
        {
            OldSection = currentName,
            NewSection = currentName,
            Department = currentDepartment,
            AvailableDepartments = managementModel.AvailableDepartments
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSection(RenameSectionInputModel model)
    {
        var wrapper = new DepartmentSectionManagementViewModel
        {
            RenameSection = model
        };

        return await RenameSection(wrapper);
    }

    private async Task<DepartmentSectionManagementViewModel> BuildDepartmentSectionManagementViewModelAsync()
    {
        var users = await _dbContext.Users
            .AsNoTracking()
            .Select(user => new
            {
                user.Department,
                user.Section
            })
            .ToListAsync();

        var tickets = await _dbContext.RepairTickets
            .AsNoTracking()
            .Select(ticket => new
            {
                ticket.Department,
                ticket.ApproverDepartment
            })
            .ToListAsync();

        var departmentUserCounts = users
            .Select(user => NormalizeMasterValue(user.Department))
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .GroupBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase);

        var requesterDepartmentCounts = tickets
            .Select(ticket => NormalizeMasterValue(ticket.Department))
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .GroupBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase);

        var approverDepartmentCounts = tickets
            .Select(ticket => NormalizeMasterValue(ticket.ApproverDepartment))
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .GroupBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase);

        var masterData = await LoadDepartmentSectionStoreAsync();
        var inactiveDepartmentSet = masterData.InactiveDepartments
            .Select(NormalizeMasterValue)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var inactiveSectionSet = masterData.InactiveSections
            .Select(NormalizeMasterValue)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var sectionDepartmentMap = masterData.SectionDepartmentMappings
            .Select(mapping => new
            {
                Section = NormalizeMasterValue(mapping.Section),
                Department = NormalizeMasterValue(mapping.Department)
            })
            .Where(item => !string.IsNullOrWhiteSpace(item.Section))
            .GroupBy(item => item.Section, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group.Select(item => item.Department).FirstOrDefault(department => !string.IsNullOrWhiteSpace(department)) ?? string.Empty,
                StringComparer.OrdinalIgnoreCase);

        var departmentNames = departmentUserCounts.Keys
            .Concat(requesterDepartmentCounts.Keys)
            .Concat(approverDepartmentCounts.Keys)
            .Concat(masterData.Departments.Select(NormalizeMasterValue))
            .Concat(masterData.InactiveDepartments.Select(NormalizeMasterValue))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        var departments = departmentNames
            .Select(name => new DepartmentUsageViewModel
            {
                Name = name,
                UserCount = departmentUserCounts.GetValueOrDefault(name),
                RequesterTicketCount = requesterDepartmentCounts.GetValueOrDefault(name),
                ApproverTicketCount = approverDepartmentCounts.GetValueOrDefault(name),
                IsActive = !inactiveDepartmentSet.Contains(name)
            })
            .ToList();

        var sectionUserCounts = users
            .Select(user => NormalizeMasterValue(user.Section))
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .GroupBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase);

        var sectionUserDepartments = users
            .Select(user => new
            {
                Section = NormalizeMasterValue(user.Section),
                Department = NormalizeMasterValue(user.Department)
            })
            .Where(item => !string.IsNullOrWhiteSpace(item.Section) && !string.IsNullOrWhiteSpace(item.Department))
            .GroupBy(item => item.Section, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(item => item.Department)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(name => name)
                    .ToList(),
                StringComparer.OrdinalIgnoreCase);

        var sectionNames = sectionUserCounts.Keys
            .Concat(masterData.Sections.Select(NormalizeMasterValue))
            .Concat(masterData.InactiveSections.Select(NormalizeMasterValue))
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        var sections = sectionNames
            .Select(name => new SectionUsageViewModel
            {
                Name = name,
                Department = ResolveSectionDepartment(name, sectionDepartmentMap, sectionUserDepartments),
                UserCount = sectionUserCounts.GetValueOrDefault(name),
                IsActive = !inactiveSectionSet.Contains(name)
            })
            .OrderBy(section => section.Name)
            .ToList();

        return new DepartmentSectionManagementViewModel
        {
            Departments = departments,
            Sections = sections,
            AvailableDepartments = departments
                .Where(item => item.IsActive)
                .Select(item => item.Name)
                .OrderBy(name => name)
                .ToList()
        };
    }

    private static string NormalizeMasterValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
    }

    private async Task<(List<string> Departments, List<string> Sections)> GetActiveDepartmentSectionOptionsAsync()
    {
        // Use the same source as DepartmentSections page so dropdown values stay in sync with Master.
        var managementModel = await BuildDepartmentSectionManagementViewModelAsync();

        var departments = managementModel.Departments
            .Where(item => item.IsActive)
            .Select(item => NormalizeMasterValue(item.Name))
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        var sections = managementModel.Sections
            .Where(item => item.IsActive)
            .Select(item => NormalizeMasterValue(item.Name))
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        return (departments, sections);
    }

    private async Task<List<DepartmentSectionOptionViewModel>> GetActiveSectionDepartmentOptionsAsync()
    {
        var managementModel = await BuildDepartmentSectionManagementViewModelAsync();

        var activeDepartments = managementModel.Departments
            .Where(item => item.IsActive)
            .Select(item => NormalizeMasterValue(item.Name))
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        var mappedOptions = managementModel.Sections
            .Where(item => item.IsActive)
            .Select(item => new DepartmentSectionOptionViewModel
            {
                Department = NormalizeMasterValue(item.Department),
                Section = NormalizeMasterValue(item.Name)
            })
            .Where(item => !string.IsNullOrWhiteSpace(item.Department) && !string.IsNullOrWhiteSpace(item.Section))
            .ToList();

        var legacyUnmappedSections = managementModel.Sections
            .Where(item => item.IsActive)
            .Select(item => new
            {
                Section = NormalizeMasterValue(item.Name),
                Department = NormalizeMasterValue(item.Department)
            })
            .Where(item => !string.IsNullOrWhiteSpace(item.Section) && string.IsNullOrWhiteSpace(item.Department))
            .Select(item => item.Section)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var section in legacyUnmappedSections)
        {
            foreach (var department in activeDepartments)
            {
                mappedOptions.Add(new DepartmentSectionOptionViewModel
                {
                    Department = department,
                    Section = section
                });
            }
        }

        return mappedOptions
            .Where(item => !string.IsNullOrWhiteSpace(item.Department) && !string.IsNullOrWhiteSpace(item.Section))
            .GroupBy(item => $"{item.Department}|||{item.Section}", StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .OrderBy(item => item.Department)
            .ThenBy(item => item.Section)
            .ToList();
    }

    private async Task<DepartmentSectionStoreModel> LoadDepartmentSectionStoreAsync()
    {
        var storePath = GetDepartmentSectionStorePath();
        if (!System.IO.File.Exists(storePath))
        {
            return new DepartmentSectionStoreModel();
        }

        await using var stream = System.IO.File.OpenRead(storePath);
        var data = await JsonSerializer.DeserializeAsync<DepartmentSectionStoreModel>(stream);
        if (data is null)
        {
            return new DepartmentSectionStoreModel();
        }

        data.Departments = data.Departments
            .Select(NormalizeMasterValue)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        data.Sections = data.Sections
            .Select(NormalizeMasterValue)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        data.SectionDepartmentMappings = data.SectionDepartmentMappings
            .Select(item => new SectionDepartmentMappingModel
            {
                Section = NormalizeMasterValue(item.Section),
                Department = NormalizeMasterValue(item.Department)
            })
            .Where(item => !string.IsNullOrWhiteSpace(item.Section))
            .GroupBy(item => item.Section, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .OrderBy(item => item.Section)
            .ToList();

        data.Sections = data.Sections
            .Concat(data.SectionDepartmentMappings.Select(item => item.Section))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        data.InactiveSections = data.InactiveSections
            .Select(NormalizeMasterValue)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        data.InactiveDepartments = data.InactiveDepartments
            .Select(NormalizeMasterValue)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        return data;
    }

    private async Task SaveDepartmentSectionStoreAsync(DepartmentSectionStoreModel data)
    {
        data.Departments = data.Departments
            .Select(NormalizeMasterValue)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        data.Sections = data.Sections
            .Select(NormalizeMasterValue)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        data.SectionDepartmentMappings = data.SectionDepartmentMappings
            .Select(item => new SectionDepartmentMappingModel
            {
                Section = NormalizeMasterValue(item.Section),
                Department = NormalizeMasterValue(item.Department)
            })
            .Where(item => !string.IsNullOrWhiteSpace(item.Section))
            .GroupBy(item => item.Section, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .OrderBy(item => item.Section)
            .ToList();

        data.Sections = data.Sections
            .Concat(data.SectionDepartmentMappings.Select(item => item.Section))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        data.InactiveSections = data.InactiveSections
            .Select(NormalizeMasterValue)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        data.InactiveDepartments = data.InactiveDepartments
            .Select(NormalizeMasterValue)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        var storePath = GetDepartmentSectionStorePath();
        var directory = Path.GetDirectoryName(storePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = System.IO.File.Create(storePath);
        await JsonSerializer.SerializeAsync(stream, data, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    private string GetDepartmentSectionStorePath()
    {
        return Path.Combine(_webHostEnvironment.ContentRootPath, DepartmentSectionStoreRelativePath);
    }

    private static string ResolveSectionDepartment(
        string sectionName,
        IReadOnlyDictionary<string, string> sectionDepartmentMap,
        IReadOnlyDictionary<string, List<string>> sectionUserDepartments)
    {
        if (sectionDepartmentMap.TryGetValue(sectionName, out var mappedDepartment) && !string.IsNullOrWhiteSpace(mappedDepartment))
        {
            return mappedDepartment;
        }

        if (!sectionUserDepartments.TryGetValue(sectionName, out var departments) || departments.Count == 0)
        {
            return string.Empty;
        }

        if (departments.Count == 1)
        {
            return departments[0];
        }

        return "(หลาย Department)";
    }

    private static void UpsertSectionDepartmentMapping(DepartmentSectionStoreModel data, string section, string department)
    {
        if (string.IsNullOrWhiteSpace(section))
        {
            return;
        }

        data.SectionDepartmentMappings.RemoveAll(item =>
            string.Equals(item.Section, section, StringComparison.OrdinalIgnoreCase));

        data.SectionDepartmentMappings.Add(new SectionDepartmentMappingModel
        {
            Section = section,
            Department = department
        });
    }

    private sealed class DepartmentSectionStoreModel
    {
        public List<string> Departments { get; set; } = new();
        public List<string> Sections { get; set; } = new();
        public List<string> InactiveDepartments { get; set; } = new();
        public List<string> InactiveSections { get; set; } = new();
        public List<SectionDepartmentMappingModel> SectionDepartmentMappings { get; set; } = new();
    }

    private sealed class SectionDepartmentMappingModel
    {
        public string Section { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }

    private static string GetActorName(ApplicationUser? user)
    {
        if (!string.IsNullOrWhiteSpace(user?.FullName))
        {
            return user.FullName;
        }

        return user?.UserName ?? user?.Email ?? "System";
    }
}
