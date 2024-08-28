using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using RssAggregator.Db;

var bld = WebApplication.CreateBuilder();
bld.Services.AddFastEndpoints();
bld.Services.SwaggerDocument();

bld.Services.AddDbContextFactory<Context>(options =>
{
    options.UseNpgsql("User ID=postgres; Password=postgres; Database=ytRss; Server=localhost; Port=5432; Include Error Detail=true;");
});

var app = bld.Build();
app.UseFastEndpoints();
app.UseSwaggerGen();
app.Run();