using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Fakes;
using Contoso.CRMPlugins.Metadata;
using System.Fakes;

namespace Contoso.CRMPlugins.UnitTests
{
    [TestClass]
    public class PostChangeOrderDetailsCreateTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException))]
        public void WhenOrganizationSeviceIsNullThenAnExceptionIsThrown()
        {
            var postChangeOrderDetailsCreate = new PostChangeOrderDetailsCreate();
            var serviceProvider = new StubIServiceProvider();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            PluginTestsHelper.MolePluginVariables(serviceProvider, pluginContext, null, 40, "Create");

            var entityAttributes = new Dictionary<string, object>();
            Entity changeOrderDetailsEntity = new Entity();
            changeOrderDetailsEntity.Attributes.Add("name", "Test");
            pluginContext.InputParametersGet = delegate
            {
                ParameterCollection parameterCollection = new ParameterCollection();
                parameterCollection.Add("Target", changeOrderDetailsEntity);
                return parameterCollection;
            };

            postChangeOrderDetailsCreate.Execute(serviceProvider);

            // This should get executed only if an exception is not thrown by the above line
            Assert.Fail();
        }

        [TestMethod]
        public void WhenChangeOrderIsCreatedFromInstalledBaseLineAndCustomerDetailsArePresentThenACommercialOrderIsCreated()
        {
            var postChangeOrderDetailsCreate = new PostChangeOrderDetailsCreate();
            var serviceProvider = new StubIServiceProvider();
            var tracingService = new StubITracingService();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            PluginTestsHelper.MolePluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create");

            var entityAttributes = new Dictionary<string, object>();
            Entity changeOrderDetailsEntity = new Entity();
            changeOrderDetailsEntity.Attributes.Add(ChangeOrderDetailsEntity.IsCreatedFromInstalledBaseLine, true);
            changeOrderDetailsEntity.Attributes.Add(ChangeOrderDetailsEntity.SelectedInstalledBaseLineGuids, "\"B344FF0E - 2567 - E711 - 81A5 - 000D3A275BAD\",\"BB5F3570 - 1466 - E711 - 81A3 - 000D3A275BAD\"");

            pluginContext.InputParametersGet = delegate
            {
                ParameterCollection parameterCollection = new ParameterCollection();
                parameterCollection.Add("Target", changeOrderDetailsEntity);
                return parameterCollection;
            };

            // Installed Base Line and Account
            organizationService.RetrieveStringGuidColumnSet = (entityName, recordId, columns) =>
            {
                if (string.Compare(entityName, InstalledBaseLineEntity.EntitySchemaName, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    var installedBaseLineEntity = new Entity();
                    installedBaseLineEntity.Id = Guid.NewGuid();
                    installedBaseLineEntity.Attributes.Add(InstalledBaseLineEntity.BillingModel, new OptionSetValue(4256364));
                    installedBaseLineEntity.Attributes.Add(InstalledBaseLineEntity.PriceList, new EntityReference("pricelevel", Guid.NewGuid()));
                    return installedBaseLineEntity;
                }
                else if (string.Compare(entityName, InstalledBaseEntity.EntitySchemaName, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    var installedBaseEntity = new Entity();
                    installedBaseEntity.Id = Guid.NewGuid();
                    installedBaseEntity.Attributes.Add(InstalledBaseEntity.ContractId, "ContractABC");
                    installedBaseEntity.Attributes.Add(InstalledBaseEntity.Customer, new EntityReference(AccountEntity.EntitySchemaName, Guid.NewGuid()));
                    installedBaseEntity.Attributes.Add(InstalledBaseEntity.Currency, new EntityReference("transactioncurrencyid", Guid.NewGuid()));
                    return installedBaseEntity;
                }
                else if (string.Compare(entityName, AccountEntity.EntitySchemaName, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    var accountEntity = new Entity();
                    accountEntity.Id = Guid.NewGuid();
                    accountEntity.Attributes.Add(AccountEntity.AccountManager, new EntityReference("systemuser", Guid.NewGuid()));
                    accountEntity.Attributes.Add(AccountEntity.TechnicalContact, new EntityReference("contact", Guid.NewGuid()));
                    accountEntity.Attributes.Add(AccountEntity.BillingContact, new EntityReference("contact", Guid.NewGuid()));
                    return accountEntity;
                }

                return null;
            };

            organizationService.CreateEntity = (e) =>
            {
                return Guid.NewGuid();
            };

            var selectedInstalledBaseLineEntities = TestDataCreator.GetSelectedInstalledBaseLineRecords(5);
            var commercialOrderRecord = PostChangeOrderDetailsCreate.GetDetailsToCreateAndCreateCommercialOrderRecordFromDetailsInInstalledBaseAndCustomer(organizationService, tracingService, selectedInstalledBaseLineEntities, changeOrderDetailsEntity);
            Assert.IsNotNull(commercialOrderRecord);
            Assert.IsTrue(commercialOrderRecord.Id != Guid.Empty);
        }

        [TestMethod]
        public void WhenSelectedInstalledBaseLinesGuidStringIsPopulatedThenInstalledBaseLineRecordsAreRetrieved()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void WhenAllChangeOrderDetailsCreationDetailsArePresentChangeOrderDetailsAreCreated()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void WhenSomeChangeOrderDetailsCreationDetailsArePresentChangeOrderDetailsAreCreated()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void WhenDetailsArePresentCommercialOrderAndChangeOrderDetailsAreCreated()
        {
            var postChangeOrderDetailsCreate = new PostChangeOrderDetailsCreate();
            var serviceProvider = new StubIServiceProvider();
            var tracingService = new StubITracingService();
            var pluginContext = new StubIPluginExecutionContext();
            var organizationService = new StubIOrganizationService();
            PluginTestsHelper.MolePluginVariables(serviceProvider, pluginContext, organizationService, 40, "Create");

            var entityAttributes = new Dictionary<string, object>();
            Entity changeOrderDetailsEntity = new Entity();
            changeOrderDetailsEntity.Attributes.Add(ChangeOrderDetailsEntity.IsCreatedFromInstalledBaseLine, true);
            changeOrderDetailsEntity.Attributes.Add(ChangeOrderDetailsEntity.SelectedInstalledBaseLineGuids, "\"B344FF0E - 2567 - E711 - 81A5 - 000D3A275BAD\",\"BB5F3570 - 1466 - E711 - 81A3 - 000D3A275BAD\"");
            changeOrderDetailsEntity.Attributes.Add(ChangeOrderDetailsEntity.Operation, new OptionSetValue(10000));
            changeOrderDetailsEntity.Attributes.Add(ChangeOrderDetailsEntity.OrderType, new OptionSetValue(10000));
            changeOrderDetailsEntity.Attributes.Add(ChangeOrderDetailsEntity.Product, new EntityReference("product", Guid.NewGuid()));

            pluginContext.InputParametersGet = delegate
            {
                ParameterCollection parameterCollection = new ParameterCollection();
                parameterCollection.Add("Target", changeOrderDetailsEntity);
                return parameterCollection;
            };

            // Installed Base Line and Account
            organizationService.RetrieveStringGuidColumnSet = (entityName, recordId, columns) =>
            {
                if (string.Compare(entityName, InstalledBaseLineEntity.EntitySchemaName, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    var installedBaseLineEntity = new Entity();
                    installedBaseLineEntity.Id = Guid.NewGuid();
                    installedBaseLineEntity.Attributes.Add(InstalledBaseLineEntity.BillingModel, new OptionSetValue(4256364));
                    installedBaseLineEntity.Attributes.Add(InstalledBaseLineEntity.PriceList, new EntityReference("pricelevel", Guid.NewGuid()));
                    installedBaseLineEntity.Attributes.Add(InstalledBaseLineEntity.InstalledBase, new EntityReference(InstalledBaseEntity.EntitySchemaName, Guid.NewGuid()));
                    return installedBaseLineEntity;
                }
                else if (string.Compare(entityName, InstalledBaseEntity.EntitySchemaName, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    var installedBaseEntity = new Entity();
                    installedBaseEntity.Id = Guid.NewGuid();
                    installedBaseEntity.Attributes.Add(InstalledBaseEntity.ContractId, "ContractABC");
                    installedBaseEntity.Attributes.Add(InstalledBaseEntity.Customer, new EntityReference(AccountEntity.EntitySchemaName, Guid.NewGuid()));
                    installedBaseEntity.Attributes.Add(InstalledBaseEntity.Currency, new EntityReference("transactioncurrencyid", Guid.NewGuid()));
                    return installedBaseEntity;
                }
                else if (string.Compare(entityName, AccountEntity.EntitySchemaName, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    var accountEntity = new Entity();
                    accountEntity.Id = Guid.NewGuid();
                    accountEntity.Attributes.Add(AccountEntity.AccountManager, new EntityReference("systemuser", Guid.NewGuid()));
                    accountEntity.Attributes.Add(AccountEntity.TechnicalContact, new EntityReference("contact", Guid.NewGuid()));
                    accountEntity.Attributes.Add(AccountEntity.BillingContact, new EntityReference("contact", Guid.NewGuid()));
                    return accountEntity;
                }

                return null;
            };

            organizationService.CreateEntity = (e) =>
            {
                return Guid.NewGuid();
            };

            postChangeOrderDetailsCreate.Execute(serviceProvider);
        }
    }
}
