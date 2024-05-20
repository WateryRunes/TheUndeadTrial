using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyController : MonoBehaviour
{
    public enum State
    {
        Patrol,
        EnemySpotted,
        Dead
    };

    private State currentState;
    private int health;
    private float speed;
    private bool attacking;
    private bool charged;
    private NavMeshAgent nav;
    private Animator anim;
    public Transform target;
    public Transform[] waypoints;
    public bool dead;
    public int currentWaypoint;
    public AudioSource attackSound;
    public AudioSource deathSound;

    // Start is called before the first frame update
    void Start()
    {
        currentWaypoint = 0;
        dead = false;
        attacking = false;
        charged = true;
        speed = 0;
        health = 60;

        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        nav.updateRotation = false;
        nav.destination = waypoints[currentWaypoint].position;
        transform.LookAt(nav.destination);

        currentState = State.Patrol;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == State.Patrol)
        {
            if (targetInLOS()) // is in LoS
            {
                currentState = State.EnemySpotted;
            }
            else
            {
                if (Vector3.Distance(waypoints[currentWaypoint].position, transform.position) < 1)
                {
                    currentWaypoint++;
                    if(currentWaypoint >= waypoints.Length)
                    {
                        currentWaypoint = 0;
                    }
                    nav.destination = waypoints[currentWaypoint].position;
                }
            }
        }
        else if (currentState == State.EnemySpotted)
        {
            nav.destination = target.position;
            if(nav.desiredVelocity != Vector3.zero)
            {
                transform.forward = nav.desiredVelocity;
            }
            if (Vector3.Distance(target.position, transform.position) < 2.75f && charged)
            {
                charged = false;
                attacking = true;
                anim.SetBool("attacking", attacking);
                nav.destination = transform.position;
                transform.LookAt(target.position);

                RaycastHit attackingRayData;
                Debug.DrawRay(transform.position, transform.forward * 5f, new Color(0, 0, 255));
                Physics.Raycast(transform.position, transform.forward*5f, out attackingRayData, 5f);
                if (attackingRayData.collider != null && attackingRayData.collider.gameObject.tag == "Player")
                {
                    // sound to hit something
                    StartCoroutine(attackPlayer(attackingRayData.collider.gameObject));
                }
                else if (attackingRayData.collider == null)
                {
                    // sound to hit nothing
                }
                StartCoroutine(ResetAttack());
            }
            else
            {
                nav.destination = target.position;
                attacking = false;
            }
        }

        if (!dead)
        {
            anim.SetFloat("speed", speed);
            anim.SetBool("attacking", attacking);
            speed = nav.desiredVelocity.magnitude;
            transform.LookAt(nav.destination);
        }
        else
        {
            currentState = State.Dead;
            nav.destination = transform.position;
            speed = 0;
        }

        IEnumerator attackPlayer(GameObject player)
        {
            yield return new WaitForSeconds(0.6f);
            attackSound.Play();
            yield return new WaitForSeconds(0.4f);
            if (Vector3.Distance(target.position, transform.position) < 3.0f)
            {
                player.GetComponent<PlayerController>().updateHealth(-20);
            }
        }

        IEnumerator ResetAttack()
        {
            yield return new WaitForSeconds(3.0f);
            charged = true;
        }
    }

    public bool targetInLOS()
    {
        Vector3 direction = target.position - transform.position;
        float angle = Vector3.Angle(transform.forward, direction);

        if(Math.Abs(angle) < 45 && Vector3.Distance(target.position, transform.position) < 15)
        {
            return true;
        }
        return false;
    }

    public void changeHealth(int change)
    {
        health += change;
        if(health <= 0)
        {
            deathSound.Play();
            dead = true;
            GetComponent<BoxCollider>().enabled = false;
            
            anim.SetBool("dead", health <= 0 ? true : false);
            StartCoroutine(wait()); // to stay on floor dead YEP
        }
    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(1);
        anim.SetBool("dead", false);
    }

}
