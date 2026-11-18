using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void UpdateMovement(Vector3 velocity)
    {
        if (animator == null)
            return;

        // Reset all directional flags, including when stationary.
        bool isMoving = velocity.sqrMagnitude > 0.0001f;
        bool horizontal = Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y);
        animator.SetBool("IsMovingRight", isMoving && horizontal && velocity.x > 0f);
        animator.SetBool("IsMovingLeft", isMoving && horizontal && velocity.x < 0f);
        animator.SetBool("IsMovingUp", isMoving && !horizontal && velocity.y > 0f);
        animator.SetBool("IsMovingDown", isMoving && !horizontal && velocity.y < 0f);
    }
}
