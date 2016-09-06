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
using Android.Support.V7.App;
using Autofac;
using ThriftPay.Mobile.Droid.Services;
using Android.Util;

namespace ThriftPay.Mobile.Droid.Activities
{
    [Activity(Label = "@string/app_name")]
    public class MainActivity : AppCompatActivity
    {
        const string LOG_TAG = "MainActivity";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            var buttonSignout = FindViewById<Button>(Resource.Id.buttonSignout);

            buttonSignout.Click += (sender, args) => {
                var accountService = App.Container.Resolve<AccountService>();

                try
                {
                    accountService.SignoutAsync();

                    var intent = new Intent(this, typeof(SigninActivity));

                    StartActivity(intent);

                    Finish();
                }catch(Exception ex)
                {
                    Log.Debug(LOG_TAG, ex.Message);

                    Toast.MakeText(this, "An unexpected error occured.", ToastLength.Short).Show();
                }
            };
        }
    }
}