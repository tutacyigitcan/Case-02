using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private string checkpointName;
    private bool isActivated = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            Debug.Log($"Checkpoint'e ulaşıldı: {checkpointName}");
            GameManager.Instance.SetCurrentCheckpoint(transform); // Checkpoint'i ata
            UIManager.Instance.ShowCheckpointNotification();
            isActivated = true; // Artık bu checkpoint'e tekrar kaydetme yapılmaz
        }
        else
        {
            Debug.Log($"Checkpoint zaten aktif: {checkpointName}");
        }
    }
}