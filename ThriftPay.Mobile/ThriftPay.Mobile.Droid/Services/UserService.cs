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

namespace ThriftPay.Mobile.Droid.Services
{
    public class UserService
    {
        private ApiService ApiService;
        private Context Context;

        public UserService(ApiService apiService, Context context)
        {
            this.ApiService = apiService;
            this.Context = context;
        }

        public async Task<ApiResponse<UserModel>> SignupAsync(SignupModel model)
        {
            try
            {
                var response = await this.ApiService.PostAsync(Context.GetString(Resource.String.signup_endpoint));

                var responseText = await response.Content.ReadAsStringAsync();

                var apiResponse = new ApiResponse<UserModel>();

                if (response.IsSuccessStatusCode)
                {
                    await Task.Factory.StartNew(() =>
                    {
                        apiResponse.Data = JsonConvert.DeserializeObject<UserModel>(responseText);
                    });
                }
                else
                {
                    await Task.Factory.StartNew(() =>
                    {
                        apiResponse.Error = JsonConvert.DeserializeObject<ErrorModel>(responseText);
                    });
                }

                return apiResponse;
            }catch(Exception ex)
            {
                throw new ApiException(ex.Message);
            }
        }
    }
}