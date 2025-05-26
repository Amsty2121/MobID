namespace MobID.MainGateway.Models.Entities
{
    public static class EntityConstants
    {
        public const string Role_USER = "USER";
        public const string Role_ADMIN = "ADMIN";

        public static readonly ICollection<string> Roles = new HashSet<string>
        {
            Role_USER, Role_ADMIN
        };

        public const string AccessLvl_BLOCKED = "BLOCKED";
        public const string AccessLvl_GUEST = "GUEST";
        public const string AccessLvl_EMPLOYEE = "EMPLOYEE";
        public const string AccessLvl_TECHEMPL = "TECHEMPL";
        public const string AccessLvl_ADMIN = "ADMIN";
        public const string AccessLvl_GOD = "GOD";

        public static readonly ICollection<string> AccessLevels = new HashSet<string>
        {
            AccessLvl_BLOCKED, AccessLvl_GUEST, AccessLvl_EMPLOYEE, AccessLvl_TECHEMPL, AccessLvl_ADMIN, AccessLvl_GOD
        };

        public static int GetAccessLevelPriority(string accessLevel) => accessLevel switch
        {
            AccessLvl_BLOCKED => -1,
            AccessLvl_GUEST => 0,
            AccessLvl_EMPLOYEE => 1,
            AccessLvl_TECHEMPL => 2,
            AccessLvl_ADMIN => 3,
            AccessLvl_GOD => 4,

            _ => throw new ArgumentException("Access level is not recognized.")
        };
    }
}
