using System.Collections.Generic;
using Android.Content;
using ThriftPay.Mobile.Droid.Utilities;
using Android.Util;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ThriftPay.Mobile.Droid.Models;
using System;

namespace ThriftPay.Mobile.Droid.Services
{
    public class AuthService
    {
        const string LOG_TAG = "AuthService";
        const string AUTH_PREFERENCES = "Auth";

        private HttpUtility HttpUtility;
        private Context Context;

        private string BaseUrl;
        private string ClientId;
        private string ClientSecret;

        public bool IsSignedIn
        {
            get
            {
                return this.GetAccessToken() != null;
            }
        }

        public AuthService(HttpUtility httpUtility, Context context)
        {
            Log.Debug(LOG_TAG, "Initializing AuthService.");

            this.HttpUtility = httpUtility;
            this.Context = context;

            this.BaseUrl = Context.GetString(Resource.String.auth_server_base_url);
            this.ClientId = Context.GetString(Resource.String.auth_server_client_id);
            this.ClientSecret = Context.GetString(Resource.String.auth_server_client_secret);
        }

        private string GetAbsoluteUrl(string url)
        {
            var baseUrl = this.BaseUrl;

            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }

            return baseUrl + url;
        }

        public async Task<UserModel> SignupAsync(SignupModel model)
        {
            try
            {
                var data = new Dictionary<string, string>() {
                    {"username", model.Username},
                    {"password", model.Password}
                };

                var url = this.GetAbsoluteUrl(Context.GetString(Resource.String.auth_server_signup_endpoint));

                var response = await this.HttpUtility.PostAsync(url, data);

                var responseText = await response.Content.ReadAsStringAsync();

                Log.Debug(LOG_TAG, responseText);

                if (response.IsSuccessStatusCode)
                {
                    var userModel = JsonConvert.DeserializeObject<UserModel>(responseText);

                    return userModel;
                }
                else
                {
                    var errorData = JsonConvert.DeserializeObject<ErrorModel>(responseText);

                    if (errorData == null)
                    {
                        errorData = new ErrorModel(response.StatusCode.ToString());
                    }

                    throw new ApiException($"{errorData.Error}: {errorData.ErrorDescription}");
                }
            }
            catch (Exception ex)
            {
                Log.Error(LOG_TAG, ex.Message);

                throw ex;
            }
        }

        public async Task GetTokenAsync(string username, string password)
        {
            var url = this.GetAbsoluteUrl(Context.GetString(Resource.String.auth_server_token_endpoint));

            var data = new Dictionary<string, string>() {
                {"grant_type", "password"},
                {"client_id", this.ClientId },
                //{"client_secret", this.ClientSecret },// Don't send secret for now
                {"username", username },
                {"password", password },
                {"scope", "openid offline_access profile email"}
            };

            var headers = new Dictionary<string, string>() {
               { "Content-Type", "application/x-www-form-url-urlencoded" }
            };

            try
            {
                var response = await HttpUtility.PostAsync(url, data, headers);
                var responseContent = await response.Content.ReadAsStringAsync();
                Log.Debug(LOG_TAG, responseContent);

                if (response.IsSuccessStatusCode)
                {
                    var token = JsonConvert.DeserializeObject<TokenModel>(responseContent);

                    Log.Debug(LOG_TAG, "access_token: " + token.AccessToken);

                    var editor = this.GetAuthPreferences().Edit();

                    editor.PutString("access_token", token.AccessToken);

                    if (!string.IsNullOrEmpty(token.RefreshToken))
                    {
                        editor.PutString("refresh_token", token.RefreshToken);
                    }

                    if (!string.IsNullOrEmpty(token.IdToken))
                    {
                        editor.PutString("id_token", token.IdToken);
                    }

                    if (!editor.Commit())
                    {
                        throw new AuthenticationException("Unable to save authentication information.");
                    }
                }
                else
                {
                    var errorData = JsonConvert.DeserializeObject<ErrorModel>(responseContent);

                    if (errorData == null)
                    {
                        errorData = new ErrorModel(response.StatusCode.ToString());
                    }

                    throw new AuthenticationException($"{errorData.Error}: {errorData.ErrorDescription}");
                }
            }
            catch (Exception ex)
            {
                Log.Error(LOG_TAG, ex.Message);

                throw ex;
            }
        }

        public async void RefreshTokenAsync()
        {
            var url = this.GetAbsoluteUrl(Context.GetString(Resource.String.auth_server_token_endpoint));

            var refreshToken = this.GetRefreshToken();

            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new AuthenticationException("No refresh token was found in storage.");
            }

            var data = new Dictionary<string, string>() {
                {"grant_type", "refresh_token"},
                {"client_id", this.ClientId },
                {"client_secret", this.ClientSecret },
                {"refresh_token", refreshToken }
            };

            var headers = new Dictionary<string, string>() {
               { "Content-Type", "application/x-www-form-url-urlencoded" }
            };

            try
            {
                var response = await HttpUtility.PostAsync(url, data, headers);
                var responseContent = await response.Content.ReadAsStringAsync();
                Log.Debug(LOG_TAG, responseContent);

                if (response.IsSuccessStatusCode)
                {
                    var token = JsonConvert.DeserializeObject<TokenModel>(responseContent);

                    Log.Debug(LOG_TAG, "access_token: " + token.AccessToken);

                    var editor = this.GetAuthPreferences().Edit();

                    editor.PutString("access_token", token.AccessToken);

                    if (!string.IsNullOrEmpty(token.RefreshToken))
                    {
                        editor.PutString("refresh_token", token.RefreshToken);
                    }

                    if (!string.IsNullOrEmpty(token.IdToken))
                    {
                        editor.PutString("id_token", token.IdToken);
                    }

                    if (!editor.Commit())
                    {
                        throw new AuthenticationException("Unable to save authentication information.");
                    }
                }
                else
                {
                    var errorData = JsonConvert.DeserializeObject<ErrorModel>(responseContent);

                    if (errorData == null)
                    {
                        errorData = new ErrorModel(response.StatusCode.ToString());
                    }

                    throw new AuthenticationException($"{errorData.Error}: {errorData.ErrorDescription}");
                }
            }
            catch (Exception ex)
            {
                Log.Debug(LOG_TAG, ex.Message);

                throw ex;
            }
        }

        public string GetAccessToken()
        {
            var preferences = this.GetAuthPreferences();

            if (preferences == null)
            {
                return null;
            }

            var accessToken = preferences.GetString("access_token", null);

            return accessToken;
        }

        public string GetRefreshToken()
        {
            var preferences = this.GetAuthPreferences();

            if (preferences == null)
            {
                return null;
            }

            var accessToken = preferences.GetString("refresh_token", null);

            return accessToken;
        }

        protected ISharedPreferences GetAuthPreferences()
        {
            var authPreferences = this.Context.ApplicationContext.GetSharedPreferences(AUTH_PREFERENCES, FileCreationMode.Private);

            return authPreferences;
        }
    }
}