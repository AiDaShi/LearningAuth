using IdentityModel.Client;
using IdentityModel.OidcClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private OidcClient _oidcClient = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var options = new OidcClientOptions()
            {
                Authority = "https://localhost:44376/",
                ClientId = "wpf",
                Scope = "openid ApiOne",
                RedirectUri = "http://localhost/sample-wpf-app",
                Browser = new WpfEmbeddedBrowser()
            };

            _oidcClient = new OidcClient(options);

            LoginResult result;
            try
            {
                result = await _oidcClient.LoginAsync();
            }
            catch (Exception ex)
            {
                //Message.Text = $"Unexpected Error: {ex.Message}";
                return;
            }

            if (result.IsError)
            {
                //Message.Text = result.Error == "UserCancel" ? "The sign-in window was closed before authorization was completed." : result.Error;

            }
            else
            {
                var name = result.User.Identity.Name;

                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                var apiResult = await client.GetStringAsync("https://localhost:44369/secret");
                //Message.Text = $"Hello {name}";
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {

            var userName ="bob";
            var password = "password";

            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:44376/");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }
            //request access token
            var ResponToken = await client.RequestPasswordTokenAsync(new PasswordTokenRequest()
            {
                Address = disco.TokenEndpoint,
                ClientId = "wpf2",
                ClientSecret = "wpf secrect",
                Scope = "ApiOne openid",
                UserName = userName,
                Password = password,
            });
            if (ResponToken.IsError)
            {
                MessageBox.Show(ResponToken.Error);
                return;
            }
            var _disco = disco;
            //获取key
            var hereAssecce = ResponToken.AccessToken;
            

            //call ApiOne
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(hereAssecce);
            var response = await apiClient.GetAsync("https://localhost:44369/secret");
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.StatusCode.ToString());
            }
            else
            {
                //得到的内容
                var content = await response.Content.ReadAsStringAsync();
            }

        }
    }
}
