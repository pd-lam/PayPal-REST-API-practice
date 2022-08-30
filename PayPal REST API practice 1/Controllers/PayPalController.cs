using PayPal.Api;
using PayPal_REST_API_practice_1.Controllers.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace PayPal_REST_API_practice_1.Controllers
{
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
            // Lấy apiContext
            APIContext apiContext = Configuration.GetAPIContext();

            try
            {
                string payerId = Request.Params["PayerID"];

                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist
                    //it is returned by the create function call of the payment class

                    // Creating a payment
                    // baseURL is the url on which paypal sendsback the data.
                    // So we have provided URL of this controller only
                    string baseURI = Request.Url.Scheme + "://" 
                        + Request.Url.Authority 
                        + "/Paypal/PaymentWithPayPal?";

                    //guid we are generating for storing the paymentID received in session
                    //after calling the create function and it is used in the payment execution

                    var guid = Convert.ToString((new Random()).Next(100000));

                    //CreatePayment function gives us the payment approval url
                    //on which payer is redirected for paypal account payment

                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);

                    //get links returned from paypal in response to Create function call

                    var links = createdPayment.links.GetEnumerator();

                    string paypalRedirectUrl = null;

                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;

                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    // saving the paymentID in the key guid
                    Session.Add(guid, createdPayment.id);

                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This section is executed when we have received all the payments parameters

                    // from the previous call to the function Create

                    // Executing a payment

                    var guid = Request.Params["guid"];

                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error" + ex.Message);
                return View("FailureView");
            }

            return View("SuccessView");
        }

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

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }
    }
}