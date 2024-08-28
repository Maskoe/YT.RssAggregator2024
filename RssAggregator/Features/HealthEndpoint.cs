using FastEndpoints;

namespace RssAggregator.Features;

public class HealthEndpoint : Endpoint<HealthRequest, HealthResponse>
{
    public override void Configure()
    {
        Get("/api/healthz");
        AllowAnonymous();
    }

    public override async Task<HealthResponse> ExecuteAsync(HealthRequest req, CancellationToken ct)
    {
        return new HealthResponse { AllCaps = req.Check.ToUpper() };
    }
}

public record HealthRequest
{
    public string Check { get; set; } = "";
}

public record HealthResponse
{
    public string AllCaps { get; set; } = "";
}