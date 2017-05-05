
Twitter API Client for Unity C#. (beta)  

Inspired by [Let's Tweet In Unity](https://www.assetstore.unity3d.com/jp/#!/content/536).  

�{���C�u������ twitter-for-unity �̌`���ł��邾���ς��Ȃ��悤�ɂ��Ȃ���A twitter-for-unity ���N���X���C�u�������������̂ł��B  

�I�u�W�F�N�g�w���̕��j�ɏ]���ď�����Ă��܂��B  

�ȉ��̃T���v���R�[�h�͖{���C�u�����ł̃T���v���R�[�h�ł��B


# Environment

- Unity 5.6.0f3

# Available API Methods

## REST API

- GET  : Available (Except Media Upload)
- POST : Available (Except Media Upload)

## Stream API

- POST statuses/filter : partly Available(beta)
- GET  statuses/sample : partly Available(beta)
- UserStreams : Not Available (now in progress)

# Usage

## Initialize
```C#
using TwitterForUnity;

public class EventHandler : MonoBehaviour {

private Client client;

void Start () {
    Oauth oauth = new Oauth(); 
    oauth.ConsumerKey       = "...";
    oauth.ConsumerSecret    = "...";
    oauth.AccessToken       = "...";
    oauth.AccessTokenSecret = "...";
    client = new Client(oauth);
  }  
}
```
## REST API

### GET search/tweets

```C#
using TwitterForUnity;

void Start() {
  Dictionary<string, string> parameters = new Dictionary<string, string>();
  parameters ["q"] = "search word";
  parameters ["count"] = 30.ToString();
  StartCoroutine (client.Get ("search/tweets", parameters, this.Callback));
}

void Callback(bool success, string response) {
  if (success) {
    SearchTweetsResponse Response = JsonUtility.FromJson<SearchTweetsResponse> (response);
  } else {
    Debug.Log (response);
  }
}
```

### GET statuses/home_timeline

```C#
using TwitterForUnity;

void Start() {
  Dictionary<string, string> parameters = new Dictionary<string, string>();
  parameters ["count"] = 30.ToString();
  StartCoroutine (client.Get ("statuses/home_timeline", parameters, this.Callback));
}

void Callback(bool success, string response) {
  if (success) {
    StatusesHomeTimelineResponse Response = JsonUtility.FromJson<StatusesHomeTimelineResponse> (response);
  } else {
    Debug.Log (response);
  }
}
```

### POST statuses/update

```C#
using TwitterForUnity;

void Start() {
  Dictionary<string, string> parameters = new Dictionary<string, string>();
  parameters ["status"] = "Tweet from Unity";
  StartCoroutine (client.Post ("statuses/update", parameters, this.Callback));
}

void Callback(bool success, string response) {
  if (success) {
    Tweet tweet = JsonUtility.FromJson<Tweet> (response);
  } else {
    Debug.Log (response);
  }
}
```

### POST statuses/retweet/:id
ex. search tweets with the word "Unity", and retweet 5 tweets.
```C#
using TwitterForUnity;

void start() {
  Dictionary<string, string> parameters = new Dictionary<string, string>();
  parameters ["q"] = "Unity";       // Search keywords
  parameters ["count"] = 5.ToString();   // Number of Tweets
  StartCoroutine (client.Get ("search/tweets", parameters, this.Callback));
}

void Callback(bool success, string response) {
  if (success) {
    SearchTweetsResponse Response = JsonUtility.FromJson<SearchTweetsResponse> (response);
    foreach (Tweet tweet in Response.statuses) { Retweet (tweet); }
  } else {
    Debug.Log (response);
  }
}

void Retweet(Tweet tweet) {
  Dictionary<string, string> parameters = new Dictionary<string, string>();
  parameters ["id"] = tweet.id_str;
  StartCoroutine (client.Post ("statuses/retweet/" + tweet.id_str, parameters, this.RetweetCallback));
}

void RetweetCallback(bool success, string response) {
  if (success) {
    Debug.Log ("Retweet Done");
  } else {
    Debug.Log (response);
  }
}
```
See https://dev.twitter.com/rest/reference for more Methods.


## Streaming API

### POST statuses/filter
```C#
using TwitterForUnity;

Stream stream;

void Start() {
  Oauth oauth = new Oauth(); 
  oauth.ConsumerKey       = "...";
  oauth.ConsumerSecret    = "...";
  oauth.AccessToken       = "...";
  oauth.AccessTokenSecret = "...";
  stream = new Stream(oauth,StreamType.Filter);
  Dictionary<string, string> streamParameters = new Dictionary<string, string>();
  streamParameters.Add("track", "iPhone");
  StartCoroutine(stream.On(streamParameters, this.OnStream));
}

void OnStream(string response) {
  try
    {
      Tweet tweet = JsonUtility.FromJson<Tweet>(response);
  } catch (System.ArgumentException e)
  {
    Debug.Log("Invalid Response");
  }
}
```
See https://dev.twitter.com/streaming/reference for more Methods.

## Response class
See `TwitterJson.cs`, and https://dev.twitter.com/overview/api/tweets , https://dev.twitter.com/overview/api/users , https://dev.twitter.com/overview/api/entities , https://dev.twitter.com/overview/api/entities-in-twitter-objects .
You can modify `TwitterJson.cs` to get a response item.
