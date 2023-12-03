using Blazored.LocalStorage;
using BlazorServerBlog.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using SharedModels.Entities.Account;
using System.IdentityModel.Tokens.Jwt;

namespace BlazorServerBlog.Services
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly IApiHelper _api;
        private readonly ILocalStorageService localStorage;
        private readonly IConfiguration config;
        private readonly AuthenticationStateProvider authStateProvider;
        private string authTokenStorageKey;

        public AuthenticationService(IApiHelper api,
                                     ILocalStorageService localStorage,
                                     IConfiguration config,
                                     AuthenticationStateProvider authStateProvider)
		{
			this._api = api;
            this.localStorage = localStorage;
            this.config = config;
            this.authStateProvider = authStateProvider;
            authTokenStorageKey = config.GetSection("APIConnection:StorageKey").Value!;
        }

		public bool register(RegisterRequest user)
		{
			var response = _api.PostData(user, "Account/registerNewUser");

			if (!response.IsSuccessStatusCode)
			{
				return false;
			}

			return true;
		}

        public async Task<bool> login(UserDTO user)
        {

            var response = _api.PostData(user, "Account/verifyLogin");

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            User result = await response.Content.ReadFromJsonAsync<User>();

            await localStorage.SetItemAsync(authTokenStorageKey, result.Token);

            await ((AuthStateProvider)authStateProvider).NotifyUserAuthentication(result.Token);
            _api.SetClientHeaders(result.Token);

            return true;
        }

        public async Task logout()
        {
            _api.BlankClientHeaders();
            await localStorage.RemoveItemAsync(authTokenStorageKey);

			await ((AuthStateProvider)authStateProvider).NotifyUserLogout();
		}
    }
}
