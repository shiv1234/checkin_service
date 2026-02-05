using System.Diagnostics.CodeAnalysis;

namespace UnlockOKR.OKRSupoort.Domain.Common
{
    [ExcludeFromCodeCoverage]
    public static class AppConstants
    {
        public const string EncryptionPrivateKey = "11b8ad45-152d-4ab4-b848-be7378a0baeb";
        public const string EncryptionSecretKey = "aB8978GGjkio02K4";
        public const string EncryptionSecretIvKey = "huI5K8o90Lhn4Jel";

        public const string Base64Regex = @"^[a-zA-Z0-9\+/]*={0,3}$";
        public const string Domain = "Domain";
        public const string Employee = "employee";
        public const string Role = "Role";
        public const string Request = "Request";
        public const string EmailTopicName = "email-topic";

        #region ImpersonationConstant

        public const string ImpersonateActivities = "ImpersonateActivities";
        public const string CreateOKR = "CreateOKR";
        public const string EditOKR = "EditOKR";
        public const string DeleteOKR = "DeleteOKR";
        public const string AddContributor = "AddContributor";
        public const string RemoveContributor = "RemoveContributor";
        public const string AlignOKR = "AlignOKR";
        public const string UpdateProgress = "UpdateProgress";
        public const string ViewAlignmentMapsAndPerformActions = "ViewAlignmentMapsAndPerformActions";
        public const string ViewDirectReport = "ViewDirectReport";
        public const string PeopleViewReportDownload = "PeopleViewReportDownload";
        public const string DirectReportDownload = "DirectReportDownload";
        public const string ViewReport = "ViewReport";
        public const string AddConversation = "AddConversation";
        public const string EditConversation = "EditConversation";
        public const string DeleteConversation = "DeleteConversation";
        public const string AddTask = "AddTask";
        public const string EditTask = "EditTask";
        public const string DeleteTask = "DeleteTask";
        public const string AddNote = "AddNote";
        public const string EditNote = "EditNote";
        public const string DeleteNote = "DeleteNote";
        public const string RequestPersonalFeedback = "RequestPersonalFeedback";
        public const string GivePersonalFeedback = "GivePersonalFeedback";
        public const string RequestPersonal1To1 = "RequestPersonal1To1";
        public const string RequestOKR1To1 = "RequestOKR1To1";
        public const string AddCheckIn = "AddCheckIn";
        public const string EditCheckIn = "EditCheckIn";
        public const string CreateUser = "CreateUser";
        public const string EditUser = "EditUser";
        public const string DeleteUser = "DeleteUser";
        public const string CreateRole = "CreateRole";
        public const string EditRole = "EditRole";
        public const string DeleteRole = "DeleteRole";
        public const string CreateOrg = "CreateOrg";
        public const string EditOrg = "EditOrg";
        public const string DeleteOrg = "DeleteOrg";
        public const string AdminReportDownload = "AdminReportDownload";
        public const string ViewAdminPanel = "ViewAdminPanel";


        #endregion

        #region Topics
        public const string QueueEmail = "Email";
        public const string QueueReport = "Report";
        public const string QueueTeam = "Team";
        public const string QueueRole = "Role";
        public const string QueueUser = "User";
        public const string QueueNotification = "Notification";
        public const string QueueAuditLog = "AuditLog";
        #endregion
    }
}
