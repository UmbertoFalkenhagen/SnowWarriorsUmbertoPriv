using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationManager : MonoBehaviour
{
    
    public Player playerscript;
    public Animator animator;

    private void Awake()
    {
        playerscript = GetComponent<Player>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("movementSpeed", playerscript.currentspeed);
        AdjustAnimationState(playerscript.animationstate);
        if (playerscript.gameObject.GetComponent<Rigidbody>() != null)
        {
            //Debug.Log(playerscript.gameObject.name + " has a velocity of " + playerscript.gameObject.GetComponent<Rigidbody>().velocity);
            if (playerscript.gameObject.GetComponent<Rigidbody>().velocity.x == 0 && playerscript.gameObject.GetComponent<Rigidbody>().velocity.z == 0 && playerscript.animationstate == 1)
            {
                Debug.Log(playerscript.gameObject.name + " isn't moving but is in running state, it will be changed to idle.");
                playerscript.ChangeMoveState(0,0);
                playerscript.animationstate = 0;
            }
        }
    }

    public void AdjustAnimationState(int animationstate)
    {
        switch (animationstate)
        {
            case 0: //Idle state
                animator.SetFloat("movementSpeed", playerscript.currentspeed);
                //animator.SetBool("isRunning", false);
                animator.SetBool("isCreating", false);
                animator.SetBool("isThrowing", false);
                animator.SetBool("isFalling", false);
                //Debug.Log("Switched to idle state");
                break;
            case 1: //Running state
                animator.SetFloat("movementSpeed", playerscript.currentspeed);
                //animator.SetBool("isRunning", true);
                animator.SetBool("isCreating", false);
                animator.SetBool("isThrowing", false);
                animator.SetBool("isFalling", false);
                //Debug.Log("Switched to running state");
                break;
            case 2: //Creating snowball state
                animator.SetFloat("movementSpeed", playerscript.currentspeed);
                //animator.SetBool("isRunning", false);
                animator.SetBool("isCreating", true);
                animator.SetBool("isThrowing", false);
                animator.SetBool("isFalling", false);
                //Debug.Log("Switched to creating snowball state");
                break;
            case 3: //Throwing snowball state
                animator.SetFloat("movementSpeed", playerscript.currentspeed);
                //animator.SetBool("isRunning", false);
                animator.SetBool("isCreating", false);
                animator.SetBool("isThrowing", true);
                animator.SetBool("isFalling", false);
                //Debug.Log("Switched to throwing snowball state");
                break;
            case 4: //Falling state
                animator.SetFloat("movementSpeed", playerscript.currentspeed);
                //animator.SetBool("isRunning", false);
                animator.SetBool("isCreating", false);
                animator.SetBool("isThrowing", false);
                animator.SetBool("isFalling", true);
                //Debug.Log("Switched to falling state");
                break;
        }
    }
}

/* Created by Umberto Falkenhagen for Game Programming Elective SS 21 (Game Lab) */