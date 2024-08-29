namespace RssAggregator.Features;

public class GetNewestPostsEndpoint : Endpoint<GetNewestPostsRequest, GetNewestPostsResponse>
{
    private readonly Context context;

    public GetNewestPostsEndpoint(Context context)
    {
        this.context = context;
    }

    public override void Configure()
    {
        Post("api/posts/new");
    }

    public override async Task<GetNewestPostsResponse> ExecuteAsync(GetNewestPostsRequest req, CancellationToken ct)
    {
        var posts = await context.Posts
            .Where(x => x.Feed.Users.Any(u => u.Id == User.ToTokenUser().Id))
            .OrderByDescending(x => x.PublishedAt)
            .Take(req.Limit ?? 200)
            .Select(dbPost => new PostDto
            {
                Title = dbPost.Title,
                Content = dbPost.Description,
                Url = dbPost.Url,
                PublishedAt = dbPost.PublishedAt,
                FeedName = dbPost.Feed.Name,
            })
            .ToListAsync();

        return new GetNewestPostsResponse(posts);
    }
}

public record GetNewestPostsRequest(int? Limit);

public record GetNewestPostsResponse(List<PostDto> Posts);

public class PostDto
{
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime PublishedAt { get; set; }
    public string Url { get; set; }
    public string FeedName { get; set; }
}