using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private string checkpointName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Checkpoint'e ulaşıldı!");
            GameManager.Instance.SetCurrentCheckpoint(transform); // Checkpoint'i ata
            // GameManager.Instance.SaveCheckpoint(checkpointName); // Verileri kaydet

            UIManager.Instance.ShowCheckpointNotification();
        }
    }
}