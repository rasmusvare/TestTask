# Technical task

The application uses SQLite as the default database and by default will seed the database with data from /Data/index.html file. This behaviour can be configured in the appsettings.json file. The database dump can be found in the Dayabase_dump directory.

## Scaffolding


### DB Migrations

~~~bash
dotnet ef migrations --project App.Domain --startup-project WebApp add Initial
dotnet ef migrations --project App.Domain --startup-project WebApp remove Initial
dotnet ef database --project App.Domain --startup-project WebApp update
dotnet ef database --project App.Domain --startup-project WebApp drop
~~~

#### MVC razor based
~~~bash
cd WebApp

dotnet aspnet-codegenerator controller -name SectorController -actions -m App.Domain.Sector -dc App.Domain.AppDbContext -outDir Controllers --useAsyncActions --useDefaultLayout --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name EntryController -actions -m App.Domain.Entry -dc App.Domain.AppDbContext -outDir Controllers --useAsyncActions --useDefaultLayout --referenceScriptLibraries -f
~~~

#### Web API
~~~bash
cd WebApp

dotnet aspnet-codegenerator controller -name SectorController -actions -m App.Domain.Sector -dc App.Domain.AppDbContext -outDir ApiControllers -api --useAsyncActions -f
dotnet aspnet-codegenerator controller -name EntryController -actions -m App.Domain.Entry -dc App.Domain.AppDbContext -outDir ApiControllers -api --useAsyncActions -f
~~~