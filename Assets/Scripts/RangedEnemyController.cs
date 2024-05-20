using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyController : MonoBehaviour
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
    public GameObject Fireball;
    public GameObject rangedAttackPos;

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
            transform.LookAt(nav.destination);
        }
        else if (currentState == State.EnemySpotted)
        {
            if (Vector3.Distance(target.position, transform.position) < 10)
            {
                if (Vector3.Distance(waypoints[currentWaypoint].position, transform.position) < 1)
                {
                    System.Random rnd = new System.Random();
                    int index = currentWaypoint;
                    while (index == currentWaypoint)
                    {
                        index = rnd.Next(0, waypoints.Length);
                    }
                    currentWaypoint = index;              
                }
                nav.destination = waypoints[currentWaypoint].position;
            }
            else if (Vector3.Distance(target.position, transform.position) > 10)
            {
                nav.destination = transform.position;
            }
            transform.LookAt(target.position);

            if (charged)
            {
                charged = false;
                attacking = true;
                anim.SetBool("attacking", attacking);

                StartCoroutine(attackPlayer(target.gameObject));

                StartCoroutine(ResetAttack());
            }
            else
            {
                attacking = false;
            }
        }

        if (!dead)
        {
            anim.SetFloat("speed", speed);
            anim.SetBool("attacking", attacking);
            speed = nav.desiredVelocity.magnitude;
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
            Instantiate(Fireball, rangedAttackPos.transform.position, rangedAttackPos.transform.rotation);
        }

        IEnumerator ResetAttack()
        {
            yield return new WaitForSeconds(3.0f);
            charged = true;
        }
    }
    public void changeHealth(int change)
    {
        health += change;
        if (health <= 0)
        {
            GetComponent<AudioSource>().Play();
            dead = true;
            GetComponent<BoxCollider>().enabled = false;

            anim.SetBool("dead", health <= 0 ? true : false);
            StartCoroutine(wait()); // to stay on floor dead YEP
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

    IEnumerator wait()
    {
        yield return new WaitForSeconds(1);
        anim.SetBool("dead", false);
    }

}
