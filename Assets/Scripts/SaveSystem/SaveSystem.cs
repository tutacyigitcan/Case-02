using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string saveFolder = Application.persistentDataPath + "/Saves/";

    public static void SaveCheckpoint(
        string checkpointName, string sceneName, Vector3 position, int health,
        List<string> inventory, int storyProgress)
    {
        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }

        string filePath = saveFolder + checkpointName + ".json";
        
        PlayerData data = new PlayerData(sceneName, position, health, inventory, storyProgress);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);

        Debug.Log($"Checkpoint kaydedildi: {checkpointName}, Konum: {position}");
    }
    
    // Save dosyasını yükle
    public static PlayerData LoadCheckpoint(string checkpointName)
    {
        string path = saveFolder + checkpointName + ".json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log(
                $"Checkpoint yüklendi: {checkpointName}, İlerleme: {data.storyProgress}%, Tarih: {data.saveTime}");
            if (data == null)
            {
                Debug.LogError("Checkpoint verisi okunamadı!");
                return null;
            }
            Debug.Log($"Checkpoint yüklendi: {checkpointName}, Can: {data.health}");
            return data;
        }
        else
        {
            Debug.LogWarning("Kaydedilmiş dosya bulunamadı" + path);
            return null;
        }
    }

    public static List<string> GetAllSaveFiles()
    {
        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }

        string[] files = Directory.GetFiles(saveFolder, "*.json");
        List<string> saveFiles = new List<string>();
        foreach (string file in files)
        {
            saveFiles.Add(Path.GetFileNameWithoutExtension(file));
        }
        Debug.Log($"Toplam {saveFiles.Count} kayıtlı dosya bulundu.");
        return saveFiles;
    }
    
    public static void DeleteAllSaves()
    {
        if (Directory.Exists(saveFolder))
        {
            string[] files = Directory.GetFiles(saveFolder);
            foreach (string file in files)
            {
                File.Delete(file);  // Dosyaları sil
            }
            Debug.Log("Tüm kayıt dosyaları başarıyla silindi.");
        }
        else
        {
            Debug.LogWarning("Kayıt klasörü bulunamadı.");
        }
    }
}