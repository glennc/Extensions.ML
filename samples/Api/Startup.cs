using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Extensions.ML;
using Api.Models;

namespace Api
{

    public interface IModelResolver
    {
        Task<object> GetModelAsync(string urlOrNameOrSomething);
    }

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
            services.AddMvc();

            services.AddPredictionEngine<SentimentIssue, SentimentPrediction>("SentimentModel.zip", "oldModel");
            services.AddPredictionEngine<SentimentIssue, SentimentPrediction>("newModel", options => options.FromUri(Configuration["BlobUri"]));

            services.AddPredictionEngine<SentimentIssue, SentimentPrediction>(new Uri(Configuration["BlobUri"]), "newModel2");
            services.AddPredictionEngine<SampleObservation, SamplePrediction>("DetoxModel.zip");
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
