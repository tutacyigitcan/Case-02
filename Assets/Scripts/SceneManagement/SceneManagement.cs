using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    private static SceneManagement instance;
    
    public static SceneManagement Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("SceneManagement");
                instance = go.AddComponent<SceneManagement>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public static void RestartScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex); // Mevcut sahneyi yeniden yükle
    }

    public void LoadScene(string sceneName)
    {
        // Oyuncu verilerini GameManager'a kaydet
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        HealthManager healthManager = player.GetComponent<HealthManager>();

        GameManager.Instance.SavePlayerData();
        
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        StartCoroutine(InitializePlayerPosition());
        SceneManager.LoadScene(nextSceneIndex);
        StartCoroutine(InitializeScene(nextSceneIndex));
    }
    
    private IEnumerator InitializeScene(int sceneIndex)
    {
        yield return new WaitForSeconds(0.1f); // Sahnenin yüklenmesini bekle

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = GameManager.Instance.PlayerPosition;
    } 
    
    private IEnumerator InitializePlayerPosition()
    {
        yield return new WaitForSeconds(0.1f); // Sahnenin yüklenmesini bekle
        GameManager.Instance.LoadLastCheckpoint("LastCheckpoint"); // Son checkpoint'ten devam et
    }
}
