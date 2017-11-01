using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Fakes;
using System;
using System.Fakes;

namespace Contoso.CRMPlugins.UnitTests
{
    public static class PluginTestsHelper
    {
        /// <summary>
        /// Moles the plugin variables.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="pluginContext">The plugin context.</param>
        /// <param name="organizationService">The organization service.</param>
        /// <param name="stageNumber">The stage number.</param>
        /// <param name="messageName">Name of the message.</param>
        public static void MolePluginVariables(
            StubIServiceProvider serviceProvider,
            StubIPluginExecutionContext pluginContext,
            StubIOrganizationService organizationService,
            int stageNumber,
            string messageName)
        {
            var serviceFactory = new StubIOrganizationServiceFactory();
            var tracingService = new StubITracingService();
            if (serviceProvider != null)
            {
                serviceProvider.GetServiceType = type =>
                {
                    if (type == typeof(IPluginExecutionContext))
                    {
                        return pluginContext;
                    }
                    else if (type == typeof(ITracingService))
                    {
                        return tracingService;
                    }
                    else if (type == typeof(IOrganizationServiceFactory))
                    {
                        return serviceFactory;
                    }

                    return null;
                };
            }

            pluginContext.DepthGet = () => 1;
            pluginContext.UserIdGet = () => new Guid();
            pluginContext.MessageNameGet = () => messageName;
            pluginContext.StageGet = () => stageNumber;
            pluginContext.InitiatingUserIdGet = () => new Guid();
            pluginContext.CorrelationIdGet = () => new Guid();
            pluginContext.PrimaryEntityIdGet = Guid.NewGuid;
            pluginContext.IsInTransactionGet = () => true;
            serviceFactory.CreateOrganizationServiceNullableOfGuid = t1 => organizationService;
            tracingService.TraceStringObjectArray = Trace;
        }

        /// <summary>
        /// Detour method for the CRM Trace method
        /// </summary>
        /// <param name="content">the message to be traced</param>
        /// <param name="value">Object of type object []</param>
        public static void Trace(string content, params object[] value)
        {
        }
    }
}
