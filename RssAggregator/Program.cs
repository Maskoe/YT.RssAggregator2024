global using FastEndpoints;
global using Microsoft.EntityFrameworkCore;
global using RssAggregator.Db;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Hangfire;
using RssAggregator.Features.Admin;

var bld = WebApplication.CreateBuilder();
bld.Services.AddFastEndpoints();
bld.Services.SwaggerDocument();

bld.Services.AddAuthenticationJwtBearer(x => x.SigningKey = bld.Configuration["JwtSecret"]);
bld.Services.AddAuthorization();

bld.Services.AddHangfire(x => x.UseInMemoryStorage());
bld.Services.AddHangfireServer();
bld.Services.AddHttpClient();

bld.Services.AddTransient<SyncFeedsJob>();

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

var sp = app.Services.CreateScope().ServiceProvider;
var syncJob = sp.GetRequiredService<SyncFeedsJob>();
var reccuringJobs = sp.GetRequiredService<IRecurringJobManager>();

// await syncJob.Wait5Times();
// await syncJob.FetchSingleFeed(Guid.Parse("2408ffc3-48f5-449b-8a3b-309069a96f40"));
reccuringJobs.AddOrUpdate("sync-feeds-concurrently", () => syncJob.FetchAllFeeds(), Cron.Minutely);


app.Run();