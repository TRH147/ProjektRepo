using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour, IRespawnable
{
    public static GameSceneManager Instance; // ADD THIS

    [Header("Player")]
    public GameObject playerPrefab;
    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    private void Awake() // ADD THIS
    {
        Instance = this;
    }

    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log($"[GameSceneManager] Szobában vagyunk, spawn-olás... Scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
            SpawnPlayer();
        }
        else
        {
            Debug.LogWarning("[GameSceneManager] Nem vagyunk Photon szobában! Visszatérés a fõmenübe...");
            UnityEngine.SceneManagement.SceneManager.LoadScene("DesertMap");
        }
    }

    // ADD THIS — satisfies IRespawnable
    public void RespawnPlayer()
    {
        if (playerPrefab == null) { Debug.LogError("[GameSceneManager] Player prefab nincs beállítva!"); return; }
        StartCoroutine(DelayedRespawn());
    }

    private IEnumerator DelayedRespawn()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);
        PhotonNetwork.Instantiate(playerPrefab.name, GetSpawnPos(), Quaternion.identity);
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null) { Debug.LogError("[GameSceneManager] Player prefab nincs beállítva!"); return; }
        Vector3 spawnPos = GetSpawnPos();
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity);
        Debug.Log($"[GameSceneManager] Játékos létrehozva: {spawnPos}");
    }

    private Vector3 GetSpawnPos()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
            if (sp != null)
            {
                Vector3 pos = sp.position;
                if (pos.y < 2f) pos.y = 5f;
                return pos;
            }
        }

        Transform[] allTransforms = FindObjectsOfType<Transform>();
        List<Transform> found = new List<Transform>();
        foreach (Transform t in allTransforms)
        {
            if (t.name.Contains("SpawnPoint"))
                found.Add(t);
        }
        if (found.Count > 0)
        {
            Vector3 pos = found[Random.Range(0, found.Count)].position;
            if (pos.y < 2f) pos.y = 5f;
            return pos;
        }

        Debug.LogWarning("[GameSceneManager] Nem találhatók spawn pointok, fallback pozíció használata.");
        return new Vector3(0, 5f, 0);
    }
}