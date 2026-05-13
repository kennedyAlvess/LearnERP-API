namespace LearnERP.Api.Authentication;

public interface ITenantContextAccessor
{
    string? TenantId { get; }

    TenantContext? Current { get; }
}

public sealed class TenantContextAccessor : ITenantContextAccessor
{
    public string? TenantId { get; private set; }

    public TenantContext? Current { get; private set; }

    public void SetTenant(string tenantId)
    {
        TenantId = tenantId;
        Current = new TenantContext(tenantId);
    }
}
