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
using ThriftPay.Mobile.Droid.Services;
using Autofac;
using ThriftPay.Mobile.Droid.Models;
using Android.Util;

namespace ThriftPay.Mobile.Droid.Activities
{
    [Activity(Label = "Sign up")]
    public class SignupActivity : Activity
    {
        const string LOG_TAG = "SignupActivity";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Signup);

            var userService = App.Container.Resolve<UserService>();

            var editTextUsername = FindViewById<EditText>(Resource.Id.editTextUsername);
            var editTextPassword = FindViewById<EditText>(Resource.Id.editTextPassword);

            var buttonSignupSubmit = FindViewById<Button>(Resource.Id.buttonSignupSubmit);

            buttonSignupSubmit.Click += async (sender, args) =>
            {
                var model = new SignupModel()
                {
                    Username = editTextUsername.Text,
                    Password = editTextPassword.Text,
                };

                try
                {
                    await userService.SignupAsync(model);

                    var authService = App.Container.Resolve<AuthService>();

                    try
                    {
                        await authService.GetTokenAsync(model.Username, model.Password);

                        StartActivity(typeof(MainActivity));

                    }catch(Exception ex)
                    {
                        Log.Debug(LOG_TAG, ex.Message);

                        var intent = new Intent(this, typeof(SigninActivity));

                        intent.PutExtra("username", "");
                        intent.PutExtra("password", "");

                        StartActivity(intent);
                    }

                    Finish();
                }
                catch (Exception ex)
                {
                    Log.Debug(LOG_TAG, ex.Message);

                    Toast.MakeText(this, "Unable to sign up at this time. Try again later.", ToastLength.Short).Show();
                }
            };

            var buttonStartSignin = FindViewById<Button>(Resource.Id.buttonStartSignin);

            buttonStartSignin.Click += (sender, arg) =>
            {
                StartActivity(typeof(SigninActivity));

                Finish();
            };
        }
    }
}