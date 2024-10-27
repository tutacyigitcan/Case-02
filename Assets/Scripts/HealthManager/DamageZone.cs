using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float damageCooldown = 1f;

    private bool canDealDamage = true;  

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
                
                canDealDamage = false;
                other.enabled = false; 
                Invoke(nameof(EnableCollider), damageCooldown);  
            }
        }
    }

    private void EnableCollider()
    {
        canDealDamage = true; 
        Debug.Log("Damage verme tekrar aktif!");
        
        Collider2D playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            playerCollider.enabled = true;
        }
    }
}