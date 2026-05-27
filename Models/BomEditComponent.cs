using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace myapp.Models
{
    // เก็บข้อมูลเปรียบเทียบก่อนแก้และหลังแก้ของ BOM หนึ่งแถวในคำร้อง Edit BOM
    public class BomEditComponent
    {
        public int Id { get; set; }

        // ค่าฝั่งเดิมก่อนแก้ไข
        public string? ItemCodeFrom { get; set; }
        public string? DescriptionFrom { get; set; }
        public decimal? ItemQuantityFrom { get; set; }
        public string? UnitFrom { get; set; }
        public string? BomUsageFrom { get; set; }
        public string? SlocFrom { get; set; }

        // ค่าฝั่งใหม่หลังแก้ไข
        public string? ItemCodeTo { get; set; }
        public string? DescriptionTo { get; set; }
        public decimal? ItemQuantityTo { get; set; }
        public string? UnitTo { get; set; }
        public string? BomUsageTo { get; set; }
        public string? SlocTo { get; set; }
        public string? PlantTo { get; set; }
    }
}
