using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Text;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace sapservice.Controllers
{

    using myapp.Data;
    using myapp.Models;
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public ReportController(ApplicationDbContext context, ICompositeViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
        {
            _context = context;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        [HttpGet]
        public IActionResult ExportToPdf(int id)
        {
            var item = _context.RequestItems
                .Include(x => x.Routings)
                .Include(x => x.BomComponents)
                .FirstOrDefault(x => x.Id == id);
            if (item == null)
                return NotFound();

            // ใช้ QuestPDF สร้าง PDF
            var document = new myapp.Documents.SAPServicePdfDocument(item);
            using var stream = new System.IO.MemoryStream();
            document.GeneratePdf(stream);
            var pdfBytes = stream.ToArray();
            var fileName = $"SAPService_{item.DocumentNumber}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }


        public IActionResult SAPService()
        {
            var items = _context.RequestItems.Where(x => x.UsageStatus == 1).OrderByDescending(x => x.RequestDate).ToList();
            return View(items);
        }

        [HttpPost]
        public IActionResult ExportToExcel()
        {
            // TODO: สร้างไฟล์ Excel จากข้อมูลและส่งออก
            return File(new byte[0], "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SAPServiceReport.xlsx");
        }

// ...existing code...


        // Render Razor view to string for PDF
        private async Task<string> RenderViewAsync(string viewName, object model)
        {
            var actionContext = new ActionContext(HttpContext, RouteData, ControllerContext.ActionDescriptor, ModelState);
            using var sw = new StringWriter();
            var viewResult = _viewEngine.FindView(actionContext, viewName, false);
            if (!viewResult.Success)
                throw new InvalidOperationException($"View '{viewName}' not found.");
            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), ModelState) { Model = model };
            var viewContext = new ViewContext(actionContext, viewResult.View, viewDictionary, new TempDataDictionary(HttpContext, _tempDataProvider), sw, new HtmlHelperOptions());
            await viewResult.View.RenderAsync(viewContext);
            return sw.ToString();
        }
    }
}
