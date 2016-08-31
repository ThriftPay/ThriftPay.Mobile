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
using ThriftPay.Mobile.Droid.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Android.Util;

namespace ThriftPay.Mobile.Droid.Services
{
    public class UserService
    {
        private ApiService ApiService;
        private Context Context;
        const string LOG_TAG = "UserService";

        public UserService(ApiService apiService, Context context)
        {
            this.ApiService = apiService;
            this.Context = context;
        }
    }
}