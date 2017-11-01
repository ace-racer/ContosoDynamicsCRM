namespace Contoso.CRMPlugins.Metadata
{
    /// <summary>
    /// The account entity fields
    /// </summary>
    public static class AccountEntity
    {
        /// <summary>
        /// The entity schema name
        /// </summary>
        public static readonly string EntitySchemaName = "account";

        /// <summary>
        /// The account manager lookup
        /// </summary>
        public static readonly string AccountManager = "contoso_accountmanagersystemuserid";

        /// <summary>
        /// The technical contact lookup
        /// </summary>
        public static readonly string TechnicalContact = "contoso_technicalcontactid";

        /// <summary>
        /// The billing contact lookup
        /// </summary>
        public static readonly string BillingContact = "contoso_billingcontactid";
    }
}
