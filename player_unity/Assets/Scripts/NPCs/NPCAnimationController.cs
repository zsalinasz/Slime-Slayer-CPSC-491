using UnityEngine;
using System;

public class NPCAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void UpdateMovement(Vector3 velocity)
    {
        if (velocity.x == 0 && velocity.y == 0)
        {
            animator.SetBool("IsIdle", true);
            animator.SetBool("IsMovingRight", false);
            animator.SetBool("IsMovingLeft", false);
            animator.SetBool("IsMovingUp", false);
            animator.SetBool("IsMovingDown", false);
        }

        if (Math.Abs(velocity.x) > Math.Abs(velocity.y))
        {
            if (velocity.x > 0)
            {
                animator.SetBool("IsMovingRight", true);
                animator.SetBool("IsMovingLeft", false);
                animator.SetBool("IsMovingUp", false);
                animator.SetBool("IsMovingDown", false);
            }
            else
            { 
                animator.SetBool("IsMovingRight", false);
                animator.SetBool("IsMovingLeft", true);
                animator.SetBool("IsMovingUp", false);
                animator.SetBool("IsMovingDown", false);
            }
        }
        else
        {
            if (velocity.y > 0)
            {
                animator.SetBool("IsMovingUp", true);
                animator.SetBool("IsMovingRight", false);
                animator.SetBool("IsMovingLeft", false);
                animator.SetBool("IsMovingDown", false);
            }
            else
            {
                animator.SetBool("IsMovingDown", true);
                animator.SetBool("IsMovingRight", false);
                animator.SetBool("IsMovingLeft", false);
                animator.SetBool("IsMovingUp", false);
            }
        }
    }
    
}
