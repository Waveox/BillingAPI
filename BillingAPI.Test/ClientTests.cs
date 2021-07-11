using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using BillingAPI;
using BillingAPI.Models;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Text;

namespace BillingAPITesting
{
    public class ClientTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public ClientTests()
        {
            _server = new TestServer(new WebHostBuilder()
               .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task OrdersListTest()
        {
            var response = await _client.GetAsync("/v1/Orders");
            response.EnsureSuccessStatusCode();

            var responseString = JsonConvert.DeserializeObject<Order[]>(await response.Content.ReadAsStringAsync());
            var order = responseString[0];

            Assert.Equal(4, responseString.Length);

            Assert.Equal("ON00001", order.OrderNumber);
            Assert.Equal("1", order.UserId);
            Assert.Equal("Product1", order.Product);
            Assert.Equal(250m, order.FullPrice);
            Assert.Equal(0m, order.PaidAmount);

            order = responseString[3];
            Assert.Equal("ON00004", order.OrderNumber);
            Assert.Equal("2", order.UserId);
            Assert.Equal("Product4", order.Product);
            Assert.Equal(500m, order.FullPrice);
            Assert.Equal(250m, order.PaidAmount);
        }


        [Fact]
        public async Task UserOrdersSingleTest()
        {
            var response = await _client.GetAsync("/v1/Orders/ListOrders/1");
            response.EnsureSuccessStatusCode();

            var responseString = JsonConvert.DeserializeObject<Order[]>(await response.Content.ReadAsStringAsync());

            Assert.Single(responseString);

            var order = responseString[0];

            Assert.Equal("ON00001", order.OrderNumber);
            Assert.Equal("1", order.UserId);
            Assert.Equal("Product1", order.Product);
            Assert.Equal(250m, order.FullPrice);
            Assert.Equal(0m, order.PaidAmount);
        }

        [Fact]
        public async Task UserOrdersMultipleTest()
        {
            var response = await _client.GetAsync("/v1/Orders/ListOrders/2");
            response.EnsureSuccessStatusCode();

            var responseString = JsonConvert.DeserializeObject<Order[]>(await response.Content.ReadAsStringAsync());

            Assert.Equal(2, responseString.Length);

            var order = responseString[0];

            Assert.Equal("ON00002", order.OrderNumber);
            Assert.Equal("2", order.UserId);
            Assert.Equal("Product2", order.Product);
            Assert.Equal(1500m, order.FullPrice);
            Assert.Equal(0m, order.PaidAmount);

            order = responseString[1];
            Assert.Equal("ON00004", order.OrderNumber);
            Assert.Equal("2", order.UserId);
            Assert.Equal("Product4", order.Product);
            Assert.Equal(500m, order.FullPrice);
            Assert.Equal(250m, order.PaidAmount);
        }


        [Fact]
        public async Task GetOrderTest()
        {
            var response = await _client.GetAsync("/v1/Orders/GetOrder?orderNumber=ON00002");
            response.EnsureSuccessStatusCode();

            var order = JsonConvert.DeserializeObject<Order>(await response.Content.ReadAsStringAsync());

            Assert.Equal("ON00002", order.OrderNumber);
            Assert.Equal("2", order.UserId);
            Assert.Equal("Product2", order.Product);
            Assert.Equal(1500m, order.FullPrice);
            Assert.Equal(0m, order.PaidAmount);
        }


        [Fact]
        public async Task GetOrderNotFoundTest()
        {
            var response = await _client.GetAsync("/v1/Orders/GetOrder?orderNumber=NOTFOUNDORDER");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task BrokenEndpointTest()
        {
            var response = await _client.GetAsync("/v1/Orders/GetOrdez");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task MakePaymentSuccessTest()
        {
            var stringContent = new StringContent("{  \"orderNumber\": \"ON00001\",  \"userId\": \"1\",  \"paymentAmount\": 250,  \"gateway\": 1,  \"description\": \"desc1\"}", Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/v1/Orders/Payment", stringContent);
            response.EnsureSuccessStatusCode();

            var receipt = JsonConvert.DeserializeObject<Receipt>(await response.Content.ReadAsStringAsync());

            Assert.Equal("ON00001", receipt.OrderNumber);
            Assert.Equal(Gateway.BankTransfer, receipt.Gateway);
            Assert.Equal(250m, receipt.PaidAmount);
            Assert.Equal(0m, receipt.UnpaidAmount);
        }

        [Fact]
        public async Task MakePaymentPartialTest()
        {
            var stringContent = new StringContent("{  \"orderNumber\": \"ON00001\",  \"userId\": \"1\",  \"paymentAmount\": 150,  \"gateway\": 1,  \"description\": \"desc1\"}", Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/v1/Orders/Payment", stringContent);
            response.EnsureSuccessStatusCode();

            var receipt = JsonConvert.DeserializeObject<Receipt>(await response.Content.ReadAsStringAsync());

            Assert.Equal("ON00001", receipt.OrderNumber);
            Assert.Equal(Gateway.BankTransfer, receipt.Gateway);
            Assert.Equal(150m, receipt.PaidAmount);
            Assert.Equal(100m, receipt.UnpaidAmount);
        }

        [Fact]
        public async Task MakePaymentOverflowTest()
        {
            var stringContent = new StringContent("{  \"orderNumber\": \"ON00001\",  \"userId\": \"1\",  \"paymentAmount\": 999999999,  \"gateway\": 1,  \"description\": \"desc1\"}", Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/v1/Orders/Payment", stringContent);
            response.EnsureSuccessStatusCode();

            var receipt = JsonConvert.DeserializeObject<Receipt>(await response.Content.ReadAsStringAsync());

            Assert.Equal("ON00001", receipt.OrderNumber);
            Assert.Equal(Gateway.BankTransfer, receipt.Gateway);
            Assert.Equal(999999999m, receipt.PaidAmount);
            Assert.Equal(0m, receipt.UnpaidAmount);
        }

        [Fact]
        public async Task MakePaymentNotFoundTest()
        {
            var stringContent = new StringContent("{  \"orderNumber\": \"ON00001\",  \"userId\": \"2\",  \"paymentAmount\": 999999999,  \"gateway\": 1,  \"description\": \"desc1\"}", Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/v1/Orders/Payment", stringContent);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task MakePaymentNoAmountTest()
        {
            var stringContent = new StringContent("{  \"orderNumber\": \"ON00001\",  \"userId\": \"2\",   \"gateway\": 1,  \"description\": \"desc1\"}", Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/v1/Orders/Payment", stringContent);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task MakePaymentFreeMoneyTest()
        {
            var stringContent = new StringContent("{  \"orderNumber\": \"ON00001\",  \"userId\": \"1\",  \"paymentAmount\": -100,  \"gateway\": 1,  \"description\": \"desc1\"}", Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/v1/Orders/Payment", stringContent);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}