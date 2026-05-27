using System;
using System.Collections.Generic;
using myapp.Models;

namespace myapp.Documents
{
    public static class RequestItemDetailsHelper
    {
        public static List<string> GetHeadersForRequestType(RequestType requestType)
        {
            var headers = new List<string>();

            switch (requestType)
            {
                case RequestType.Request:
                    headers.AddRange(new[] { "AttachmentFileName", "AttachmentPath" });
                    break;
                case RequestType.FG:
                    headers.AddRange(new[] { "ItemCode", "EnglishMatDescription", "ModelName", "BaseUnit", "MaterialGroup", "Plant", "DivisionCode", "ProfitCenter", "DistributionChannel", "BoiCode", "MrpController", "StorageLocation", "ValClass", "ProductionSupervisor", "CostingLotSize", "Price" });
                    break;
                case RequestType.SM:
                    headers.AddRange(new[] { "CurrentICS", "ItemCode", "EnglishMatDescription", "BaseUnit", "MaterialGroup", "Plant", "DivisionCode", "ProfitCenter", "MrpController", "StorageLocation", "ProductionSupervisor", "CostingLotSize" });
                    break;
                case RequestType.RM:
                    headers.AddRange(new[] { "ItemCode", "EnglishMatDescription", "ModelName", "BaseUnit", "MaterialGroup", "Plant", "DivisionCode", "ProfitCenter", "BoiDescription", "MrpController", "StorageLocation", "ValClass", "CostingLotSize", "Price", "Currency", "PurchasingGroup", "MakerMfrPartNumber", "CommCodeTariffCode", "TraffCodePercentage", "PriceControl", "SupplierCode" });
                    break;
                case RequestType.Passthrough:
                case RequestType.CrossPlantPurchase:
                    headers.AddRange(new[] { "ItemCode", "EnglishMatDescription", "ModelName", "BaseUnit", "MaterialGroup", "Plant", "DivisionCode", "ProfitCenter", "BoiDescription", "MrpController", "StorageLocation", "ValClass", "Price", "PurchasingGroup", "MakerMfrPartNumber", "CommCodeTariffCode", "TraffCodePercentage", "PriceControl", "SupplierCode" });
                    break;
                case RequestType.ToolingB:
                    headers.AddRange(new[] { "ItemCode", "EnglishMatDescription", "MaterialType", "ModelName", "BaseUnit", "MaterialGroup", "Plant", "DivisionCode", "ProfitCenter", "Price", "PriceUnit", "PurchasingGroup", "QuotationNumber", "PoNumber", "StatusInA", "ToolingBSection" });
                    break;
                case RequestType.ToolingB_FG:
                    headers.AddRange(new[] { "ItemCode", "EnglishMatDescription", "BaseUnit", "MaterialGroup", "Plant", "DivisionCode", "ProfitCenter", "DistributionChannel", "BoiCode", "MrpController", "StorageLocation", "ValClass", "ProductionSupervisor", "CostingLotSize", "Price" });
                    break;
                case RequestType.ToolingB_PU:
                    headers.AddRange(new[] { "ItemCode", "EnglishMatDescription", "BaseUnit", "MaterialGroup", "Plant", "DivisionCode", "ProfitCenter", "DistributionChannel", "StorageLocation", "Price", "PriceUnit", "PurchasingGroup" });
                    break;
                case RequestType.BOM:
                    headers.AddRange(new[] { "Level", "Item", "ItemCat", "ComponentNumber", "Description", "ItemQuantity", "Unit", "BomUsage", "Sloc", "Plant" });
                    break;
                case RequestType.EditBOM:
                    headers.AddRange(new[] { "ItemCodeFrom", "DescriptionFrom", "ItemQuantityFrom", "UnitFrom", "BomUsageFrom", "SlocFrom", "ItemCodeTo", "DescriptionTo", "ItemQuantityTo", "UnitTo", "BomUsageTo", "SlocTo", "PlantTo" });
                    break;
                case RequestType.Routing:
                    headers.AddRange(new[] { "Counter", "Plant", "Material", "Description", "WorkCenter", "BaseQty", "Unit", "DirectLaborCosts", "DirectExpenses", "AllocationExpense", "ProductionVersionCode", "Version", "ValidFrom", "ValidTo", "MaximumLotSize", "Alternative", "BomUsage", "Group", "GroupCounter" });
                    break;
                case RequestType.AddStorage:
                    headers.AddRange(new[] { "ItemCode", "Plant", "StorageLocation" });
                    break;
                case RequestType.DistributionChanel:
                    headers.AddRange(new[] { "ItemCode", "EnglishMatDescription", "Plant", "DivisionCode", "ProfitCenter", "DistributionChannel", "BoiCode", "AccountAssignment", "StorageLocation" });
                    break;
                case RequestType.IPO:
                    headers.AddRange(new[] { "ItemCode", "EnglishMatDescription", "ModelName", "BaseUnit", "MaterialGroup", "Plant", "DivisionCode", "ProfitCenter", "DistributionChannel", "BoiCode", "MrpController", "StorageLocation", "ValClass", "ProductionSupervisor", "CostingLotSize", "Price", "PurchasingGroup", "TariffCode", "Planner", "PriceControl" });
                    break;
                case RequestType.LicensePermission:
                    headers.AddRange(new[] { "ItemCode", "Plant" });
                    break;
            }

            return headers;
        }
    }
}
