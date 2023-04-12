// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using API.Data;
// using Microsoft.EntityFrameworkCore;

// namespace API.Extensions
// {
//     public static class DatabaseInitializer
//     {
//             public static async Task Initialize(IServiceProvider serviceProvider)
//         {
//             using var scope = serviceProvider.CreateScope();
//             var services = scope.ServiceProvider;
//             try
//             {
//                 var context = services.GetRequiredService<DataContext>();
//                 await context.Database.MigrateAsync();
//                 await Seed.SeedUsers(context);
//             }
//             catch (System.Exception ex)
//             {
//                 var logger = services.GetService<ILogger<Program>>();
//                 logger.LogError(ex , "An error occurred during migration");
//             }

//         }
//     }
// }