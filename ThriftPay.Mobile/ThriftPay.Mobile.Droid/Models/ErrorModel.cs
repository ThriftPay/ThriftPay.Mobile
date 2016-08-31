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
using Newtonsoft.Json;

namespace ThriftPay.Mobile.Droid.Models
{
    public class ErrorModel
    {
        public ErrorModel(string error, string errorDescription = null)
        {
            Error = error;
            ErrorDescription = ErrorDescription;
        }

        public ErrorModel()
        {

        }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
    }
}