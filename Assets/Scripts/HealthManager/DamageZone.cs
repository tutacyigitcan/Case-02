using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;  // Verilecek hasar
    [SerializeField] private float damageCooldown = 1f;  // Hasar bekleme süresi

    private bool canDealDamage = true;  // Hasar verilip verilemeyeceğini kontrol eder

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canDealDamage)
        {
            HealthManager healthManager = other.GetComponent<HealthManager>();

            if (healthManager != null)
            {
                Debug.Log($"OnTriggerEnter2D tetiklendi: {other.name}");
                healthManager.TakeDamage(damageAmount);  // Hasar uygula
                Debug.Log($"Damage verildi! Kalan Can: {healthManager.Health}");

                // Collider’i geçici olarak devre dışı bırak
                canDealDamage = false;
                other.enabled = false;  // Player collider'ını devre dışı bırak
                Invoke(nameof(EnableCollider), damageCooldown);  // Collider’i tekrar aktif et
            }
        }
    }

    private void EnableCollider()
    {
        canDealDamage = true;  // Cooldown tamamlandıktan sonra tekrar hasar verebilir
        Debug.Log("Damage verme tekrar aktif!");

        // Player collider'ını tekrar aktif et
        Collider2D playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            playerCollider.enabled = true;
        }
    }
}