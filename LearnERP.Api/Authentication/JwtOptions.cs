namespace LearnERP.Api.Authentication;

public sealed class JwtOptions
{
    public const string SectionName = "Authentication:Jwt";

    public string Issuer { get; init; } = "LearnERP";

    public string Audience { get; init; } = "LearnERP.Api";

    public string? Secret { get; init; }

    public int ClockSkewMinutes { get; init; } = 2;
}
