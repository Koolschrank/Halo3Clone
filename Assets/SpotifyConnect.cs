
using UnityEngine;

public class SpotifyConnect : MonoBehaviour
{
	[Header("Spotify Settings")]
	public string clientId = "082f099b38b4409289670ef7ac6142cc";
	public string redirectUri = "https://example.com/callback";

	public string[] scopes = new string[]
	{
		"user-modify-playback-state",
		"user-read-playback-state",
		"user-read-currently-playing"
	};


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OpenSpotify(bool whatever)
    {
        OpenSpotify();
	}

	public void OpenSpotify()
    {
		string scopeStr = string.Join(" ", scopes);

		string authUrl =
			"https://accounts.spotify.com/authorize" +
			"?client_id=" + clientId +
			"&response_type=code" +   // implicit grant (simplest way, token comes in redirect URL)
			"&redirect_uri=" + WWW.EscapeURL(redirectUri) +
			"&scope=" + WWW.EscapeURL(scopeStr);

		Debug.Log("Opening Spotify Auth URL: " + authUrl);
		Application.OpenURL(authUrl);
	}
}
