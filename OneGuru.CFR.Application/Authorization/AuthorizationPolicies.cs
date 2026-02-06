#nullable enable

namespace OneGuru.CFR.Application.Authorization;

/// <summary>
/// Contains constant strings for authorization policy names used throughout the CFR application.
/// </summary>
public static class AuthorizationPolicies
{
    // Check-in related policies
    public const string CanViewCheckins = "CanViewCheckins";
    public const string CanCreateCheckins = "CanCreateCheckins";
    public const string CanEditCheckins = "CanEditCheckins";
    public const string CanDeleteCheckins = "CanDeleteCheckins";
    public const string CanSubmitCheckins = "CanSubmitCheckins";

    // Team dashboard policies
    public const string CanViewTeamDashboard = "CanViewTeamDashboard";
    public const string CanManageTeam = "CanManageTeam";

    // Manager review policies
    public const string CanReviewCheckins = "CanReviewCheckins";
    public const string CanAddManagerNotes = "CanAddManagerNotes";

    // Role-based policies
    public const string RequireManagerRole = "RequireManagerRole";
    public const string RequireAdminRole = "RequireAdminRole";
    public const string RequireUserRole = "RequireUserRole";
}
