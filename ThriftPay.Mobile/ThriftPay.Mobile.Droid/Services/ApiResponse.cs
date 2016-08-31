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

namespace ThriftPay.Mobile.Droid.Services
{
    public class ApiResponse<T>
    {
        public ErrorModel Error { get; set; }
        public bool IsSuccessful
        {
            get
            {
                return this.Error == null;
            }
        }

        public T Data { get; set; }

        public ApiResponse()
        {
        }

        public ApiResponse(T data)
        {
            this.Data = data;
        }

        public ApiResponse(ErrorModel error)
        {
            this.Error = error;
        }
    }
}