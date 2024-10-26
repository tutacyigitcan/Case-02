using System.Collections;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int maxLives = 5; // Maksimum can
    [SerializeField] private Transform respawnPoint; // Respawn noktası
    
    //public int Health { get; private set; } // Oyuncunun canı
    
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
        
        // Sahnedeki uygun respawn noktasını ayarla
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
            // UI'de can göstergesini güncelle
           // UIManager.Instance.UpdateHearts(Health);
            StartCoroutine(HandleRespawn());
        }
        
        /*
        if (Health > 0)  // Eğer can kaldıysa
        {
            StartCoroutine(HandleRespawn());  // Yeniden doğma işlemi başlat
        }
        else  // Eğer can 0'a ulaştıysa
        {
            isDead = true;
            Debug.Log("Oyuncu öldü. Tüm canlar bitti.");
            GameManager.Instance.SavePlayerData();  // Veriyi kaydet
            StartCoroutine(GameOver());  // Oyunu sonlandır veya ana menüye yönlendir
        }*/
        
        /*
       if (Health <= 0)
       {
           isDead = true;
           Debug.Log("Oyuncu öldü. Yeniden doğuluyor...");
           GameManager.Instance.SavePlayerData(); // Oyuncu verilerini kaydet
           StartCoroutine(HandleRespawn()); // Respawn işlemi başlat
       }


       else if (Health > 0 && Health <= maxLives)
       {
           // Eğer can hala varsa ama tam değilse sahneyi yeniden başlat
           SceneManagement.RestartScene();
       }
       else
       {
           StartCoroutine(DamageCooldown());
       }*/
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator HandleRespawn()
    {
        canTakeDamage = false;  // Yeniden doğana kadar hasar almayı engelle
        anim.SetTrigger("Die");  // Ölüm animasyonu tetikle

        yield return new WaitForSeconds(2f);  // 2 saniye bekle

        //GameManager.Instance.LoadLastCheckpoint("LastCheckpoint");  // Son checkpoint'e ışınla
         GameManager.Instance.RespawnPlayer();
       
      //  Health = GameManager.Instance.Health;
        isDead = false;  // Ölü durumunu sıfırla
        canTakeDamage = true;  // Yeniden hasar alabilir hale getir
        Debug.Log("Oyuncu yeniden doğdu.");
        anim.SetTrigger("Respawn");
        /*
        isDead = true;
        anim.SetTrigger("Die");
        yield return new WaitForSeconds(2f);
        GameManager.Instance.RespawnPlayer();
        Health = maxLives; // Can sıfırlanır
        isDead = false;
        canTakeDamage = true;
        anim.SetTrigger("Respawn");
        //Respawn(); // Oyuncuyu yeniden doğur
        */
    }
    
    private IEnumerator DamageCooldown()
    {
        canTakeDamage = false;  // Geçici olarak hasar almayı durdur
        yield return new WaitForSeconds(1f);  // 1 saniyelik cooldown süresi
        canTakeDamage = true;  // Tekrar hasar alabilir hale getir
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
            transform.position = respawnPoint.position; // Oyuncuyu respawn noktasına taşı
            anim.SetTrigger("Respawn"); // Respawn animasyonunu tetikle
            isDead = false; // Ölüm durumunu sıfırla
            Health = maxLives; // Canı maksimuma getir
            canTakeDamage = true; // Hasar alabilir hale getir
            Debug.Log("Oyuncu yeniden doğdu.");
        }
        else
        {
            Debug.LogError("Respawn point is not assigned!");
        }
    }
}
