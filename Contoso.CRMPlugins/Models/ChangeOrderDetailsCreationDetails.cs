namespace Contoso.CRMPlugins.Models
{
    using System.Collections.Generic;
    using Microsoft.Xrm.Sdk;

    /// <summary>
    /// The details required to create change order details
    /// </summary>
    public class ChangeOrderDetailsCreationDetails
    {
        /// <summary>
        /// Gets or sets the commercial order record.
        /// </summary>
        /// <value>
        /// The commercial order record.
        /// </value>
        public Entity CommercialOrderRecord { get; set; }

        /// <summary>
        /// Gets or sets the selected installed base line records.
        /// </summary>
        /// <value>
        /// The selected installed base line records.
        /// </value>
        public List<Entity> SelectedInstalledBaseLineRecords { get; set; }

        /// <summary>
        /// Gets or sets the newly created change order details record.
        /// </summary>
        /// <value>
        /// The newly created change order details record.
        /// </value>
        public Entity NewlyCreatedChangeOrderDetailsRecord { get; set; }
    }
}
