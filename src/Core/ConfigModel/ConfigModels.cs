namespace Core.ConfigModel
{
    /// <summary>
    /// DEFAULT CONFIG
    /// </summary>
    public static class DefaultConfig
    {
        public static readonly int DefaultPageNumber = 1;
        public static readonly int DefaultPageSize = 20;
        public const string LoginFailedCookieName = "LoginFailed";
        public const int MaxNumOfLoginFailed = 5;
        public const int LockTimeMinutes = 30;

    }
}
