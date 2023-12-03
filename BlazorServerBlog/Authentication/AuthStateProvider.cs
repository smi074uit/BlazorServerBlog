using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using BlazorServerBlog.Services;

    namespace BlazorServerBlog.Authentication
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        // private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorage;
        private readonly IApiHelper apiHelper;
        private readonly IConfiguration config;
        private readonly AuthenticationState anonymous;
        private readonly string token = "";
        private string authTokenStorageKey;

        public AuthStateProvider(ILocalStorageService localStorage, IApiHelper apiHelper, IConfiguration config)
        {
            authTokenStorageKey = config.GetSection("APIConnection:StorageKey").Value!;
            // this.httpClient = httpClient;
            this.localStorage = localStorage;
            this.apiHelper = apiHelper;
            this.config = config;
            anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await localStorage.GetItemAsync<string>(authTokenStorageKey);


            if (string.IsNullOrWhiteSpace(token))
            {
                return anonymous;
            }

            bool isAuthenticated = await NotifyUserAuthentication(token);
            if (!isAuthenticated)
            {
                return anonymous;
            }

            //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType")));
        }

        public async Task<bool> NotifyUserAuthentication(string token)
        {
            bool isAuthenticatedOutput;
            Task<AuthenticationState> authState;
            try
            {
                apiHelper.SetClientHeaders(token);
                var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType"));
                authState = Task.FromResult(new AuthenticationState(authenticatedUser));
                NotifyAuthenticationStateChanged(authState);
                isAuthenticatedOutput = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                //authState = Task.FromResult(anonymous);
                await NotifyUserLogout();
                isAuthenticatedOutput = false;

            }
            return isAuthenticatedOutput;



        }

        public async Task NotifyUserLogout()
        {
            await localStorage.RemoveItemAsync("authToken");
            NotifyAuthenticationStateChanged(Task.FromResult(anonymous));
            apiHelper.BlankClientHeaders();
            //  httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}

