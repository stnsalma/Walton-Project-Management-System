﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProjectManagement.DAL.DbModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class RBSYNERGYEntities : DbContext
    {
        public RBSYNERGYEntities()
            : base("name=RBSYNERGYEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<aspnet_Applications> aspnet_Applications { get; set; }
        public virtual DbSet<aspnet_Membership> aspnet_Membership { get; set; }
        public virtual DbSet<aspnet_Paths> aspnet_Paths { get; set; }
        public virtual DbSet<aspnet_PersonalizationAllUsers> aspnet_PersonalizationAllUsers { get; set; }
        public virtual DbSet<aspnet_PersonalizationPerUser> aspnet_PersonalizationPerUser { get; set; }
        public virtual DbSet<aspnet_Profile> aspnet_Profile { get; set; }
        public virtual DbSet<aspnet_Roles> aspnet_Roles { get; set; }
        public virtual DbSet<aspnet_SchemaVersions> aspnet_SchemaVersions { get; set; }
        public virtual DbSet<aspnet_Users> aspnet_Users { get; set; }
        public virtual DbSet<aspnet_WebEvent_Events> aspnet_WebEvent_Events { get; set; }
        public virtual DbSet<DispatchToMarketRemainingStock> DispatchToMarketRemainingStocks { get; set; }
        public virtual DbSet<DistributorNetwork> DistributorNetworks { get; set; }
        public virtual DbSet<FifteenDaysWiseMailTable> FifteenDaysWiseMailTables { get; set; }
        public virtual DbSet<FifteenDaysWiseMailTableNew> FifteenDaysWiseMailTableNews { get; set; }
        public virtual DbSet<ModelWiseLock> ModelWiseLocks { get; set; }
        public virtual DbSet<ModelWiseServiceQuantityInDay> ModelWiseServiceQuantityInDays { get; set; }
        public virtual DbSet<ModelWiseServiceQuantityInDaysByOrder> ModelWiseServiceQuantityInDaysByOrders { get; set; }
        public virtual DbSet<NonGradedImeiInventory> NonGradedImeiInventories { get; set; }
        public virtual DbSet<PlazaSale> PlazaSales { get; set; }
        public virtual DbSet<PlazaSalesAndActivationRate> PlazaSalesAndActivationRates { get; set; }
        public virtual DbSet<RemainingMarketDetailsWithImei> RemainingMarketDetailsWithImeis { get; set; }
        public virtual DbSet<SalesForecastingReport> SalesForecastingReports { get; set; }
        public virtual DbSet<ServiceToSalesRatio> ServiceToSalesRatios { get; set; }
        public virtual DbSet<SIMInfo> SIMInfoes { get; set; }
        public virtual DbSet<SMSDetail> SMSDetails { get; set; }
        public virtual DbSet<SMSGroup> SMSGroups { get; set; }
        public virtual DbSet<SMSMaster> SMSMasters { get; set; }
        public virtual DbSet<SMSMember> SMSMembers { get; set; }
        public virtual DbSet<SMSRoleDetail> SMSRoleDetails { get; set; }
        public virtual DbSet<SMSRoleMaster> SMSRoleMasters { get; set; }
        public virtual DbSet<SparePartsEmail> SparePartsEmails { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<tblBarCodeInv> tblBarCodeInvs { get; set; }
        public virtual DbSet<tblCashIn> tblCashIns { get; set; }
        public virtual DbSet<tblCashOut> tblCashOuts { get; set; }
        public virtual DbSet<tblcdmaGSMMatching> tblcdmaGSMMatchings { get; set; }
        public virtual DbSet<tblCellPhoneDepriciationPrice> tblCellPhoneDepriciationPrices { get; set; }
        public virtual DbSet<tblCellPhonePricing> tblCellPhonePricings { get; set; }
        public virtual DbSet<tblCompanyInfo> tblCompanyInfoes { get; set; }
        public virtual DbSet<tblDailyClosingStockDetail> tblDailyClosingStockDetails { get; set; }
        public virtual DbSet<tblDateTimeDimantion> tblDateTimeDimantions { get; set; }
        public virtual DbSet<tblDealerDistributionDetail> tblDealerDistributionDetails { get; set; }
        public virtual DbSet<tblDealerDistributionLog> tblDealerDistributionLogs { get; set; }
        public virtual DbSet<tblDealerInfo> tblDealerInfoes { get; set; }
        public virtual DbSet<tblMargineOfError> tblMargineOfErrors { get; set; }
        public virtual DbSet<tblOffer> tblOffers { get; set; }
        public virtual DbSet<tblProductMaster> tblProductMasters { get; set; }
        public virtual DbSet<tblProductRegistration> tblProductRegistrations { get; set; }
        public virtual DbSet<tblRmReturn> tblRmReturns { get; set; }
        public virtual DbSet<tblSalesPoint> tblSalesPoints { get; set; }
        public virtual DbSet<tblSalesPointDailyStock> tblSalesPointDailyStocks { get; set; }
        public virtual DbSet<tblSalesPointTransHead> tblSalesPointTransHeads { get; set; }
        public virtual DbSet<tblSMSInbox> tblSMSInboxes { get; set; }
        public virtual DbSet<tblTransectionHead> tblTransectionHeads { get; set; }
        public virtual DbSet<TsoWiseRegistration> TsoWiseRegistrations { get; set; }
        public virtual DbSet<AfterServiceReplacement> AfterServiceReplacements { get; set; }
        public virtual DbSet<AfterServiceReplacementLog> AfterServiceReplacementLogs { get; set; }
        public virtual DbSet<ApprovalMaster> ApprovalMasters { get; set; }
        public virtual DbSet<FaultySparePartsDetail> FaultySparePartsDetails { get; set; }
        public virtual DbSet<IMEIReplacementMaster> IMEIReplacementMasters { get; set; }
        public virtual DbSet<ManagementApprovalLog> ManagementApprovalLogs { get; set; }
        public virtual DbSet<PrimaryRecomLog> PrimaryRecomLogs { get; set; }
        public virtual DbSet<ReplacementLog> ReplacementLogs { get; set; }
        public virtual DbSet<SatisfactionReplacementHandsetMovement> SatisfactionReplacementHandsetMovements { get; set; }
        public virtual DbSet<tblDealerSale> tblDealerSales { get; set; }
        public virtual DbSet<tblDisplayProduct> tblDisplayProducts { get; set; }
        public virtual DbSet<tblStockReconciliation> tblStockReconciliations { get; set; }
        public virtual DbSet<WareHouseApproval> WareHouseApprovals { get; set; }
        public virtual DbSet<WareHouseCorporateStoreLog> WareHouseCorporateStoreLogs { get; set; }
        public virtual DbSet<WastageManagementInventory> WastageManagementInventories { get; set; }
        public virtual DbSet<WastageManagementMasterLog> WastageManagementMasterLogs { get; set; }
        public virtual DbSet<Model> Models { get; set; }
        public virtual DbSet<RemainingMarketDetail> RemainingMarketDetails { get; set; }
        public virtual DbSet<tblSupplierModelInfo> tblSupplierModelInfoes { get; set; }
        public virtual DbSet<TimeTable> TimeTables { get; set; }
        public virtual DbSet<tblActivatedInvoiceValueVsSpareValue> tblActivatedInvoiceValueVsSpareValues { get; set; }
        public virtual DbSet<OrderWiseDailyServiceToSalesRatio> OrderWiseDailyServiceToSalesRatios { get; set; }
        public virtual DbSet<ProjectOrderPerformanceSum> ProjectOrderPerformanceSums { get; set; }
        public virtual DbSet<ProjectOrderProblemDetail> ProjectOrderProblemDetails { get; set; }
        public virtual DbSet<ProjectOrderSpareUsedDetail> ProjectOrderSpareUsedDetails { get; set; }
    }
}
