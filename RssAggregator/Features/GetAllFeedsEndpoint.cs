using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using RssAggregator.Db;

namespace RssAggregator.Features;

public class GetAllFeedsEndpoint : EndpointWithoutRequest<GetFeedsResponse>
{
    private readonly Context context;

    public GetAllFeedsEndpoint(Context context)
    {
        this.context = context;
    }

    public override void Configure()
    {
        Get("api/feeds");
    }

    public override async Task<GetFeedsResponse> ExecuteAsync(CancellationToken ct)
    {
        var dtos = await context.Feeds
            .Select(x => new FeedDto(x.Id, x.Name))
            .ToListAsync();

        return new GetFeedsResponse(dtos);
    }
}

public record GetFeedsResponse(List<FeedDto> Feeds);

public record FeedDto(Guid Id, string Name);