using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace DSelling.IdentiityServer.Client
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			var client = new HttpClient();
			var disco = client.GetDiscoveryDocumentAsync("https://localhost:5001").Result;
			if (disco.IsError)
			{
				Console.WriteLine(disco.Error);
				return;
			}

			var tokenResponse = client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
			{
				Address = disco.TokenEndpoint,

				ClientId = "client",
				ClientSecret = "secret",
				Scope = "api1"
			}).Result;

			if (tokenResponse.IsError)
			{
				Console.WriteLine(tokenResponse.Error);
				return;
			}

			Console.WriteLine(tokenResponse.Json);


			// call api
			var apiClient = new HttpClient();
			apiClient.SetBearerToken(tokenResponse.AccessToken);

			var response = apiClient.GetAsync("https://localhost:5002/identity").Result;
			if (!response.IsSuccessStatusCode)
			{
				Console.WriteLine(response.StatusCode);
			}
			else
			{
				var content = response.Content.ReadAsStringAsync().Result;
				Console.WriteLine(JArray.Parse(content));
			}
		}
	}
}
