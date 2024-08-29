global using FastEndpoints;
global using Microsoft.EntityFrameworkCore;
global using RssAggregator.Db;

using FastEndpoints.Security;
using FastEndpoints.Swagger;

var bld = WebApplication.CreateBuilder();
bld.Services.AddFastEndpoints();
bld.Services.SwaggerDocument();

bld.Services.AddAuthenticationJwtBearer(x => x.SigningKey = bld.Configuration["JwtSecret"]);
bld.Services.AddAuthorization();

bld.Services.AddDbContextFactory<Context>(options =>
{
    options.UseNpgsql("User ID=postgres; Password=postgres; Database=ytRss2; Server=localhost; Port=5432; Include Error Detail=true;");
    options.EnableSensitiveDataLogging();
});

var app = bld.Build();
app.UseFastEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwaggerGen();
app.Run();