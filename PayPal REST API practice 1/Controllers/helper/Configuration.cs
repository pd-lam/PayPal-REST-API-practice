using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayPal_REST_API_practice_1.Controllers.helper
{
    public static class Configuration
    {
        // Lấy các thuộc tính từ web.config
        public static Dictionary<string, string> GetConfig()
        {
            return ConfigManager.Instance.GetProperties();
        }

        // Hai biến dùng để lưu trữ Client Id và Client Secret Key
        // bằng cách đọc từ web.config
        public readonly static string ClientId;
        public readonly static string ClientSecret;
        static Configuration()
        {
            var config = GetConfig();
            ClientId = config["clientId"];
            ClientSecret = config["clientSecret"];
        }

        // Tạo Access Token
        private static string GetAccessToken()
        {
            // Nhận Access Token từ OAuthTokenCredential
            // bằng cách truyền vào ClientID và ClientSecret.
            // Không bắt buộc phải tạo lại Access Token mỗi lần gọi hàm.
            // Thông thường, Access Token có thể được tạo một lần và
            // được sử dụng lại trong thời hạn hết hạn.
            string accessToken = new OAuthTokenCredential(ClientId, ClientSecret,
            GetConfig()).GetAccessToken();
            return accessToken;
        }

        // Trả về APIContext object
        public static APIContext GetAPIContext()
        {
            // Chuyển vào một đối tượng `APIContext`
            // để xác thực lời gọi và để gửi một id yêu cầu duy nhất
            // (đảm bảo tính nhanh chóng). SDK tạo một id yêu cầu
            // nếu bạn không chuyển một cách rõ ràng.
            APIContext apiContext = new APIContext(GetAccessToken());
            apiContext.Config = GetConfig();
            return apiContext;
        }
    }
}