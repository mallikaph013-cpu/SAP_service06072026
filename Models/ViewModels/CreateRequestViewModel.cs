using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace myapp.Models.ViewModels
{
    // ViewModel สำหรับหน้า Create/Edit คำร้อง ทำหน้าที่รับค่าจากฟอร์มและแปลงชนิดข้อมูลก่อน map เข้า entity
    public class CreateRequestViewModel
    {
        public int Id { get; set; }

        // ข้อมูลส่วนหัวของคำร้องจากหน้าจอ
        [Required]
        public RequestType RequestType { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        // ข้อมูลผู้ยื่นคำร้อง ใช้เติมค่าเริ่มต้นจากผู้ใช้ที่ login และแสดงกลับบนหน้าฟอร์ม
        public string RequesterName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string RequesterPlant { get; set; } = string.Empty; // Renamed from 'Plant'

        // เก็บค่าผู้รับผิดชอบถัดไปจาก dropdown ในรูปแบบ userId|routingId
        public string? NextResponsibleUserId { get; set; }

        // สถานะและเหตุผลการ reject ใช้ในหน้าแก้ไขหรือการอนุมัติ
        public string Status { get; set; } = "Pending";
        public string? RejectionRemark { get; set; }

        // กลุ่ม field หลักของ material ที่แสดง/ซ่อนตามประเภทคำร้อง

        public string? Plant { get; set; } // Renamed from PlantFG

        public string? ItemCode { get; set; }
        // ปริมาณระดับบน ใช้กับคำร้องที่มีข้อมูล BOM หรือข้อมูลเชิงโครงสร้าง
        public decimal? Quantity { get; set; }
        public string? EnglishMatDescription { get; set; }
        public string? ModelName { get; set; }
        public string? BaseUnit { get; set; }
        public string? MaterialGroup { get; set; }
        public string? ExternalMaterialGroup { get; set; }
        public string? DivisionCode { get; set; }
        public string? ProfitCenter { get; set; }
        public string? DistributionChannel { get; set; }
        public string? BoiCode { get; set; }
        public string? MrpController { get; set; }
        public string? StorageLocation { get; set; }
        public string? ProductionSupervisor { get; set; }
        public string? CostingLotSize { get; set; }
        public string? ValClass { get; set; }
        public string? StandardPack { get; set; }
        public string? BoiDescription { get; set; }
        public string? MakerMfrPartNumber { get; set; }
        public string? CommCodeTariffCode { get; set; }
        public string? TraffCodePercentage { get; set; }
        public string? StorageLocationB1 { get; set; }
        public string? PriceControl { get; set; }
        public string? Currency { get; set; }
        public string? SupplierCode { get; set; }
        public string? MaterialType { get; set; }

        // Alias เพื่อรองรับ view/logic เดิมที่ยังอ้าง MatType ระหว่างช่วงเปลี่ยนชื่อ field
        public string? MatType
        {
            get => MaterialType;
            set => MaterialType = value;
        }

        // กลุ่ม field เสริมที่ใช้เฉพาะบาง request type เช่น Tooling, IPO, Routing support และ purchase flow
        public bool Check { get; set; }
        public string? DevicePlant { get; set; }
        public string? AssemblyPlant { get; set; }
        public string? IpoPlant { get; set; }
        public string? AsiOfPlant { get; set; }
        public string? PriceUnit { get; set; }
        public string? StorageLocationEP { get; set; }
        public string? ToolingBSection { get; set; }
        public string? PoNumber { get; set; }
        public string? StatusInA { get; set; }
        public string? DateIn { get; set; }
        public string? QuotationNumber { get; set; }
        public string? ToolingBModel { get; set; }
        public string? TariffCode { get; set; }
        public string? Planner { get; set; }
        public string? CurrentICS { get; set; }
        public string? Level { get; set; }
        public string? Rohs { get; set; }
        public string? CodenMid { get; set; }
        public string? SalesOrg { get; set; }
        public string? TaxTh { get; set; }
        public string? MaterialStatisticsGroup { get; set; }
        public string? AccountAssignment { get; set; }
        public string? GeneralItemCategory { get; set; }
        public string? Availability { get; set; }
        public string? Transportation { get; set; }
        public string? LoadingGroup { get; set; }
        public string? PlanDelTime { get; set; }
        public string? SchedMargin { get; set; }
        public string? MinLot { get; set; }
        public string? MaxLot { get; set; }
        public string? FixedLot { get; set; }
        public string? Rounding { get; set; }
        public string? Mtlsm { get; set; }
        public string? Effective { get; set; }
        public string? StorageLoc { get; set; }
        public string? ReceiveStorage { get; set; }
        public string? PurchasingGroup { get; set; }
        public string? SapModule { get; set; }
        public string? Price { get; set; }

        // ใช้ควบคุมข้อมูลหน้าจอกรณี Edit BOM
        public string? EditBomFg { get; set; }
        public bool EditBomAllFg { get; set; }

        // ตารางลูกที่ bind จาก form แบบหลายแถว เช่น BOM, Routing, Edit BOM และสิทธิ์ SAP
        public List<BomComponentViewModel> Components { get; set; } = new List<BomComponentViewModel>();
        public List<RoutingViewModel> Routings { get; set; } = new List<RoutingViewModel>();

        public List<BomEditComponentViewModel> EditBOM { get; set; } = new List<BomEditComponentViewModel>();
        public List<LicensePermissionViewModel> LicensePermissions { get; set; } = new List<LicensePermissionViewModel>();

        // ใช้บอกหน้าฟอร์มว่าเปิดมาจาก import เพื่อผ่อน validation บางส่วนชั่วคราว
        public bool FromImport { get; set; }
    }
}
