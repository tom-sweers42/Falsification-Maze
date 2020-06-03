using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;

// class for JSON Body (add class variables to increase JSON keys)
public class Data
{
    public string playerId;
    public string age;
    public string gender;
    public string comment;
}

// main class for making HTTP REST API requests
public class FirebaseWebRequester : MonoBehaviour
{
    // define UI elements
    public Button sendButton;
    public InputField playerId, age, gender, comment;

    // initialize default URIs and API Key
    private readonly string tokenApi = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=AIzaSyC21UEISTGjswnV665STXXQ4ksiIXnkyZw";
    private readonly string firebaseApi = "https://falsificationmaze.firebaseio.com/users/";
    private string apiKey = "";

    void Start()
    {
        Debug.Log("Start Game...");
        // set up button listener (not relevant for Maze game)
        sendButton.onClick.AddListener(TaskOnClick);
    }
    void TaskOnClick()
    {
        Debug.Log("Player ID:" + playerId.text);
        Debug.Log("Age:" + age.text);
        Debug.Log("Gender:" + gender.text);
        Debug.Log("Comment:" + comment.text);

        // main function to send data
        StartCoroutine(SendData());
    }

    IEnumerator SendData()
    {
        // get API Auth Key
        using (UnityWebRequest apiKeyRequest = UnityWebRequest.Post(tokenApi, new WWWForm()))
        {
            // send request
            yield return apiKeyRequest.SendWebRequest();

            // check for errors
            if (apiKeyRequest.isNetworkError || apiKeyRequest.isHttpError)
            {
                Debug.LogError(apiKeyRequest.error);
            }
            else
            {
                // parse API response
                JSONNode apiKeyResponse = JSON.Parse(apiKeyRequest.downloadHandler.text);

                // fetch API Auth Key
                apiKey = apiKeyResponse["idToken"];

                Debug.Log("API Key: " + apiKey);
            }
            
        }

        // define user data
        Data userData = new Data
        {
            playerId = playerId.text,
            age = age.text,
            gender = gender.text,
            comment = comment.text
        };
        // define JSON body for HTTP REST PUT request
        string jsonBody = JsonUtility.ToJson(userData);

        Debug.Log(jsonBody);

        // define firebaseUri
        // (using base Uri + new userId json filename + auth key)
        string firebasePutUri = firebaseApi + playerId.text + ".json?auth="+apiKey;

        Debug.Log(firebasePutUri);

        // send user data
        using (UnityWebRequest firebaseRequest = UnityWebRequest.Put(firebasePutUri, jsonBody))
        {
            // send request
            yield return firebaseRequest.SendWebRequest();

            // check for errors
            if (firebaseRequest.isNetworkError || firebaseRequest.isHttpError)
            {
                Debug.LogError(firebaseRequest.error);
            }
            else
            {
                Debug.Log("Data Sent!");
            }
        }
    }
}
