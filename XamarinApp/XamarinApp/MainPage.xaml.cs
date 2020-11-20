using IdentityModel.OidcClient.Browser;
using IdentityModel.OidcClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net.Http.Headers;

namespace XamarinApp
{
    public partial class MainPage : ContentPage
    {
        OidcClient _client;
        LoginResult _result;

        Lazy<HttpClient> _apiClient = new Lazy<HttpClient>(() => new HttpClient());
        public MainPage()
        {
            //注意这里需要通过命令打开服务器和接口

            InitializeComponent();

            var browser = DependencyService.Get<IBrowser>();

            var options = new OidcClientOptions
            {
                Authority = "http://127.0.0.1:5000",
                ClientId = "xamarin",
                Scope = "openid ApiOne",
                RedirectUri = "xamarinformsclients://callback",
                Browser = browser,

                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect
            };

            options.Policy.Discovery.RequireHttps = false;

            _client = new OidcClient(options);
            //_apiClient.Value.BaseAddress = new Uri("http://127.0.0.1:5002/");
        }

        private async void Login(object sender, EventArgs e)
        {
            _result = await _client.LoginAsync(new LoginRequest());

            if (_result.IsError)
            {
                return;
            }

            _apiClient.Value.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _result?.AccessToken ?? "");
        }

        private async void CallApi(object sender, EventArgs e)
        {
            var result = await _apiClient.Value.GetStringAsync("secret");

        }
    }
}
