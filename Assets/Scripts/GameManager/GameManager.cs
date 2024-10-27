using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private GameObject canvasPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject cameraPrefab;

    public GameObject playerInstance;
    private GameObject cameraInstance;
    private GameObject canvasInstance;

    public Transform respawnPoint;
    private Transform currentCheckpoint;
    private Dictionary<int, List<Transform>> respawnPointsByScene = new Dictionary<int, List<Transform>>();
    
    private Dictionary<string, int> checkpointsPerScene = new Dictionary<string, int>();
    private Dictionary<string, int> passedCheckpointsPerScene = new Dictionary<string, int>();
    private HashSet<string> activatedCheckpoints = new HashSet<string>();
    
    [Header("Player Data")]
    public int maxLives = 5;
    public int Health;
    public Vector3 PlayerPosition = Vector3.zero;
    public List<string> Inventory { get; private set; } = new List<string>();
    public int StoryProgress { get; private set; } = 0;

    public string SceneName = "MainMenu";

    public GameObject[] checkpoints;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
        
        InitializeSceneCheckpoints();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    public void UpdateLives(int newLives)
    {
        Health = newLives;
        SavePlayerData();  // Oyuncu verisini kaydet
        Debug.Log("GameManager: Can güncellendi: " + Health);
    }

    #region TESTER
    private void InitializeSceneCheckpoints()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        int totalCheckpoints = GameObject.FindGameObjectsWithTag("Checkpoint").Length;

        if (!checkpointsPerScene.ContainsKey(currentScene))
        {
            checkpointsPerScene[currentScene] = totalCheckpoints;
            passedCheckpointsPerScene[currentScene] = 0;
        }
    }
    
    public void CheckpointPassed()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (passedCheckpointsPerScene.ContainsKey(currentScene))
        {
            passedCheckpointsPerScene[currentScene]++;
            SaveCurrentProgress();
        }
    }
    
    private void SaveCurrentProgress()
    {
        int totalPassed = GetTotalPassedCheckpoints();
        int totalCheckpoints = GetTotalCheckpoints();
        int progress = (int)((float)totalPassed / totalCheckpoints * 100);

        SaveSystem.SaveCheckpoint(
            "LastCheckpoint",
            SceneManager.GetActiveScene().name,
            playerInstance.transform.position,
            Health,
            Inventory,
            progress
        );
    }
    
    private int GetTotalCheckpoints()
    {
        int total = 0;
        foreach (var count in checkpointsPerScene.Values)
        {
            total += count;
        }
        return total;
    }

    private int GetTotalPassedCheckpoints()
    {
        int total = 0;
        foreach (var count in passedCheckpointsPerScene.Values)
        {
            total += count;
        }
        return total;
    }
    #endregion
    

    // Sahne yüklendiğinde çağrılır
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainMenu")
        {
            Debug.Log($"Sahne yüklendi: {scene.name}");
            InstantiateEssentialPrefabs();  // Gerekli prefabları oluştur

            // Checkpoint veya varsayılan pozisyondan başlat
            bool success = LoadLastCheckpoint("LastCheckpoint");

            if (!success)
            {
                Debug.LogWarning("Checkpoint bulunamadı, varsayılan pozisyonda başlatılıyor.");
                playerInstance.transform.position = Vector3.zero;
            }
        }
    }

    // Oyuncu, canvas ve kamera prefablarını yaratır
    private void InstantiateEssentialPrefabs()
    {
        if (canvasPrefab != null && canvasInstance == null)
        {
            canvasInstance = Instantiate(canvasPrefab);
            DontDestroyOnLoad(canvasInstance);
        }

        if (cameraPrefab != null && cameraInstance == null)
        {
            cameraInstance = Instantiate(cameraPrefab);
            DontDestroyOnLoad(cameraInstance);
        }

        if (playerPrefab != null && playerInstance == null)
        {
            playerInstance = Instantiate(playerPrefab);
            DontDestroyOnLoad(playerInstance);
            Debug.Log("Player yaratıldı: " + playerInstance.name);
        }
    }
    
    public void RespawnPlayer()
    {
        if (playerInstance != null && currentCheckpoint != null)
        {
            DisablePassedCheckpoints(currentCheckpoint.position.x);
            playerInstance.transform.position = currentCheckpoint.position;
            Debug.Log("Oyuncu checkpoint'ten doğdu.");
        }
        else
        {
            Debug.LogWarning("Checkpoint veya oyuncu nesnesi bulunamadı!");
        }
        
        if (currentCheckpoint != null)  // Eğer checkpoint varsa buradan başla
        {
            DisablePassedCheckpoints(currentCheckpoint.position.x);
            playerInstance.transform.position = currentCheckpoint.position;
            Debug.Log("Oyuncu checkpoint'ten doğdu.");
        }
        else if (respawnPoint != null)  // Checkpoint yoksa respawn noktasından başla
        {
            DisablePassedCheckpoints(respawnPoint.position.x);
            playerInstance.transform.position = respawnPoint.position;
            Debug.Log("Oyuncu respawn noktasından doğdu.");
        }
        else
        {
            playerInstance.transform.position = Vector3.zero;  // Başlangıç pozisyonuna dön
            Debug.LogWarning("Respawn veya checkpoint bulunamadı. Başlangıç pozisyonuna döndü.");
        }
    }

    // Sahne geçişini başlat
    public void LoadScene(string sceneName)
    {
        StartCoroutine(HandleSceneTransition(sceneName));
    }

    // Sahne geçişini asenkron olarak yönetir
    private IEnumerator HandleSceneTransition(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);  // Yükleme sonrası kısa bekleme
        InitializePlayer();  // Oyuncuyu doğru konuma yerleştir
    }
    
    public Transform GetRespawnPointForCurrentScene()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (respawnPointsByScene.ContainsKey(sceneIndex) && respawnPointsByScene[sceneIndex].Count > 0)
        {
            // Listenin ilk elemanını döndür
            return respawnPointsByScene[sceneIndex][0];
        }
        else
        {
            Debug.LogWarning("Sahne için respawn point bulunamadı: " + sceneIndex);
            return null;
        }
    }

    // Son checkpoint'i yükler
    public bool LoadLastCheckpoint(string checkpointName)
    {
        
        PlayerData data = SaveSystem.LoadCheckpoint(checkpointName);
        if (data != null && playerInstance != null)
        {
            PlayerPosition = new Vector3(data.position[0], data.position[1], data.position[2]);
            Health = data.health;
            Inventory = new List<string>(data.inventory);
            StoryProgress = data.storyProgress;
            DisablePassedCheckpoints(PlayerPosition.x);

            playerInstance.transform.position = PlayerPosition;
            Debug.Log($"Checkpoint'ten yüklendi: {checkpointName}, Pozisyon: {PlayerPosition}");
            return true;
        }
        return false;
    }
    
    public void DisablePassedCheckpoints(float playerPosition) {
        foreach (var checkpoint in checkpoints) {
            if (checkpoint.transform.position.x <= playerPosition) {
                var collider = checkpoint.GetComponent<BoxCollider2D>();
                if (collider != null) {
                    collider.enabled = false;
                }
            }
        }
    }

    // Yeni checkpoint'i kaydeder
    public void SetCurrentCheckpoint(Transform checkpoint)
    {
        string checkpointName = checkpoint.name;

        // Eğer checkpoint zaten aktifse tekrar kaydetme
        if (activatedCheckpoints.Contains(checkpointName))
        {
            Debug.Log($"Checkpoint zaten aktif: {checkpointName}");
            return;
        }
        
        activatedCheckpoints.Add(checkpointName);
        currentCheckpoint = checkpoint;
        PlayerPosition = checkpoint.position;
        SceneName = SceneManager.GetActiveScene().name;

        SaveSystem.SaveCheckpoint("LastCheckpoint", SceneName, PlayerPosition, Health, Inventory, StoryProgress);
        Debug.Log($"Checkpoint kaydedildi! Pozisyon: {PlayerPosition}");
    }

    // Oyuncuyu başlatır veya checkpoint'e yerleştirir
    public void InitializePlayer()
    {
        if (playerInstance == null)
        {
            playerInstance = Instantiate(playerPrefab);
            DontDestroyOnLoad(playerInstance);
        }

        if (currentCheckpoint != null)
        {
            playerInstance.transform.position = currentCheckpoint.position;
            Debug.Log("Oyuncu checkpoint'ten doğdu.");
        }
        else if (respawnPoint != null)
        {
            playerInstance.transform.position = respawnPoint.position;
            Debug.Log("Oyuncu respawn noktasından doğdu.");
        }
        else
        {
            playerInstance.transform.position = Vector3.zero;
            Debug.LogWarning("Checkpoint veya respawn noktası bulunamadı.");
        }
    }
    
    
    // Oyuncu verilerini kaydeder
    public void SavePlayerData()
    {
        SaveSystem.SaveCheckpoint("LastCheckPoint", SceneName, PlayerPosition, Health, Inventory, StoryProgress);
    }

    // Oyuncu verilerini yükler
    public void LoadPlayerData()
    {
        PlayerData data = SaveSystem.LoadCheckpoint("LastCheckPoint");
        if (data != null)
        {
            PlayerPosition = new Vector3(data.position[0], data.position[1], data.position[2]);
            Health = data.health > 0 ? data.health : maxLives;
            Inventory = new List<string>(data.inventory);
            StoryProgress = data.storyProgress;
        }
        else
        {
            Debug.LogWarning("Oyuncu verisi yüklenemedi.");
        }
    }
}
