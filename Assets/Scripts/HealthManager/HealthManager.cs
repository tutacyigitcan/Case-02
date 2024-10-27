using System.Collections;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int maxLives = 5;
    [SerializeField] private Transform respawnPoint; 
    
    private int health;
    public int Health
    {
        get { return health; }
        private set
        {
            if (health != value) // Sadece sağlık değiştiğinde çalışır.
            {
                health = value;
                UIManager.Instance.UpdateHearts(health); // UI'yi güncelle
                Debug.Log($"HealthManager - Can güncellendi: {health}");
            }
        }
    }

    private Animator anim;
    private bool isDead = false;
    private bool canTakeDamage = true;

    private void Start()
    {
        anim = GetComponent<Animator>();
        Health = GameManager.Instance.Health;

        Debug.Log($"HealthManager - Başlangıç canı: {Health}");
        
        Transform point = GameManager.Instance.GetRespawnPointForCurrentScene();
        if (point != null)
        {
            SetRespawnPoint(point);
        }
        else
        {
            Debug.LogWarning("Respawn point bulunamadı!");
        }
    }

    public void SetRespawnPoint(Transform newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
        Debug.Log($"Respawn noktası atandı: {newRespawnPoint.name}");
    }

    public void TakeDamage(int amount)
    {
        if (!canTakeDamage || isDead) return;

        Health -= amount;
        GameManager.Instance.UpdateLives(Health);
        
        if (Health <= 0)
        {
            isDead = true;
            Debug.Log("Oyuncu öldü. Yeniden doğuluyor...");
            StartCoroutine(GameOver());
        }
        else
        {
            StartCoroutine(HandleRespawn());
        }
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator HandleRespawn()
    {
        canTakeDamage = false;
        anim.SetTrigger("Die");  
        yield return new WaitForSeconds(2f);
        GameManager.Instance.RespawnPlayer();
        isDead = false; 
        canTakeDamage = true;
        Debug.Log("Oyuncu yeniden doğdu.");
        anim.SetTrigger("Respawn");
    }
    
    private IEnumerator GameOver()
    {
        anim.SetTrigger("Die");
        yield return new WaitForSeconds(2f);
        Debug.Log("Oyun bitti. Ana menüye dönüyor.");
        GameManager.Instance.LoadScene("MainMenu");
        // Ana menüye yönlendirme veya oyunu yeniden başlatma işlemi burada yapılabilir.
    }
    
    private void Respawn()
    {
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
            anim.SetTrigger("Respawn"); 
            isDead = false;
            Health = maxLives; 
            canTakeDamage = true; 
            Debug.Log("Oyuncu yeniden doğdu.");
        }
        else
        {
            Debug.LogError("Respawn point is not assigned!");
        }
    }
}
