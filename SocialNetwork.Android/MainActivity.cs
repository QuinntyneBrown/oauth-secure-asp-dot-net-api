using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Widget;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SocialNetwork.Android
{
    [Activity(Label = "SocialNetwork.Android", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private ProgressDialog progressDialog;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            progressDialog = new ProgressDialog(this);
            progressDialog.SetTitle("Logging in..");
            progressDialog.SetCancelable(false);

            SetContentView(Resource.Layout.Main);

            FindViewById<Button>(Resource.Id.loginButton).Click += async (sender, args) =>
            {
                progressDialog.Show();

                var token = await GetTokenAsync();

                FindViewById<TextView>(Resource.Id.accessToken).Text
                    = token.AccessToken;

                progressDialog.Hide();
            };
        }

        private async Task<TokenResponse> GetTokenAsync()
        {
            using (var client = new HttpClient())
            {
                var basicAuth =
                    Convert.ToBase64String(
                        Encoding.UTF8.GetBytes("socialnetwork:secret"));

                client.DefaultRequestHeaders.Authorization
                    = new AuthenticationHeaderValue("Basic", basicAuth);

                var rawResult = await client.PostAsync("http://192.168.0.3:22710/connect/token",
                    new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>(
                            "grant_type",
                            "password"),

                        new KeyValuePair<string, string>(
                            "scope",
                            "openid profile offline_access"),

                        new KeyValuePair<string, string>(
                            "username",
                            FindViewById<EditText>(Resource.Id.usernameText).Text),

                        new KeyValuePair<string, string>(
                            "password",
                            FindViewById<EditText>(Resource.Id.passwordText).Text),
                    }));

                var data = await rawResult.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<TokenResponse>(data);
            }
        }
    }

    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}