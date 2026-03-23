using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using StandardAssets.Characters.ThirdPerson.PunDemos;

public class KillFeedManager : MonoBehaviourPun
{
    public static KillFeedManager Instance;

    [Header("UI")]
    public GameObject killfeedItemPrefab;
    public Transform killfeedItemParent;

    private void Awake()
    {
        Instance = this;
    }

    [PunRPC]
    public void RPC_GetKill(string _killer, string _victim)
    {
        GameObject item = Instantiate(killfeedItemPrefab, killfeedItemParent);
        item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _killer + " killed " + _victim;

        Destroy(item, 2f);
    }
}