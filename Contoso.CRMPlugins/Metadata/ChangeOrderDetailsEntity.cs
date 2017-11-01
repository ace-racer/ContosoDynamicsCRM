namespace Contoso.CRMPlugins.Metadata
{
    /// <summary>
    /// The details of the change order details entity
    /// </summary>
    public static class ChangeOrderDetailsEntity
    {
        /// <summary>
        /// The entity schema name
        /// </summary>
        public static readonly string EntitySchemaName = "contoso_salesordertypedetails";

        /// <summary>
        /// The is created from installed base line
        /// </summary>
        public static readonly string IsCreatedFromInstalledBaseLine = "contoso_iscreatedfrominstalledbaseline";

        /// <summary>
        /// The selected installed base line guids
        /// </summary>
        public static readonly string SelectedInstalledBaseLineGuids = "contoso_selectedinstalledbaselinerecordguids";

        /// <summary>
        /// The customer lookup
        /// </summary>
        public static readonly string Customer = "contoso_customerid";

        /// <summary>
        /// The order type option set
        /// </summary>
        public static readonly string OrderType = "contoso_ordertypecode";

        /// <summary>
        /// The commercial order lookup
        /// </summary>
        public static readonly string CommercialOrder = "contoso_salesorderid";

        /// <summary>
        /// The price list lookup
        /// </summary>
        public static readonly string PriceList = "contoso_pricelevelid";

        /// <summary>
        /// The currency lookup
        /// </summary>
        public static readonly string Currency = "transactioncurrencyid";

        /// <summary>
        /// The billing model option set
        /// </summary>
        public static readonly string BillingModel = "contoso_billingmodelcode";

        /// <summary>
        /// The operation
        /// </summary>
        public static readonly string Operation = "contoso_operationcode";

        /// <summary>
        /// The installed base line lookup
        /// </summary>
        public static readonly string InstalledBaseLine = "contoso_installedbaselineid";

        /// <summary>
        /// The product lookup
        /// </summary>
        public static readonly string Product = "contoso_productid";

        /// <summary>
        /// The quantity
        /// </summary>
        public static readonly string Quantity = "contoso_quantity";

        /// <summary>
        /// The old customer lookup
        /// </summary>
        public static readonly string OldCustomer = "contoso_reallocatedcustomerid";

        /// <summary>
        /// The old branch lookup
        /// </summary>
        public static readonly string OldBranch = "contoso_oldbranchid";

        /// <summary>
        /// The new branch lookup
        /// </summary>
        public static readonly string NewBranch = "contoso_branchid";

        /// <summary>
        /// The add on expiration date
        /// </summary>
        public static readonly string AddOnExpirationDate = "contoso_addonexpirationdate";

        /// <summary>
        /// The schedule termination duration
        /// </summary>
        public static readonly string ScheduleTerminationDuration = "contoso_scheduleterminationduration";

        /// <summary>
        /// The schedule units two option set
        /// </summary>
        public static readonly string ScheduleUnits = "contoso_scheduleunitsboolean";

        /// <summary>
        /// The sales termination reason option set
        /// </summary>
        public static readonly string SalesTerminationReason = "contoso_salesterminationreasoncode";

        /// <summary>
        /// The billing termination reason option set
        /// </summary>
        public static readonly string BillingTerminationReason = "contoso_billingterminationreasoncode";

        /// <summary>
        /// The hardware termination reason option set
        /// </summary>
        public static readonly string HardwareTerminationReason = "contoso_hardwareterminationreasoncode";

        /// <summary>
        /// The add on termination reason option set
        /// </summary>
        public static readonly string AddOnTerminationReason = "contoso_addonterminationreasoncode";

        /// <summary>
        /// The sales suspension reason option set
        /// </summary>
        public static readonly string SalesSuspensionReason = "contoso_salessuspensionreasoncode";

        /// <summary>
        /// The billing suspension reason option set
        /// </summary>
        public static readonly string BillingSuspensionReason = "contoso_billingsuspensionreasoncode";

        /// <summary>
        /// The sales reactivation reason option set
        /// </summary>
        public static readonly string SalesReactivationReason = "contoso_salesreactivationreasoncode";

        /// <summary>
        /// The billing reactivation reason option set
        /// </summary>
        public static readonly string BillingReactivationReason = "contoso_billingreactivationreasoncode";
    }
}
