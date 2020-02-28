using System;
using System.Configuration;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Polly;
using ShipModel;

namespace Ship
{
    public class BattleshipsImpl : IBattleships
    {
        #region Configuration Settings
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenant = ConfigurationManager.AppSettings["ida:Tenant"];
        private static string clientId = ConfigurationManager.AppSettings["ida:TeamAppId"];
        private static string clientSecret = ConfigurationManager.AppSettings["ida:TeamAppSecret"];
        private static Uri redirectUri = new Uri(ConfigurationManager.AppSettings["ida:RedirectUri"]);
        private static string authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);

        private static string GameAPIAppId = ConfigurationManager.AppSettings["todo:GameAPIAppId"];
        private static string GameAPIBaseAddress = ConfigurationManager.AppSettings["todo:GameAPIBaseAddress"];

        public string AccessToken { get; private set; }
        #endregion

        public BattleshipsImpl()
        {
            AuthenticateToAzureAD();

            DemoCallAPI();
        }

        public void DemoCallAPI()
        {
            Task t2 = Task.Run(async () =>
            {
                // Retrieve the data
                Console.WriteLine("Calling test API....");
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, GameAPIBaseAddress + "/api/test");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                HttpResponseMessage response = await client.SendAsync(request);

                Console.WriteLine("Call sucessfull = " + response.IsSuccessStatusCode);
                Console.WriteLine("Result: HTTP{0}", response.StatusCode);
                Console.WriteLine("Response success/error message = " + response.ReasonPhrase);

                string responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response data = " + responseString);
                Console.WriteLine();
            });
            t2.Wait();
        }

        public void AuthenticateToAzureAD()
        {
            // Using Active Directory Authentication Library (ADAL).
            AuthenticationResult result = null;
            AuthenticationContext authContext = new AuthenticationContext(authority, new FileTokenCache());

            Console.WriteLine("Authenticating via Azure AD....");
            Task t1 = Task.Run(async () =>
            {
                // Make connection using application credientials: uses appid + secret.
                ClientCredential clientCredential = new ClientCredential(clientId, clientSecret);
                result = await authContext.AcquireTokenAsync(GameAPIAppId, clientCredential);

                AccessToken = result.AccessToken;
                Console.WriteLine("Access Token = " + AccessToken);
                Console.WriteLine();
            });
            t1.Wait();
        }

        public MasterGameState StatusAPI()
        {
            MasterGameState state = null;

            Task t2 = Task.Run(async () =>
            {
                // Retrieve the data
                Console.WriteLine("Calling Status API....");
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, GameAPIBaseAddress + "/api/status");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                HttpResponseMessage response = await client.SendAsync(request);

                Console.WriteLine("Call sucessfull = " + response.IsSuccessStatusCode);
                Console.WriteLine("Result: HTTP{0}", response.StatusCode);
                Console.WriteLine("Response success/error message = " + response.ReasonPhrase);

                string responseString = await response.Content.ReadAsStringAsync();
                state = JsonConvert.DeserializeObject<MasterGameState>(responseString);
            });
            t2.Wait();

            return (state);
        }

        public void PlaceShip(string payload)
        {
            Console.WriteLine("Placing test ship....");
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, GameAPIBaseAddress + "/api/placeship");
            request.Content = new System.Net.Http.StringContent(payload, Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            Task t = Task.Run(async () =>
            {
                HttpResponseMessage response = await client.SendAsync(request);
                Console.WriteLine("Call sucessfull = " + response.IsSuccessStatusCode);
                Console.WriteLine("Result: HTTP{0}", response.StatusCode);
                Console.WriteLine("Response success/error message = " + response.ReasonPhrase);

                string responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response data = " + responseString);
                Console.WriteLine();
            });

            t.Wait();

        }

        public bool Shoot(int x, int y)
        {
            string responseString = null;

            Console.WriteLine("Taking a shot....");
            HttpClient client = new HttpClient();

            Task t = Task.Run(async () =>
            {
                while (true)
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, GameAPIBaseAddress + "/api/shoot");
                    request.Content = new System.Net.Http.StringContent("{'positionX': " + x + ",'positionY': " + y + "}", Encoding.UTF8, "application/json");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

                    HttpResponseMessage response = await client.SendAsync(request);

                    Console.WriteLine("Call sucessfull = " + response.IsSuccessStatusCode);

                    Console.WriteLine("Result: HTTP{0}", response.StatusCode);
                    Console.WriteLine("Response success/error message = " + response.ReasonPhrase);

                    responseString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response data = " + responseString);
                    Console.WriteLine();

                    if (response.IsSuccessStatusCode)
                    {
                        break;
                    }
                    else
                    {
                        Task.Delay(5000).Wait(); // Wait 2 seconds with blocking
                        await Task.Delay(5000);
                    }
                }
            });

            t.Wait();

            return Boolean.Parse(responseString);
        }
    }
}
