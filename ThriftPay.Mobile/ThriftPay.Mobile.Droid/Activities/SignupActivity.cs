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
using Android.Support.V7.App;

namespace ThriftPay.Mobile.Droid.Activities
{
    [Activity(Label = "Sign up")]
    public class SignupActivity : AppCompatActivity
    {
        const string LOG_TAG = "SignupActivity";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Signup);

            var editTextUsername = FindViewById<EditText>(Resource.Id.editTextUsername);
            var editTextPassword = FindViewById<EditText>(Resource.Id.editTextPassword);

            var buttonSignupSubmit = FindViewById<Button>(Resource.Id.buttonSignupSubmit);

            buttonSignupSubmit.Click += async (sender, args) =>
            {
                var accountService = App.Container.Resolve<AccountService>();

                var model = new SignupModel()
                {
                    Username = editTextUsername.Text.Trim().ToLower(),
                    Password = editTextPassword.Text.Trim(),
                };

                try
                {
                    var userModel = await accountService.SignupAsync(model);

                    try
                    {
                        await accountService.GetTokenAsync(model.Username, model.Password);

                        StartActivity(typeof(MainActivity));
                    }
                    catch (Exception ex)
                    {
                        Log.Debug(LOG_TAG, ex.Message);

                        var intent = new Intent(this, typeof(SigninActivity));

                        intent.PutExtra("username", model.Username);
                        intent.PutExtra("password", model.Password);

                        StartActivity(intent);
                    }

                    Finish();

                }
                catch (ApiException ex)
                {
                    Log.Debug(LOG_TAG, ex.Message);

                    Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                }
                catch (Exception ex)
                {
                    Log.Debug(LOG_TAG, ex.Message);

                    Toast.MakeText(this, App.Container.Resolve<Context>().GetString(Resource.String.signup_exception), ToastLength.Short).Show();
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