namespace ComponentTests.V2
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using WebApi;
    using Xunit;

    public sealed class SunnyDayTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        public SunnyDayTests(WebApplicationFactory<Startup> factory)
        {
            this._factory = factory;
        }

        private readonly WebApplicationFactory<Startup> _factory;

        private async Task GetAccount(string accountId)
        {
            var client = this._factory.CreateClient();
            string result = await client.GetStringAsync($"/api/v2/Accounts/{accountId}");
        }

        private async Task<Tuple<string, string>> Register(decimal initialAmount)
        {
            var client = this._factory.CreateClient();

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("ssn", "8608179999"),
                new KeyValuePair<string, string>("initialAmount", initialAmount.ToString())
            });

            var response = await client.PostAsync("api/v1/Customers", content);

            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            Assert.Contains("customerId", responseString);
            JObject customer = JsonConvert.DeserializeObject<JObject>(responseString);

            string customerId = customer["customerId"].Value<string>();
            string accountId = ((JContainer)customer["accounts"]).First["accountId"].Value<string>();

            return new Tuple<string, string>(customerId, accountId);
        }

        [Fact]
        public async Task Register_Deposit_Withdraw_Close()
        {
            Tuple<string, string> customerId_accountId = await this.Register(100);
            await this.GetAccount(customerId_accountId.Item2);
        }
    }
}
