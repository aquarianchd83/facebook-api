using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.WebPages.OAuth;
using FB_api.Models;

namespace FB_api
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");

            //OAuthWebSecurity.RegisterTwitterClient(
            //    consumerKey: "",
            //    consumerSecret: "");

            var facebooksocialData = new Dictionary<string, object>();
            facebooksocialData.Add("scope", "user_friends, email, user_likes, friends_likes, user_birthday");

            OAuthWebSecurity.RegisterFacebookClient(
                appId: "173397536023423",
                appSecret: "0b0a17a6fcf64de7b9bd0cc7039b3068",
                displayName: "Login to Facebook",
                extraData: facebooksocialData);

            //OAuthWebSecurity.RegisterGoogleClient();
        }
    }
}
