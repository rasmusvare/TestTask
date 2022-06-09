using System.Text.RegularExpressions;
using System.Web;
using AngleSharp;
using App.DAL.EF;
using App.Domain;
using Microsoft.EntityFrameworkCore;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace WebApp;

public static class AppDataHelper
{
    
    public static async void SetupAppData(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration conf)
    {
        using var serviceScope = app
            .ApplicationServices
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();

        using var context = serviceScope
            .ServiceProvider
            .GetService<AppDbContext>();

        if (context == null)
        {
            throw new ApplicationException("Problem in services. No DB context!");
        }

        if (conf.GetValue<bool>("DataInitialization:DropDatabase"))
        {
            context.Database.EnsureDeleted();
        }

        if (conf.GetValue<bool>("DataInitialization:MigrateDatabase"))
        {
            context.Database.Migrate();
        }

        if (conf.GetValue<bool>("DataInitialization:SeedData"))
        {
            const string spaceSymbol = "&nbsp;";
            const int tabLength = 4;
            
            var filePath = Path.Combine(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar + "index.html");

            var html = File.ReadAllText(filePath);

            var configAs = Configuration.Default;
            using var contextAs = BrowsingContext.New(configAs);
            using var doc = await contextAs.OpenAsync(req => req.Content(html));

            var selectOptions = doc.QuerySelectorAll("option");

            Stack<Guid?> parentIds = new Stack<Guid?>();

            var level = 0;
            Guid? parentId = null;
            Guid? prevId = null;
            foreach (var each in selectOptions)
            {
                var option = each.InnerHtml.Split(spaceSymbol);

                var name = Regex.Replace(option.Last(), @"\s+", " ");
                var value = each.Attributes.GetNamedItem("value")?.Value;

                if (value == null)
                {
                    throw new ApplicationException("No value found");
                }

                Sector? sector;
                if (option.Length == 1)
                {
                    parentIds.Clear();
                    level = 0;

                    sector = new Sector
                    {
                        Name = HttpUtility.UrlEncode(name),
                        Value = value,
                        ParentId = null
                    };

                    context.Sectors.Add(sector);
                    await context.SaveChangesAsync();

                    prevId = sector.Id;
                    parentId = sector.Id;

                    continue;
                }

                if ((option.Length - 1) / tabLength > level)
                {
                    parentIds.Push(parentId);
                    parentId = prevId;
                }
                else if ((option.Length - 1) / tabLength < level)
                {
                    for (int i = 0; i < level - (option.Length - 1) / tabLength; i++)
                    {
                        parentId = parentIds.Pop();
                    }
                }

                level = (option.Length - 1) / tabLength;

                sector = new Sector
                {
                    Name = HttpUtility.HtmlDecode(name),
                    Value = value,
                    ParentId = parentId
                };

                context.Sectors.Add(sector);
                await context.SaveChangesAsync();

                prevId = sector.Id;
            }
        }
    }
}