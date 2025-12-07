using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    float wanderOffset = 20.0f;
    float movementDelay = 60.0f;
    float movementTimer;

    bool isLeftOrRight = false;

    Rigidbody enemyRb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movementTimer = movementDelay;

        enemyRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {

        ++movementTimer;

        if (isLeftOrRight)
        {
            enemyRb.AddForce(-transform.right * wanderOffset, ForceMode.Force);
        }
        else
        {
            enemyRb.AddForce(transform.right * wanderOffset, ForceMode.Force);
        }



        if (movementTimer >= movementDelay)
        {

            enemyRb.linearVelocity = Vector3.zero;
            movementTimer = 0;
            isLeftOrRight = !isLeftOrRight;
        }
    }
}
