namespace REIstack.Domain.Models;

public enum Permission
{
    ManageOrganization,
    UpdateOrganizationSettings,
    DeleteOrganization,
    ViewMembers,
    InviteMembers,
    RemoveMembers,
    UpdateMemberRoles,
    ManageBilling,
    ViewInvoices,
    UpdateSubscription,
    ManageApiKeys,
    ViewApiKeys,
    ManageCustomDomain,
    UpdateBranding,
    ConfigureIntegrations,
    ViewAnalytics,
    ExportData,
    ViewAuditLogs,
    AccessDashboard,
    AccessApi
}