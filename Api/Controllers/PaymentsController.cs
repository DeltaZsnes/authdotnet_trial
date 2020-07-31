using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers.Bases;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentsController : ControllerBase
    {
        private string loginId = "97jXnJ82N";
        private string transactionKey = "7xqM5U7qgX98cu8m";
        private string signatureKey = "1AD49CED6D3081D97DF213AAE7A9432FF9661B3B615DCE5E8B064E7AF8EF67D205633875157BC2A71F2728E8121CF897E2D399491D48A113A2C38B9A02B96CE8";
        private string publicClientKey = "49s26qMXJ7P6q6ewdcTyjzgB728WLBExPLT72Z73yWJ7v9P9yjBuJ8jB94pWbWt8";

        public PaymentsController()
        {
        }

        [HttpPost("Verify")]
        public ActionResult Verify()
        {
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = loginId,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = transactionKey,
            };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authOnlyTransaction.ToString(),
                amount = 1.00m,
                currencyCode = "USD",
                refTransId = "invoiceNumber",
            };

            var hostedPaymentIFrameCommunicatorUrl = "https://mysite.com/IFrameCommunicator.html";
            var settings = new settingType[3];

            settings[0] = new settingType();
            settings[0].settingName = settingNameEnum.hostedPaymentButtonOptions.ToString();
            settings[0].settingValue = "{\"text\": \"Pay\"}";

            settings[1] = new settingType();
            settings[1].settingName = settingNameEnum.hostedPaymentIFrameCommunicatorUrl.ToString();
            settings[1].settingValue = "{\"url\":\" " + hostedPaymentIFrameCommunicatorUrl + "\"}";

            settings[2] = new settingType();
            settings[2].settingName = settingNameEnum.hostedPaymentReturnOptions.ToString();
            settings[2].settingValue = "{\"showReceipt\": true}";

            var request = new getHostedPaymentPageRequest
            {
                transactionRequest = transactionRequest,
                hostedPaymentSettings = settings,
            };

            var controller = new AuthorizeNet.Api.Controllers.getHostedPaymentPageController(request);
            controller.Execute();
            var response = controller.GetApiResponse();
            return Ok(response);
        }

        [HttpPost("Checkout")]
        public ActionResult Checkout([FromForm]string hostedPaymentIFrameCommunicatorUrl)
        {
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = loginId,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = transactionKey,
            };

            var settings = new List<settingType>();
            settings.Add(new settingType
            {
                settingName = settingNameEnum.hostedPaymentButtonOptions.ToString(),
                settingValue = "{\"text\": \"Pay\"}",
            });
            settings.Add(new settingType
            {
                settingName = settingNameEnum.hostedPaymentIFrameCommunicatorUrl.ToString(),
                settingValue = "{\"url\":\" " + hostedPaymentIFrameCommunicatorUrl + "\"}",
            });
            settings.Add(new settingType
            {
                settingName = settingNameEnum.hostedPaymentReturnOptions.ToString(),
                settingValue = "{\"showReceipt\": true}",
            });

            var lineItems = new List<lineItemType>();
            //lineItems.Add(new lineItemType()
            //{
            //    name = "Lot Name",
            //    description = "Lot Description",
            //    quantity = 1,
            //    taxAmount = 0,
            //    taxRate = 0,
            //    discountAmount = 0,
            //    totalAmount = 1,
            //    vatRate = 0,
            //    unitPrice = 0,
            //    discountRate = 0,
            //    taxIncludedInTotal = true,
            //    alternateTaxAmount = 0,
            //    taxable = true,
            //});

            var order = new orderType
            {
                invoiceNumber = "12394382",
                description = "descriptions",
            };

            var billingAddress = new customerAddressType
            {
                country = "US",
                state = "NY",
                city = "Anaheim",
                zip = "92805",
                address = "4255 Bel Meadow Drive",
                firstName = "Alejandro",
                lastName = "Garcia",
                phoneNumber = "909-431-4008",
            };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),
                currencyCode = "USD",
                amount = 1,
                //order = order,
                billTo = billingAddress,
                lineItems = lineItems.ToArray(),
            };

            var request = new getHostedPaymentPageRequest
            {
                transactionRequest = transactionRequest,
                hostedPaymentSettings = settings.ToArray(),
            };

            var controller = new AuthorizeNet.Api.Controllers.getHostedPaymentPageController(request);
            controller.Execute();
            var response = controller.GetApiResponse();
            return Ok(new {
                ResultCode = Enum.GetName(typeof(messageTypeEnum), response?.messages?.resultCode),
                Token = response?.token,
                MessageCode = response?.messages?.message?.FirstOrDefault()?.code,
                MessageText = response?.messages?.message?.FirstOrDefault()?.text,
            });
        }

        [HttpPost("Webhook")]
        public ActionResult Webhook([FromBody]string body)
        {
            return Ok();
        }

        [HttpPost("Test")]
        public ActionResult Test()
        {
            return Ok(new { Text = "Hello World" });
        }
    }
}
