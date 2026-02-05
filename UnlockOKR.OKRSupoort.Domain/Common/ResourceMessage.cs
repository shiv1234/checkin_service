using System.Diagnostics.CodeAnalysis;

namespace UnlockOKR.OKRSupoort.Domain.Common
{
    [ExcludeFromCodeCoverage]
    public static class ResourceMessage
    {
        public static string RecordNotFoundMessage => AuthResources.GetResourceKeyValue("RecordNotFound");
        public static string NewTaskCreated => AuthResources.GetResourceKeyValue("NewTaskCreated");
        public static string TaskUpdationMessage => AuthResources.GetResourceKeyValue("TaskUpdationMessage");
        public static string RoleIdInvalid => AuthResources.GetResourceKeyValue("RoleIdInvalid");
        public static string TaskAlreadyExist => AuthResources.GetResourceKeyValue("TaskAlreadyExist");
        public static string TaskRequired => AuthResources.GetResourceKeyValue("TaskRequired");
        public static string SomethingWentWrong => AuthResources.GetResourceKeyValue("SomethingWentWrong");
        public static string AssignRoleSuccessfully => AuthResources.GetResourceKeyValue("AssignRoleSuccessfully");
        public static string Must20CharactersLong => AuthResources.GetResourceKeyValue("Must20CharactersLong");
        public static string Required => AuthResources.GetResourceKeyValue("Required");
        public static string RecordNotFound => AuthResources.GetResourceKeyValue("RecordNotFound");
        public static string RoleNameUpdatedSuccessfully => AuthResources.GetResourceKeyValue("RoleNameUpdatedSuccessfully");
        public static string RolePermissionSuccessfully => AuthResources.GetResourceKeyValue("RolePermissionSuccessfully");
        public static string CeoRoleCantBeEdited => AuthResources.GetResourceKeyValue("CeoRoleCantBeEdited");
        public static string CeoRoleHaveOneUserOnly => AuthResources.GetResourceKeyValue("CeoRoleHaveOneUserOnly");
        public static string TaskDeletedMsg => AuthResources.GetResourceKeyValue("TaskDeletedMsg");


    }
}
