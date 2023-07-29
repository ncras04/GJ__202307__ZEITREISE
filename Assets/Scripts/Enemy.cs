using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Specs")]
    public bool canShoot = true;
    [Range(0f, 2f)]
    [Tooltip("The Shoot Cd")]
    public float Cooldown = 1;
    [Tooltip("Range where the Palyer can be Spotted")]
    public float ViewRange = 10;
    [Header("Movement Variables")]
    public float MovementSpeed;
    public float RotationSpeed;

    // For Extra purposes only
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

    private GameObject victim; // Could be a Waypoint to move to or the Player
    public PlayerManager playerManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        playerManager = GameManager.Instance.PlayerManager;
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.GameState != GameState.Playing)
        {
            return;
        }
        
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
            //else
            //{
            //    if (currentState == States.Idle) return;
            //    currentState = States.Idle;
            //}
        }
        else
        {
            if (ReturnDistance(playerManager.GetTopPlayer(false).gameObject, gameObject) < ViewRange)
            {
                currentState = States.Attack;
            }
            //else
            //{
            //    if (currentState == States.Idle) return;
            //    currentState = States.Idle;
            //}
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
                animator.SetBool("isAttacking", false);
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
            StartCoroutine(Wait(1.5f));
        }
    }

    private void Move()
    {
        animator.SetFloat("mSpeed", 1);

        RotateToVictim(Waypoints[waypointIndex]);

        var step = MovementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, Waypoints[waypointIndex].transform.position, step);

        if (Vector3.Distance(transform.position, Waypoints[waypointIndex].transform.position) < 1.5f)
        {
            currentState = States.Idle;
            waypointIndex += 1;
            if (waypointIndex > Waypoints.Length - 1)
            {
                waypointIndex = 0;
            }
        }
    }

    private void UseAttack()
    {
        victim = EnemyOnTop ? playerManager.GetTopPlayer(true).gameObject : playerManager.GetTopPlayer(false).gameObject;

        RotateToVictim(victim);

        if (ReturnDistance(victim, gameObject) < ViewRange)
        {
            if (canShoot)
            {
                GameObject projectile = Instantiate(bullet, bulletSpawnPoint.position, Quaternion.identity);
                projectile.transform.LookAt(victim.transform.position + new Vector3(0, victim.transform.localScale.y / 2,0));
                StartCoroutine(Reload());
            }
        }
        else
        {
            currentState = States.Idle;
        }
    }

    void RotateToVictim(GameObject Victim)
    {
        Quaternion lookRotation;
        Vector3 direction;

        direction = (Victim.transform.position - transform.position).normalized;
        lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);
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
        Debug.Log("Turn");
        yield return new WaitForSeconds(time);

        currentState = States.Move;
        isTurning = false;
    }

    float ReturnDistance(GameObject a, GameObject b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, ViewRange);
    }

    #region Obsolte

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.tag == "Player1" || other.tag == "Player2")
    //    {
    //        currentState = States.Attack;
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.tag == "Player1" || other.tag == "Player2")
    //    {
    //        currentState = States.Idle;
    //    }
    //}
    #endregion 
}
