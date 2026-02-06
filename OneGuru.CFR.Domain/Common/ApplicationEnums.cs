namespace OneGuru.CFR.Domain.Common
{
    public enum Modules
    {
        General = 1,
        Organization,
        UserManagement,
        RoleManagement,
        CoachFeature
    }

    public enum Permissions
    {
        CreateOkrs = 1,
        EditOkrs,
        AssignOkr,
        AllowtoaddContributorforOkr,
        Feedbackmodule,
        OneToOneModule,
        ViewOrganizationManagementPage = 7,
        CreateTeams,
        EditMainOrganization,
        EditTeams,
        DeleteTeams,
        ViewUserManagementPage = 12,
        AddNewUsers,
        EditUsersFrom,
        DeleteUsersFrom,
        ViewRoleManagement = 16,
        AddNewRole,
        EditExistingRole,
        DeleteRole,
        AllowCreateOkrsOnBehalfOfAnotherPerson = 20
    }

    public enum GeneralPermissions
    {
        CreateOkrs = 1,
        AssignOkr,
        AllowtoaddContributorforOkr,
        Feedbackmodule,
        OneToOneModule
    }

    public enum OrganizationPermissions
    {
        ViewOrganizationManagementPage = 6,
        CreateTeams,
        EditMainOrganization,
        EditTeams,
        DeleteTeams
    }

    public enum UserManagementPermissions
    {
        ViewUserManagementPage = 11,
        AddNewUsers,
        EditUsersFrom,
        DeleteUsersFrom
    }

    public enum RoleManagementPermissions
    {
        ViewRoleManagement = 15,
        AddNewRole,
        EditExistingRole,
        DeleteRole
    }

    public enum CoachFeaturePermissions
    {
        AllowCreateOkrsOnBehalfOfAnotherPerson = 19
    }

    public enum MessageType
    {
        /// <summary>
        /// The information
        /// </summary>
        Info,
        /// <summary>
        /// The success
        /// </summary>
        Success,
        /// <summary>
        /// The alert
        /// </summary>
        Alert,
        /// <summary>
        /// The warning
        /// </summary>
        Warning,
        /// <summary>
        /// The error
        /// </summary>
        Error
    }

    public enum EmployeePermissionType
    {
        PermissionRemoved,
        PermissionAdded
    }
    public enum EmployeeDefaultRole
    {
        CEO = 2,
        User = 4

    }
    public enum AzureBusServiceName
    {
        Email = 1,
        Report,
        Team,
        Role,
        User,
        Notification,
        AuditLog
    }
}
