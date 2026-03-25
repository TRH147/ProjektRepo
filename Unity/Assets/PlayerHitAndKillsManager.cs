using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using System.Collections;

public class PlayerHitAndKillsManager : MonoBehaviour
{
    [Header("UI")]
    public Animation hitMarkerAnimation;
    public AudioSource hitMarkerAudioSource;
    [Space]
    public Animation killMarkerAnimation;
    public AudioSource killMarkerAudioSource;

    private int userId; // Set this after login
    private bool isReadyToSendStats = false;

    private void Start()
    {
        // Check if WebStatsManager exists
        if (WebStatsManager.Instance == null)
        {
            Debug.LogError("WebStatsManager.Instance is NULL! Make sure WebStatsManager is in the scene.");
        }
        else
        {
            Debug.Log($"WebStatsManager.Instance exists. Auth token set: {!string.IsNullOrEmpty(WebStatsManager.Instance.GetAuthToken())}");
        }

        // Get userId from GameManager
        if (GameManager.Instance != null)
        {
            userId = GameManager.Instance.CurrentUserId;
        }
        else
        {
            // Fallback: try to get from PlayerPrefs directly
            userId = PlayerPrefs.GetInt("UserId", 0);
        }

        if (userId > 0)
        {
            isReadyToSendStats = true;
            Debug.Log($"Stats will be sent for user ID: {userId}");
        }
        else
        {
            Debug.LogWarning("User ID not found, stats won't be saved - starting fallback initialization");
            StartCoroutine(InitializeUserId());
        }
    }

    private IEnumerator InitializeUserId()
    {
        // Wait until we have the userId from PlayerPrefs
        int waitAttempts = 0;
        while (waitAttempts < 20) // Max 10 seconds (20 * 0.5s)
        {
            userId = PlayerPrefs.GetInt("UserId", 0);

            if (userId > 0)
            {
                isReadyToSendStats = true;
                Debug.Log($"Ready to send stats for user ID: {userId}");
                yield break;
            }

            waitAttempts++;
            yield return new WaitForSeconds(0.5f);
        }

        Debug.LogError("Failed to get user ID after multiple attempts");
    }

    public void GetHit(int _damage)
    {
        hitMarkerAnimation.Stop();
        hitMarkerAnimation.Play();

        hitMarkerAudioSource.Stop();
        hitMarkerAudioSource.Play();

        // Update Photon score
        PhotonNetwork.LocalPlayer.AddScore(_damage);

        // Send score to backend
        if (isReadyToSendStats && userId > 0 && WebStatsManager.Instance != null)
        {
            StartCoroutine(WebStatsManager.Instance.SendScore(userId, _damage));
        }
    }

    public void GetKill(string _victimName)
    {
        killMarkerAnimation.Stop();
        killMarkerAnimation.Play();

        hitMarkerAudioSource.Stop();
        hitMarkerAudioSource.Play();

        // Update Photon score (25 points per kill)
        PhotonNetwork.LocalPlayer.AddScore(25);

        // Track kill locally
        if (LocalPlayerKDManager.Instance != null)
            LocalPlayerKDManager.Instance.GetKill();

        // Send kill and score to backend
        if (isReadyToSendStats && userId > 0 && WebStatsManager.Instance != null)
        {
            StartCoroutine(WebStatsManager.Instance.SendKill(userId, 1));      // +1 kill
            StartCoroutine(WebStatsManager.Instance.SendScore(userId, 25));    // +25 score
        }

        // Show in kill feed
        if (KillFeedManager.Instance != null && KillFeedManager.Instance.photonView != null)
        {
            KillFeedManager.Instance.photonView.RPC("RPC_GetKill", RpcTarget.All,
                PhotonNetwork.LocalPlayer.NickName, _victimName);
        }
    }

    // Call this method when the player dies
    public void OnPlayerDeath()
    {
        Debug.Log("Player died - sending death to backend");

        // Send death to backend
        if (isReadyToSendStats && userId > 0 && WebStatsManager.Instance != null)
        {
            StartCoroutine(WebStatsManager.Instance.SendDeath(userId, 1));    // +1 death
        }
    }

    // Optional: Send damage on hit
    public void GetHitWithDamage(int _damage)
    {
        GetHit(_damage);
    }
}