using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class WebStatsManager : MonoBehaviour
{
    public static WebStatsManager Instance { get; private set; }

    private string baseUrl = "https://fa96490d-4e09-4a53-9cc9-5a23f1a7f426-00-b1jir99p5w51.kirk.replit.dev";
    private string authToken;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetAuthToken(string token)
    {
        authToken = token;
        Debug.Log($"Auth token set in WebStatsManager. Length: {token?.Length ?? 0}");
    }

    public string GetAuthToken()
    {
        return authToken;
    }

    public IEnumerator SendKill(int userId, int amount = 1)
    {
        string url = $"{baseUrl}/api/UserStats/{userId}/kills?amount={amount}";
        Debug.Log($"Sending KILL request to: {url}");
        Debug.Log($"Auth token present: {!string.IsNullOrEmpty(authToken)}");

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "POST"))
        {
            if (!string.IsNullOrEmpty(authToken))
            {
                request.SetRequestHeader("Authorization", $"Bearer {authToken}");
                Debug.Log("Authorization header added");
            }

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            Debug.Log($"Response Code: {request.responseCode}");
            Debug.Log($"Response: {request.downloadHandler.text}");

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Kill sent successfully: {request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"Failed to send kill: {request.error}");
                Debug.LogError($"Response Code: {request.responseCode}");
                Debug.LogError($"Response Body: {request.downloadHandler.text}");
            }
        }
    }

    public IEnumerator SendDeath(int userId, int amount = 1)
    {
        string url = $"{baseUrl}/api/UserStats/{userId}/death?amount={amount}";
        Debug.Log($"Sending DEATH request to: {url}");
        Debug.Log($"Auth token present: {!string.IsNullOrEmpty(authToken)}");

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "POST"))
        {
            if (!string.IsNullOrEmpty(authToken))
            {
                request.SetRequestHeader("Authorization", $"Bearer {authToken}");
                Debug.Log("Authorization header added");
            }

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            Debug.Log($"Response Code: {request.responseCode}");
            Debug.Log($"Response: {request.downloadHandler.text}");

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Death sent successfully: {request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"Failed to send death: {request.error}");
                Debug.LogError($"Response Code: {request.responseCode}");
                Debug.LogError($"Response Body: {request.downloadHandler.text}");
            }
        }
    }

    public IEnumerator SendScore(int userId, int amount)
    {
        string url = $"{baseUrl}/api/UserStats/{userId}/score?amount={amount}";
        Debug.Log($"Sending SCORE request to: {url}");
        Debug.Log($"Auth token present: {!string.IsNullOrEmpty(authToken)}");

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "POST"))
        {
            if (!string.IsNullOrEmpty(authToken))
            {
                request.SetRequestHeader("Authorization", $"Bearer {authToken}");
                Debug.Log("Authorization header added");
            }

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            Debug.Log($"Response Code: {request.responseCode}");
            Debug.Log($"Response: {request.downloadHandler.text}");

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Score sent successfully: {request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"Failed to send score: {request.error}");
                Debug.LogError($"Response Code: {request.responseCode}");
                Debug.LogError($"Response Body: {request.downloadHandler.text}");
            }
        }
    }
}