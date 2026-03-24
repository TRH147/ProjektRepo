
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int CurrentUserId { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            PhotonNetwork.SendRate = 64;
            PhotonNetwork.SerializationRate = 64;

            // Load saved user ID if exists
            if (PlayerPrefs.HasKey("UserId"))
            {
                CurrentUserId = PlayerPrefs.GetInt("UserId");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}