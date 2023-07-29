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
    public float ViewRange = 10;
    public float RotationSpeed;
    public float MovementSpeed;
    bool isTurning = false;

    [Space]
    public Transform bulletSpawnPoint;
    [Space]
    public States currentState = States.Move;
    public enum States { Idle, Attack, Move }
    public bool EnemyOnTop = true;

    [Space]
    [Header("Waypoints")]
    public GameObject[] Waypoints;
    int waypointIndex;

    Animator animator;

    // The Bullt to Spawn
    public GameObject bullet;
    [SerializeField]
    private GameObject victim;
    public PlayerManager playerManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        return;
        CheckStatus();
        ExecuteStates();
    }

    private void CheckStatus()
    {
        if (EnemyOnTop)
        {
            if (ReturnDistance(playerManager.GetTopPlayer(true).gameObject, gameObject) < ViewRange)
            {
                currentState = States.Attack;
            }
            else
            {
                currentState = States.Move;
            }
        }
        else
        {
            if (ReturnDistance(playerManager.GetTopPlayer(false).gameObject, gameObject) < ViewRange)
            {
                currentState = States.Attack;
            }
            else
            {
                currentState = States.Move;
            }
        }
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
        victim = EnemyOnTop ? playerManager.GetTopPlayer(true).gameObject : playerManager.GetTopPlayer(false).gameObject;

        if (ReturnDistance(victim, gameObject) < ViewRange)
        {
            if (canShoot)
            {
                GameObject projectile = Instantiate(bullet, bulletSpawnPoint.position, Quaternion.identity);
                projectile.transform.LookAt(victim.transform.position);
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

    float ReturnDistance(GameObject a, GameObject b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    #region Obsolte

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
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, ViewRange);
    }
}
