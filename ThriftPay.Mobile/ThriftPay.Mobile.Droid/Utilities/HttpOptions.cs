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

namespace ThriftPay.Mobile.Droid.Utilities
{
    public class HttpOptions
    {
        public HttpMethod Method { get; set; }
        public string Url { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public IDictionary<string, string> Data { get; set; }
        public IDictionary<string, string> Params { get; set; }
    }
}