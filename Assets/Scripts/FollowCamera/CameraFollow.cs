using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
   [SerializeField] private Transform player;
   [SerializeField] private Vector3 offset = new Vector3(0, 2, -10);
   [SerializeField] private float smoothSpeed = 0.125f;

   private void Awake()
   {
      DontDestroyOnLoad(gameObject);
   }

   private void Start()
   {
      player = GameObject.FindGameObjectWithTag("Player").transform;
   }

   private void LateUpdate()
   {
      Vector3 desiredPosition = player.position + offset;
      Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
      transform.position = smoothedPosition;
   }
}
