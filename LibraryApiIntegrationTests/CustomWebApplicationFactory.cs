using LibraryApi;
using LibraryApi.Controllers;
using LibraryApi.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApiIntegrationTests
{
    public class CustomWebApplicationFactory: WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // this runs after configure in our startup runs. 
            // all the services (including our ILookupServerStatus) are setup and ready to go.
            // We are going to reach in and replace the WillsHealthCheckServerStatus with Folger's Crystals
            // We are going to fake it

            builder.ConfigureServices(services =>
            {
                // find the service that uses ILookupServerStatus
                var descriptor = services.Single(services =>
                    services.ServiceType == typeof(ILookupServerStatus)
                );
                // remove it
                services.Remove(descriptor);
                //replace it with this
                services.AddTransient<ILookupServerStatus, FakeServerStatus>();
            });
        }
    }

    public class FakeServerStatus : ILookupServerStatus
    {
        public StatusResponse GetStatusFor()
        {
            return new StatusResponse
            {
                Message = "Jeff was here",
                LastChecked = new DateTime(1969, 4, 20, 23, 59, 00)
            };
        }
    }
}
