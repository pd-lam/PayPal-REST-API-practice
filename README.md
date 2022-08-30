# **THỰC HÀNH TÍCH HỢP PAYPAL REST API VÀO WEBSITE**

# **1. Giới thiệu**

Đây là bài thực hành đầu tiên trong môn học Thương mại điện tử trường SPKT, giúp các bạn có cái nhìn đầu tiên về cách tiếp cận API một cách đơn giản.

Bài thực hành được tham khảo tại: https://www.codeproject.com/Articles/870870/Using-Paypal-Rest-API-with-ASP-NETMVC

Trước khi bắt đầu code thì ta cần một vài yêu cầu sau:
  - Phải có tài khoản PayPal dành cho nhà phát triển. 
  - Lấy được Client Id và Client Secret Key (ở chế độ sanbox) ([Link hướng dẫn tại đây](https://www.knowband.com/blog/tips/get-paypal-client-id-secret/)).
  
# **2. Các bước thực hành**

  ## B1: Tạo và cấu hình cho project
  - Tạo một project ASP NET MVC Web Application.
  - Thêm thư viện Paypal thông qua NuGet Package Manager hoặc sử dụng lệnh "Install-Package Paypal" trong Package Manager Console
  - Cấu hình file web.config:
    - Phía dưới thẻ `<configuration>` ta thêm đoạn code này
    ```
    <configSections>
      <section name="paypal" type="PayPal.SDKConfigHandler, PayPal" />
    </configSections>

    <!-- PayPal SDK settings -->
    <paypal>
      <settings>
        <add name="mode" value="sandbox"/>
        <add name="connectionTimeout" value="360000"/>
        <add name="requestRetries" value="1"/>
        <add name="clientId" value=""/>
        <add name="clientSecret" value="Client Secret Key"/>
      </settings>
    </paypal>
    ```
       - `<add name="mode" value="sandbox"/>` sandbox là môi trường kiểm thử, live là môi trường thực tế.
       - `<add name="clientId" value=""/>` đặt Client ID vào biến value.
       - `<add name="clientSecret" value=""/>` đặt Client Secret Key vào biến value.
   ## B2: Tạo controller thực hiện thanh toán qua PayPal đặt tên PaypalController.cs
   - Thêm các object và action mới như sau:
   ```
   public class PayPalController : Controller
    {
        private Payment payment;
        // GET: PayPal
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PaymentWithPaypal()
        {
            return null;
        }

        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            return null;
        }

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            return null;
        }
    }
   ```
   
   - Trong thư mục Controllers tạo thư mục helper và thêm 2 file Configuration.cs và Logger.cs
   ```
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
   ```
   
   ```
      public class Logger
    {
        public static string LogDirectoryPath = Environment.CurrentDirectory;
        public static void Log(String lines)
        {
            //Ghi lại nhật ký
            try
            {
                System.IO.StreamWriter file = new
                System.IO.StreamWriter(LogDirectoryPath + "\\Error.log", true);
                file.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " --> " + lines);
                file.Close();
            }
            catch
            { }
        }
    }
   ```
   - Thêm vào hàm CreatePayment đoạn code sau:
   ```
   private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            // Tạo một danh sách các sản phẩm cần thanh toán.
            // Các giá trị bao gồm danh sách sản phẩm, thông tin đơn hàng
            // sẽ được thay đổi bằng hành vi thao tác mua hàng trên website.
            var itemList = new ItemList() { items = new List<Item>() };

            itemList.items.Add(new Item()
            {
                //Thông tin đơn hàng
                name = "Item Name",
                currency = "USD",
                price = "5",
                quantity = "1",
                sku = "sku"
            });
            int _subtotal = Int32.Parse(itemList.items[0].price) * Int32.Parse(itemList.items[0].quantity);


            // Tạo một đối tượng payer với phương thức trả tiền là PayPal
            var payer = new Payer() { payment_method = "paypal" };

            // Thông tin đơn hàng
            var details = new Details()
            {
                tax = "1",
                shipping = "1",
                subtotal = _subtotal.ToString(),
            };
            int _total = Int32.Parse(details.tax)
                + Int32.Parse(details.shipping)
                + Int32.Parse(details.subtotal);

            //Đơn vị tiền tệ và tổng đơn hàng cần thanh toán
            var amount = new Amount()
            {
                currency = "USD",
                total = _total.ToString(),
                details = details
            };

            // Định cấu hình chuyển hướng
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl,
                return_url = redirectUrl
            };

            var transactionList = new List<Transaction>();
            //Tất cả thông tin thanh toán cần đưa vào transaction
            transactionList.Add(new Transaction()
            {
                description = "Transaction description.",
                invoice_number = "your invoice number",
                amount = amount,
                item_list = itemList
            });

            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            // Tạo thanh toán bằng cách sử dụng APIContext
            return this.payment.Create(apiContext);
        }
   ```
   - Thêm vào hàm ExecutePayment đoạn code sau:
   ```
   private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }
   ```
   - Thêm vào Action PaymentWithPaypal đoạn code sau:
