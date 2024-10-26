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
        
        List<string> saveFiles = SaveSystem.GetAllSaveFiles();
        foreach (string saveFile in saveFiles)
        {
            
            GameObject button = Instantiate(saveFileButtonPrefab, checkpointListParent);
            button.GetComponentInChildren<Text>().text = saveFile;
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