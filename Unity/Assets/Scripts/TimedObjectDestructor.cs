using UnityEngine;
using Photon.Pun;
using System.Collections;

public class TimedObjectDestructor : MonoBehaviourPun
{
    public float lifetime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (photonView.IsMine)
        {
            StartCoroutine(DelayedDestroy());
        }
    }
    IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(lifetime);

        PhotonNetwork.Destroy(gameObject);
    }
}
