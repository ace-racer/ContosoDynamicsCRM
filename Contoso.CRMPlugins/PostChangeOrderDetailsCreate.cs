namespace Contoso.CRMPlugins
{
    using Contoso.CRMPlugins.Metadata;
    using Contoso.CRMPlugins.Models;
    using Microsoft.Xrm.Sdk;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.ServiceModel;

    public class PostChangeOrderDetailsCreate : PluginBase
    {
        /// <summary>
        /// The maximum expected plugin depth
        /// </summary>
        public const int MaximumExpectedPluginDepth = 1;

        /// <summary>
        /// The installed base line ids separator
        /// </summary>
        private const string InstalledBaseLineIdsSeparator = ",";

        /// <summary>
        /// Initializes a new instance of the <see cref="PostChangeOrderDetailsCreate"/> class.
        /// </summary>      
        public PostChangeOrderDetailsCreate()
            : base(typeof(PostChangeOrderDetailsCreate))
        {
        }

        /// <summary>
        /// Gets the details to create and create commercial order record from details in installed base and customer.
        /// </summary>
        /// <param name="orgService">The org service.</param>
        /// <param name="tracingService">The tracing service.</param>
        /// <param name="selectedInstalledBaseLineRecords">The selected installed base line records.</param>
        /// <param name="changeOrderDetailsRecord">The change order details.</param>
        /// <returns>The Commercial order record with the details populated</returns>
        public static Entity GetDetailsToCreateAndCreateCommercialOrderRecordFromDetailsInInstalledBaseAndCustomer(IOrganizationService orgService, ITracingService tracingService, List<Entity> selectedInstalledBaseLineRecords, Entity changeOrderDetailsRecord)
        {
            if (selectedInstalledBaseLineRecords != null && selectedInstalledBaseLineRecords.Count > 0 && changeOrderDetailsRecord != null)
            {
                tracingService.Trace("Getting the Installed Base from the Installed Base Line records that have been selected");

                // Get the Installed Base record from the first Installed Base Line record which has the Installed Base populated
                EntityReference installedBaseEntityReference = null;
                Entity installedBaseLineRecord = null;
                foreach (var selectedInstalledBaseLineRecord in selectedInstalledBaseLineRecords)
                {
                    var currentInstalledBaseEntityReference = selectedInstalledBaseLineRecord.GetAttributeValue<EntityReference>(InstalledBaseLineEntity.InstalledBase);
                    if (currentInstalledBaseEntityReference != null)
                    {
                        installedBaseEntityReference = currentInstalledBaseEntityReference;
                        installedBaseLineRecord = selectedInstalledBaseLineRecord;
                        break;
                    }
                }

                // Get the relevant details from the Installed Base and related records
                if (installedBaseEntityReference != null)
                {
                    tracingService.Trace("There is an Installed base with atleast one of the Installed Base Line records");
                    var installedBaseEntity = orgService.Retrieve(InstalledBaseEntity.EntitySchemaName, installedBaseEntityReference.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(InstalledBaseEntity.ContractId, InstalledBaseEntity.Customer, InstalledBaseEntity.Currency));

                    tracingService.Trace("Trying to retrieve customer details");
                    var customerEntityReference = installedBaseEntity.GetAttributeValue<EntityReference>(InstalledBaseEntity.Customer);
                    Entity customerEntity = null;
                    if (customerEntityReference != null)
                    {
                        customerEntity = orgService.Retrieve(AccountEntity.EntitySchemaName, customerEntityReference.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(AccountEntity.AccountManager, AccountEntity.TechnicalContact, AccountEntity.BillingContact));
                    }
                    else
                    {
                        tracingService.Trace("The selected Installed Base does not have a customer");
                    }

                    return CreateCommercialOrderRecordFromExistingDetails(
                        orgService,
                        tracingService,
                        new CommercialOrderCreationDetails()
                        {
                            InstalledBase = installedBaseEntity,
                            InstalledBaseLine = installedBaseLineRecord,
                            ChangeOrderDetails = changeOrderDetailsRecord,
                            Customer = customerEntity
                        });
                }
                else
                {
                    tracingService.Trace("None of the Installed Base Line records have the Installed Base populated");
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the selected installed base line records from ids.
        /// </summary>
        /// <param name="selectedInstalledBaseLineIds">The selected installed base line ids.</param>
        /// <param name="orgService">The org service.</param>
        /// <param name="tracingService">The tracing service.</param>
        /// <returns>The Installed Base lines based on the IDs passed</returns>
        public static List<Entity> GetSelectedInstalledBaseLineRecordsFromIds(string selectedInstalledBaseLineIds, IOrganizationService orgService, ITracingService tracingService)
        {
            if (!string.IsNullOrWhiteSpace(selectedInstalledBaseLineIds))
            {
                tracingService.Trace("The value of the combined selected installed base line guids: " + selectedInstalledBaseLineIds);
                selectedInstalledBaseLineIds = selectedInstalledBaseLineIds.Replace("\"", string.Empty);
                var separator = new string[] { InstalledBaseLineIdsSeparator };
                var separatedInstalledBaseLineIds = selectedInstalledBaseLineIds.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (separatedInstalledBaseLineIds != null && separatedInstalledBaseLineIds.Length > 0)
                {
                    var installedBaseLineRecords = new List<Entity>();
                    foreach (var separatedInstalledBaseLineId in separatedInstalledBaseLineIds)
                    {
                        var processedSeparatedInstalledBaseLineId = separatedInstalledBaseLineId.Replace(" ", string.Empty);
                        var installedBaseLineRecordGuid = Guid.Empty;
                        if (Guid.TryParse(processedSeparatedInstalledBaseLineId, out installedBaseLineRecordGuid))
                        {
                            var installedBaseLineRecord = orgService.Retrieve(InstalledBaseLineEntity.EntitySchemaName, installedBaseLineRecordGuid, new Microsoft.Xrm.Sdk.Query.ColumnSet(InstalledBaseLineEntity.BillingModel, InstalledBaseLineEntity.PriceList, InstalledBaseLineEntity.OrderType));
                            installedBaseLineRecords.Add(installedBaseLineRecord);
                        }
                        else
                        {
                            tracingService.Trace("The GUID string: " + separatedInstalledBaseLineId + " could not be parsed.");
                        }
                    }

                    return installedBaseLineRecords;
                }
            }

            return null;
        }

        /// <summary>
        /// Creates the change order details from available details.
        /// </summary>
        /// <param name="orgService">The org service.</param>
        /// <param name="tracingService">The tracing service.</param>
        /// <param name="changeOrderDetailsCreationDetails">The change order details creation details.</param>
        /// <returns>The guids of the newly create Change Order Details records.</returns>
        public static List<Guid> CreateChangeOrderDetailsFromAvailableDetails(IOrganizationService orgService, ITracingService tracingService, ChangeOrderDetailsCreationDetails changeOrderDetailsCreationDetails)
        {
            if (changeOrderDetailsCreationDetails != null && changeOrderDetailsCreationDetails.SelectedInstalledBaseLineRecords != null && changeOrderDetailsCreationDetails.SelectedInstalledBaseLineRecords.Count > 0)
            {
                var newlyCreatedChangeOrderDetailsRecordGuids = new List<Guid>();
                foreach (var selectedInstalledBaseLineRecord in changeOrderDetailsCreationDetails.SelectedInstalledBaseLineRecords)
                {
                    var changeOrderDetailsRecord = new Entity(ChangeOrderDetailsEntity.EntitySchemaName);

                    // Populate the Installed Base Line reference
                    changeOrderDetailsRecord[ChangeOrderDetailsEntity.InstalledBaseLine] = new EntityReference(InstalledBaseLineEntity.EntitySchemaName, selectedInstalledBaseLineRecord.Id);
                    tracingService.Trace("ID of installed base line record added: " + selectedInstalledBaseLineRecord.Id);

                    // Get the details from the commercial order record
                    if (changeOrderDetailsCreationDetails.CommercialOrderRecord != null)
                    {
                        changeOrderDetailsRecord[ChangeOrderDetailsEntity.Customer] = changeOrderDetailsCreationDetails.CommercialOrderRecord.GetAttributeValue<EntityReference>(CommercialOrderEntity.Customer);
                        changeOrderDetailsRecord[ChangeOrderDetailsEntity.CommercialOrder] = new EntityReference(CommercialOrderEntity.EntitySchemaName, changeOrderDetailsCreationDetails.CommercialOrderRecord.Id);
                        tracingService.Trace("ID of commercial order record added: " + changeOrderDetailsCreationDetails.CommercialOrderRecord.Id);
                        changeOrderDetailsRecord[ChangeOrderDetailsEntity.PriceList] = changeOrderDetailsCreationDetails.CommercialOrderRecord.GetAttributeValue<EntityReference>(CommercialOrderEntity.PriceList);
                        changeOrderDetailsRecord[ChangeOrderDetailsEntity.Currency] = changeOrderDetailsCreationDetails.CommercialOrderRecord.GetAttributeValue<EntityReference>(CommercialOrderEntity.Currency);
                        changeOrderDetailsRecord[ChangeOrderDetailsEntity.BillingModel] = changeOrderDetailsCreationDetails.CommercialOrderRecord.GetAttributeValue<OptionSetValue>(CommercialOrderEntity.BillingModel);
                    }

                    // Get the details from the newly created Change order details
                    if (changeOrderDetailsCreationDetails.NewlyCreatedChangeOrderDetailsRecord != null)
                    {
                        changeOrderDetailsRecord[ChangeOrderDetailsEntity.Operation] = changeOrderDetailsCreationDetails.NewlyCreatedChangeOrderDetailsRecord.GetAttributeValue<OptionSetValue>(ChangeOrderDetailsEntity.Operation);
                        changeOrderDetailsRecord[ChangeOrderDetailsEntity.OrderType] = changeOrderDetailsCreationDetails.NewlyCreatedChangeOrderDetailsRecord.GetAttributeValue<OptionSetValue>(ChangeOrderDetailsEntity.OrderType);
                        changeOrderDetailsRecord[ChangeOrderDetailsEntity.Product] = changeOrderDetailsCreationDetails.NewlyCreatedChangeOrderDetailsRecord.GetAttributeValue<EntityReference>(ChangeOrderDetailsEntity.Product);
                        changeOrderDetailsRecord[ChangeOrderDetailsEntity.OldCustomer] = changeOrderDetailsCreationDetails.NewlyCreatedChangeOrderDetailsRecord.GetAttributeValue<EntityReference>(ChangeOrderDetailsEntity.OldCustomer);
                        changeOrderDetailsRecord[ChangeOrderDetailsEntity.OldBranch] = changeOrderDetailsCreationDetails.NewlyCreatedChangeOrderDetailsRecord.GetAttributeValue<EntityReference>(ChangeOrderDetailsEntity.OldBranch);
                        changeOrderDetailsRecord[ChangeOrderDetailsEntity.NewBranch] = changeOrderDetailsCreationDetails.NewlyCreatedChangeOrderDetailsRecord.GetAttributeValue<EntityReference>(ChangeOrderDetailsEntity.NewBranch);
                        changeOrderDetailsRecord[ChangeOrderDetailsEntity.ScheduleTerminationDuration] = changeOrderDetailsCreationDetails.NewlyCreatedChangeOrderDetailsRecord.GetAttributeValue<int>(ChangeOrderDetailsEntity.ScheduleTerminationDuration);
                        changeOrderDetailsRecord[ChangeOrderDetailsEntity.ScheduleUnits] = changeOrderDetailsCreationDetails.NewlyCreatedChangeOrderDetailsRecord.GetAttributeValue<bool>(ChangeOrderDetailsEntity.ScheduleUnits);
                        changeOrderDetailsRecord[ChangeOrderDetailsEntity.SalesTerminationReason] = changeOrderDetailsCreationDetails.NewlyCreatedChangeOrderDetailsRecord.GetAttributeValue<OptionSetValue>(ChangeOrderDetailsEntity.SalesTerminationReason);
                        changeOrderDetailsRecord[ChangeOrderDetailsEntity.BillingTerminationReason] = changeOrderDetailsCreationDetails.NewlyCreatedChangeOrderDetailsRecord.GetAttributeValue<OptionSetValue>(ChangeOrderDetailsEntity.BillingTerminationReason);
                        changeOrderDetailsRecord[ChangeOrderDetailsEntity.HardwareTerminationReason] = changeOrderDetailsCreationDetails.NewlyCreatedChangeOrderDetailsRecord.GetAttributeValue<OptionSetValue>(ChangeOrderDetailsEntity.HardwareTerminationReason);
                        changeOrderDetailsRecord[ChangeOrderDetailsEntity.AddOnTerminationReason] = changeOrderDetailsCreationDetails.NewlyCreatedChangeOrderDetailsRecord.GetAttributeValue<OptionSetValue>(ChangeOrderDetailsEntity.AddOnTerminationReason);
                        changeOrderDetailsRecord[ChangeOrderDetailsEntity.SalesSuspensionReason] = changeOrderDetailsCreationDetails.NewlyCreatedChangeOrderDetailsRecord.GetAttributeValue<OptionSetValue>(ChangeOrderDetailsEntity.SalesSuspensionReason);
                        changeOrderDetailsRecord[ChangeOrderDetailsEntity.BillingSuspensionReason] = changeOrderDetailsCreationDetails.NewlyCreatedChangeOrderDetailsRecord.GetAttributeValue<OptionSetValue>(ChangeOrderDetailsEntity.BillingSuspensionReason);
                        changeOrderDetailsRecord[ChangeOrderDetailsEntity.SalesReactivationReason] = changeOrderDetailsCreationDetails.NewlyCreatedChangeOrderDetailsRecord.GetAttributeValue<OptionSetValue>(ChangeOrderDetailsEntity.SalesReactivationReason);
                        changeOrderDetailsRecord[ChangeOrderDetailsEntity.BillingReactivationReason] = changeOrderDetailsCreationDetails.NewlyCreatedChangeOrderDetailsRecord.GetAttributeValue<OptionSetValue>(ChangeOrderDetailsEntity.BillingReactivationReason);
                    }

                    // Populate the technical details
                    changeOrderDetailsRecord[ChangeOrderDetailsEntity.IsCreatedFromInstalledBaseLine] = false;

                    var newlyCreatedChangeOrderDetailsRecordGuid = orgService.Create(changeOrderDetailsRecord);
                    newlyCreatedChangeOrderDetailsRecordGuids.Add(newlyCreatedChangeOrderDetailsRecordGuid);
                }

                return newlyCreatedChangeOrderDetailsRecordGuids;
            }

            return null;
        }

        /// <summary>
        /// Main entry point for he business logic that the plug-in is to execute.
        /// </summary>
        /// <param name="localcontext">The <see cref="LocalPluginContext"/> which contains the
        /// <see cref="IPluginExecutionContext"/>,
        /// <see cref="IOrganizationService"/>
        /// and <see cref="ITracingService"/>
        /// </param>
        /// <remarks>
        /// For improved performance, Microsoft Dynamics 365 caches plug-in instances.
        /// The plug-in's Execute method should be written to be stateless as the constructor
        /// is not called for every invocation of the plug-in. Also, multiple system threads
        /// could execute the plug-in at the same time. All per invocation state information
        /// is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            if (localcontext == null)
            {
                throw new InvalidPluginExecutionException("localcontext");
            }

            IOrganizationService orgService = localcontext.OrganizationService;
            IPluginExecutionContext context = localcontext.PluginExecutionContext;
            ITracingService tracingService = localcontext.TracingService;
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                try
                {
                    if (tracingService == null)
                    {
                        return;
                    }

                    if (orgService == null)
                    {
                        throw new ArgumentNullException("orgService");
                    }

                    // The plugin is not expected to be called from other server side operations. If it is expected then change the value accordingly.
                    if (context.Depth > MaximumExpectedPluginDepth)
                    {
                        tracingService.Trace("The depth for the plugin is more than 1 and exiting.");
                        return;
                    }

                    tracingService.Trace("Started with the execution of the PostChangeOrderDetailsCreate plugin");
                    var currentChangeOrderDetailsRecord = (Entity)context.InputParameters["Target"];
                    var isCurrentChangeOrderDetailsCreatedFromInstalledBaseLine = currentChangeOrderDetailsRecord.GetAttributeValue<bool>(ChangeOrderDetailsEntity.IsCreatedFromInstalledBaseLine);
                    if (isCurrentChangeOrderDetailsCreatedFromInstalledBaseLine)
                    {
                        tracingService.Trace("The current change order is created from installed base line grid");
                        var selectedInstalledBaseLineGuids = currentChangeOrderDetailsRecord.GetAttributeValue<string>(ChangeOrderDetailsEntity.SelectedInstalledBaseLineGuids);
                        var selectedInstalledBaseLineRecords = GetSelectedInstalledBaseLineRecordsFromIds(selectedInstalledBaseLineGuids, orgService, tracingService);
                        var commercialOrderEntity = GetDetailsToCreateAndCreateCommercialOrderRecordFromDetailsInInstalledBaseAndCustomer(orgService, tracingService, selectedInstalledBaseLineRecords, currentChangeOrderDetailsRecord);
                        if (commercialOrderEntity != null)
                        {
                            tracingService.Trace("Trying to create the change order details records");
                            var changeOrderDetailsRecordGuids = CreateChangeOrderDetailsFromAvailableDetails(
                                orgService,
                                tracingService,
                                new ChangeOrderDetailsCreationDetails()
                                {
                                    NewlyCreatedChangeOrderDetailsRecord = currentChangeOrderDetailsRecord,
                                    SelectedInstalledBaseLineRecords = selectedInstalledBaseLineRecords,
                                    CommercialOrderRecord = commercialOrderEntity
                                });

                            tracingService.Trace("There were: " + changeOrderDetailsRecordGuids.Count + " change order detail records created");

                            orgService.Delete(ChangeOrderDetailsEntity.EntitySchemaName, currentChangeOrderDetailsRecord.Id);
                            tracingService.Trace("Deleted the change order details record that was created to trigger Commercial Order and Commercial Order Line creation from the Installed Base Line");
                            tracingService.Trace("Plugin execution completed successfully...");
                        }
                        else
                        {
                            tracingService.Trace("The commercial order record was not created and so not creating the Change Order details");
                        }
                    }
                    else
                    {
                        tracingService.Trace("The current change order is not created from installed base line grid and so logic will not be executed");
                    }
                }
                catch (FaultException<OrganizationServiceFault> fault)
                {
                    tracingService.Trace(string.Format(
                                                        CultureInfo.CurrentCulture,
                                                        "Error in Plugin : PostChangeOrderDetailsCreate. Entity Name : {0} Error Message : {1}",
                                                        context.PrimaryEntityName,
                                                        fault.Detail.InnerFault != null ? fault.Detail.InnerFault.Message : fault.Detail.Message));
                    throw new InvalidPluginExecutionException(OperationStatus.Failed, fault.Message);
                }
                catch (Exception ex)
                {
                    tracingService.Trace(string.Format(
                                                        CultureInfo.CurrentCulture,
                                                        "Error in Plugin : PostChangeOrderDetailsCreate. Entity Name : {0} Error Message : {1}",
                                                        context.PrimaryEntityName,
                                                        ex.Message));
                    throw new InvalidPluginExecutionException(OperationStatus.Failed, ex.Message);
                }
            }
        }

        /// <summary>
        /// Creates the commercial order record from existing details.
        /// </summary>
        /// <param name="orgService">The org service.</param>
        /// <param name="tracingService">The tracing service.</param>
        /// <param name="commercialOrderCreationDetails">The commercial order creation details.</param>
        /// <returns>The Commercial order record with all details populated</returns>
        private static Entity CreateCommercialOrderRecordFromExistingDetails(IOrganizationService orgService, ITracingService tracingService, CommercialOrderCreationDetails commercialOrderCreationDetails)
        {
            if (commercialOrderCreationDetails != null)
            {
                tracingService.Trace("Creating the commercial order record from the details obtained");
                var commercialOrderRecord = new Entity(CommercialOrderEntity.EntitySchemaName);

                // Get the required details from the installed base
                if (commercialOrderCreationDetails.InstalledBase != null)
                {
                    commercialOrderRecord[CommercialOrderEntity.Name] = commercialOrderCreationDetails.InstalledBase.GetAttributeValue<string>(InstalledBaseEntity.ContractId);
                    commercialOrderRecord[CommercialOrderEntity.Customer] = commercialOrderCreationDetails.InstalledBase.GetAttributeValue<EntityReference>(InstalledBaseEntity.Customer);
                    commercialOrderRecord[CommercialOrderEntity.Currency] = commercialOrderCreationDetails.InstalledBase.GetAttributeValue<EntityReference>(InstalledBaseEntity.Currency);
                }

                // Get the required details from the customer
                if (commercialOrderCreationDetails.Customer != null)
                {
                    commercialOrderRecord[CommercialOrderEntity.AccountManager] = commercialOrderCreationDetails.Customer.GetAttributeValue<EntityReference>(AccountEntity.AccountManager);
                    commercialOrderRecord[CommercialOrderEntity.TechnicalContact] = commercialOrderCreationDetails.Customer.GetAttributeValue<EntityReference>(AccountEntity.TechnicalContact);
                    commercialOrderRecord[CommercialOrderEntity.BillingContact] = commercialOrderCreationDetails.Customer.GetAttributeValue<EntityReference>(AccountEntity.BillingContact);
                }

                // Get the required details from the Installed Base Line
                if (commercialOrderCreationDetails.InstalledBaseLine != null)
                {
                    commercialOrderRecord[CommercialOrderEntity.BillingModel] = commercialOrderCreationDetails.InstalledBaseLine.GetAttributeValue<OptionSetValue>(InstalledBaseLineEntity.BillingModel);
                    commercialOrderRecord[CommercialOrderEntity.PriceList] = commercialOrderCreationDetails.InstalledBaseLine.GetAttributeValue<EntityReference>(InstalledBaseLineEntity.PriceList);
                }

                // Get the details from the newly created Change Order
                if (commercialOrderCreationDetails.ChangeOrderDetails != null)
                {
                    commercialOrderRecord[CommercialOrderEntity.Operation] = commercialOrderCreationDetails.ChangeOrderDetails.GetAttributeValue<OptionSetValue>(ChangeOrderDetailsEntity.Operation);
                    commercialOrderRecord[CommercialOrderEntity.OrderType] = commercialOrderCreationDetails.ChangeOrderDetails.GetAttributeValue<OptionSetValue>(ChangeOrderDetailsEntity.OrderType);
                }

                commercialOrderRecord.Id = orgService.Create(commercialOrderRecord);
                tracingService.Trace("Commercial order is created successfully with GUID: " + commercialOrderRecord.Id);
                return commercialOrderRecord;
            }

            return null;
        }
    }
}
