using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class NicoAI : Player
{
    private GameObject closestEnemy;
    private Vector3 closestInteractable;
    private Vector3 _position;
    private bool canExecute = true;
    private AIStates state;

    private void Start()
    {
        state = AIStates.Aggressive;
        StartCoroutine(enemyScan());
        StartCoroutine(ExecuteBehavior());
    }

    #region LOGIC

    private void Update() => ChooseBehavior();

    private void ChooseBehavior()
    {
        if (snowballsInSight.Count == 0)
        {
            if (playerHealth < 50)
            {
                state = AIStates.Fleeing;
            } else if (playerHealth >= 50 && playerHealth < 70)
            {
                state = AIStates.Passive;
            } else if (playerHealth >= 70)
            {
                state = AIStates.Aggressive;
            }
        }
        else
        {
            StartCoroutine(DodgeSnowball());
        }
    }
    
    private IEnumerator ExecuteBehavior()
    {
        while (true)
        {
            yield return StartCoroutine(state.ToString());
        }
    }

    private IEnumerator Fleeing()
    {
        while (state == AIStates.Fleeing)
        {
            //Debug.Log("executing fleeing state");
            debugoutput = "Running for dear life";
            if (collectablesInRange.Count == 0)
            {
                _position = RandomPosition();
                Move(_position);
            }
            else
            {
                _position = FindHealth();
                Move(_position);
            }
            yield return new WaitUntil(() => navMeshAgent.remainingDistance < 1f);
        }
        //Debug.Log("exiting fleeing state");
        yield return null;
    }
    
    private IEnumerator Passive()
    {
        while (state == AIStates.Passive)
        {
            //Debug.Log("executing passive state");
            if (collectablesInRange.Count == 0)
            {
                if (closestEnemy == null)
                {
                    yield return null;
                }
                else
                {
                    if (canExecute) StartCoroutine(Fire());
                    yield return new WaitForSeconds(1f);
                    canExecute = true;
                }
            }
            else
            {
                _position = FindHealth();
                Move(_position);
                yield return new WaitUntil(() => navMeshAgent.remainingDistance < 1f);
            }
            yield return null;
        }
        //Debug.Log("exiting passive state");
        yield return null;
    }
    
    private IEnumerator Aggressive()
    {
        while (state == AIStates.Aggressive)
        {
            //Debug.Log("executing aggressive state");

            if (closestEnemy == null)
            {
                if (playerSmallSnowballCount < 4)
                {
                    if (canExecute) StartCoroutine(RefillAmmo());
                    yield return new WaitUntil(() => playerSmallSnowballCount >= 6);
                    canExecute = true;
                }
                else
                {
                    _position = FindStamina();
                    Move(_position);
                    yield return new WaitUntil(() => navMeshAgent.remainingDistance < 1f);
                }
            }
            else
            {
                if (playerStamina >= 1)
                {
                    if (canExecute) StartCoroutine(Fire());
                    yield return new WaitForSeconds(1f);
                    canExecute = true;
                }
                else
                {
                    _position = FindStamina();
                    Move(_position);
                    yield return new WaitUntil(() => navMeshAgent.remainingDistance < 1f);
                }
            }
            yield return null;
        }
        //Debug.Log("exiting aggressive state");
        yield return null;
    }


    #endregion
    
    #region ACTIONS

    //scans for enemies every second
    private IEnumerator enemyScan()
    {
        //first check for closest enemy in hearing range, if nothing is found check the sight range
        closestEnemy = FindClosestHeard();
        //if (closestEnemy != null) Debug.Log("heard enemy " + closestEnemy);
        if (closestEnemy == null) closestEnemy = FindClosestSight();
        //if (closestEnemy != null) Debug.Log("heard enemy " + closestEnemy);
        
        //loop every second
        yield return new WaitForSeconds(1f);
        StartCoroutine(enemyScan());
    }
    
    private IEnumerator RefillAmmo()
    {
        canExecute = false;
        debugoutput = "Refilling ammo...";
        for (int i = playerSmallSnowballCount; i < 7; i++)
        {
            CreateSmallSnowball();
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator Fire()
    {
        canExecute = false;
        if (!holdingLargeSnowball && playerSmallSnowballCount <= 0)
        {
            debugoutput = "Refilling ammo...";
            CreateSmallSnowball();
            yield return new WaitForSeconds(1f);
            
        } else if (closestEnemy != null)
        {
            //stop navmesh movement by setting the destination as the player's current position
            Move(transform.position);
            transform.LookAt(closestEnemy.transform);
            debugoutput = "Throwing snowball!";
            ThrowSnowball(closestEnemy.transform.position);
            yield return null;
        }
        else
        {
            yield return null;
        }
    }
    
    private GameObject FindClosestHeard()
    {
        //find closest enemy in hearing range
        //Debug.Log("checking hearing range!");
        if (enemiesInHearingRange.Count != 0)
        {
            float closestEnemyDistance = 10;        //sample starter value
            GameObject closestEnemy = null;

            //loop through all the enemies in hearing range and find the closest one
            foreach (var enemy in enemiesInHearingRange)
            {
                //find distance between player and current enemy
                var currentEnemyDistance = Vector3.Distance(enemy.transform.position, transform.position);
                
                //if distance to current enemy is smaller, set it to closest enemy
                if (currentEnemyDistance < closestEnemyDistance)
                {
                    closestEnemyDistance = currentEnemyDistance;
                    closestEnemy = enemy;
                }
            }

            return closestEnemy;
        }
        else
        {
            return null;
        }
    }

    private GameObject FindClosestSight()
    {
        //Debug.Log("checking sight range!");
        if (enemiesInSight.Count != 0)
        {
            float closestEnemyDistance = 10;
            GameObject closestEnemy = null;

            //loop through all the enemies in sight range and find the closest one
            foreach (var enemy in enemiesInSight)
            {
                //find distance between player and current enemy
                var currentEnemyDistance = Vector3.Distance(enemy.transform.position, transform.position);
                
                //if distance from current enemy is smaller, set it to closest enemy
                if (currentEnemyDistance < closestEnemyDistance)
                {
                    closestEnemyDistance = currentEnemyDistance;
                    closestEnemy = enemy;
                }
            }

            return closestEnemy;
        }
        else
        {
            return null;
        }
    }

    private Vector3 FindHealth()
    {
        Vector3 origin = new Vector3(0, 0, 0);
        float closestDistance = 1000;
        Vector3 closestInteractable = origin;

        foreach (var collectable in collectablesInRange)
        {
            if (collectable != null)
            {
                if (collectable.name == "HealthPickup(Clone)")
                {
                    var currentDistance = Vector3.Distance(collectable.transform.position, transform.position);

                    if (currentDistance < closestDistance)
                    {
                        closestDistance = currentDistance;
                        closestInteractable = collectable.transform.position;
                    }
                }
            }
        }

        if (closestInteractable != origin)
        {
            debugoutput = "Going for HP";
            return closestInteractable;
        }
        else
        {
            //Debug.Log("bravo six, going random");
            return RandomPosition();
        }
    }

    private Vector3 FindStamina()
    {
        Vector3 origin = new Vector3(0, 0, 0);
        float closestDistance = 1000;
        Vector3 closestInteractable = origin;
        
        foreach (var collectable in collectablesInRange)
        {
            if (collectable != null)
            {
                if (collectable.name == "StaminaPickup(Clone)")
                {
                    var currentDistance = Vector3.Distance(collectable.transform.position, transform.position);

                    if (currentDistance < closestDistance)
                    {
                        closestDistance = currentDistance;
                        closestInteractable = collectable.transform.position;
                    }
                }
            }
            
        }

        if (closestInteractable != origin)
        {
            debugoutput = "Going for Stamina";
            return closestInteractable;
        }
        else
        {
            //Debug.Log("bravo six, going random");
            return RandomPosition();
        }
    }
    
    private Vector3 RandomPosition()
    {
        debugoutput = "Looking around...";
        float radius = 10f;
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        //randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) {
            finalPosition = hit.position;            
        }
        return finalPosition;
    }

    private IEnumerator DodgeSnowball()
    {
        _position = RandomPosition();
        Dodge(_position);
        yield return new WaitUntil(() => navMeshAgent.remainingDistance < 1f);
    }

    #endregion

    
    private enum AIStates
    {
        Fleeing = 0,
        Passive = 1,
        Aggressive = 2
    }
    
}

/* Created by Niccolo Chiodo for Game Programming Elective SS 21 (Game Lab) */
