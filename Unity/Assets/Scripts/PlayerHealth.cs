using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviourPun
{
    [Header("Health Set Up")]
    public int maxHealth;

    [Header("UI Set Up")]
    public TextMeshProUGUI healthText;
    public Image healthFillImage;

    [HideInInspector] public int health;
    public bool isDead = false;

    private void Start()
    {
        health = maxHealth;
        isDead = false;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (healthText != null)
            healthText.text = $"<b>{health}/</b>{maxHealth}";
        if (healthFillImage != null)
            healthFillImage.fillAmount = (float)health / maxHealth;
    }

    [PunRPC]
    public void RPC_TakeDamage(int _damage)
    {
        // Don't process damage if already dead
        if (isDead) return;

        health = Mathf.Max(0, health - _damage);

        if (photonView.IsMine)
        {
            UpdateUI();
        }

        if (health <= 0 && !isDead)
        {
            isDead = true;

            if (photonView.IsMine)
            {
                // Get the PlayerHitAndKillsManager component and call OnPlayerDeath
                PlayerHitAndKillsManager killsManager = GetComponent<PlayerHitAndKillsManager>();
                if (killsManager != null)
                {
                    killsManager.OnPlayerDeath();
                    Debug.Log("Death sent to backend");
                }
                else
                {
                    Debug.LogError("PlayerHitAndKillsManager not found on player!");
                }
            }

            // THIS IS THE KEY - Make player invisible immediately on ALL clients
            SetPlayerVisible(false);

            if (photonView.IsMine)
            {
                LocalPlayerKDManager.Instance.OnDied();

                if (GameSceneManager.Instance != null)
                {
                    GameSceneManager.Instance.RespawnPlayer();
                }
                else if (RoomManager.Instance != null)
                {
                    RoomManager.Instance.RespawnPlayer();
                }
                else
                {
                    Debug.LogError("[PlayerHealth] Nincs respawn manager a scene-ben!");
                }

                // Destroy after a tiny delay to ensure all RPCs are processed
                // The player is already invisible, so this won't be noticeable
                Invoke(nameof(DestroyPlayer), 0.2f);
            }
        }
    }

    private void SetPlayerVisible(bool visible)
    {
        // Disable/enable all renderers (makes player invisible/visible)
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer ren in renderers)
        {
            ren.enabled = visible;
        }

        // Disable/enable all skinned mesh renderers (for characters with animations)
        SkinnedMeshRenderer[] skinnedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer ren in skinnedRenderers)
        {
            ren.enabled = visible;
        }

        // Disable/enable all canvas/UI elements (like name tags)
        Canvas[] canvases = GetComponentsInChildren<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            canvas.enabled = visible;
        }

        // Disable/enable all TextMeshPro components
        TMPro.TMP_Text[] texts = GetComponentsInChildren<TMPro.TMP_Text>();
        foreach (TMPro.TMP_Text text in texts)
        {
            text.enabled = visible;
        }

        // Disable colliders so player can't be hit
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = visible;
        }
    }

    private void DestroyPlayer()
    {
        if (photonView.IsMine && gameObject != null)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    // Optional: If you want a death effect before disappearing
    private void OnDestroy()
    {
        // Clean up any lingering effects
    }
}