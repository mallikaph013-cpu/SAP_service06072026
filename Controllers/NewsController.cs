using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myapp.Data;
using myapp.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;

namespace myapp.Controllers
{
    [Authorize]
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public NewsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: News
        [Authorize(Roles = "IT")]
        public async Task<IActionResult> Index()
        {
            var articles = await _context.NewsArticles.OrderByDescending(a => a.PublishedDate).ToListAsync();
            return View(articles);
        }

        // GET: News/Details/5
        public async Task<IActionResult> Details(int? id)
        {   
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.NewsArticles
                .Include(a => a.Attachments)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            // Map NewsAttachment เป็น AttachmentViewModel
            var attachments = article.Attachments.Select(att => new AttachmentViewModel
            {
                FileName = att.FileName,
                Url = att.FilePath
            }).ToList();

            var viewModel = new NewsArticleViewModel
            {
                Id = article.Id,
                Title = article.Title,
                IsFeatured = article.IsFeatured,
                Excerpt = article.Content,
                ImageUrl = article.ImageUrl,
                PublishedDate = article.PublishedDate,
                Author = article.Author,
                ArticleUrl = string.Empty,
                Attachments = attachments
            };

            return View(viewModel);
        }

        // GET: News/Create
        [Authorize(Roles = "IT")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: News/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "IT")]
        public async Task<IActionResult> Create(NewsArticle article, List<IFormFile> attachments)
        {
            if (ModelState.IsValid)
            {
                var actor = User.Identity?.Name ?? "System";
                var now = DateTime.UtcNow;

                article.CreatedAt = now;
                article.UpdatedAt = now;
                article.CreatedBy = actor;
                article.UpdatedBy = actor;

                _context.Add(article);
                await _context.SaveChangesAsync();

                // แนบไฟล์หลายไฟล์
                if (attachments != null && attachments.Count > 0)
                {
                    var uploadsDirectory = Path.Combine(_environment.WebRootPath, "uploads", "news");
                    Directory.CreateDirectory(uploadsDirectory);
                    const long maxFileSize = 10 * 1024 * 1024; // 10 MB
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt" };

                    foreach (var file in attachments)
                    {
                        if (file.Length > 0)
                        {
                            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                            if (file.Length > maxFileSize) continue;
                            if (!allowedExtensions.Contains(extension)) continue;

                            var fileName = $"{Guid.NewGuid():N}{extension}";
                            var filePath = Path.Combine(uploadsDirectory, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            var attachment = new NewsAttachment
                            {
                                NewsArticleId = article.Id,
                                FileName = file.FileName,
                                FilePath = $"/uploads/news/{fileName}",
                                UploadedAt = DateTime.UtcNow
                            };
                            _context.NewsAttachments.Add(attachment);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["SaveSuccessMessage"] = "Article created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        // GET: News/Edit/5
        [Authorize(Roles = "IT")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.NewsArticles
                .Include(a => a.Attachments)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }

        // POST: News/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "IT")]
        public async Task<IActionResult> Edit(int id, NewsArticle article, IFormFile? imageFile)
        {
            if (id != article.Id)
            {
                return NotFound();
            }

            var existing = await _context.NewsArticles.Include(a => a.Attachments).FirstOrDefaultAsync(a => a.Id == id);
            if (existing == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // แนบไฟล์หลายไฟล์
                    var attachments = Request.Form.Files.Where(f => f.Name == "attachments").ToList();
                    if (attachments != null && attachments.Count > 0)
                    {
                        var uploadsDirectory = Path.Combine(_environment.WebRootPath, "uploads", "news");
                        Directory.CreateDirectory(uploadsDirectory);
                        const long maxFileSize = 10 * 1024 * 1024; // 10 MB
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt" };

                        foreach (var file in attachments)
                        {
                            if (file.Length > 0)
                            {
                                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                                if (file.Length > maxFileSize) continue;
                                if (!allowedExtensions.Contains(extension)) continue;

                                var fileName = $"{Guid.NewGuid():N}{extension}";
                                var filePath = Path.Combine(uploadsDirectory, fileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                var attachment = new NewsAttachment
                                {
                                    NewsArticleId = existing.Id,
                                    FileName = file.FileName,
                                    FilePath = $"/uploads/news/{fileName}",
                                    UploadedAt = DateTime.UtcNow
                                };
                                _context.NewsAttachments.Add(attachment);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                    existing.Title = article.Title;
                    existing.Content = article.Content;
                    existing.PublishedDate = article.PublishedDate;
                    existing.Author = article.Author;
                    existing.IsFeatured = article.IsFeatured;
                    existing.UpdatedAt = DateTime.UtcNow;
                    existing.UpdatedBy = User.Identity?.Name ?? "System";

                    await _context.SaveChangesAsync();
                    TempData["SaveSuccessMessage"] = "Article updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ArticleExists(article.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        // GET: News/Delete/5
        [Authorize(Roles = "IT")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.NewsArticles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "IT")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.NewsArticles.FindAsync(id);
            if (article != null) {
                 _context.NewsArticles.Remove(article);
                 await _context.SaveChangesAsync();
                 TempData["SuccessMessage"] = "Article deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ArticleExists(int id)
        {
            return await _context.NewsArticles.AnyAsync(e => e.Id == id);
        }
    }
}
