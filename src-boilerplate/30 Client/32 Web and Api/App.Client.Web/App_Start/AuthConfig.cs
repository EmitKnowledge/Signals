using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace App.Client.Web
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            /*
            // login with twitter
            var twitterClient = new TwitterClient(AppConfiguration.Instance.ExternalApisConfigurationElement.Twitter.ConsumerKey,
                                                  AppConfiguration.Instance.ExternalApisConfigurationElement.Twitter.ConsumerSecret);
            OAuthWebSecurity.RegisterClient(twitterClient, "twitter", new Dictionary<string, object>());

            // login with fb
            OAuthWebSecurity.RegisterClient(
                new FacebookClient(
                    appId: AppConfiguration.Instance.ExternalApisConfigurationElement.Facebook.AppId,
                    appSecret: AppConfiguration.Instance.ExternalApisConfigurationElement.Facebook.AppSecret),
                AppConfiguration.Instance.ExternalApisConfigurationElement.Facebook.Name, null
            );

            // login with slack
            OAuthWebSecurity.RegisterClient(
                new SlackClient(
                    appId: AppConfiguration.Instance.ExternalApisConfigurationElement.Slack.CliendId,
                    appSecret: AppConfiguration.Instance.ExternalApisConfigurationElement.Slack.ClientSecret),
                AppConfiguration.Instance.ExternalApisConfigurationElement.Slack.Name, null
            );

            // login with google
            //var client = new GoogleOAuth2Client("APIKEY.apps.googleusercontent.com", "APISECRET");
            //var extraData = new Dictionary<string, object>();
            //OAuthWebSecurity.RegisterClient(client, "Google", extraData);
             * */
        }
    }
    
    /*
        public class FacebookClient : OAuth2Client
        {
            private const string AuthorizationEP = "https://www.facebook.com/dialog/oauth";
            private const string TokenEP = "https://graph.facebook.com/oauth/access_token";
            private readonly string _appId;
            private readonly string _appSecret;

            public class FacebookUserData
            {
                public string id { get; set; }
                public string email { get; set; }
                public string first_name { get; set; }
                public string gender { get; set; }
                public string last_name { get; set; }
                public string link { get; set; }
                public string locale { get; set; }
                public string name { get; set; }
                public string updated_time { get; set; }
                public bool verified { get; set; }
            }

            public class FacebookPermissionsDataResponse
            {
                public List<FacebookPermissionsData> data { get; set; }

                public FacebookPermissionsDataResponse()
                {
                    data = new List<FacebookPermissionsData>();
                }
            }

            public class FacebookPermissionsData
            {
                public string permission { get; set; }

                public string status { get; set; }
            }

            public FacebookClient(string appId, string appSecret)
                : base("facebook")
            {
                this._appId = appId;
                this._appSecret = appSecret;
            }


            protected override Uri GetServiceLoginUrl(Uri returnUrl)
            {
                return new Uri(
                            AuthorizationEP
                            + "?client_id=" + this._appId
                            + "&redirect_uri=" + HttpUtility.UrlEncode(returnUrl.ToString())
                            + "&scope=email,publish_actions"
                            + "&display=page"
                        );
            }

            protected override IDictionary<string, string> GetUserData(string accessToken)
            {
                WebClient client = new WebClient();
                string content = client.DownloadString(
                    "https://graph.facebook.com/me?access_token=" + accessToken
                );
                var data = SerializationHelper.DeserializeFromJSon<FacebookUserData>(content);

                content = client.DownloadString(
                    "https://graph.facebook.com/" + data.id + "/permissions?access_token=" + accessToken
                );
                var permissionsData = SerializationHelper.DeserializeFromJSon<FacebookPermissionsDataResponse>(content);
                var canPublish = false;
                if (!permissionsData.IsNull() && !permissionsData.data.IsNullOrHasZeroElements())
                {
                    canPublish = permissionsData.data.Any(x => x.permission == @"publish_actions" && x.status == @"granted");
                }

                return new Dictionary<string, string> {
                    {
                        "id",
                        data.id
                    },
                    {
                        "name",
                        data.name
                    },
                    {
                        "email",
                        data.email
                    }
                    ,
                    {
                        "username",
                        data.email
                    }
                    ,
                    {
                        "canPublish",
                        canPublish.ToString()
                    }
                };
            }

            protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
            {
                WebClient client = new WebClient();
                string content = client.DownloadString(
                    TokenEP
                    + "?client_id=" + this._appId
                    + "&client_secret=" + this._appSecret
                    + "&redirect_uri=" + HttpUtility.UrlEncode(returnUrl.ToString())
                    + "&code=" + authorizationCode
                );

                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(content);
                if (nameValueCollection != null)
                {
                    string result = nameValueCollection["access_token"];
                    return result;
                }
                return null;
            }
        }

        public class SlackClient : OAuth2Client
        {
            private const string AuthorizationEP = "https://slack.com/oauth/authorize";
            private const string TokenEP = "https://slack.com/api/oauth.access";
            private readonly string _appId;
            private readonly string _appSecret;
            private string _state;
            private string _generatedRedirectionUrl;

            public class SlackTokenData
            {
                public bool ok { get; set; }
                public string access_token { get; set; }
                public string scope { get; set; }
                public string team_name { get; set; }
            }

            public class SlackUserData
            {
                public bool ok { get; set; }
                public string url { get; set; }
                public string team { get; set; }
                public string user { get; set; }
                public string user_id { get; set; }
                public string team_id { get; set; }
            }

            public class SlackUserExtendedDataResponse
            {
                public bool ok { get; set; }
                public SlackUserExtendedData user { get; set; }
            }

            public class SlackUserExtendedData
            {
                public string id { get; set; }
                public string name { get; set; }
                public SlackUserExtendedProfileData profile { get; set; }
            }

            public class SlackUserExtendedProfileData
            {
                public string first_name { get; set; }
                public string last_name { get; set; }
                public string real_name { get; set; }
                public string email { get; set; }
                public string skype { get; set; }
                public string phone { get; set; }
                public string image_24 { get; set; }
                public string image_32 { get; set; }
                public string image_48 { get; set; }
                public string image_72 { get; set; }
                public string image_192 { get; set; }
            }


            public SlackClient(string appId, string appSecret)
                : base("slack")
            {
                this._appId = appId;
                this._appSecret = appSecret;
                _state = HashHelper.GenerateSalt(16);
            }


            protected override Uri GetServiceLoginUrl(Uri returnUrl)
            {
                _generatedRedirectionUrl = returnUrl.ToString();
                var serviceUrl = new Uri(
                            AuthorizationEP
                            + "?client_id=" + this._appId
                            + "&redirect_uri=" + HttpUtility.UrlEncode(_generatedRedirectionUrl)
                    //+ "&scope=identify,read,post,client"
                    // new scope that we need to update to
                            + "&scope=channels:read%20chat:write:bot%20chat:write:user%20files:write:user%20groups:read%20im:read%20users:read%20identify"
                            + "&state=" + _state
                        );
                return serviceUrl;
            }

            protected override IDictionary<string, string> GetUserData(string accessToken)
            {
                WebClient client = new WebClient();
                string content = client.DownloadString(@"https://slack.com/api/auth.test?token=" + accessToken);
                var data = SerializationHelper.DeserializeFromJSon<SlackUserData>(content);
                var userId = data != null ? data.user_id : string.Empty;
                content = client.DownloadString(@"https://slack.com/api/users.info?token=" + accessToken + @"&user=" + userId);
                var extendedData = SerializationHelper.DeserializeFromJSon<SlackUserExtendedDataResponse>(content);

                if (data != null && data.ok && extendedData != null && extendedData.ok && extendedData.user != null)
                {
                    return new Dictionary<string, string>
                    {
                        {
                            @"id",
                            data.user_id
                        },
                        {
                            @"username",
                            data.user
                        },
                        {
                            @"email",
                            extendedData.user.profile != null ? extendedData.user.profile.email : null
                        },
                        {
                            @"name",
                            extendedData.user.profile != null ? extendedData.user.profile.real_name : null
                        },
                        {
                            @"team",
                            data.team
                        }
                        ,
                        {
                            @"teamid",
                            data.team_id
                        }
                        ,
                        {
                            @"url",
                            data.url
                        }
                        ,
                        {
                            "canPublish",
                            true.ToString()
                        }
                    };
                }

                return null;
            }

            protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
            {
                WebClient client = new WebClient();
                string content = client.DownloadString(
                    TokenEP
                    + "?client_id=" + this._appId
                    + "&client_secret=" + this._appSecret
                    + "&redirect_uri=" + HttpUtility.UrlEncode(_generatedRedirectionUrl)
                    + "&code=" + authorizationCode
                );

                var tokenData = SerializationHelper.DeserializeFromJSon<SlackTokenData>(content);
                if (tokenData != null && tokenData.ok)
                {
                    string result = tokenData.access_token;
                    return result;
                }
                return null;
            }
        }

        public class TwitterClient : OAuthClient
        {
            public class CustomDotNetOpenAuthWebConsumer : IOAuthWebWorker, IDisposable
            {
                private readonly WebConsumer _webConsumer;

                public CustomDotNetOpenAuthWebConsumer(ServiceProviderDescription serviceDescription, IConsumerTokenManager tokenManager)
                {
                    if (serviceDescription == null) throw new ArgumentNullException("serviceDescription");
                    if (tokenManager == null) throw new ArgumentNullException("tokenManager");

                    _webConsumer = new WebConsumer(serviceDescription, tokenManager);
                }

                public HttpWebRequest PrepareAuthorizedRequest(MessageReceivingEndpoint profileEndpoint, string accessToken)
                {
                    return _webConsumer.PrepareAuthorizedRequest(profileEndpoint, accessToken);
                }

                public AuthorizedTokenResponse ProcessUserAuthorization()
                {
                    return _webConsumer.ProcessUserAuthorization();
                }

                public void RequestAuthentication(Uri callback)
                {
                    var redirectParameters = new Dictionary<string, string>();
                    var request = _webConsumer.PrepareRequestUserAuthorization(callback, null, redirectParameters);

                    _webConsumer.Channel.PrepareResponse(request).Send();
                }

                public void Dispose()
                {
                    Dispose(true);
                    GC.SuppressFinalize(this);
                }

                protected virtual void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        _webConsumer.Dispose();
                    }
                }
            }

            private static readonly string[] UriRfc3986CharsToEscape = new[] { "!", "*", "'", "(", ")" };

            public static readonly ServiceProviderDescription TwitterServiceDescription = new ServiceProviderDescription
            {
                RequestTokenEndpoint = new MessageReceivingEndpoint("https://api.twitter.com/oauth/request_token", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
                UserAuthorizationEndpoint = new MessageReceivingEndpoint("https://api.twitter.com/oauth/authenticate", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
                AccessTokenEndpoint = new MessageReceivingEndpoint("https://api.twitter.com/oauth/access_token", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
                TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
            };

            public TwitterClient(string consumerKey, string consumerSecret)
                : this(consumerKey, consumerSecret, new AuthenticationOnlyCookieOAuthTokenManager())
            {
            }

            public TwitterClient(string consumerKey, string consumerSecret, IOAuthTokenManager tokenManager)
                : base("twitter", new CustomDotNetOpenAuthWebConsumer(TwitterServiceDescription, new SimpleConsumerTokenManager(consumerKey, consumerSecret, tokenManager)))
            {
            }

            protected override AuthenticationResult VerifyAuthenticationCore(AuthorizedTokenResponse response)
            {
                var accessToken = response.AccessToken;
                var accessTokenSecret = (response as ITokenSecretContainingMessage).TokenSecret;
                var userId = response.ExtraData["user_id"];
                var userName = response.ExtraData["screen_name"];

                var profileRequestUrl = new Uri("https://api.twitter.com/1/users/show.xml?user_id=" + EscapeUriDataStringRfc3986(userId));
                var profileEndpoint = new MessageReceivingEndpoint(profileRequestUrl, HttpDeliveryMethods.GetRequest);
                var request = WebWorker.PrepareAuthorizedRequest(profileEndpoint, accessToken);

                var extraData = new Dictionary<string, string>
                {
                    { "accesstoken", accessToken },
                    { "accesssecret",  accessTokenSecret },
                    { "canPublish", true.ToString()}
                };

                try
                {
                    using (var profileResponse = request.GetResponse())
                    {
                        using (var responseStream = profileResponse.GetResponseStream())
                        {
                            var document = xLoadXDocumentFromStream(responseStream);

                            AddDataIfNotEmpty(extraData, document, "name");
                            AddDataIfNotEmpty(extraData, document, "location");
                            AddDataIfNotEmpty(extraData, document, "description");
                            AddDataIfNotEmpty(extraData, document, "url");
                            AddDataIfNotEmpty(extraData, document, "url");
                        }
                    }
                }
                catch
                {
                    // At this point, the authentication is already successful. Here we are just trying to get additional data if we can. If it fails, no problem.
                }

                return new AuthenticationResult(true, ProviderName, userId, userName, extraData);
            }

            private static XDocument xLoadXDocumentFromStream(Stream stream)
            {
                const int maxChars = 0x10000; // 64k

                var settings = new XmlReaderSettings
                {
                    MaxCharactersInDocument = maxChars
                };

                return XDocument.Load(XmlReader.Create(stream, settings));
            }

            private static void AddDataIfNotEmpty(Dictionary<string, string> dictionary, XDocument document, string elementName)
            {
                var element = document.Root.Element(elementName);

                if (element != null)
                {
                    AddItemIfNotEmpty(dictionary, elementName, element.Value);
                }
            }

            private static void AddItemIfNotEmpty(IDictionary<string, string> dictionary, string key, string value)
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }

                if (!string.IsNullOrEmpty(value))
                {
                    dictionary[key] = value;
                }
            }

            private static string EscapeUriDataStringRfc3986(string value)
            {
                var escaped = new StringBuilder(Uri.EscapeDataString(value));

                for (var i = 0; i < UriRfc3986CharsToEscape.Length; i++)
                {
                    escaped.Replace(UriRfc3986CharsToEscape[i], Uri.HexEscape(UriRfc3986CharsToEscape[i][0]));
                }

                return escaped.ToString();
            }
        }

        /// <summary>
        /// A DotNetOpenAuth client for logging in to Google using OAuth2.
        /// Reference: https://developers.google.com/accounts/docs/OAuth2
        /// </summary>
        public class GoogleOAuth2Client : OAuth2Client
        {
            #region Constants and Fields

            /// <summary>
            /// The authorization endpoint.
            /// </summary>
            private const string AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/auth";

            /// <summary>
            /// The token endpoint.
            /// </summary>
            private const string TokenEndpoint = "https://accounts.google.com/o/oauth2/token";

            /// <summary>
            /// The user info endpoint.
            /// </summary>
            private const string UserInfoEndpoint = "https://www.googleapis.com/oauth2/v1/userinfo";

            /// <summary>
            /// The base uri for scopes.
            /// </summary>
            private const string ScopeBaseUri = "https://www.googleapis.com/auth/";

            /// <summary>
            /// The _app id.
            /// </summary>
            private readonly string _clientId;

            /// <summary>
            /// The _app secret.
            /// </summary>
            private readonly string _clientSecret;

            /// <summary>
            /// The requested scopes.
            /// </summary>
            private readonly string[] _requestedScopes;

            #endregion

            /// <summary>
            /// Creates a new Google OAuth2 Client, requesting the default "userinfo.profile" and "userinfo.email" scopes.
            /// </summary>
            /// <param name="clientId">The Google Client Id</param>
            /// <param name="clientSecret">The Google Client Secret</param>
            public GoogleOAuth2Client(string clientId, string clientSecret)
                : this(clientId, clientSecret, new[] { "userinfo.profile", "userinfo.email" }) { }

            /// <summary>
            /// Creates a new Google OAuth2 client.
            /// </summary>
            /// <param name="clientId">The Google Client Id</param>
            /// <param name="clientSecret">The Google Client Secret</param>
            /// <param name="requestedScopes">One or more requested scopes, passed without the base URI.</param>
            public GoogleOAuth2Client(string clientId, string clientSecret, params string[] requestedScopes)
                : base("google")
            {
                if (string.IsNullOrWhiteSpace(clientId))
                    throw new ArgumentNullException("clientId");

                if (string.IsNullOrWhiteSpace(clientSecret))
                    throw new ArgumentNullException("clientSecret");

                if (requestedScopes == null)
                    throw new ArgumentNullException("requestedScopes");

                if (requestedScopes.Length == 0)
                    throw new ArgumentException("One or more scopes must be requested.", "requestedScopes");

                _clientId = clientId;
                _clientSecret = clientSecret;
                _requestedScopes = requestedScopes;
            }

            protected override Uri GetServiceLoginUrl(Uri returnUrl)
            {
                var scopes = _requestedScopes.Select(x => !x.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? ScopeBaseUri + x : x);
                var state = string.IsNullOrEmpty(returnUrl.Query) ? string.Empty : returnUrl.Query.Substring(1);

                return BuildUri(AuthorizationEndpoint, new NameValueCollection
                    {
                        { "response_type", "code" },
                        { "client_id", _clientId },
                        { "scope", string.Join(" ", scopes) },
                        { "redirect_uri", returnUrl.GetLeftPart(UriPartial.Path) },
                        { "state", state },
                    });
            }

            protected override IDictionary<string, string> GetUserData(string accessToken)
            {
                var uri = BuildUri(UserInfoEndpoint, new NameValueCollection { { "access_token", accessToken } });

                var webRequest = (HttpWebRequest)WebRequest.Create(uri);

                using (var webResponse = webRequest.GetResponse())
                using (var stream = webResponse.GetResponseStream())
                {
                    if (stream == null)
                        return null;

                    using (var textReader = new StreamReader(stream))
                    {
                        var json = textReader.ReadToEnd();
                        var extraData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                        return extraData;
                    }
                }
            }

            protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
            {
                var postData = HttpUtility.ParseQueryString(string.Empty);
                postData.Add(new NameValueCollection
                    {
                        { "grant_type", "authorization_code" },
                        { "code", authorizationCode },
                        { "client_id", _clientId },
                        { "client_secret", _clientSecret },
                        { "redirect_uri", returnUrl.GetLeftPart(UriPartial.Path) },
                    });

                var webRequest = (HttpWebRequest)WebRequest.Create(TokenEndpoint);

                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";

                using (var s = webRequest.GetRequestStream())
                using (var sw = new StreamWriter(s))
                    sw.Write(postData.ToString());

                using (var webResponse = webRequest.GetResponse())
                {
                    var responseStream = webResponse.GetResponseStream();
                    if (responseStream == null)
                        return null;

                    using (var reader = new StreamReader(responseStream))
                    {
                        var response = reader.ReadToEnd();
                        var json = JObject.Parse(response);
                        var accessToken = json.Value<string>("access_token");
                        return accessToken;
                    }
                }
            }

            private static Uri BuildUri(string baseUri, NameValueCollection queryParameters)
            {
                var keyValuePairs = queryParameters.AllKeys.Select(k => HttpUtility.UrlEncode(k) + "=" + HttpUtility.UrlEncode(queryParameters[k]));
                var qs = String.Join("&", keyValuePairs);

                var builder = new UriBuilder(baseUri) { Query = qs };
                return builder.Uri;
            }

            /// <summary>
            /// Google requires that all return data be packed into a "state" parameter.
            /// This should be called before verifying the request, so that the url is rewritten to support this.
            /// </summary>
            public static void RewriteRequest()
            {
                var ctx = HttpContext.Current;

                var stateString = HttpUtility.UrlDecode(ctx.Request.QueryString["state"]);
                if (stateString == null || !stateString.Contains("__provider__=google"))
                    return;

                var q = HttpUtility.ParseQueryString(stateString);
                q.Add(ctx.Request.QueryString);
                q.Remove("state");

                ctx.RewritePath(ctx.Request.Path + "?" + q);
            }
        }
    */
}