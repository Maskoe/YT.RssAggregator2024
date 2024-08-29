namespace RssAggregator.Features;

public class GetUserFeedsEndpoint : EndpointWithoutRequest<GetUserFeedsResponse>
{
    private readonly Context context;

    public GetUserFeedsEndpoint(Context context)
    {
        this.context = context;
    }

    public override void Configure()
    {
        Get("api/feeds/me");
    }

    public override async Task<GetUserFeedsResponse> ExecuteAsync(CancellationToken ct)
    {
        var currentUser = User.ToTokenUser();

        var dtos = await context.Feeds
            .Where(feed => feed.Subscriptions.Any(sub => sub.UserId == currentUser.Id))
            .Select(dbUser => new FeedDto(dbUser.Id, dbUser.Name))
            .ToListAsync();

        return new GetUserFeedsResponse(dtos);
    }
}

public record GetUserFeedsResponse(List<FeedDto> Feeds);