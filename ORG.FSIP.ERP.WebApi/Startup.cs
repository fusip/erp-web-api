﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ORG.FSIP.ERP.Core.DAL.Providers.SqlServer.Extensions.DependencyInjection;
using ORG.FSIP.ERP.WebApi.Extensions;
using ORG.FSIP.ERP.WebApi.Services;
using PartialResponse.AspNetCore.Mvc;
using PartialResponse.Extensions.DependencyInjection;

namespace ORG.FSIP.ERP.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IModuleService, ModuleService>();

            services.AddApiVersioning();

            services.AddMvc(options => options.OutputFormatters.RemoveType<JsonOutputFormatter>())
                .AddPartialJsonFormatters(options => {
                    options.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.NullValueHandling = NullValueHandling.Ignore;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.Configure<MvcPartialJsonOptions>(options => options.IgnoreCase = true);

            services.AddSqlServerDataProviderForCore(Configuration.GetConnectionString("DataBaseConnection"));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseApiResponseWrapper();

            app.UseMvc();
        }
    }
}
