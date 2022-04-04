using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public GameObject targetObject;
    public AIState aiState = AIState.Idle;
    // If target seen within pursueDistance, AI will start to pursue the target.
    public float pursueDistance = 150;
    // If target is within attackDistance, AI will attempt to make an attack.
    public float attackDistance = 100;
    // View angle that AI can see in front of itself
    public float visionAngle = 360;
    // Speed that AI should move when patrolling
    public float patrolSpeed = 100;
    // Speed that AI should move when pursuing player
    public float pursueSpeed = 200;
    // Path of waypoints to patrol
    public GameObject[] patrolPath;
    // Time that AI should idle at each waypoint once reached
    public float[] patrolPathIdleTimes;
    private int health = 2;

    private NavMeshAgent agent;
    private Animator anim;
    private Rigidbody2D rb;
    // Create a layer mask that ignores enemies and the player, may need to change if player hides behind an enemy
    private LayerMask visMask;

    // Current idle timer
    private float idleTime = 1;
    // The current patrol waypoint, starts at -1 so when setNextWaypoint is first called it will go to first waypoint
    private int currWaypoint = -1;

    private float bouncesTime = 1;


    public enum AIState
    {
        Idle,
        Patrol,
        Pursue,
        Attack,
        Bounce
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        // Creates a layermask that ignores the player and enemies.
        visMask = ~LayerMask.GetMask("Player", "Enemy");
        this.transform.position = new Vector2(this.transform.position.x, this.transform.position.y);

        agent.speed = patrolSpeed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.acceleration = 9999f;
        agent.angularSpeed = 9999f;
    }

    // Update is called once per frame
    void Update()
    {

        if (targetObject == null)
        {
            Debug.Log("GG");
            StopAllCoroutines();
            UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit(0);
        }

        float targetDist = Mathf.Abs(Vector2.SqrMagnitude(new Vector2(targetObject.transform.position.x, targetObject.transform.position.y) - rb.position));

        switch (aiState)
        {
            case AIState.Idle:
                // Update current idling timer
                idleTime -= Time.deltaTime;

                // Transition to Pursue state if target is visible
                if (CanSee(pursueDistance, visionAngle))
                {
                    agent.speed = pursueSpeed;
                    aiState = AIState.Pursue;
                } // Transition to moving to next waypoint once idle time elapsed
                else if (idleTime <= 0)
                {
                    SetNextWaypoint();
                    agent.speed = patrolSpeed;
                    aiState = AIState.Patrol;
                }
                break;
            case AIState.Patrol:

                // Transition to Pursue state if target is visible within check distance
                if (CanSee(pursueDistance, visionAngle))
                {
                    agent.speed = pursueSpeed;
                    aiState = AIState.Pursue;
                } // Transition to idle state once current waypoint reached
                else if (agent.remainingDistance < 1 && !agent.pathPending)
                {
                    aiState = AIState.Idle;
                }
                break;
            case AIState.Pursue:
                agent.SetDestination(targetObject.transform.position);

                // Transition to attacking state
                if (targetDist <= attackDistance)
                {
                    aiState = AIState.Attack;
                }
                break;
            case AIState.Attack:
                agent.SetDestination(targetObject.transform.position);
                // Transition back to pursue state if target moves out of attack range
                if (targetDist > attackDistance)
                {
                    aiState = AIState.Pursue;
                }
                break;            
            case AIState.Bounce:
                if (bouncesTime > 0)
                {
                    Debug.Log("time");
                    bouncesTime -= Time.deltaTime;
                }
                else {
                    bouncesTime = 1;
                    agent.enabled = true;
                    aiState = AIState.Pursue;
                    rb.velocity = Vector2.zero;
                }
                break;
            default:

                break;
        }

        this.transform.position = new Vector2(this.transform.position.x, this.transform.position.y);

    }

    public void DeductHealth(int dmg)
    {
        health -= dmg;
        checkCurrentHealth();
    }    
    
    private void checkCurrentHealth()
    {
        if (health <= 0) {
            targetObject.GetComponent<Player>().goldIncrement();
            StopAllCoroutines();
            for (int i = 0; i < patrolPath.Length; i++) {
                Destroy(patrolPath[i]);
            }
            Destroy(this.gameObject);
        }
    }




    // Checks if AI can see RigidBody target object within the provided distance visDistance and angle visAngle
    private bool CanSee(float visDistance, float visAngle)
    {
        Vector2 targetDir = new Vector2(targetObject.transform.position.x, targetObject.transform.position.y) - rb.position;

        float targetDist = Mathf.Abs(Vector3.Magnitude(targetDir));
        float targetAngle = Vector2.Angle(transform.forward, Vector3.Normalize(targetDir));



        if (targetDist <= visDistance && targetAngle <= visAngle)
        {

            // Use a raycast to make secondary check whether target is hidden behind something
            RaycastHit hit;
            bool isHit = Physics.Raycast(rb.position, targetDir, out hit, visDistance, visMask);

            // Return true if nothing blocking vision to target, or if raycast hit target itself
            return !isHit || hit.collider.gameObject == targetObject;

        }
        else
        {
            return false;
        }
    }

    private void SetNextWaypoint()
    {
        if (patrolPath.Length != patrolPathIdleTimes.Length)
        {
            throw new MissingReferenceException("Number of patrol waypoint objects does not match number of patrol idle times.");
        }

        currWaypoint++;
        if (currWaypoint >= patrolPath.Length)
        {
            currWaypoint = 0;
        }
        agent.SetDestination(patrolPath[currWaypoint].transform.position);
        idleTime = patrolPathIdleTimes[currWaypoint];
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            aiState = AIState.Bounce;
            Debug.Log("passed");
            rb.AddForce(-agent.velocity *2, ForceMode2D.Impulse);
            collision.gameObject.GetComponent<PlayerMovement>().bounce(agent.velocity);
            agent.enabled = false;
        }
    }
}
