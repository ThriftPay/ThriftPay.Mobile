using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using Autofac;
using ThriftPay.Mobile.Droid.Services;

namespace ThriftPay.Mobile.Droid.Activities
{
	[Activity (Label = "@string/app_name", MainLauncher = true, Theme = "@style/ThriftPayTheme.Splash", Icon = "@drawable/icon", NoHistory = true)]
	public class SplashActivity : Activity
	{
        protected override async void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

            // ensure that the system bar color gets drawn
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            // await a new task
            await Task.Factory.StartNew(() => {

                var context = App.Container.Resolve<Context>();
                var accountService = App.Container.Resolve<AccountService>();

                if(accountService.IsSignedIn)
                {
                    StartActivity(new Intent(context, typeof(MainActivity)));
                }
                else
                {
                    StartActivity(new Intent(context, typeof(SigninActivity)));
                }
            });            
		}
	}
}


