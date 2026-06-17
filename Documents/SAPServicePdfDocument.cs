using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using myapp.Models;
using System;

namespace myapp.Documents
{
    public class SAPServicePdfDocument : IDocument
    {
        private readonly RequestItem _item;
        public SAPServicePdfDocument(RequestItem item)
        {
            _item = item;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontFamily("Tahoma").FontSize(12));
                page.Content()
                    .Column(col =>
                    {
                        col.Item().Text("SAP Required").FontSize(24).Bold();
                        col.Item().Text($"Document No. {_item.DocumentNumber}").Bold();
                        col.Item().PaddingVertical(10).Text("รายละเอียดผู้ร้องขอ").Bold().FontSize(16);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(90);
                                columns.RelativeColumn(2);
                                columns.ConstantColumn(95);
                                columns.RelativeColumn(2);
                            });
                            table.Cell().Element(CellStyle).Text("Name:").SemiBold();
                            table.Cell().Element(CellStyle).Text(string.IsNullOrWhiteSpace(_item.Requester) ? "-" : _item.Requester);
                            

                            table.Cell().Element(CellStyle).Text("Department:").SemiBold();
                            table.Cell().Element(CellStyle).Text(string.IsNullOrWhiteSpace(_item.RequesterDepartment) ? "-" : _item.RequesterDepartment);
                            table.Cell().Element(CellStyle).Text("Email:").SemiBold();
                            table.Cell().Element(CellStyle).Text(string.IsNullOrWhiteSpace(_item.RequesterEmail) ? "-" : _item.RequesterEmail);
                        });
                        col.Item().PaddingVertical(10).Text("รายละเอียดการร้องขอ").Bold().FontSize(16);
                        col.Item().Text($"Request Type: {_item.RequestType}\nStatus: {_item.Status}");
                        col.Item().PaddingVertical(10).Text("รายละเอียด").Bold().FontSize(16);
                        // แสดงรายละเอียดตาม RequestType ถ้ามี header, ถ้าไม่มีให้แสดง Description
                        if (Enum.TryParse<RequestType>(_item.RequestType, out var rt))
                        {
                            var headers = myapp.Documents.RequestItemDetailsHelper.GetHeadersForRequestType(rt);
                            if ((rt == RequestType.Routing && _item.Routings != null && _item.Routings.Count > 0)
                                || (rt == RequestType.BOM && _item.BomComponents != null && _item.BomComponents.Count > 0))
                            {
                                // แสดง Routing หรือ BOM เป็นตาราง พร้อมเส้นตารางและขนาดตัวหนังสือเล็กลง
                                var isBom = rt == RequestType.BOM;
                                var dataList = isBom ? (System.Collections.IEnumerable)_item.BomComponents : _item.Routings;
                                col.Item().PaddingTop(5).Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        foreach (var header in headers)
                                            columns.RelativeColumn();
                                    });
                                    // Header row
                                    foreach (var header in headers)
                                        table.Cell().Border(1).Background("#f0f0f0").Padding(2).Text(header).FontSize(10).Bold();
                                    // Data rows
                                    if (dataList != null)
                                    {
                                        foreach (var row in dataList)
                                        {
                                            foreach (var header in headers)
                                            {
                                                var prop = row.GetType().GetProperty(header);
                                                var value = prop?.GetValue(row, null);
                                                table.Cell().Border(0.5f).Padding(2).Text(value?.ToString() ?? "-").FontSize(9);
                                            }
                                        }
                                    }
                                });
                            }
                            else if (headers.Count > 0)
                            {
                                col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });
                                    foreach (var header in headers)
                                    {
                                        var prop = _item.GetType().GetProperty(header);
                                        var value = prop?.GetValue(_item, null);
                                        if (prop != null && value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                                        {
                                            table.Cell().Element(CellStyle).Text(header + ":");
                                            table.Cell().Element(CellStyle).Text(value.ToString());
                                        }
                                    }
                                });
                            }
                            else
                            {
                                col.Item().Background("#cccccc").Padding(20).Text(_item.Description);
                            }
                        }
                        else
                        {
                            col.Item().Background("#cccccc").Padding(20).Text(_item.Description);
                        }
                        col.Item().PaddingVertical(10).Text("การดำเนินการ").Bold().FontSize(16);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });
                            // แถวชื่อ
                            table.Cell().Element(CellStyle).Text((_item.Requester ?? "-") + "\nผู้ร้องขอ");
                            table.Cell().Element(CellStyle).Text((!string.IsNullOrWhiteSpace(_item.UpdatedByDisplayName) ? _item.UpdatedByDisplayName : (!string.IsNullOrWhiteSpace(_item.UpdatedBy) ? _item.UpdatedBy : "-")) + "\nเจ้าหน้าที่ IT");
                            // แถววันที่
                            table.Cell().Element(CellStyle).Text(_item.RequestDate.ToString("dd/MM/yy"));
                            table.Cell().Element(CellStyle).Text(_item.UpdatedAt?.ToString("dd/MM/yy") ?? "");
                        });
                    });
            });
        }

        private IContainer CellStyle(IContainer container) => container.Padding(2);
    }
}
