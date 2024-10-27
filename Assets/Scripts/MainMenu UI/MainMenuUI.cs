using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject checkpointPanel;
    [SerializeField] private Transform checkpointListParent; 
    [SerializeField] private GameObject saveFileButtonPrefab;
    
    public void OnLoadButtonClicked()
    {
        checkpointPanel.SetActive(true);
        LoadSaveFiles();
    }

    public void saveDelete()
    {
        SaveSystem.DeleteAllSaves();
        Debug.Log("Tüm kayıtlar silindi.");
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