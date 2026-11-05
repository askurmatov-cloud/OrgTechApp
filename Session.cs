namespace OrgTechApp
{
    public static class Session
    {
        public static int UserId { get; set; }
        public static string Login { get; set; } = string.Empty;
        public static string Role { get; set; } = string.Empty;
        public static string RoleName { get; set; } = string.Empty;

        public static bool IsAdmin
        {
            get { return Role == "ADMIN"; }
        }

        public static bool CanEditEquipment
        {
            get { return Role == "ADMIN" || Role == "MANAGER"; }
        }

        public static bool CanOperate
        {
            get { return Role == "ADMIN" || Role == "MANAGER" || Role == "OPERATOR"; }
        }

        public static bool CanViewReports
        {
            get { return Role == "ADMIN" || Role == "MANAGER" || Role == "AUDITOR"; }
        }

        public static bool CanCreateRequest
        {
            get { return Role == "USER"; }
        }

        public static bool CanProcessRequests
        {
            get { return Role == "ADMIN" || Role == "MANAGER" || Role == "OPERATOR"; }
        }

        public static bool CanViewAllRequests
        {
            get { return Role == "ADMIN" || Role == "MANAGER" || Role == "OPERATOR" || Role == "AUDITOR"; }
        }

        public static void Clear()
        {
            UserId = 0;
            Login = string.Empty;
            Role = string.Empty;
            RoleName = string.Empty;
        }
    }
}
