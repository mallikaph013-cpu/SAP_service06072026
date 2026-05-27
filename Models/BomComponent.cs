using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace myapp.Models
{
    // รายการชิ้นส่วนย่อยของ BOM หนึ่งแถว ภายใต้คำร้องหลักหนึ่งรายการ
    public class BomComponent
    {
        public int Id { get; set; }

        // Foreign key กลับไปยังคำร้องหลัก
        [Required]
        public int RequestItemId { get; set; }
        [ForeignKey("RequestItemId")]
        public RequestItem? RequestItem { get; set; }

        // ข้อมูลหนึ่งแถวในตาราง BOM ที่ใช้ทั้งบนฟอร์มและการ import จาก Excel
        public int Level { get; set; }
        public string? Item { get; set; }
        public string? ItemCat { get; set; } 
        public string? ComponentNumber { get; set; }
        public string? Description { get; set; }

        // ปริมาณเก็บเป็น decimal เพื่อรองรับค่าทศนิยมของ BOM
        [Column(TypeName = "decimal(18, 5)")]
        public decimal? ItemQuantity { get; set; }
        public string? Unit { get; set; }
        public string? BomUsage { get; set; }
        public string? Plant { get; set; }
        public string? Sloc { get; set; }
    }
}
