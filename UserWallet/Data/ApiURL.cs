namespace UserWallet.Data
{
    public static class ApiURL
    {
        public const string BASE_PATH = @"http://localhost:5140";
        public const string LOGIN_URL = BASE_PATH + "/auth/login";
        public const string LOGOUT_URL = BASE_PATH + "/auth/logout";
    }
}
