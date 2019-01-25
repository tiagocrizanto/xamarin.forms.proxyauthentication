using Xamarin.Forms;

namespace ProxyWebView.Htttp
{
    public class HybridWebView : WebView
    {
        public HybridWebView()
        {
        }

        public static readonly BindableProperty UsernameProperty =
            BindableProperty.Create("UserName", typeof(string), typeof(HybridWebView), null, BindingMode.TwoWay);

        public static readonly BindableProperty PasswordProperty =
            BindableProperty.Create("Pwd", typeof(string), typeof(HybridWebView), null, BindingMode.TwoWay);

        public string Username
        {
            get { return GetValue(UsernameProperty).ToString(); }
            set { SetValue(UsernameProperty, value); }
        }

        public string Password
        {
            get { return GetValue(PasswordProperty).ToString(); }
            set { SetValue(PasswordProperty, value); }
        }
    }
}