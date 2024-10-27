using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [SerializeField] private List<Image> heartIcons;
    
    [SerializeField] private Text checkpointNotification;
    public Transform saveFileListParent;
    public GameObject saveFileButtonPrefab;

    [SerializeField] private GameObject teleportPanel;
    [SerializeField] private Transform teleportListParent;
    [SerializeField] private GameObject checkpointButtonPrefab;
    
    private HealthManager healthManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        healthManager = FindObjectOfType<HealthManager>();
        UpdateHearts(GameManager.Instance.Health);
    }

    private void Update()
    {
        UpdateHearts(GameManager.Instance.Health);
    }

    public void UpdateHearts(int currentHealth)
    {
       // int currentLives = GameManager.Instance.Health;

        for (int i = 0; i < heartIcons.Count; i++)
        {
            heartIcons[i].enabled = i < currentHealth;
        }
    }
    
    
    public void ShowCheckpointNotification()
    {
        StartCoroutine(ShowNotification());
    }
    

    private IEnumerator ShowNotification()
    {
        checkpointNotification.gameObject.SetActive(true); // UI açılır
        checkpointNotification.text = "Checkpoint'e Ulaşıldı! Oyun Kaydedildi.";
        yield return new WaitForSeconds(2f); // 2 saniye göster
        checkpointNotification.gameObject.SetActive(false); // UI kapanır
    }
    
    public void LoadSaveFiles()
    {
        // Var olan save dosyalarını al
        List<string> saveFiles = SaveSystem.GetAllSaveFiles();

        // Listeyi temizle
        foreach (Transform child in saveFileListParent)
        {
            Destroy(child.gameObject);
        }

        // Her dosya için bir buton oluştur
        foreach (string saveFile in saveFiles)
        {
            GameObject button = Instantiate(saveFileButtonPrefab, saveFileListParent);
            button.GetComponentInChildren<Text>().text = saveFile;
            button.GetComponent<Button>().onClick.AddListener(() => OnSaveFileClicked(saveFile));
        }
    }

    private void OnSaveFileClicked(string saveFile)
    {
        GameManager.Instance.LoadLastCheckpoint(saveFile); // Seçilen dosyayı yükle
    }
    
    public void TeleportUI(List<Transform> activeCheckpoints)
    {
        teleportPanel.SetActive(true);
        
        foreach (Transform child in teleportListParent)
        {
            Destroy(child.gameObject);
        }
        Debug.Log($"Aktif checkpoint sayısı: {activeCheckpoints.Count}");
        foreach (Transform checkpoint in activeCheckpoints)
        {
            GameObject button = Instantiate(checkpointButtonPrefab, teleportListParent);
            button.GetComponentInChildren<Text>().text = checkpoint.name;  // Buton ismini ata
            
            button.GetComponent<Button>().onClick.AddListener(() => {
                GameManager.Instance.TeleportToCheckpoint(checkpoint);  // Oyuncuyu ışınla
                teleportPanel.SetActive(false);  // Paneli kapat
                Debug.Log($"Tıklanan checkpoint: {checkpoint.name}");
            });
        }
    }
    
    public void HideTeleportUI()
    {
        teleportPanel.SetActive(false);
    }
    
}
