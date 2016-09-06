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
using Android.Util;
using Autofac;
using ThriftPay.Mobile.Droid.Utilities;
using ThriftPay.Mobile.Droid.Services;

namespace ThriftPay.Mobile.Droid
{
    [Application(Icon = "@drawable/icon", Label = "@string/app_name")]
    class App : Application
    {
        const string LOG_TAG = "App";
        public static IContainer Container { get; set; }

        public App(IntPtr handle, JniHandleOwnership transfer)
            : base(handle, transfer)
        {
            Log.Debug(LOG_TAG, "Initializing application.");
        }

        public override void OnCreate()
        {
            var builder = new ContainerBuilder();

            var httpUtility = new HttpUtility();
            var accountService = new AccountService(httpUtility, this.ApplicationContext);
            var apiService = new ApiService(httpUtility, this.ApplicationContext, accountService);
            var userService = new UserService(apiService, this.ApplicationContext);

            builder.RegisterInstance(this.ApplicationContext).As<Context>();
            builder.RegisterInstance(httpUtility).As<HttpUtility>();
            builder.RegisterInstance(accountService).As<AccountService>();
            builder.RegisterInstance(apiService).As<ApiService>();
            builder.RegisterInstance(userService).As<UserService>();

            App.Container = builder.Build();

            base.OnCreate();
        }
    }
}