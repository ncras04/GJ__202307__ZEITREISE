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
    [Space]
    public States currentState = States.Idle;
    public enum States { Idle, Attack, Scout }

    Animator animator;

    public GameObject bullet;

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
                break;
            case States.Attack:
                UseAttack();
                animator.SetBool("isAttacking", true);
                break;
            case States.Scout:
                break;
            default:
                break;
        }
    }

    private void UseAttack()
    {
        if (canShoot)
        {
            GameObject tempBullet = Instantiate(bullet, transform.position, Quaternion.identity);
            tempBullet.GetComponent<Rigidbody>().AddForce(tempBullet.transform.forward * 50);
            Destroy(tempBullet,10);
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        canShoot = false;
        yield return new WaitForSeconds(Cooldown);
        canShoot = true;
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
}
