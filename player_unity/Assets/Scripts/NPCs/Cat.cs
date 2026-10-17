using UnityEngine;

public class Cat : MonoBehaviour
{
    Vector3 unit_velocity;
    float currentSpeed;
    [SerializeField] public float patrolLength;
    float currentPatrolProgress;

    [SerializeField] private NPCAnimationController animationController;
    private int waitTicks = 0;

    public void Awake()
    {

    }

    public void Start()
    {
        unit_velocity = Vector3.down;
        currentSpeed = 0.0f;
        if (patrolLength <= 0)
        {
            patrolLength = 2.0f;
        }
        currentPatrolProgress = 0.0f;
    }

    public void Update()
    {
        if (currentPatrolProgress < patrolLength && waitTicks == 0)
        {
            currentSpeed = 2.0f;
            Vector3 final = transform.position + (unit_velocity * currentSpeed * Time.deltaTime);
            float distance = Vector3.Distance(final, transform.position);
            currentPatrolProgress += distance;
            transform.position = final;
            animationController.UpdateMovement(unit_velocity);
        }
        else if (waitTicks != 0)
        {
            waitTicks = (waitTicks+1)%100;
            animationController.UpdateMovement(Vector3.zero);
        }
        else
        {
            unit_velocity = Quaternion.Euler(0, 0, 90) * unit_velocity;
            animationController.UpdateMovement(unit_velocity);
            currentPatrolProgress = 0;
            waitTicks = (waitTicks+1)%100;
        }
    }
    
}
