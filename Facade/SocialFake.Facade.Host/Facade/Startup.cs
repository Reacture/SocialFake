using System;
using System.Configuration;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Khala.Messaging;
using Khala.Messaging.Azure;
using Microsoft.Owin;
using Microsoft.ServiceBus.Messaging;
using Owin;
using SocialFake.Facade;
using SocialFake.Facade.Controllers;
using SocialFake.Facade.ReadModel;
using SocialFake.Identity.Domain;
using Swashbuckle.Application;
using Swashbuckle.Swagger;

[assembly: OwinStartup(typeof(Startup))]

namespace SocialFake.Facade
{
    public class Startup
    {
        public static string AppName { get; } = typeof(Startup).Assembly.GetName().Name;

        public static string InstanceName { get; } = $"{AppName}-{Guid.NewGuid().ToString("n")}";

        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            IMessageSerializer messageSerializer = new JsonMessageSerializer();
            EventDataSerializer eventSerializer = new EventDataSerializer(messageSerializer);

            var eventHubClient = EventHubClient.CreateFromConnectionString(
                ConfigurationManager.ConnectionStrings["MessagingNamespace"].ConnectionString,
                ConfigurationManager.AppSettings["MessageStream"]);

            IMessageBus messageBus = new EventHubMessageBus(eventHubClient, eventSerializer);

            var eventHandlerHost = new EventProcessorHost(
                InstanceName,
                ConfigurationManager.AppSettings["MessageStream"],
                ConfigurationManager.AppSettings["FacadeConsumerGroup"],
                ConfigurationManager.ConnectionStrings["MessagingNamespace"].ConnectionString,
                ConfigurationManager.ConnectionStrings["MessagingStorage"].ConnectionString);

            IMessageHandler messageHandler = new ReadModelGenerator(() => new SocialFakeDbContext());

            app.UseEventMessageProcessor(
                eventHandlerHost,
                eventSerializer,
                messageHandler);

            IdentityService identityService = new IdentityService(
                new Uri(ConfigurationManager.AppSettings["IdentityHostAddress"]));

            var builder = new ContainerBuilder();
            builder.RegisterInstance(messageBus);
            builder.RegisterType<SocialFakeDbContext>();
            builder.RegisterType<ReadModelFacade>();
            builder.RegisterType<UsersController>();
            builder.RegisterInstance(identityService);
            IContainer container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            config.MapHttpAttributeRoutes();
            config.EnableSwagger(ConfigureSwagger).EnableSwaggerUi();

            app.UseAutofacWebApi(config);
            app.UseWebApi(config);
        }

        private void ConfigureSwagger(SwaggerDocsConfig config)
        {
            Schema GuidSchemaFactory()
            {
                return new Schema
                {
                    type = "string",
                    format = "uuid",
                    example = Guid.NewGuid().ToString()
                };
            }

            config.MapType<Guid>(GuidSchemaFactory);
            config.SingleApiVersion("v1", "Facade API v1");
        }
    }
}
