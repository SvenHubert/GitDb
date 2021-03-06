﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace GitDb.ServerNew
{
	class Program
	{
		public static void Main(string[] args)
		{
			//var users = new List<User>
			//{
			//	new User{ UserName = "GitAdmin", Password = "LCz8ovCZiddM4FGH1T3V", Roles = new [] { "admin","read","write" }},
			//	new User{ UserName = "GitReader",Password = "IUFYTF2oPuK04OfnVl5H",Roles = new [] { "read" }},
			//	new User{ UserName = "GitWriter", Password = "4yzvqhPkHPZbSbuGN4aQ6b",Roles = new [] { "write" }}
			//};
			//var auth = new Authentication(users);

			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.Build();
	}

}