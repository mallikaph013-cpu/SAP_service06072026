using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace myapp.Models
{
    // รายการสิทธิ์ SAP หนึ่งแถว เช่น T-Code ที่ต้องการเพิ่มให้ผู้ใช้
    public class LicensePermissionItem
    {
        public int Id { get; set; }

        // Foreign key กลับไปยังคำร้องหลัก
        [Required]
        public int RequestItemId { get; set; }

        [ForeignKey("RequestItemId")]
        public RequestItem? RequestItem { get; set; }

        // ผู้ใช้ SAP เป้าหมายและสิทธิ์ที่ต้องการเพิ่ม
        public string? SapUsername { get; set; }
        public string? TCode { get; set; }
    }
}