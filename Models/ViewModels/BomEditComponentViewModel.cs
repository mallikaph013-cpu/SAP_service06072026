namespace myapp.Models.ViewModels
{
    // ViewModel สำหรับตาราง Edit BOM หนึ่งแถว โดยแยกข้อมูลเดิมและข้อมูลใหม่ออกจากกัน
    public class BomEditComponentViewModel
    {
        public int Id { get; set; }

        // ข้อมูลเดิมก่อนแก้ไข
        public string? ItemCodeFrom { get; set; }
        public string? DescriptionFrom { get; set; }
        public decimal? ItemQuantityFrom { get; set; }
        public string? UnitFrom { get; set; }
        public string? BomUsageFrom { get; set; }
        public string? SlocFrom { get; set; }

        // ข้อมูลใหม่หลังแก้ไข
        public string? ItemCodeTo { get; set; }
        public string? DescriptionTo { get; set; }
        public decimal? ItemQuantityTo { get; set; }
        public string? UnitTo { get; set; }
        public string? BomUsageTo { get; set; }
        public string? SlocTo { get; set; }
        public string? PlantTo { get; set; }
    }
}

