using Foundation;
using ProxyWebView.Htttp;
using ProxyWebView.iOS;
using System;
using System.ComponentModel;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace ProxyWebView.iOS
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, WKWebView>
    {
        WKUserContentController userController;

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var formsWebView = sender as WebView;

            if (formsWebView != null)
            {
                userController = new WKUserContentController();
                var config = new WKWebViewConfiguration { UserContentController = userController };
                var webView = new WKWebView(Frame, config);

                SetNativeControl(webView);
                Control.AllowsBackForwardNavigationGestures = true;
                Control.AllowsLinkPreview = true;
                Control.NavigationDelegate = new CustomWebViewClient(Element);

                if ((formsWebView.Source as UrlWebViewSource) != null)
                {
                    string url = System.Web.HttpUtility.UrlPathEncode((formsWebView.Source as UrlWebViewSource).Url);
                    Control.LoadRequest(new NSUrlRequest(new NSUrl(url)));
                }
                else if ((formsWebView.Source as HtmlWebViewSource) != null)
                {
                    if (!string.IsNullOrEmpty((formsWebView.Source as HtmlWebViewSource).BaseUrl))
                        Control.LoadHtmlString((formsWebView.Source as HtmlWebViewSource).Html, new NSUrl(System.Web.HttpUtility.UrlPathEncode((formsWebView.Source as HtmlWebViewSource).BaseUrl)));
                    else
                        //Option to load local HTML
                        Control.LoadHtmlString((formsWebView.Source as HtmlWebViewSource).Html, NSUrl.CreateFileUrl(new[] { NSBundle.MainBundle.BundlePath }));
                }
            }
        }

        public class CustomWebViewClient : WKNavigationDelegate, INSUrlConnectionDataDelegate
        {
            private HybridWebView _webclient;
            private WKWebView _webView;

            public CustomWebViewClient(HybridWebView webclient)
            {
                _webclient = webclient;
            }

            public override void DidReceiveAuthenticationChallenge(WKWebView webView, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition, NSUrlCredential> completionHandler)
            {
                var crendential = new NSUrlCredential(_webclient.Username, _webclient.Password, NSUrlCredentialPersistence.ForSession);
                completionHandler(NSUrlSessionAuthChallengeDisposition.UseCredential, crendential);
            }

            public override void DidStartProvisionalNavigation(WKWebView webView, WKNavigation navigation)
            {
                //if you need store the navigation history you can do it in this method
                // e.g: _webclient.History.Add(webView.Url.AbsoluteString);
                //_webclient is an instance of HybridWebView and History is a list of string in HybridWebView class
            }

            public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
            {
                //If the page don't allow zoom, insert metatag to allow it
                string allowZoom = @"javascript:
                    var newMeta = document.createElement('meta');
                    newMeta.setAttribute('name', 'viewport');
                    newMeta.setAttribute('content', 'user-scalable=yes, width=device-width');
                    document.getElementsByTagName('head')[0].appendChild(newMeta);";

                webView.EvaluateJavaScript(allowZoom, null);

                _webView = webView;
            }
        }
    }
}