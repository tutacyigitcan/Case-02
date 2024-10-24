using UnityEngine;
using UnityEngine.UIElements;

public class AnimatorManager : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void UpdateAnimations(float speed, float verticalVelocity, bool isGrounded, bool isSprinting)
    {
        animator.SetFloat("Speed",Mathf.Abs(speed));
        animator.SetBool("IsGrounded",isGrounded);
        animator.SetBool("IsSprinting", isSprinting);
        
        if (isGrounded && speed < 0.1f)
        {
            SetIdleAnimation();
        }
    }

    public void SetJumpAnimation()
    {
        animator.SetTrigger("Jump");
    }
    
    public void SetIdleAnimation()
    {
        animator.SetFloat("Speed", 0);
    }

    public void ResetJumpTrigger()
    {
        animator.ResetTrigger("Jumlp");
    }
}
