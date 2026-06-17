using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace myapp.Models
{
    // รายการ routing หนึ่งแถวของคำร้อง ใช้เก็บขั้นตอนการผลิตและต้นทุนที่เกี่ยวข้อง
    public class Routing
    {
        public int Id { get; set; }

        // Foreign key กลับไปยังคำร้องหลัก
        [Required]
        public int RequestItemId { get; set; }
        [ForeignKey("RequestItemId")]
        public RequestItem? RequestItem { get; set; }

        // ข้อมูลทั่วไปของ routing เช่น material, plant, work center และ operation
        public string? Material { get; set; }
        public string? Description { get; set; }
        public string? Counter { get; set; }
        public string? Plant { get; set; }
        public string? WorkCenter { get; set; }
        public string? Operation { get; set; }

        // กลุ่มค่าเชิงปริมาณและต้นทุน ใช้ decimal เพื่อให้รองรับค่าทศนิยมจาก SAP/Excel
        [Column(TypeName = "decimal(18, 5)")]
        public decimal? BaseQty { get; set; }
        public string? Unit { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal? DirectLaborCosts { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal? DirectExpenses { get; set; }

        [Column(TypeName = "decimal(18, 5)")]
        public decimal? AllocationExpense { get; set; }

        // ข้อมูลควบคุมเวอร์ชันและช่วงเวลาที่ routing นี้มีผลใช้งาน
        public string? ProductionVersionCode { get; set; }
        public string? Version { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        // ค่ากำหนดเพิ่มเติมสำหรับการผูก routing กับ BOM และกลุ่มการผลิต
        [Column(TypeName = "decimal(18, 5)")]
        public decimal? MaximumLotSize { get; set; }
        public string? Alternative { get; set; }
        public string? BomUsage { get; set; }
        public string? Group { get; set; }
        public string? GroupCounter { get; set; }
    }
}
