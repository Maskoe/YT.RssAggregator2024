using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using RssAggregator.Db;

namespace RssAggregator.Features.UserManagement;

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly Context context;

    public LoginEndpoint(Context context)
    {
        this.context = context;
    }

    public override void Configure()
    {
        Post("auth/login");
        AllowAnonymous();
    }

    public override async Task<LoginResponse> ExecuteAsync(LoginRequest req, CancellationToken ct)
    {
        var userFromDb = await context.Users.FirstOrDefaultAsync(x => x.Email.ToUpper() == req.Email.ToUpper()
                                                                      && x.Password == req.Password);
        if (userFromDb is null)
            ThrowError("Could not log you in", StatusCodes.Status404NotFound);

        var jwt = JwtBearer.CreateToken(options =>
        {
            options.SigningKey = Config["JwtSecret"];
            options.User.Claims.Add(new Claim(JwtRegisteredClaimNames.Sub, userFromDb.Id.ToString()));
            options.User.Claims.Add(new Claim(JwtRegisteredClaimNames.Name, userFromDb.Email));
            options.User.Roles.Add(userFromDb.Role);
            options.ExpireAt = DateTime.UtcNow.AddDays(7);
        });

        return new LoginResponse(jwt, userFromDb.Email);
    }
}

public record LoginRequest(string Email, string Password);

public record LoginResponse(string Jwt, string Email);