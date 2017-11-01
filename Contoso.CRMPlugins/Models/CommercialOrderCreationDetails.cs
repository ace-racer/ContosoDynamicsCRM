namespace Contoso.CRMPlugins.Models
{
    using Microsoft.Xrm.Sdk;

    /// <summary>
    /// The details required to create commercial order
    /// </summary>
    public class CommercialOrderCreationDetails
    {
        /// <summary>
        /// Gets or sets the installed base.
        /// </summary>
        /// <value>
        /// The installed base.
        /// </value>
        public Entity InstalledBase { get; set; }

        /// <summary>
        /// Gets or sets the customer.
        /// </summary>
        /// <value>
        /// The customer.
        /// </value>
        public Entity Customer { get; set; }

        /// <summary>
        /// Gets or sets the installed base line.
        /// </summary>
        /// <value>
        /// The installed base line.
        /// </value>
        public Entity InstalledBaseLine { get; set; }

        /// <summary>
        /// Gets or sets the change order details.
        /// </summary>
        /// <value>
        /// The change order details.
        /// </value>
        public Entity ChangeOrderDetails { get; set; }
    }
}
