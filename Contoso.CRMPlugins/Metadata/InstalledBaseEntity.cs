namespace Contoso.CRMPlugins.Metadata
{
    public static class InstalledBaseEntity
    {
        /// <summary>
        /// The entity schema name
        /// </summary>
        public static readonly string EntitySchemaName = "contract";

        /// <summary>
        /// The contract identifier
        /// </summary>
        public static readonly string ContractId = "contractnumber";

        /// <summary>
        /// The customer lookup
        /// </summary>
        public static readonly string Customer = "customerid";

        /// <summary>
        /// The currency lookup
        /// </summary>
        public static readonly string Currency = "transactioncurrencyid";
    }
}
