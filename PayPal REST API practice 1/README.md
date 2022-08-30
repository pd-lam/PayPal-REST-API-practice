#**THANH TOÁN TÍCH HỢP PAYPAL VÀO WEBSITE SỬ DỤNG CÔNG NGHỆ ASP.NET MVC**

##**1. Giới thiệu**##
Bài thực hành đầu tiên của môn học Thương mại điện tử [Link tham khảo](https://www.codeproject.com/Articles/870870/Using-Paypal-Rest-API-with-ASP-NETMVC).
Trước khi bắt đầu làm bài, ta cần thực hiện được các yêu cầu sau:
- Đăng ký một tài khoản PayPal và tạo một tài khoản dành cho nhà phát triển [Link đăng ký tại đây](https://developer.paypal.com).
- Có được ClientId và ClientSecretKey [Link hướng dẫn](https://www.tucalendi.com/en/s/integrations/how-to-get-live-paypal-client-id-and-secret-key-171).
- Đăng ký một tài khoản PayPal và tạo một tài khoản dành cho nhà phát triển

##**2. Bắt đầu vào thực hành**##
B1: Tạo và cấu hình ban đầu cho project:
- Tạo một ASP.Net MVC project.
- Thêm thư viện PayPal thông qua NuGet Package Manager.
- Vào file web.config để cấu hình thông tin thanh toán từ PayPal.
- Phía dưới thẻ <configuration> ta thêm vào đoạn code sau:
**1. Giới thiệu**
	Bài thực hành đầu tiên của môn học Thương mại điện tử [Link tham khảo](https://www.codeproject.com/Articles/870870/Using-Paypal-Rest-API-with-ASP-NETMVC).
	Trước khi bắt đầu làm bài, ta cần thực hiện được các yêu cầu sau:
	- Đăng ký một tài khoản PayPal và tạo một tài khoản dành cho nhà phát triển [Link đăng ký tại đây](https://developer.paypal.com).
	- Có được ClientId và ClientSecretKey [Link hướng dẫn](https://www.tucalendi.com/en/s/integrations/how-to-get-live-paypal-client-id-and-secret-key-171).

**2. Bắt đầu vào thực hành**
>>>>>>>>> Temporary merge branch 2
	B1: Tạo và cấu hình ban đầu cho project:
	- Tạo một ASP.Net MVC project.
	- Thêm thư viện PayPal thông qua NuGet Package Manager.
	- Vào file web.config để cấu hình thông tin thanh toán từ PayPal.
	- Phía dưới thẻ <configuration> ta thêm vào đoạn code sau:
```
<configSections>
  <section name="paypal" type="PayPal.SDKConfigHandler, PayPal" />
</configSections>

<!-- PayPal SDK settings -->
<!-- 
Mode sandbox chỉ là môi trường kiểm thử, nếu muốn phát triển một ứng dụng
thanh toán thực tế thì ta cần đổi sang mode live và cập nhật lại 
ClientId và ClientSecretKey.
-->
<paypal>
  <settings>
    <add name="mode" value="sandbox"/>
    <add name="connectionTimeout" value="360000"/>
    <add name="requestRetries" value="1"/>
    <add name="clientId" value="Đặt ClientId tại đây"/>
    <add name="clientSecret" value="Đặt ClientSecretKey tại đây"/>
  </settings>
</paypal>
```
