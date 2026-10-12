using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    //Just a place holder. Will be needing to implement this.
    //

    private void Awake()
    {
    }
    private void Start()
    {

    }
    private void Update()
    {

    }
    private void rotation_methodA()
    {
        //Attempting to use matrix rotation.
	    Vector3 test_vec = (transform.position - player.position);
	    //Limiting rotation
	    float maxRotation = 5.0f;
	    float currentRotation = (float)engagementTicks % maxRotation;
    	test_vec = Quaternion.Euler(0, 0, currentRotation) * test_vec;
	    transform.position = test_vec;
    	Debug.Log($"currentRotation: {currentRotation}");
    }
    private void rotation_methodB()
    {
	    //Using a circle around the player as the movement curve, we use sin and cos functions to calculate the current (ideal) location of the enemy based on how long it's been in the Engage state (measured by engagementTicks)
    	float engagementDistance = 3.0f;
	    float dampener = 16.0f;
    	double x = player.position.x + engagementDistance * Math.Cos(engagementTicks / dampener);
	    double y = player.position.y + engagementDistance * Math.Sin(engagementTicks / dampener);

        //Once the ideal x,y coordinates are calculated, we set the enemy's current transform.position to those x and way (by first calculated the difference between the enemy's current pos to the correct pos and then subtracting it back in.)
        Vector3 diff = transform.position;
    	diff.x -= (float)x;
	    diff.y -= (float)y;

    	unit_velocity = -1*diff;
	    unit_velocity = unit_velocity.normalized;

    }

    private void JaggedApproach()
    {
    	Vector3 vec_to_player = player.position - transform.position;
	    unit_velocity = jaggedDirection * (Quaternion.Euler(0,0,alpha) * vec_to_player).normalized;
    	if (jaggedPosition >= jaggedOffset)
	    {
	    jaggedDirection *= -1;
	    jaggedPosition = 0;
    	}
	
    }



}
