using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using TMPro;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks, IRespawnable
{
    public static RoomManager Instance;

    [HideInInspector]
    public string roomCode = "DesertMap";

    public GameObject player;
    public Transform[] spawnPoints;

    public GameObject roomCamera;

    [Header("UI Panels - Drag from Hierarchy")]
    public GameObject nameUIPanel;
    public GameObject mapSelectionUIPanel;
    public GameObject connectingUIPanel;

    [Header("Login References (inside Name UI)")]
    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;
    public Button loginButton;
    public TextMeshProUGUI feedbackText;

    [Header("Map Selection References (inside MapSelection UI)")]
    public GameObject[] mapPanels;
    public Sprite[] mapSprites;
    public string[] mapDisplayNames;
    public string[] mapRoomCodes;
    public string[] mapSceneNames;
    public Button joinGameButton;
    public TextMeshProUGUI mapSelectionFeedbackText;

    [Header("Connection UI References (inside Connecting UI)")]
    public TextMeshProUGUI connectionStatusText;

    private int selectedMapIndex = -1;
    private string currentName;
    private string authToken;
    private bool isAuthenticated = false;
    private bool isQuitting = false;

    private string loginApiUrl = "https://fa96490d-4e09-4a53-9cc9-5a23f1a7f426-00-b1jir99p5w51.kirk.replit.dev/api/Users/login";

    [System.Serializable]
    public class AuthResponseDto
    {
        public string message;
        public string token;
        public UserData user;
    }

    [System.Serializable]
    public class UserLoginDto
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class UserData
    {
        public int userId;
        public string username;
        public string email;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        isQuitting = false;

        if (nameUIPanel != null) nameUIPanel.SetActive(true);
        if (mapSelectionUIPanel != null) mapSelectionUIPanel.SetActive(false);
        if (connectingUIPanel != null) connectingUIPanel.SetActive(false);

        InitializeMapPanels();

        if (joinGameButton != null)
        {
            joinGameButton.interactable = false;
            joinGameButton.onClick.AddListener(OnJoinGameButtonClicked);
        }

        if (loginButton != null)
        {
            loginButton.onClick.RemoveAllListeners();
            loginButton.onClick.AddListener(OnLoginButtonClicked);
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
            FindSpawnPoints();

        ValidateReferences();
    }

    private void InitializeMapPanels()
    {
        if (mapPanels == null || mapPanels.Length == 0)
        {
            Debug.LogWarning("Nincsenek map panelek beállítva!");
            return;
        }

        for (int i = 0; i < mapPanels.Length; i++)
        {
            if (mapPanels[i] == null) continue;

            int index = i;

            Button btn = mapPanels[i].GetComponent<Button>();
            if (btn == null) btn = mapPanels[i].AddComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnMapPanelClicked(index));

            Image mapImage = mapPanels[i].transform.Find("MapImage")?.GetComponent<Image>();
            if (mapImage != null && mapSprites != null && i < mapSprites.Length)
                mapImage.sprite = mapSprites[i];

            TextMeshProUGUI mapName = mapPanels[i].transform.Find("MapName")?.GetComponent<TextMeshProUGUI>();
            if (mapName != null && mapDisplayNames != null && i < mapDisplayNames.Length)
                mapName.text = mapDisplayNames[i];

            Outline outline = mapPanels[i].GetComponent<Outline>();
            if (outline == null) outline = mapPanels[i].AddComponent<Outline>();
            outline.effectColor = new Color(1f, 1f, 0f, 0f);
            outline.enabled = true;
        }

        Debug.Log($"Map panelek inicializálva: {mapPanels.Length} db");
    }

    private void OnMapPanelClicked(int index)
    {
        for (int i = 0; i < mapPanels.Length; i++)
        {
            Outline o = mapPanels[i]?.GetComponent<Outline>();
            if (o != null) o.effectColor = new Color(1f, 1f, 0f, 0f);
        }

        Outline selected = mapPanels[index].GetComponent<Outline>();
        if (selected != null) selected.effectColor = new Color(1f, 1f, 0f, 1f);

        selectedMapIndex = index;

        if (mapRoomCodes != null && index < mapRoomCodes.Length && !string.IsNullOrEmpty(mapRoomCodes[index]))
            roomCode = mapRoomCodes[index];
        else
            roomCode = "Map" + (index + 1);

        Debug.Log($"Kiválasztott pálya: {GetSelectedMapName()} | RoomCode: {roomCode} | Scene: {GetTargetSceneName()}");

        if (isAuthenticated && joinGameButton != null)
            joinGameButton.interactable = true;

        if (mapSelectionFeedbackText != null)
            mapSelectionFeedbackText.text = $"Kiválasztva: {GetSelectedMapName()}";
    }

    public string GetSelectedMapName()
    {
        if (mapDisplayNames != null && selectedMapIndex >= 0 && selectedMapIndex < mapDisplayNames.Length)
            return mapDisplayNames[selectedMapIndex];
        return "Ismeretlen";
    }

    public bool IsMapSelected()
    {
        return selectedMapIndex >= 0 && mapPanels != null && selectedMapIndex < mapPanels.Length;
    }

    private string GetTargetSceneName()
    {
        if (mapSceneNames != null && selectedMapIndex >= 0 && selectedMapIndex < mapSceneNames.Length
            && !string.IsNullOrEmpty(mapSceneNames[selectedMapIndex]))
        {
            return mapSceneNames[selectedMapIndex];
        }
        return roomCode;
    }

    private void OnJoinGameButtonClicked()
    {
        if (!isAuthenticated)
        {
            ShowFeedback("Kérlek lépj be elõször!", Color.red);
            return;
        }

        if (!IsMapSelected())
        {
            ShowFeedback("Kérlek válassz pályát!", Color.red);
            return;
        }

        Debug.Log($"Join gomb megnyomva | Pálya: {GetSelectedMapName()} | Room: {roomCode}");

        if (mapSelectionUIPanel != null) mapSelectionUIPanel.SetActive(false);
        if (connectingUIPanel != null) connectingUIPanel.SetActive(true);
        if (connectionStatusText != null) connectionStatusText.text = "Csatlakozás...";

        ConnectToPhoton();
    }

    public void OnLoginButtonClicked()
    {
        if (usernameInputField == null || passwordInputField == null)
        {
            Debug.LogError("UI referenciák hiányoznak!");
            return;
        }

        string username = usernameInputField.text.Trim();
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(username)) { ShowFeedback("Add meg a felhasználónevet!", Color.red); return; }
        if (string.IsNullOrEmpty(password)) { ShowFeedback("Add meg a jelszót!", Color.red); return; }

        loginButton.interactable = false;
        ShowFeedback("Ellenõrzés...", Color.yellow);
        currentName = username;
        StartCoroutine(LoginUser(username, password));
    }

    private void ShowMapSelection()
    {
        if (nameUIPanel != null) nameUIPanel.SetActive(false);
        if (mapSelectionUIPanel != null) mapSelectionUIPanel.SetActive(true);
        if (connectingUIPanel != null) connectingUIPanel.SetActive(false);

        selectedMapIndex = -1;
        if (joinGameButton != null) joinGameButton.interactable = false;

        if (mapPanels != null)
        {
            foreach (var panel in mapPanels)
            {
                Outline o = panel?.GetComponent<Outline>();
                if (o != null) o.effectColor = new Color(1f, 1f, 0f, 0f);
            }
        }

        Debug.Log("Map választó megjelenik");
    }

    private void ShowFeedback(string message, Color color)
    {
        Debug.Log($"[Feedback] {message}");
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;
            if (color != Color.yellow)
                StartCoroutine(ClearFeedback());
        }
    }

    private IEnumerator ClearFeedback()
    {
        yield return new WaitForSeconds(3f);
        if (feedbackText != null) feedbackText.text = "";
    }

    private void ResetLoginState()
    {
        isAuthenticated = false;
        if (loginButton != null) loginButton.interactable = true;
        if (nameUIPanel != null) nameUIPanel.SetActive(true);
        if (mapSelectionUIPanel != null) mapSelectionUIPanel.SetActive(false);
    }

    private IEnumerator LoginUser(string username, string password)
    {
        UserLoginDto loginData = new UserLoginDto { username = username, password = password };
        string jsonData = JsonUtility.ToJson(loginData);

        using (UnityWebRequest request = new UnityWebRequest(loginApiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 10;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                ProcessSuccessfulLogin(request.downloadHandler.text, username);
            else
                ProcessFailedLogin(request.responseCode);
        }
    }

    private void ProcessSuccessfulLogin(string responseText, string username)
    {
        try
        {
            AuthResponseDto response = JsonUtility.FromJson<AuthResponseDto>(responseText);

            if (response != null && !string.IsNullOrEmpty(response.token) && response.user != null)
            {
                authToken = response.token;
                isAuthenticated = true;

                PlayerPrefs.SetInt("UserId", response.user.userId);
                PlayerPrefs.SetString("AuthToken", authToken);
                PlayerPrefs.Save();

                if (WebStatsManager.Instance != null)
                    WebStatsManager.Instance.SetAuthToken(authToken);

                ShowMapSelection();
                ShowFeedback("Sikeres bejelentkezés! Válassz pályát.", Color.green);
                loginButton.interactable = true;
            }
            else
            {
                ShowFeedback("Érvénytelen szerver válasz", Color.red);
                ResetLoginState();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Parse hiba: {ex.Message}");
            ShowFeedback("Szerver hiba", Color.red);
            ResetLoginState();
        }
    }

    private void ProcessFailedLogin(long responseCode)
    {
        string msg = responseCode switch
        {
            401 => "Hibás felhasználónév vagy jelszó",
            404 => "A felhasználó nem található",
            400 => "Érvénytelen kérés",
            500 => "Szerver hiba",
            _ => "Bejelentkezés sikertelen"
        };
        ShowFeedback(msg, Color.red);
        ResetLoginState();
    }

    private void ConnectToPhoton()
    {
        if (!string.IsNullOrEmpty(currentName))
            PhotonNetwork.LocalPlayer.NickName = currentName;

        if (PhotonNetwork.IsConnected)
        {
            JoinOrCreateSelectedRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void JoinOrCreateSelectedRoom()
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 10,
            IsVisible = true,
            IsOpen = true,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "scene", GetTargetSceneName() },
                { "map",   GetSelectedMapName() }
            },
            CustomRoomPropertiesForLobby = new string[] { "scene", "map" }
        };

        PhotonNetwork.JoinOrCreateRoom(roomCode, roomOptions, TypedLobby.Default);
    }

    public void RespawnPlayer()
    {
        if (player == null) { Debug.LogError("Player prefab nincs beállítva!"); return; }
        StartCoroutine(DelayedRespawn());
    }

    private IEnumerator DelayedRespawn()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);
        PhotonNetwork.Instantiate(player.name, GetRandomSpawnPos(), Quaternion.identity);
    }

    private void FindSpawnPoints()
    {
        List<Transform> found = new List<Transform>();
        foreach (Transform t in FindObjectsOfType<Transform>())
        {
            if (t.name.Contains("SpawnPoint"))
            {
                Vector3 pos = t.position;
                if (pos.y < 1f) { pos.y = 5f; t.position = pos; }
                found.Add(t);
            }
        }
        spawnPoints = found.ToArray();
    }

    public Vector3 GetRandomSpawnPos()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            FindSpawnPoints();

        if (spawnPoints == null || spawnPoints.Length == 0)
            return new Vector3(0, 15f, 0);

        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        if (sp == null) return new Vector3(0, 15f, 0);

        Vector3 pos = sp.position;
        if (pos.y < 2f) pos.y = 10f;
        return pos;
    }

    private void ValidateReferences()
    {
        if (nameUIPanel == null) Debug.LogWarning("[RoomManager] Name UI Panel nincs beállítva!");
        if (mapSelectionUIPanel == null) Debug.LogWarning("[RoomManager] Map Selection UI Panel nincs beállítva!");
        if (connectingUIPanel == null) Debug.LogWarning("[RoomManager] Connecting UI Panel nincs beállítva!");
        if (usernameInputField == null) Debug.LogWarning("[RoomManager] Username Input Field nincs beállítva!");
        if (passwordInputField == null) Debug.LogWarning("[RoomManager] Password Input Field nincs beállítva!");
        if (loginButton == null) Debug.LogWarning("[RoomManager] Login Button nincs beállítva!");
        if (joinGameButton == null) Debug.LogWarning("[RoomManager] Join Game Button nincs beállítva!");
        if (mapPanels == null || mapPanels.Length == 0) Debug.LogWarning("[RoomManager] Map Panels nincsenek beállítva!");
        if (mapSceneNames == null || mapSceneNames.Length == 0) Debug.LogWarning("[RoomManager] Map Scene Names nincsenek beállítva!");
    }

    public override void OnConnectedToMaster()
    {
        if (isQuitting) return;
        if (connectionStatusText != null) connectionStatusText.text = "Csatlakozva! Lobby...";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        if (isQuitting) return;
        if (connectionStatusText != null) connectionStatusText.text = $"Csatlakozás: {GetSelectedMapName()}...";
        JoinOrCreateSelectedRoom();
    }

    public override void OnJoinedRoom()
    {
        if (isQuitting) return;

        if (connectingUIPanel != null) connectingUIPanel.SetActive(false);
        if (roomCamera != null) roomCamera.SetActive(false);

        string targetScene = GetTargetSceneName();
        string currentScene = SceneManager.GetActiveScene().name;

        if (targetScene != currentScene)
        {
            if (connectionStatusText != null) connectionStatusText.text = $"Betöltés: {GetSelectedMapName()}...";
            PhotonNetwork.LoadLevel(targetScene);
        }
        else
        {
            if (player == null) { Debug.LogError("Player prefab nincs beállítva!"); return; }
            PhotonNetwork.Instantiate(player.name, GetRandomSpawnPos(), Quaternion.identity);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (isQuitting) return;
        if (connectingUIPanel != null) connectingUIPanel.SetActive(false);
        if (isAuthenticated) { ShowFeedback("Kapcsolat megszakadt", Color.red); ResetLoginState(); }
    }

    public void SetQuittingFlag(bool quitting)
    {
        isQuitting = quitting;
    }
}