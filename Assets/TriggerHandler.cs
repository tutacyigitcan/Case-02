using System;
using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    private void OnTriggerEnter2D(Collider2D other)
    {
        SceneManagement.Instance.LoadScene(sceneToLoad);
    }
}
