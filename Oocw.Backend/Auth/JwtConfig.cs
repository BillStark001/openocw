namespace Oocw.Backend.Auth;

public class JwtConfig
{
    public string Secret { get; set; } = "RCDWRCDWRCDWRCDW";
    public int AccessExpiration { get; set; } = 1;
    public int RefreshExpiration { get; set; } = 60;
}