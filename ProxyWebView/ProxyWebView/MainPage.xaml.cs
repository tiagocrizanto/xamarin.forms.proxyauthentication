using Xamarin.Forms;

namespace ProxyWebView
{
    public partial class MainPage : ContentPage
    {
        private MainPage _mainPage;
        public MainPage()
        {
            InitializeComponent();
            _mainPage = this;

            hwebview.Username = "username"; //usually active directory user
            hwebview.Password = "password";
            hwebview.Source = "web_site_proxy_uri";
        }
    }
}
