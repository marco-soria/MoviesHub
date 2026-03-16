namespace MoviesHub.Web.Utility
{
    public static class SD
    {
        public static string MovieAPIBase { get; set; }
        public static string ReviewAPIBase { get; set; }
        public static string AuthAPIBase { get; set; }

        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE,
            PATCH
        }

        public const string RoleAdmin = "Admin";
        public const string RoleUser = "User";
        public const string RoleManager = "Manager";

        public const string TokenCookie = "NuevoToken";
    }
}
