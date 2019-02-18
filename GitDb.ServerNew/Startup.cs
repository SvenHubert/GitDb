using System;
using GitDb.Core;
using GitDb.Core.Interfaces;
using GitDb.Local;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GitDb.ServerNew
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
			services.AddSingleton<IGitDb>(e =>
			{
				return new LocalGitDb(Configuration.GetValue<string>("Settings:Git:RepositoryPath"), 
					new Logger(),
					Configuration.GetValue<string>("Settings:Git:Remote:URL"),
					Configuration.GetValue<string>("Settings:Git:Remote:User:Name"),
					Configuration.GetValue<string>("Settings:Git:Remote:User:Email"),
					Configuration.GetValue<string>("Settings:Git:Remote:User:Password"));
			});
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
			app.UseMvc();
		}
	}
	class Logger : ILogger
	{
		public void Trace(string format, params object[] args) => Console.WriteLine("TRACE: " + format, args);
		public void Info(string format, params object[] args) => Console.WriteLine("INFO: " + format, args);
		public void Debug(string format, params object[] args) => Console.WriteLine("DEBUG: " + format, args);
		public void Warn(string format, params object[] args) => Console.WriteLine("WARB: " + format, args);
		public void Error(string format, params object[] args) => Console.WriteLine("ERROR: " + format, args);
	}
}
