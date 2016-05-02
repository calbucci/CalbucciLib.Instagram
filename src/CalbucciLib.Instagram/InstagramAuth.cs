using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CalbucciLib.Instagram.Model;
using Newtonsoft.Json;

namespace CalbucciLib.Instagram
{
    public class InstagramAuth
    {
        [ThreadStatic] public static InstagramAuthError LastError;

        // ====================================================================
        //
        //  Constructor
        //
        // ====================================================================

        public InstagramAuth(string clientId, string clientSecret, string redirectUri)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            RedirectUri = redirectUri;
        }

        // ====================================================================
        //
        //   Authorization
        //
        // ====================================================================

        public string GetAuthorizationUrl(InstagramScope scopes, string state = null, string redirectUri = null)
        {
            LastError = null;

            StringBuilder sb = new StringBuilder(250);
            sb.Append("https://api.instagram.com/oauth/authorize/?response_type=code&client_id=");
            sb.Append(HttpUtility.UrlEncode(ClientId));
            sb.Append("&redirect_uri=");
            sb.Append(HttpUtility.UrlEncode(redirectUri ?? RedirectUri));
            if (scopes  > 0 || DefaultScopes > 0)
            {
                sb.Append("&scope=");
                if (scopes == 0)
                    scopes = DefaultScopes;
                sb.Append(ScopesToString(scopes));
            }
            if (!string.IsNullOrWhiteSpace(state))
            {
                sb.Append("&state=");
                sb.Append(HttpUtility.UrlEncode(state));
            }

            return sb.ToString();
        }

        public InstagramToken ExchangeToken(Uri redirectUri, string originalRedirectUrl = null)
        {
            LastError = null;

            var qss = redirectUri.Query;

            var qs = HttpUtility.ParseQueryString(qss.Substring(qss.IndexOf('?')).Split('#')[0]);

            if (qs["error"] != null)
            {
                LastError = new InstagramAuthError
                {
                    Error = qs["error"],
                    ErrorReason = qs["error_reason"],
                    ErrorDescription = qs["error_description"]
                };
                return null;
            }

            string exchangeUrl = "https://api.instagram.com/oauth/access_token";

            string data = null;
            try
            {
                using (var wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("client_id={0}", HttpUtility.UrlEncode(ClientId));
                    sb.AppendFormat("&client_secret={0}", HttpUtility.UrlEncode(ClientSecret));
                    sb.Append("&grant_type=authorization_code");
                    sb.AppendFormat("&redirect_uri={0}", HttpUtility.UrlEncode(originalRedirectUrl ?? RedirectUri));
                    sb.AppendFormat("&code=" + HttpUtility.UrlEncode(qs["code"]));

                    data = sb.ToString();

                    var resp = wc.UploadString(exchangeUrl, data);

                    var ia = JsonConvert.DeserializeObject<InstagramToken>(resp);
                    return ia;
                }
            }
            catch (WebException wex)
            {
                var resp = (HttpWebResponse)wex.Response;
                if (resp != null)
                {
                    using (var sr = new StreamReader(resp.GetResponseStream()))
                    {
                        string content = sr.ReadToEnd();
                        Logger.LogException(wex, content, data);
                    }
                }
                else
                {
                    Logger.LogException(wex, data);
                }
                return null;
            }
        }
        


        // ====================================================================
        //
        //  Helpers
        //
        // ====================================================================
        protected string ScopesToString(InstagramScope scope)
        {
            if (scope == 0)
                return "";

            List<string> items = new List<string>();
            if ((scope & InstagramScope.Basic) > 0)
                items.Add("basic");
            if ((scope & InstagramScope.PublicContent) > 0)
                items.Add("public_content");
            if ((scope & InstagramScope.FollowerList) > 0)
                items.Add("follower_list");
            if ((scope & InstagramScope.Comments) > 0)
                items.Add("comments");
            if ((scope & InstagramScope.Relationships) > 0)
                items.Add("relationships");
            if ((scope & InstagramScope.Likes) > 0)
                items.Add("likes");

            return string.Join("+", items);
        }


        // ====================================================================
        //
        //  Properties
        //
        // ====================================================================

        public InstagramScope DefaultScopes { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
    }
}
