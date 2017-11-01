using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.CRMPlugins.Metadata
{
    /// <summary>
    /// The details of the commercial order entity
    /// </summary>
    public static class CommercialOrderEntity
    {
        /// <summary>
        /// The entity schema name
        /// </summary>
        public static readonly string EntitySchemaName = "salesorder";

        /// <summary>
        /// The name
        /// </summary>
        public static readonly string Name = "name";

        /// <summary>
        /// The operation option set
        /// </summary>
        public static readonly string Operation = "contoso_operationcode";

        /// <summary>
        /// The order type option set
        /// </summary>
        public static readonly string OrderType = "contoso_ordertypecode";

        /// <summary>
        /// The customer lookup
        /// </summary>
        public static readonly string Customer = "contoso_customeraccountid";

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

        /// <summary>
        /// The currency
        /// </summary>
        public static readonly string Currency = "transactioncurrencyid";

        /// <summary>
        /// The billing model option set
        /// </summary>
        public static readonly string BillingModel = "contoso_billingmodelcode";

        /// <summary>
        /// The price list lookup
        /// </summary>
        public static readonly string PriceList = "pricelevelid";
    }
}
