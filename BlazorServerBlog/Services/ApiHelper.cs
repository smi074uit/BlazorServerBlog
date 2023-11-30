using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using SharedModels.Entities.Account;

namespace BlazorServerBlog.Services
{
	public class ApiHelper : IApiHelper
	{

		public HttpClient client;
		private readonly IConfiguration config;
		private readonly ILogger logger;
		private object _lock = new object();


		public ApiHelper(IConfiguration config, ILogger<ApiHelper> logger)
		{
			this.config = config;
			this.logger = logger;
			client = new();
			client.BaseAddress = new Uri(config.GetSection("APIConnection:BlogURL").Value!);
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


		}


		public async Task<bool> login(UserDTO user)
		{
			//UserDTO user = new()
			//{ Username = username, Password = password };

			var response = PostData(user, "Account/verifyLogin");

			if (!response.IsSuccessStatusCode)
			{
				return false;
			}

			User result = await response.Content.ReadFromJsonAsync<User>();
			
			var responseContent = response.Content.ReadAsStringAsync().Result;
			var handler = new JwtSecurityTokenHandler();


			JwtSecurityToken jwt = new JwtSecurityToken(result.Token);

			client.DefaultRequestHeaders.Clear();
			client.DefaultRequestHeaders.Add("Authorization", $"Bearer {result.Token}");

			return true;
		}

		public void SetClientHeaders(string token)
		{
			var handler = new JwtSecurityTokenHandler();
			JwtSecurityToken jwt = new JwtSecurityToken(token.Trim('"'));

			client.DefaultRequestHeaders.Clear();
			client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.Trim('"')}");

		}

		public void BlankClientHeaders()
		{
			client.DefaultRequestHeaders.Clear();

		}

		public void RefreshLogin()
		{

		}


		public HttpResponseMessage PostData<T>(T input, string endpoint)
		{
			StringContent content = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, "application/json");
			var response = client.PostAsync(endpoint, content).Result;
			var responseContent = response.Content.ReadAsStringAsync().Result;
			return response;
		}


		public HttpResponseMessage PostDataString(string input, string endpoint)
		{
			StringContent content = new StringContent(input);
			var response = client.PostAsync(endpoint, content).Result;
			var responseContent = response.Content.ReadAsStringAsync().Result;
			return response;
		}

		public HttpResponseMessage PutData<T>(T input, string endpoint)
		{
			StringContent content = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, "application/json");
			var response = client.PutAsync(endpoint, content).Result;
			var responseContent = response.Content.ReadAsStringAsync().Result;
			return response;
		}

		public HttpResponseMessage GetData(string endpoint)
		{
			var response = client.GetAsync(endpoint).Result;
			var responseContent = response.Content.ReadAsStringAsync().Result;
			return response;
		}

		public HttpResponseMessage DeleteData(string endpoint)
		{

			var response = client.DeleteAsync(endpoint).Result;
			var responseContent = response.Content.ReadAsStringAsync().Result;
			return response;
		}


	}
}
