using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPun
{
    public GameObject fpCamera;
    public Movement movement;
    public GameObject tpPlayer;
    [Space]
    public TextMeshProUGUI nameText;

    private void Start()
    {
        if (photonView.IsMine)
        {
            if (fpCamera != null)
                fpCamera.SetActive(true);

            if (movement != null)
                movement.enabled = true;

            if (tpPlayer != null)
                tpPlayer.gameObject.SetActive(false);

            if (nameText != null)
                nameText.gameObject.SetActive(false);
        }
        else
        {
            if (fpCamera != null)
                fpCamera.SetActive(false);

            if (movement != null)
                movement.enabled = false;

            if (nameText != null)
            {
                nameText.gameObject.SetActive(true);
                nameText.text = photonView.Owner.NickName;
            }
        }
    }

    private void OnDisable()
    {
        // Disable name text to prevent errors when player dies
        if (nameText != null)
            nameText.gameObject.SetActive(false);
    }
}