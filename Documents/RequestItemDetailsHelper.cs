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
                case RequestType.FG:
                    headers.AddRange(new[] { "Plant", "ItemCode", "EnglishMatDescription", "ModelName", "BaseUnit", "MaterialGroup", "ExternalMaterialGroup", "DivisionCode", "ProfitCenter", "DistributionChannel", "BoiCode", "MrpController", "StorageLocation", "ProductionSupervisor", "CostingLotSize", "ValClass" });
                    break;
                case RequestType.SM:
                    headers.AddRange(new[] { "ItemCode", "EnglishMatDescription", "BaseUnit", "Plant", "MaterialGroup", "DivisionCode", "ProfitCenter", "MrpController", "StorageLocation", "ProductionSupervisor", "CostingLotSize", "StandardPack" });
                    break;
                case RequestType.RM:
                    headers.AddRange(new[] { "ItemCode", "EnglishMatDescription", "ModelName", "BaseUnit", "BoiDescription", "Plant", "MaterialGroup", "ExternalMaterialGroup", "DivisionCode", "ProfitCenter", "PurchasingGroup", "MakerMfrPartNumber", "CommCodeTariffCode", "TraffCodePercentage", "MrpController", "StorageLocation", "StorageLocationB1", "PriceControl", "ValClass", "Price", "Currency", "CostingLotSize", "SupplierCode" });
                    break;
                case RequestType.Passthrough:
                case RequestType.CrossPlantPurchase:
                    headers.AddRange(new[] { "ItemCode", "EnglishMatDescription", "ModelName", "BaseUnit", "BoiDescription", "Plant", "MaterialGroup", "ExternalMaterialGroup", "DivisionCode", "ProfitCenter", "PurchasingGroup", "MakerMfrPartNumber", "CommCodeTariffCode", "TraffCodePercentage", "MrpController", "StorageLocation", "PriceControl", "ValClass", "Price", "SupplierCode" });
                    break;
                case RequestType.ToolingB:
                    headers.AddRange(new[] { "ItemCode", "MatType", "Check", "EnglishMatDescription", "MaterialGroup", "BaseUnit", "ExternalMaterialGroup", "Plant", "DevicePlant", "AssemblyPlant", "IpoPlant", "AsiOfPlant", "PurchasingGroup", "DivisionCode", "ProfitCenter", "Price", "PriceUnit", "StorageLocationEP", "ToolingBModel", "ToolingBSection", "PoNumber", "StatusInA", "DateIn", "QuotationNumber" });
                    break;
                case RequestType.ToolingB_FG:
                    headers.AddRange(new[] { "CurrentICS", "ItemCode", "EnglishMatDescription", "Level", "Rohs", "MaterialGroup", "BaseUnit", "CodenMid", "DevicePlant", "AssemblyPlant", "IpoPlant", "AsiOfPlant", "Plant", "SalesOrg", "DistributionChannel", "DivisionCode", "TaxTh", "MaterialStatisticsGroup", "AccountAssignment", "GeneralItemCategory", "Availability", "Transportation", "LoadingGroup", "BoiCode", "PurchasingGroup", "ProfitCenter", "PlanDelTime", "SchedMargin", "ValClass", "Price", "PriceUnit", "CostingLotSize", "MrpController", "MinLot", "MaxLot", "FixedLot", "Rounding", "Mtlsm", "Effective", "StorageLoc", "ReceiveStorage", "ProductionSupervisor", "QuotationNumber", "PoNumber", "StatusInA", "ToolingBSection", "DateIn", "ModelName" });
                    break;
                case RequestType.ToolingB_PU:
                    headers.AddRange(new[] { "CurrentICS", "ItemCode", "EnglishMatDescription", "Level", "Rohs", "MaterialGroup", "BaseUnit", "ExternalMaterialGroup", "DivisionCode", "DevicePlant", "AssemblyPlant", "IpoPlant", "AsiOfPlant", "Plant", "SalesOrg", "DistributionChannel" });
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
                    headers.AddRange(new[] { "ItemCode", "Plant", "StorageLocation", "DistributionChannel", "DivisionCode", "AccountAssignment", "ProfitCenter", "BoiCode" });
                    break;
                case RequestType.IPO:
                    headers.AddRange(new[] { "ItemCode", "EnglishMatDescription", "ModelName", "BaseUnit", "Plant", "MaterialGroup", "ExternalMaterialGroup", "DivisionCode", "ProfitCenter", "DistributionChannel", "BoiCode", "PurchasingGroup", "TariffCode", "MrpController", "StorageLocation", "ValClass", "Price", "Planner" });
                    break;
            }
            return headers;
        }
    }
}
