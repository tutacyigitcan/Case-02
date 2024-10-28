using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject checkpointPanel;
    [SerializeField] private Transform checkpointListParent; 
    [SerializeField] private GameObject saveFileButtonPrefab;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button continueButton;
    
    private void Start()
    {
        SetupMenu();
    }
    
    private void SetupMenu()
    {
        List<string> saveFiles = SaveSystem.GetAllSaveFiles();
        
        if (saveFiles.Count > 0)
        {
            continueButton.gameObject.SetActive(true);
            startGameButton.gameObject.SetActive(true);
        }
        else
        {
            continueButton.gameObject.SetActive(false);
            startGameButton.gameObject.SetActive(true);
        }
    }
    
    public void OnStartGameButtonClicked()
    {
        List<string> saveFiles = SaveSystem.GetAllSaveFiles();
        if (saveFiles.Count > 0)
        {
            bool userConfirmed = ConfirmDeleteSave();  // Bu metot uyarıyı gösterir ve onay alır.
            if (!userConfirmed)
            {
                Debug.Log("Kullanıcı yeni oyuna başlamayı iptal etti.");
                return;
            }

            SaveSystem.DeleteAllSaves();
            Debug.Log("Mevcut kayıtlar silindi. Oyun sıfırdan başlıyor.");
        }
        SceneManager.LoadScene("GameScene 01");
    }
    
    private bool ConfirmDeleteSave()
    {
        return UnityEditor.EditorUtility.DisplayDialog(
            "Kayıtları Sil", 
            "Yeni oyuna başlarsanız mevcut kayıtlar silinecek. Devam etmek istiyor musunuz?", 
            "Evet", "Hayır"
        );
    }
    
    public void OnContinueButtonClicked()
    {
        // Son checkpoint'i yükle
        PlayerData lastCheckpoint = SaveSystem.LoadCheckpoint("LastCheckpoint");
        if (lastCheckpoint != null)
        {
            Debug.Log("Son kayıt yükleniyor: " + lastCheckpoint.sceneName);
            GameManager.Instance.LoadScene(lastCheckpoint.sceneName);
        }
        else
        {
            Debug.LogWarning("Kayıt bulunamadı! Yeni oyuna başlanıyor.");
            OnStartGameButtonClicked();
        }
    }
    
    public void OnLoadButtonClicked()
    {
        checkpointPanel.SetActive(true);
        LoadSaveFiles();
    }

    public void LoadGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void saveDelete()
    {
        SaveSystem.DeleteAllSaves();
        Debug.Log("Tüm kayıtlar silindi.");
        
        LoadSaveFiles();
    }
    
    public void OnBackButtonClicked()
    {
        checkpointPanel.SetActive(false); // Paneli kapat
        Debug.Log("Checkpoint paneli kapatıldı.");
    }


    private void LoadSaveFiles()
    {
        // Önce var olan butonları temizleyelim
        foreach (Transform child in checkpointListParent)
        {
            Destroy(child.gameObject);
        }

        // Tüm kayıt dosyalarını al
        List<string> saveFiles = SaveSystem.GetAllSaveFiles();

        foreach (string saveFile in saveFiles)
        {
            // Butonu oluştur
            GameObject button = Instantiate(saveFileButtonPrefab, checkpointListParent);
            Text buttonText = button.GetComponentInChildren<Text>();

            // Kayıt dosyasını yükle ve bilgileri yazdır
            PlayerData data = SaveSystem.LoadCheckpoint(saveFile);
            if (data != null)
            {
                // İlerleme yüzdesini ve kaydetme zamanını göster
                buttonText.text = $"{saveFile}\nİlerleme: {data.storyProgress}%\nTarih: {data.saveTime}";
            }
            else
            {
                buttonText.text = $"{saveFile}\nVeri Yüklenemedi!";
            }

            // Butona tıklama olayı ekle
            button.GetComponent<Button>().onClick.AddListener(() => OnSaveFileClicked(saveFile));
        }
    }
    
    public void OnSaveFileClicked(string saveFile)
    {
        Debug.Log($"Tıklanan save dosyası: {saveFile}");
        PlayerData data = SaveSystem.LoadCheckpoint(saveFile);

        if (data != null)
        {
            Debug.Log($"Checkpoint yüklendi: {saveFile}, Sahne: {data.sceneName}");
            GameManager.Instance.LoadScene(data.sceneName);
        }
        else
        {
            Debug.LogWarning("Kayıt dosyası bulunamadı!");
        }
    }
}