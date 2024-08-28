using FastEndpoints;
using RssAggregator.Db;

namespace RssAggregator.Features.Admin;

public class CreateFeedEndpoint : Endpoint<CreateFeedRequest>
{
    private readonly Context context;

    public CreateFeedEndpoint(Context context)
    {
        this.context = context;
    }
    
    public override void Configure()
    {
        Post("admin/createFeed");
        Roles("admin");
    }

    public override async Task HandleAsync(CreateFeedRequest req, CancellationToken ct)
    {
        var newFeed = new Feed
        {
            Name = req.Name,
            Url = req.Url,
        };
        context.Feeds.Add(newFeed);

        await context.SaveChangesAsync();
    }
}

public record CreateFeedRequest(string Name, string Url);