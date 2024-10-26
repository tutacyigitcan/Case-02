using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnPoint : MonoBehaviour
{
    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) // Başlangıç sahnesi için
        {
            //GameManager.Instance.SetRespawnPoint(transform);
        }
    }
}