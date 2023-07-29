using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Specs")]
    public bool canShoot = true;
    [Range(0f, 2f)]
    public float Cooldown = 1;
    public float RotationSpeed;
    public float MovementSpeed;
    bool isTurning = false;
    [Space]
    public Transform bulletSpawnPoint;
    [Space]
    public States currentState = States.Move;
    public enum States { Idle, Attack, Move }

    [Space]
    [Header("Waypoints")]
    public GameObject[] Waypoints;
    int waypointIndex;

    Animator animator;

    public GameObject bullet;
    public GameObject victim;

    public bool isSwapped = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        ExecuteStates();
    }

    private void ExecuteStates()
    {
        switch (currentState)
        {
            case States.Idle:
                animator.SetBool("isAttacking", false);
                Idle();
                break;
            case States.Attack:
                UseAttack();
                animator.SetBool("isAttacking", true);
                break;
            case States.Move:
                Move();
                break;
            default:
                break;
        }
    }

    private void Idle()
    {
        animator.SetFloat("mSpeed", 0);
        if (!isTurning)
        {
            StartCoroutine(Wait(.5f));
        }
    }

    private void Move()
    {
        Quaternion lookRotation;
        Vector3 direction;

        direction = (Waypoints[waypointIndex].transform.position - transform.position).normalized;
        lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);

        var step = MovementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, Waypoints[waypointIndex].transform.position, step);

        if (Vector3.Distance(transform.position, Waypoints[waypointIndex].transform.position) < 1.5f)
        {
            waypointIndex += 1;
            if (waypointIndex > Waypoints.Length - 1)
            {
                waypointIndex = 0;
            }

            currentState = States.Idle;
        }
    }

    private void UseAttack()
    {
        if (!isSwapped)
        {
            victim = GameObject.FindGameObjectWithTag("Player1");
        }
        else
        {
            victim = GameObject.FindGameObjectWithTag("Player2");
        }

        if (Distance(victim, gameObject) < 15)
        {
            if (canShoot)
            {
                GameObject projectile = Instantiate(bullet, bulletSpawnPoint.position, Quaternion.identity);
                projectile.transform.LookAt(victim.transform.position);
                //Vector3 direction = bulletSpawnPoint.position - victim.transform.position;
                //projectile.GetComponent<Rigidbody>().velocity = -direction * 5;
                StartCoroutine(Reload());
            }
        }
    }

    private IEnumerator Reload()
    {
        canShoot = false;
        yield return new WaitForSeconds(Cooldown);
        canShoot = true;
    }

    private IEnumerator Wait(float time)
    {
        isTurning = true;
        yield return new WaitForSeconds(time);
        animator.SetFloat("mSpeed", 1);
        currentState = States.Move;
        isTurning = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player1" || other.tag == "Player2")
        {
            currentState = States.Attack;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player1" || other.tag == "Player2")
        {
            currentState = States.Idle;
        }
    }

    float Distance(GameObject a, GameObject b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }
}
