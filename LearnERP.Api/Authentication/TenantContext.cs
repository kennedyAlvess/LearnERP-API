namespace LearnERP.Api.Authentication;

public sealed class TenantContext(string tenantId)
{
    public string TenantId { get; } = tenantId;
}
