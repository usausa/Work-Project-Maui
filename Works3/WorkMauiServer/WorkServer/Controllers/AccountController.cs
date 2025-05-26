namespace WorkServer.Controllers;

using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using WorkServer.Settings;

public class AccountController : BaseApiController
{
    private readonly JwtSetting setting;

    public AccountController(JwtSetting setting)
    {
        this.setting = setting;
    }

    [HttpPost]
    public IActionResult Login([FromBody] AccountLoginRequest request)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, request.Id)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = new JwtSecurityToken(
            setting.Issuer,
            setting.Audience,
            claims,
            expires: DateTime.UtcNow.AddDays(setting.ExpireDays),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.SecretKey)), SecurityAlgorithms.HmacSha256Signature)
        );

        return Ok(new AccountLoginResponse
        {
            Token = tokenHandler.WriteToken(token)
        });
    }
}
