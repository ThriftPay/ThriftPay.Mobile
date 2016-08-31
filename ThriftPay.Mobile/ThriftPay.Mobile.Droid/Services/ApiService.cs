using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ThriftPay.Mobile.Droid.Utilities;
using Android.Util;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace ThriftPay.Mobile.Droid.Services
{
    public class ApiService
    {
        const string LOG_TAG = "ApiService";

        private HttpUtility HttpUtility;
        private Context Context;

        private AuthService AuthService;
        private string BaseUrl;
        private string ApiVersion;

        private string GetAbsoluteUrl(string url)
        {
            var baseUrl = this.BaseUrl;

            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }

            baseUrl += this.ApiVersion + "/";

            return baseUrl + url;
        }

        public ApiService(HttpUtility httpUtility, Context context, AuthService authService)
        {
            Log.Debug(LOG_TAG, "Initializing ApiService.");

            this.HttpUtility = httpUtility;
            this.Context = context;
            this.AuthService = authService;

            this.BaseUrl = Context.GetString(Resource.String.api_base_url);
            this.ApiVersion = Context.GetString(Resource.String.api_version);
        }

        public async Task<HttpResponseMessage> PostAsync(string url, IDictionary<string, string> data = null)
        {
            var options = new HttpOptions();

            options.Method = HttpMethod.Post;
            options.Url = url;
            options.Data = data;

            return await this.RequestAsync(options);
        }

        public async Task<HttpResponseMessage> GetAsync(string url, IDictionary<string, string> parameters = null)
        {
            var options = new HttpOptions();

            options.Method = HttpMethod.Get;
            options.Url = url;
            options.Params = parameters;

            return await this.RequestAsync(options);
        }

        public async Task<HttpResponseMessage> RequestAsync(HttpOptions options)
        {
            options.Url = GetAbsoluteUrl(options.Url);

            options.Headers = new Dictionary<string, string>();

            options.Headers.Add("Content-Type", "application/json");

            var accessToken = this.AuthService.GetAccessToken();

            if (!string.IsNullOrEmpty(accessToken))
            {
                Log.Debug(LOG_TAG, $"Adding 'Authorization' header 'Bearer {accessToken}'");

                options.Headers.Add("Authorization", $"Bearer {accessToken}");
            }

            var response = await this.HttpUtility.RequestAsync(options);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Log.Debug(LOG_TAG, $"Unauthorized request. Access token may have expired.");

                var refreshToken = this.AuthService.GetRefreshToken();

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    Log.Debug(LOG_TAG, $"Attempting to refresh the access token.");

                    this.AuthService.RefreshTokenAsync();

                    accessToken = this.AuthService.GetAccessToken();

                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        Log.Debug(LOG_TAG, $"Retrying the request with 'Authorization' header 'Bearer {accessToken}'.");

                        options.Headers.Add("Authorization", $"Bearer {accessToken}");

                        return await this.HttpUtility.RequestAsync(options);
                    }

                }
            }

            return response;
        }
    }
}