using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnviScanner : MonoBehaviour
{
    public List<GameObject> obstaclesInRange;
    public List<GameObject> collectablesInRange;
    public List<GameObject> enemiesInSight;
    public List<GameObject> enemiesInHearingRange;
    public List<GameObject> snowballsInSight;

    //Event delegates and actions
    
    
    //Events
    public event Action<GameObject> OnEnemyEntersFOV; //Is fired whenever a new enemy is added to the enemiesInSight List
    public event Action<GameObject> OnEnemyExitsFOV; //Is fired whenever a enemy object is removed from the enemiesInSight List

    public event Action<GameObject> OnHeardEnemy; //Is fired whenever a enemy object is added to the enemiesInHearingRange List

    public event Action<GameObject> OnSnowballEntersFOV;

    public event Action<GameObject, string> OnCollectableEntersRange;

    public event Action<GameObject, string> OnCollectableLeavesRange; 
    

    public float viewDistance;
    public float viewAngle;
    public Transform visionOffset;
    public Light spotLight;
    public List<GameObject> obstaclesglobal;
    public List<GameObject> collectablesglobal;
    public List<GameObject> enemiesglobal;
    public List<GameObject> snowballsglobal;
    public Player playerscript;
    

    private void Start()
    {
        viewDistance = spotLight.range;
        viewAngle = spotLight.spotAngle;
        obstaclesglobal = GameObject.FindGameObjectsWithTag("Obstacles").ToList();
        collectablesglobal = GameObject.FindGameObjectsWithTag("Collectable").ToList();
        enemiesglobal = GameObject.FindGameObjectsWithTag("Player").ToList();
        playerscript = GetComponentInParent<Player>();
        // OnEnemyEntersFOV += Testing_OnEnemyEntersFOV;
        // OnEnemyExitsFOV += Testing_OnEnemyExitsFOV;

    }

    // private void Testing_OnEnemyExitsFOV(GameObject obj)
    // {
    //     Debug.Log(obj.name + " has left FOV of " + this.GetComponentInParent<Player>().gameObject.name);
    // }
    //
    // private void Testing_OnEnemyEntersFOV(GameObject obj)
    // {
    //     Debug.Log(obj.name + " has entered FOV of " + this.GetComponentInParent<Player>().gameObject.name);
    // }


    private void Update()
    {
        //Debug.Log(enemiesInHearingRange);
        //UpdateAllGlobalLists();
        
        if (enemiesInSight.Count != 0)
        {
            spotLight.intensity = 100;
        }
        else
        {
            spotLight.intensity = 0;
        } 
        //as already happening with the spotlight being turned on when theres a player in sight,
        //there could also be a indicator popping up above the character if they hear an enemy...
    }

    private void LateUpdate()
    {
        UpdateAllGlobalLists();
    }

    private void UpdateAllGlobalLists()
    {
        UpdateObjectsInRange();
        UpdateEnemiesInSight();
        UpdateEnemiesInHearingRange();
        UpdateSnowballsInSight();
        playerscript.UpdateEnvironmentLists(obstaclesInRange, collectablesInRange, enemiesInSight, enemiesInHearingRange, snowballsInSight);
    }

    public void UpdateObjectsInRange()
    {
        obstaclesglobal = GameObject.FindGameObjectsWithTag("Obstacles").ToList();
        collectablesglobal = GameObject.FindGameObjectsWithTag("Collectable").ToList();
        foreach (var obstacle in obstaclesglobal)
        {
            if (Vector3.Distance(transform.position, obstacle.transform.position) < viewDistance)
            {
                if (!obstaclesInRange.Contains(obstacle))
                {
                    obstaclesInRange.Add(obstacle);
                }
            }
            else
            {
                if (obstaclesInRange.Contains(obstacle))
                {
                    obstaclesInRange.Remove(obstacle);
                }
            }
        }

        foreach (var collectable in collectablesglobal)
        {
            if (Vector3.Distance(transform.position, collectable.transform.position) < viewDistance)
            {
                if (!collectablesInRange.Contains(collectable))
                {
                    if (collectable.GetComponent<HealthPickUp>() != null)
                    {
                        OnCollectableEntersRange?.Invoke(collectable, "health");
                    } else if (collectable.GetComponent<StaminaPickUp>() != null)
                    {
                        OnCollectableEntersRange?.Invoke(collectable, "stamina");
                    }
                    collectablesInRange.Add(collectable);
                }
            }
            else
            {
                if (collectable.GetComponent<HealthPickUp>() != null)
                {
                    OnCollectableLeavesRange?.Invoke(collectable, "health");
                } else if (collectable.GetComponent<StaminaPickUp>() != null)
                {
                    OnCollectableLeavesRange?.Invoke(collectable, "stamina");
                }
                if (collectablesInRange.Contains(collectable))
                {
                    collectablesInRange.Remove(collectable);
                }
            }
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void UpdateEnemiesInSight()
    {
        enemiesglobal = GameObject.FindGameObjectsWithTag("Player").ToList();
        foreach (var enemy in enemiesglobal)
        {
            //Debug.Log("Distance to " + enemy.name + " is " + Vector3.Distance(visionOffset.position, enemy.transform.position));
            if (Vector3.Distance(visionOffset.position, enemy.transform.position) < viewDistance)
            {
                //Debug.Log(enemy.name + " is in viewing range...");
                Vector3 dirToPlayer = (enemy.transform.position - visionOffset.position).normalized;
                float angleBetweenPlayerAndEnemy = Vector3.Angle(visionOffset.forward, dirToPlayer);
                if (angleBetweenPlayerAndEnemy < viewAngle / 2)
                {
                    //Debug.Log(enemy.name + " is in viewing angle...");
                    RaycastHit linecastHit;
                    Physics.Linecast(visionOffset.position, enemy.transform.position, out linecastHit);
                    if (linecastHit.collider != null)
                    {
                        if (linecastHit.collider.gameObject.CompareTag("Player"))
                        {
                            if (enemy.gameObject != this.gameObject && !enemiesInSight.Contains(enemy))
                            {
                                OnEnemyEntersFOV?.Invoke(enemy);
                                //Debug.Log(enemy.name + " was spotted!");
                                enemiesInSight.Add(enemy);
                            }
                        }
                        else
                        {
                            if (enemiesInSight.Contains(enemy))
                            {
                                enemiesInSight.Remove(enemy);
                                OnEnemyExitsFOV?.Invoke(enemy);
                            }
                            //Debug.Log(enemy.name + " is covered by " + linecastHit.collider.gameObject.name);
                        }
                    }
                    
                }
                else
                {
                    if (enemiesInSight.Contains(enemy))
                    {
                        enemiesInSight.Remove(enemy);
                        OnEnemyExitsFOV?.Invoke(enemy);
                    }
                }
            }
            else
            {
                if (enemiesInSight.Contains(enemy))
                {
                    enemiesInSight.Remove(enemy);
                    OnEnemyExitsFOV?.Invoke(enemy);
                }
            }
        }
    }
    
    public void UpdateSnowballsInSight()
    {
        snowballsglobal = GameObject.FindGameObjectsWithTag("Snowball").ToList();
        foreach (var snowball in snowballsglobal)
        {
            //Debug.Log("Distance to " + enemy.name + " is " + Vector3.Distance(visionOffset.position, enemy.transform.position));
            if (Vector3.Distance(visionOffset.position, snowball.transform.position) < viewDistance)
            {
                //Debug.Log(enemy.name + " is in viewing range...");
                Vector3 dirToPlayer = (snowball.transform.position - visionOffset.position).normalized;
                float angleBetweenPlayerAndSnowball = Vector3.Angle(visionOffset.forward, dirToPlayer);
                if (angleBetweenPlayerAndSnowball < viewAngle / 2)
                {
                    //Debug.Log(enemy.name + " is in viewing angle...");
                    RaycastHit linecastHit;
                    Physics.Linecast(visionOffset.position, snowball.transform.position, out linecastHit);
                    if (linecastHit.collider != null)
                    {
                        if (linecastHit.collider.gameObject.CompareTag("Snowball"))
                        {
                            if (snowball.gameObject != this.gameObject && !snowballsInSight.Contains(snowball) && snowball.GetComponent<SnowBall>().throwingPlayer != this.gameObject)
                            {
                                //Debug.Log(enemy.name + " was spotted!");
                                snowballsInSight.Add(snowball);
                                OnSnowballEntersFOV?.Invoke(snowball);
                            }
                        }
                        else
                        {
                            if (snowballsInSight.Contains(snowball))
                            {
                                snowballsInSight.Remove(snowball);
                            }
                            //Debug.Log(enemy.name + " is covered by " + linecastHit.collider.gameObject.name);
                        }
                    }
                    
                }
                else
                {
                    if (snowballsInSight.Contains(snowball))
                    {
                        snowballsInSight.Remove(snowball);
                    }
                }
            }
            else
            {
                if (snowballsInSight.Contains(snowball))
                {
                    snowballsInSight.Remove(snowball);
                }
            }
        }
    }

    public void UpdateEnemiesInHearingRange()
    {
        
        foreach (var enemy in enemiesglobal)
        {
            if (/*enemy.GetComponent<Player>().currentspeed*/ 5 >= Vector3.Distance(playerscript.gameObject.transform.position, enemy.transform.position))
            {
                if (!enemiesInHearingRange.Contains(enemy) && enemy.gameObject != playerscript.gameObject)
                {
                    enemiesInHearingRange.Add(enemy);
                    OnHeardEnemy?.Invoke(enemy);
                }
            }
            else
            {
                if (enemiesInHearingRange.Contains(enemy))
                {
                    enemiesInHearingRange.Remove(enemy);
                }
            }
            
        }
    }

    public void RemoveObjectFromList(GameObject gameObject)
    {
        if (collectablesglobal.Contains(gameObject))
        {
            if (gameObject.GetComponent<HealthPickUp>() != null)
            {
                OnCollectableLeavesRange?.Invoke(gameObject, "health");
            } else if (gameObject.GetComponent<StaminaPickUp>() != null)
            {
                OnCollectableLeavesRange?.Invoke(gameObject, "stamina");
            }
            collectablesglobal.Remove(gameObject);
            
        }
        if (enemiesglobal.Contains(gameObject))
        {
            enemiesglobal.Remove(gameObject);
        }
        if (obstaclesglobal.Contains(gameObject))
        {
            obstaclesglobal.Remove(gameObject);
        }
        if (snowballsglobal.Contains(gameObject))
        {
            snowballsglobal.Remove(gameObject);
        }
        
        UpdateAllGlobalLists();
    }
}

/* Created by Umberto Falkenhagen for Game Programming Elective SS 21 (Game Lab) */