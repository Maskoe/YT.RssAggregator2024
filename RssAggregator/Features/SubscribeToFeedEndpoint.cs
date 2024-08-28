using FastEndpoints;
using RssAggregator.Db;

namespace RssAggregator.Features;

public class SubscribeToFeedEndpoint : Endpoint<SubscribeToFeedRequest>
{
    private readonly Context context;

    public SubscribeToFeedEndpoint(Context context)
    {
        this.context = context;
    }

    public override void Configure()
    {
        Post("api/feeds/subscribe");
    }

    public override async Task HandleAsync(SubscribeToFeedRequest req, CancellationToken ct)
    {
        var newSub = new Subscription
        {
            FeedId = req.FeedId, 
            UserId = User.ToTokenUser().Id
        };
        context.Set<Subscription>().Add(newSub);

        await context.SaveChangesAsync();
    }
}

public record SubscribeToFeedRequest(Guid FeedId);