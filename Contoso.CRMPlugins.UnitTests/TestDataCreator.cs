using Contoso.CRMPlugins.Metadata;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace Contoso.CRMPlugins.UnitTests
{
    public static class TestDataCreator
    {
        /// <summary>
        /// Gets the selected installed base line records.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns>The list of Installed Base Line records</returns>
        public static List<Entity> GetSelectedInstalledBaseLineRecords(int count)
        {
            if (count == 0)
            {
                return null;
            }

            var installedBaseLineRecords = new List<Entity>();
            var installedBaseLineRecord = new Entity();
            installedBaseLineRecord.Id = Guid.NewGuid();
            installedBaseLineRecord.Attributes.Add(InstalledBaseLineEntity.BillingModel, new OptionSetValue(4256364));
            installedBaseLineRecord.Attributes.Add(InstalledBaseLineEntity.PriceList, new EntityReference("pricelevel", Guid.NewGuid()));
            installedBaseLineRecord.Attributes.Add(InstalledBaseLineEntity.OrderType, new OptionSetValue(100000));
            installedBaseLineRecord.Attributes.Add(InstalledBaseLineEntity.InstalledBase, new EntityReference(InstalledBaseEntity.EntitySchemaName, Guid.NewGuid()));

            installedBaseLineRecords.Add(installedBaseLineRecord);
            var restOfInstalledBaseLineRecords = GetSelectedInstalledBaseLineRecords(count - 1);
            if (restOfInstalledBaseLineRecords != null)
            {
                installedBaseLineRecords.AddRange(restOfInstalledBaseLineRecords);
            }

            return installedBaseLineRecords;
        }
    }
}
