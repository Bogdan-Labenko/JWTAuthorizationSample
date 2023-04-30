namespace DotNetExam.Services
{
    public class CookiesService
    {
        private readonly IConfiguration _configuration;

        public CookiesService(IConfiguration configuration) =>
            _configuration = configuration;

        public void AddRefreshCookieToResponse(HttpResponse response, string refreshToken)
        {
            response.Cookies.Append("refresh-token", refreshToken);
        }
        public string GetRefreshTokenFromCookie(HttpRequest request)
        {
            return request.Cookies.FirstOrDefault(cookie => cookie.Key == "refresh-token").Value;
        }
    }
}
