using FastEndpoints;
using RssAggregator.Db;

namespace RssAggregator.Features.UserManagement;

public class RegisterEndpoint : Endpoint<RegisterRequest, RegisterResponse>
{
    private readonly Context context;

    public RegisterEndpoint(Context context)
    {
        this.context = context;
    }

    public override void Configure()
    {
        Post("auth/register");
        AllowAnonymous();
    }

    public override async Task<RegisterResponse> ExecuteAsync(RegisterRequest req, CancellationToken ct)
    {
        var newUser = new AppUser
        {
            Email = req.Email, 
            Password = req.Password, 
            Role = req.Role
        };
        context.Users.Add(newUser);

        await context.SaveChangesAsync();

        var res = new RegisterResponse
        {
            Id = newUser.Id,
            Email = newUser.Email
        };
        return res;
    }
}

public class RegisterRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}

public class RegisterResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; }
}