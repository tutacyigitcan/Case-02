using System;
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

    private GameObject playerInstance;
    private GameObject cameraInstance;
    private GameObject canvasInstance;
    
    public Transform respawnPoint;
    private Transform currentCheckpoint;
    private Transform lastCheckpoint;
    
    private Dictionary<int, List<Transform>> respawnPointsByScene = new Dictionary<int, List<Transform>>();

    [Header("Player Data")] 
    public int maxLives = 5;
    public int Health;
    public Vector3 PlayerPosition = Vector3.zero;
    public List<string> Inventory { get; private set; } = new List<string>();
    public int StoryProgress { get; private set; } = 0;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InstantiateEssentialPrefabs();
            LoadPlayerData();
            InitializePlayerData();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializePlayerData()
    {
        Health = maxLives;
    }
    
    public void UpdateLives(int newLives)
    {
        Health = newLives; // Can bilgisini güncelle
        Debug.Log("GameManager: CurrentLives güncellendi: " + Health);
    }
    
    // Gerekli prefabs'ları sahneye ekle
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
        }
    }
    
    public void RespawnPlayer()
    {
        if (currentCheckpoint != null) // En son checkpoint'ten başla
        {
            playerInstance.transform.position = currentCheckpoint.position;
            Debug.Log("Oyuncu checkpoint'ten doğdu.");
        }
        else if (respawnPoint != null) // Eğer checkpoint yoksa respawn point'ten başla
        {
            playerInstance.transform.position = respawnPoint.position;
            Debug.Log("Oyuncu respawn point'ten doğdu.");
        }
        else
        {
            playerInstance.transform.position = Vector3.zero;
            Debug.LogWarning("Respawn veya checkpoint bulunamadı, başlangıç noktasına döndü.");
        }
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
    
    // Yeni respawn point ata
    public void SetRespawnPoint(Transform point)
    {
        respawnPoint = point;
    }
    
    // Yeni checkpoint ata ve oyuncu verilerini kaydet
    public void SetCurrentCheckpoint(Transform checkpoint)
    {
        currentCheckpoint = checkpoint;
        SaveCheckpointData(); // Veriyi kaydet
    }
    
    private void SaveCheckpointData()
    {
        PlayerPosition = currentCheckpoint.position;
        SaveSystem.SaveCheckpoint("LastCheckpoint", PlayerPosition, Health, Inventory, StoryProgress);
    }
    
    public void SetLastCheckpoint(Transform checkpoint)
    {
        lastCheckpoint = checkpoint;
        Debug.Log($"GameManager: Checkpoint kaydedildi: {checkpoint.name}");
    }
    
    public Transform GetLastCheckpoint()
    {
        return lastCheckpoint;
    }
    
    // Checkpoint'ten yükleme yap
    public bool LoadLastCheckpoint(string checkpointName)
    {
        PlayerData data = SaveSystem.LoadCheckpoint("LastCheckpoint");
        if (data != null)
        {
            PlayerPosition = new Vector3(data.position[0], data.position[1], data.position[2]);
            Health = data.currentLives;
            Inventory = new List<string>(data.inventory);
            StoryProgress = data.storyProgress;

            InitializePlayer(); // Oyuncuyu pozisyona yerleştir
            Debug.Log("Checkpoint yüklendi: " + checkpointName);
            return true;
        }
        else
        {
            Debug.LogWarning("Kaydedilmiş checkpoint bulunamadı: " + checkpointName);
            return false;
        }
    }
    
    
    
    // Oyuncuyu başlangıçta veya checkpoint'ten başlat
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
        }
        else if (respawnPoint != null)
        {
            playerInstance.transform.position = respawnPoint.position;
        }
        else
        {
            playerInstance.transform.position = Vector3.zero; // Başlangıç pozisyonu
        }
    }
    
    // Oyuncu verilerini kaydet
    public void SavePlayerData()
    {
        SaveSystem.SaveCheckpoint("LastSave",PlayerPosition, Health, Inventory, StoryProgress);
    }
    
    // Oyuncu verilerini yükle
    public void LoadPlayerData()
    {
        PlayerData data = SaveSystem.LoadCheckpoint("LastSave");
        if (data != null)
        {
            PlayerPosition = new Vector3(data.position[0], data.position[1], data.position[2]);
            Health = data.currentLives;
            Inventory = new List<string>(data.inventory);
            StoryProgress = data.storyProgress;
        }
        else
        {
            Debug.LogWarning("Kaydedilmiş veri bulunamadı.");
        }
    }
    
    // Envantere yeni bir eşya ekle
    public void AddItemToInventory(string item)
    {
        Inventory.Add(item);
        Debug.Log(item + " envantere eklendi.");
    }

    // Hikaye ilerlemesini güncelle
    public void UpdateStoryProgress(int progress)
    {
        StoryProgress = progress;
        Debug.Log("Hikaye ilerlemesi: " + progress);
    }
}