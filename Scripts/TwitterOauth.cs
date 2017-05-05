﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace TwitterForUnity
{
    public class Oauth
    {
        public Oauth()
        {

        }

        public Oauth(string consumerKey,string consumerSecret,string accessToken,string accessTokenSecret)
        {
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
            AccessToken = accessToken;
            AccessTokenSecret = accessTokenSecret;
            
        }

        #region Tokens
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        #endregion

        #region Public Method
        public string GenerateHeaderWithAccessToken(SortedDictionary<string, string> parameters, string requestMethod, string requestURL)
        {
            string signature = GenerateSignatureWithAccessToken(parameters, requestMethod, requestURL);

            StringBuilder requestParamsString = new StringBuilder();
            foreach (KeyValuePair<string, string> param in parameters)
            {
                requestParamsString.Append(String.Format("{0}=\"{1}\",", Helper.UrlEncode(param.Key), Helper.UrlEncode(param.Value)));
            }

            string authHeader = "OAuth realm=\"Twitter API\",";
            string requestSignature = String.Format("oauth_signature=\"{0}\"", Helper.UrlEncode(signature));
            authHeader += requestParamsString.ToString() + requestSignature;
            return authHeader;
        }
        #endregion

        #region HelperMethods

        private string GenerateSignatureWithAccessToken(SortedDictionary<string, string> parameters, string requestMethod, string requestURL)
        {
            AddDefaultOauthParams(parameters, ConsumerKey);
            parameters.Add("oauth_token", AccessToken);

            StringBuilder paramString = new StringBuilder();
            foreach (KeyValuePair<string, string> param in parameters)
            {
                paramString.Append(Helper.UrlEncode(param.Key) + "=" + Helper.UrlEncode(param.Value) + "&");
            }
            paramString.Length -= 1; // Remove "&" at the last of string


            string requestHeader = Helper.UrlEncode(requestMethod) + "&" + Helper.UrlEncode(requestURL);
            string signatureData = requestHeader + "&" + Helper.UrlEncode(paramString.ToString());

            string signatureKey = Helper.UrlEncode(ConsumerSecret) + "&" + Helper.UrlEncode(AccessTokenSecret);
            HMACSHA1 hmacsha1 = new HMACSHA1(Encoding.ASCII.GetBytes(signatureKey));
            byte[] signatureBytes = hmacsha1.ComputeHash(Encoding.ASCII.GetBytes(signatureData));
            return Convert.ToBase64String(signatureBytes);
        }

        private static void AddDefaultOauthParams(SortedDictionary<string, string> parameters, string consumerKey)
        {
            parameters.Add("oauth_consumer_key", consumerKey);
            parameters.Add("oauth_signature_method", "HMAC-SHA1");
            parameters.Add("oauth_timestamp", GenerateTimeStamp());
            parameters.Add("oauth_nonce", GenerateNonce());
            parameters.Add("oauth_version", "1.0");
        }

        private static string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        private static string GenerateNonce()
        {
            return new System.Random().Next(123400, int.MaxValue).ToString("X");
        }

        #endregion

    }



}

