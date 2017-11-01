using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.CRMPlugins.Metadata
{
    public static class InstalledBaseLineEntity
    {
        /// <summary>
        /// The entity schema name
        /// </summary>
        public static readonly string EntitySchemaName = "contractdetail";

        /// <summary>
        /// The order type option set
        /// </summary>
        public static readonly string OrderType = "contoso_ordertypecode";

        /// <summary>
        /// The billing model option set
        /// </summary>
        public static readonly string BillingModel = "contoso_billingmodelcode";

        /// <summary>
        /// The price list
        /// </summary>
        public static readonly string PriceList = "contoso_pricelistid";

        /// <summary>
        /// The installed Base lookup
        /// </summary>
        public static readonly string InstalledBase = "contractid";
    }
}
