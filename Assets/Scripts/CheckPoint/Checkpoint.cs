using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private string checkpointName;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Checkpoint'e ulaşıldı: {checkpointName}");
            GameManager.Instance.SetCurrentCheckpoint(transform); // Checkpoint'i ata
            GameManager.Instance.CheckpointPassed();
            
            UIManager.Instance.ShowCheckpointNotification();
            GetComponent<BoxCollider2D>().enabled = false;
            GameManager.Instance.AddActiveCheckpoint(transform);
        }
        else
        {
            Debug.Log($"Checkpoint zaten aktif: {checkpointName}");
        }
    }
}