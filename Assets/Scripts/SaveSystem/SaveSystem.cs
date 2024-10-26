using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string saveFolder = Application.persistentDataPath + "/Saves/";

    public static void SaveCheckpoint(string checkpointName, Vector3 position,int health, List<string> inventory,
        int storyProgress)
    {
        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder); // Klasör yoksa oluştur
        }

        PlayerData data = new PlayerData(position, health, inventory, storyProgress);
        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(saveFolder + checkpointName + ".json", json);
        Debug.Log($"Checkpoint kaydedildi: {checkpointName}, Can: {health}");
    }
    
    // Save dosyasını yükle
    public static PlayerData LoadCheckpoint(string checkpointName)
    {
        string path = saveFolder + checkpointName + ".json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            // Geçersiz sağlık değerleri için kontrol ekleyin
            if (data.health <= 0)
            {
                data.health = 5;  // Negatif sağlık durumunda sıfırla
                Debug.LogWarning("Sağlık değeri geçersiz! 0 olarak ayarlandı.");
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

        return saveFiles;
    }
}