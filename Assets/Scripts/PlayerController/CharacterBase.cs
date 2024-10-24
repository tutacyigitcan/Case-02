using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
   protected Rigidbody2D rb;
   protected bool isFacingRight = true;
   
   protected virtual void Start()
   {
      rb = GetComponent<Rigidbody2D>();
   }
   
   public abstract void Move(float moveInput);
   public abstract void Jump(float jumpForce);
   
   
}
