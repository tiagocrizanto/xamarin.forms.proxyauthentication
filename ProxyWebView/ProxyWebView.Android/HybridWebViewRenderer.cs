using Android.Webkit;
using FcaBrain.Droid.Renderers;
using ProxyWebView.Htttp;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace FcaBrain.Droid.Renderers
{
    public class HybridWebViewRenderer : WebViewRenderer
    {
        private new HybridWebView Element { get { return (HybridWebView)base.Element; } }

        public HybridWebViewRenderer(Android.Content.Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
        {
            base.OnElementChanged(e);

            var formsWebView = e.NewElement as Xamarin.Forms.WebView;

            if (formsWebView != null)
            {
                var nativeWebView = Control as Android.Webkit.WebView;

                nativeWebView.Settings.BuiltInZoomControls = true;
                nativeWebView.Settings.DisplayZoomControls = true;
                nativeWebView.Settings.LoadWithOverviewMode = true;

                nativeWebView.Settings.DomStorageEnabled = true;
                nativeWebView.Settings.JavaScriptEnabled = true;
                nativeWebView.Settings.MixedContentMode = MixedContentHandling.AlwaysAllow;
                nativeWebView.SetWebViewClient(new CustomWebViewClient(Element));

                if ((e.NewElement.Source as UrlWebViewSource) != null)
                {
                    string localUrl = System.Web.HttpUtility.UrlPathEncode((e.NewElement.Source as UrlWebViewSource).Url);
                    Control.LoadUrl(localUrl);
                }
            }
        }

        private class CustomWebViewClient : WebViewClient
        {
            private HybridWebView _webclient;
            private Android.Webkit.WebView _view;


            public CustomWebViewClient(HybridWebView webclient)
            {
                _webclient = webclient;
            }

            public override void OnReceivedHttpAuthRequest(Android.Webkit.WebView view, Android.Webkit.HttpAuthHandler handler, string host, string realm)
            {
                handler.Proceed(_webclient.Username, _webclient.Password);
            }

            public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView view, string url)
            {
                return base.ShouldOverrideUrlLoading(view, url);
            }

            public override void OnPageFinished(Android.Webkit.WebView view, string url)
            {
                view.Settings.BuiltInZoomControls = true;
                view.Settings.DisplayZoomControls = true;

                //If the page don't allow zoom, insert metatag to allow it
                view.LoadUrl(@"javascript:
                    var newMeta = document.createElement('meta');
                    newMeta.setAttribute('name', 'viewport');
                    newMeta.setAttribute('content', 'user-scalable=yes, width=device-width');
                    document.getElementsByTagName('head')[0].appendChild(newMeta);");

                base.OnPageFinished(view, url);
                _view = view;
            }

            private void GoBackNow(string goBack) => _view.GoBack();
        }
    }
}