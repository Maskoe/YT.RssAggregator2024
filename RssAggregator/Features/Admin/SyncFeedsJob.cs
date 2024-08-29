using System.Xml.Serialization;
using Hangfire;

namespace RssAggregator.Features.Admin;

public class SyncFeedsJob
{
    private readonly Context context;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IBackgroundJobClient backgroundJobs;

    public SyncFeedsJob(Context context, IHttpClientFactory httpClientFactory, IBackgroundJobClient backgroundJobs)
    {
        this.context = context;
        this.httpClientFactory = httpClientFactory;
        this.backgroundJobs = backgroundJobs;
    }

    public async Task FetchAllFeeds()
    {
        Console.WriteLine("Executing FetchAllFeeds");
        var allFeeds = await context.Feeds.ToListAsync();

        foreach (var feed in allFeeds)
            await FetchSingleFeed(feed.Id);
    }

    public async Task FetchSingleFeed(Guid feedId)
    {
        Console.Write("Executing FetchSingleFeed ");
        // get the feed from db with the url
        // scrape all the posts
        // save them into our database

        var dbFeed = await context.Feeds
            .Include(x => x.Posts)
            .FirstAsync(x => x.Id == feedId);

        var httpClient = httpClientFactory.CreateClient();
        var xmlString = await httpClient.GetStringAsync(dbFeed.Url);

        var serializer = new XmlSerializer(typeof(RssRoot));
        using var reader = new StringReader(xmlString);

        var feedFromInternet = (RssRoot)serializer.Deserialize(reader);

        var posts = feedFromInternet.channel.items
            .Where(scrapedPost => dbFeed.Posts.All(dbPost => scrapedPost.link != dbPost.Url))
            .Select(scrapedPost => new Post
            {
                Title = scrapedPost.title,
                Url = scrapedPost.link,
                Description = scrapedPost.description,
                PublishedAt = DateTime.Parse(scrapedPost.pubDate).ToUniversalTime(),
            }).ToList();

        dbFeed.Posts.AddRange(posts);
        dbFeed.LastFetchedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
    }


    public async Task Wait5Times()
    {
        for (int i = 0; i < 5; i++)
            backgroundJobs.Enqueue(() => WaitTwoSeconds());
    }

    public async Task WaitTwoSeconds()
    {
        Console.WriteLine("Starting at " + DateTime.Now.ToLongTimeString());
        await Task.Delay(2000);
        Console.WriteLine("Finished at " + DateTime.Now.ToLongTimeString());
    }
}

[XmlRoot("rss")]
public class RssRoot
{
    public RssFeed channel { get; set; }
}

public class RssFeed
{
    public string title { get; set; }
    public string link { get; set; }
    public string description { get; set; }
    public string language { get; set; }

    [XmlElement("item")] public List<RssItem> items { get; set; }
}

public class RssItem
{
    public string title { get; set; }
    public string link { get; set; }
    public string description { get; set; }
    public string pubDate { get; set; }
}