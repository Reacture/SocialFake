using System;
using System.Configuration;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Khala.EventSourcing;
using Khala.EventSourcing.Sql;
using Khala.Messaging;
using Khala.Messaging.Azure;
using Microsoft.ServiceBus.Messaging;
using Owin;
using SocialFake.Identity.Domain;
using SocialFake.Identity.Domain.Controllers;
using Swashbuckle.Application;
using Swashbuckle.Swagger;

[assembly: Microsoft.Owin.OwinStartup(typeof(Startup))]

namespace SocialFake.Identity.Domain
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

            var eventHandlerHost = new EventProcessorHost(
                InstanceName,
                ConfigurationManager.AppSettings["MessageStream"],
                ConfigurationManager.AppSettings["DomainConsumerGroup"],
                ConfigurationManager.ConnectionStrings["MessagingNamespace"].ConnectionString,
                ConfigurationManager.ConnectionStrings["MessagingStorage"].ConnectionString);

            IMessageBus messageBus = new EventHubMessageBus(eventHubClient, eventSerializer);

            IEventSourcedRepository<User> repository = new SqlEventSourcedRepository<User>(
                () => new EventStoreDbContext("SocialFakeEventStore"),
                messageSerializer,
                messageBus,
                User.Factory);

            IMessageHandler messageHandler = new UserCommandHandler(new GrootPasswordHasher(), repository);

            app.UseEventMessageProcessor(
                eventHandlerHost,
                eventSerializer,
                messageHandler);

            var builder = new ContainerBuilder();
            builder.RegisterInstance(messageBus);
            builder.RegisterInstance(messageHandler);
            builder.RegisterType<CommandsController>();
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
            config.SingleApiVersion("v1", "Identity Domain API v1");
        }
    }
}
