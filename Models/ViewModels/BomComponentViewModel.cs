namespace myapp.Models.ViewModels
{
    // ViewModel ของ BOM หนึ่งแถว ใช้ bind ตาราง BOM ในหน้า Create/Edit
    public class BomComponentViewModel
    {
        public int Id { get; set; }

        // โครงสร้างข้อมูลต้องสอดคล้องกับ BomComponent เพื่อ map ไปกลับได้ตรงกัน
        public int Level { get; set; }
        public string? Item { get; set; }
        public string? ItemCat { get; set; } 
        public string? ComponentNumber { get; set; }
        public string? Description { get; set; }
        public decimal? ItemQuantity { get; set; }
        public string? Unit { get; set; }
        public string? BomUsage { get; set; }
        public string? Plant { get; set; }
        public string? Sloc { get; set; }
    }
}
