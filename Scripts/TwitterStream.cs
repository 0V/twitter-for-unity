using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace TwitterForUnity
{

    public delegate void TwitterStreamCallback(string response);

    public class Stream
    {

        private string REQUEST_URL;
        private UnityWebRequest request;
        public Oauth Oauth { get; private set; }

        public Stream(StreamType type, Oauth oauth)
        {
            Oauth = oauth;
            string[] endpoints = { "statuses/filter", "statuses/sample", "user", "site" };

            if (type == StreamType.Filter || type == StreamType.Sample)
            {
                REQUEST_URL = "https://stream.twitter.com/1.1/" + endpoints[(int)type] + ".json";
            }
            else if (type == StreamType.User)
            {
                REQUEST_URL = "https://userstream.twitter.com/1.1/user.json";
            }
            else if (type == StreamType.Site)
            {
                REQUEST_URL = "https://sitestream.twitter.com/1.1/site.json";
            }

        }

        public IEnumerator StartStream(Dictionary<string, string> APIParams, TwitterStreamCallback callback)
        {
            SortedDictionary<string, string> parameters = Helper.ConvertToSortedDictionary(APIParams);

            WWWForm form = new WWWForm();
            foreach (KeyValuePair<string, string> parameter in APIParams)
            {
                form.AddField(parameter.Key, parameter.Value);
            }

            request = UnityWebRequest.Post(REQUEST_URL, form);
            request.SetRequestHeader("ContentType", "application/x-www-form-urlencoded");
            request.SetRequestHeader("Authorization", Oauth.GenerateHeaderWithAccessToken(parameters, "POST", REQUEST_URL));
            request.downloadHandler = new StreamingDownloadHandler(callback);
            yield return request.Send();

        }

        public void StopStream()
        {
            Debug.Log("Connection Aborted");
            request.Abort();

        }


    }

    public class StreamingDownloadHandler : DownloadHandlerScript
    {

        TwitterStreamCallback callback;

        public StreamingDownloadHandler(TwitterStreamCallback callback)
        {
            this.callback = callback;
        }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (data == null || data.Length < 1)
            {
                Debug.Log("LoggingDownloadHandler :: ReceiveData - received a null/empty buffer");
                return false;
            }
            string response = System.Text.Encoding.ASCII.GetString(data);
            callback(JsonHelper.ArrayToObject(response));
            return true;
        }

    }

    public enum StreamType
    {
        Filter, // POST statuses/filter
        Sample, // GET statuses/sample
        User,   // GET user
        Site    // GET site
    }

}

