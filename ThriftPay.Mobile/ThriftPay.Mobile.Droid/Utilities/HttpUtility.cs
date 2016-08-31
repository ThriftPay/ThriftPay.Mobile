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
using System.Net.Http;
using Android.Util;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ThriftPay.Mobile.Droid.Utilities
{
    public class HttpUtility
    {
        const string LOG_TAG = "HttpUtility";
        private HttpClient Client;
        public HttpUtility()
        {
            Log.Debug(LOG_TAG, "Initializing HttpUtility.");

            this.Client = this.GetHttpClient();
        }
        private HttpClient GetHttpClient()
        {
            var client = new HttpClient();

            return client;
        }

        public async Task<HttpResponseMessage> PostAsync(string url, IDictionary<string, string> data = null, IDictionary<string, string> headers = null)
        {
            var options = new HttpOptions();

            options.Method = HttpMethod.Post;
            options.Url = url;
            options.Data = data;
            options.Headers = headers;

            return await this.RequestAsync(options);
        }

        public async Task<HttpResponseMessage> GetAsync(string url, IDictionary<string, string> parameters = null, IDictionary<string, string> headers = null)
        {
            var options = new HttpOptions();

            options.Method = HttpMethod.Get;
            options.Url = url;
            options.Params = parameters;
            options.Headers = headers;

            return await this.RequestAsync(options);
        }

        public async Task<HttpResponseMessage> RequestAsync(HttpOptions options)
        {
            var method = options.Method;
            var url = options.Url;

            var request = new HttpRequestMessage(method, url);

            if (options.Data == null)
            {
                options.Data = new Dictionary<string, string>();
            }

            if (options.Headers == null)
            {
                options.Headers = new Dictionary<string, string>();
            }
            if (!options.Headers.ContainsKey("Content-Type"))
            {
                options.Headers.Add("Content-Type", "application/json");
            }

            if (method == HttpMethod.Post)
            {
                if (options.Headers["Content-Type"] == "application/x-www-form-url-urlencoded")
                {
                    request.Content = new FormUrlEncodedContent(options.Data);
                }
                else
                {
                    request.Content = new StringContent(JsonConvert.SerializeObject(options.Data), Encoding.UTF8, options.Headers["Content-Type"]);
                }
            }

            return await this.Client.SendAsync(request);
        }
    }
}