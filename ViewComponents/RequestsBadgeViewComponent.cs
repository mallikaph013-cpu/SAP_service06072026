using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using myapp.Data;
using myapp.Models;

namespace myapp.ViewComponents
{
    public class RequestsBadgeViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RequestsBadgeViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return Content(string.Empty);
            }

            var currentUserId = user.Id;

            var requestCandidates = await _context.RequestItems
                .AsNoTracking()
                .Where(request => request.UsageStatus != 9)
                .Select(request => new
                {
                    request.NextApproverId,
                    request.Status,
                    request.AssignedITUserId
                })
                .ToListAsync();

            var badgeCount = requestCandidates.Count(request =>
                !string.IsNullOrWhiteSpace(request.NextApproverId)
                && string.Equals(
                    request.NextApproverId.Split('|', StringSplitOptions.RemoveEmptyEntries)[0],
                    currentUserId,
                    StringComparison.OrdinalIgnoreCase)
                && !string.Equals(request.Status, "Complete", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(request.Status, "Rejected", StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrWhiteSpace(request.AssignedITUserId));

            return View(badgeCount);
        }
    }
}