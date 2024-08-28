using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using RssAggregator.Db;

namespace RssAggregator.Features;

public class UnsubscribeToFeedEndpoint : Endpoint<UnsubscribeToFeedRequest>
{
    private readonly Context context;

    public UnsubscribeToFeedEndpoint(Context context)
    {
        this.context = context;
    }

    public override void Configure()
    {
        Post("api/feeds/unsubscribe");
    }

    public override async Task HandleAsync(UnsubscribeToFeedRequest req, CancellationToken ct)
    {
        await context.Set<Subscription>()
            .Where(sub => sub.UserId == User.ToTokenUser().Id && sub.FeedId == req.FeedId)
            .ExecuteDeleteAsync();
    }
}

public record UnsubscribeToFeedRequest(Guid FeedId);