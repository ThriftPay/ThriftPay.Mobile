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
using Android;
using Autofac;
using ThriftPay.Mobile.Droid.Services;
using Android.Util;

namespace ThriftPay.Mobile.Droid.Activities
{
    [Activity(Label = "Sign in")]
    class SigninActivity : Activity
    {
        const string LOG_TAG = "SigninActivity";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var authService = App.Container.Resolve<AuthService>();

            SetContentView(Resource.Layout.Signin);

            var editTextUsername = FindViewById<EditText>(Resource.Id.editTextUsername);
            var editTextPassword = FindViewById<EditText>(Resource.Id.editTextPassword);


            if(bundle != null)
            {
                var username = bundle.GetString("username", null);
                var password = bundle.GetString("password", null);

                if (!string.IsNullOrEmpty(username))
                {
                    editTextUsername.Text = username;
                }
                if (!string.IsNullOrEmpty(password))
                {
                    editTextPassword.Text = password;
                }
            }

            var buttonSigninSubmit = FindViewById<Button>(Resource.Id.buttonSigninSubmit);

            buttonSigninSubmit.Click += async (sender, args) =>
            {
                try
                {
                    await authService.GetTokenAsync(editTextUsername.Text, editTextPassword.Text);

                    StartActivity(typeof(MainActivity));

                    Finish();
                }
                catch (AuthenticationException ex)
                {
                    Log.Debug(LOG_TAG, ex.Message);

                    Toast.MakeText(this, App.Container.Resolve<Context>().GetString(Resource.String.signin_invalid_username_or_password), ToastLength.Short).Show();
                }
                catch (Exception ex)
                {
                    Log.Debug(LOG_TAG, ex.Message);

                    Toast.MakeText(this, App.Container.Resolve<Context>().GetString(Resource.String.signin_exception), ToastLength.Short).Show();
                }
            };

            var buttonStartSignup = FindViewById<Button>(Resource.Id.buttonStartSignup);

            buttonStartSignup.Click += (sender, arg) =>
            {
                StartActivity(typeof(SignupActivity));
            };
        }
    }
}