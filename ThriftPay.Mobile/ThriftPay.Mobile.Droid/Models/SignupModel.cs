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

namespace ThriftPay.Mobile.Droid.Models
{
    public class SignupModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}