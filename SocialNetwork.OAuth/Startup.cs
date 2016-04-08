using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using IdentityServer3.EntityFramework;
using Microsoft.Owin;
using Owin;
using SocialNetwork.Data.Repositories;

[assembly: OwinStartup(typeof(SocialNetwork.OAuth.Startup))]

namespace SocialNetwork.OAuth
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var entityFrameworkOptions = new EntityFrameworkServiceOptions
            {
                ConnectionString = 
                    ConfigurationManager.ConnectionStrings["SocialNetwork.Idsvr"].ConnectionString
            };

            var inMemoryManager = new InMemoryManager();
            SetupClients(inMemoryManager.GetClients(), entityFrameworkOptions);
            SetupScopes(inMemoryManager.GetScopes(), entityFrameworkOptions);
            
            var userRepository = new UserRepository(
                () => new SqlConnection(ConfigurationManager.ConnectionStrings["SocialNetwork"].ConnectionString)
            );

            var viewServiceOptions = new DefaultViewServiceOptions();
            viewServiceOptions.Stylesheets.Add("/css/site.css");

            var factory = new IdentityServerServiceFactory();
            factory.RegisterConfigurationServices(entityFrameworkOptions);
            factory.RegisterOperationalServices(entityFrameworkOptions);
            factory.UserService = new Registration<IUserService>(
                typeof(SocialNetworkUserService));
            factory.Register(new Registration<IUserRepository>(userRepository));
            factory.ConfigureDefaultViewService(viewServiceOptions);

            new TokenCleanup(entityFrameworkOptions, 1).Start();

            var certificate = Convert.FromBase64String(ConfigurationManager.AppSettings["SigningCertificate"]);
            
            var options = new IdentityServerOptions
            {
                SiteName = "OAuth is fun!",
                SigningCertificate = new X509Certificate2(certificate, ConfigurationManager.AppSettings["SigningCertificatePassword"]),
                RequireSsl = false, // DO NOT DO THIS IN 
                Factory = factory,
            };

            app.UseIdentityServer(options);
        }

        public void SetupClients(IEnumerable<Client> clients,
                                    EntityFrameworkServiceOptions options)
        {
            using (var context =
                new ClientConfigurationDbContext(options.ConnectionString, 
                                                options.Schema))
            {
                if (context.Clients.Any()) return;

                foreach (var client in clients)
                {
                    context.Clients.Add(client.ToEntity());
                }

                context.SaveChanges();
            }
        }

        public void SetupScopes(IEnumerable<Scope> scopes,
                                 EntityFrameworkServiceOptions options)
        {
            using (var context =
                new ScopeConfigurationDbContext(options.ConnectionString, 
                                                options.Schema))
            {
                if (context.Scopes.Any()) return;

                foreach (var scope in scopes)
                {
                    context.Scopes.Add(scope.ToEntity());
                }

                context.SaveChanges();
            }
        }


    }
}
