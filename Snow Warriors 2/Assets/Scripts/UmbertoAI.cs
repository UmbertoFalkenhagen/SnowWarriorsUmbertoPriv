using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class CombatAction
{
    public int weight;
    public string actiontype;
    
    public bool recentlyexecuted;

    public CombatAction(int _weight, string _actiontype)
    {
        weight = _weight;
        actiontype = _actiontype;
        recentlyexecuted = false;
    }
}


public class UmbertoAI : Player
{
    private GameManager _gameManager;
    private EnviScanner _enviScanner;
    public List<CombatAction> combatActions;

    private bool executeNextAction = true;
    private bool executingemergencyAction = false;
    private string currentactiontype;
    public int executedActionsSinceLastReset = 0;
    public List<GameObject> healthpickups;
    public List<GameObject> staminapickups;
    public float passedTimeSinceLastActionCompletion;

    private event Action OnExecutedCombatActionEvent;


    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _enviScanner = GetComponentInChildren<EnviScanner>();
        combatActions = new List<CombatAction>(6);
        //Debug.Log(combatActions.Count);
        //Debug.Log(combatActions.Capacity);
        combatActions.Add(new CombatAction(0, "dodge"));
        combatActions.Add(new CombatAction(4, "attack"));
        combatActions.Add(new CombatAction(2, "hide"));
        combatActions.Add(new CombatAction(3, "health"));
        combatActions.Add(new CombatAction(1, "stamina"));
        combatActions.Add(new CombatAction(5, "snowball"));
        //ResetCombatActionList();
        healthpickups = new List<GameObject>();
        staminapickups = new List<GameObject>();
        _enviScanner.OnEnemyEntersFOV += OnEnemyEntersFOV;
        _enviScanner.OnEnemyExitsFOV += OnEnemyExitsFOV;
        _enviScanner.OnHeardEnemy += OnHeardEnemy;
        _enviScanner.OnSnowballEntersFOV += OnSnowballEntersFOV;
        _enviScanner.OnCollectableEntersRange += OnCollectableEntersRange;
        _enviScanner.OnCollectableLeavesRange += OnCollectableLeavesRange;
        OnExecutedCombatActionEvent += OnExecutedCombatAction;
    }


    private void Update()
    {
        SortCombatActionsList();
        // if (combatActions[combatActions.Count-1].weight > 30 && !executingemergencyAction)
        // {
        //     //executingemergencyAction = true;
        //     //StopAllCoroutines();
        //     executeNextAction = true;
        // }
        
        if (executeNextAction)
        {
            StartCoroutine(ExecuteHighestPriorityAction());
        }
        if (executedActionsSinceLastReset > 15) //reset actionlist after every 15th action
        {
            ResetCombatActionList();
        }

        passedTimeSinceLastActionCompletion += Time.deltaTime;
    }



    private IEnumerator ExecuteHighestPriorityAction()
    {
        executeNextAction = false;
        passedTimeSinceLastActionCompletion = 0;
        switch (combatActions[combatActions.Count-1].actiontype)
        {
            case "dodge":
                StartCoroutine(WaitUntilActionIsExecuted(DodgeFromSnowballs, "dodge"));
                break;
            case "attack":
                StartCoroutine(WaitUntilActionIsExecuted(AttackEnemies, "attack"));
                break;
            case "hide":
                StartCoroutine(FindHidingSpot());
                break;
            case "health":
                StartCoroutine(FindCollectableOfType("health"));
                break;
            case "stamina":
                StartCoroutine(FindCollectableOfType("stamina"));
                break;
            case "snowball":
                StartCoroutine(CreateAmmo());
                break;
            
        }

        currentactiontype = combatActions[combatActions.Count-1].actiontype;
        //Debug.Log("executing action " + currentactiontype);
        yield return new WaitUntil(() =>
            combatActions[FindCombatActionIndex(currentactiontype)].recentlyexecuted ||
            passedTimeSinceLastActionCompletion > 3);
        combatActions[FindCombatActionIndex(currentactiontype)].recentlyexecuted = false;
        combatActions[FindCombatActionIndex(currentactiontype)].weight = 3;
        OnExecutedCombatActionEvent?.Invoke();
        
        executedActionsSinceLastReset++;
        executeNextAction = true;
        if (executingemergencyAction)
        {
            executingemergencyAction = false;
        }
    }

    #region CombatActionManagement

    

    private void ResetCombatActionList() //resets actionlist to avoid priorities stacking up
    {
        if (combatActions.Count != 0)
        {
            //Debug.Log(combatActions.Count);
            combatActions.Clear();
        }
        combatActions = new List<CombatAction>(6);
        //Debug.Log(combatActions.Count);
        //Debug.Log(combatActions.Capacity);
        combatActions.Add(new CombatAction(0, "dodge"));
        combatActions.Add(new CombatAction(4, "attack"));
        combatActions.Add(new CombatAction(2, "hide"));
        combatActions.Add(new CombatAction(3, "health"));
        combatActions.Add(new CombatAction(1, "stamina"));
        combatActions.Add(new CombatAction(5, "snowball"));

        executedActionsSinceLastReset = 0;
    }
    private int FindCombatActionIndex(string _actiontype)
    {
        for (int i = 0; i < combatActions.Count; i++)
        {
            if (combatActions[i].actiontype == _actiontype)
            {
                return i;
            }
        }

        return -1;
    }
    
    private void SortCombatActionsList()
    {
        combatActions.Sort(delegate(CombatAction action1, CombatAction action2)
        {
            return action1.weight.CompareTo(action2.weight);
            
        });
    }
    
    private IEnumerator WaitUntilActionIsExecuted(Func<bool> executingFunction, string actiontype)
    {
        yield return new WaitUntil(executingFunction);
        combatActions[FindCombatActionIndex(actiontype)].recentlyexecuted = true;
    }

    #endregion


    #region CombatActions

    private IEnumerator RunToRandomObstacleOrPositionInRange()
    {
        int randomindex;
        Vector3 targetposition = Vector3.zero;
        switch (Random.Range(0, 5))
        {
            case 0:
                if (obstaclesInRange != null)
                {
                    randomindex = Random.Range(0, obstaclesInRange.Count);
                    targetposition = obstaclesInRange[randomindex].transform.position;
                }
                break;
            case 1:
                targetposition = transform.position;
                targetposition.x += 5;
                break;
            case 2:
                targetposition = transform.position;
                targetposition.x -= 5;
                break;
            case 3:
                targetposition = transform.position;
                targetposition.z -= 5;
                break;
            case 4:
                targetposition = transform.position;
                targetposition.z += 5;
                break;
        }
        Move(targetposition);
        yield return new WaitForSeconds(1);
        
    }
    private bool DodgeFromSnowballs()
    {
        debugoutput = "You can't hit me!";
        if (snowballsInSight.Count == 0)
        {
            StartCoroutine(RunToRandomObstacleOrPositionInRange());
            return true;
        }
        Dodge(DetermineDodgeDirection(snowballsInSight[0]));
        return true;
    }

    private bool AttackEnemies()
    {
        
        if (enemiesInSight.Count == 0 && enemiesInHearingRange.Count == 0)
        {
            StartCoroutine(RunToRandomObstacleOrPositionInRange());
        }
        if (playerSmallSnowballCount <= 0 && !holdingLargeSnowball && enemiesInSight.Count == 0 && enemiesInHearingRange.Count == 0)
        {
            return true;
        }
        else
        {
            if (enemiesInSight.Count > 0 )
            {
                ThrowSnowball(enemiesInSight[0].transform.position);
                return true;
                
            }
            
            if (enemiesInHearingRange.Count > 0)
            {
                ThrowSnowball(enemiesInHearingRange[0].transform.position);
                return true;
                
            }

            return true;
        }

        
    }
    
    private IEnumerator FindHidingSpot()
    {
        debugoutput = "Moving to hiding spot";
        Vector3 hidingspot = Vector3.zero;
        foreach (var enemy in enemiesInHearingRange)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < 15)
            {
                hidingspot += (enemy.transform.position - (enemy.transform.forward * 15));
            }
        }
        Move(hidingspot);
        
        yield return new WaitForSeconds(2f);
        combatActions[FindCombatActionIndex("hide")].recentlyexecuted = true;
    }

    private IEnumerator FindCollectableOfType(string collectabletype)
    {
        debugoutput = "Grabbing that collectable!";
        Vector3 targetposition = Vector3.zero;
        if (collectabletype == "health")
        {
            if (healthpickups.Count == 0)
            {
                StartCoroutine(RunToRandomObstacleOrPositionInRange());
                debugoutput = "no health pickup in range, PANIC!";
                combatActions[FindCombatActionIndex(collectabletype)].recentlyexecuted = true;
                yield break;
            }

            try
            {
                targetposition = healthpickups[0].transform.position;
            }
            catch (Exception e)
            {
                StartCoroutine(RunToRandomObstacleOrPositionInRange());
                debugoutput = "no health pickup in range, PANIC!";
            }
            

        } else if (collectabletype == "stamina")
        {
            if (staminapickups.Count == 0)
            {
                StartCoroutine(RunToRandomObstacleOrPositionInRange());
                debugoutput = "no stamina pickup in range, PANIC!";
                combatActions[FindCombatActionIndex(collectabletype)].recentlyexecuted = true;
                yield break;
            }

            try
            {
                targetposition = staminapickups[0].transform.position;
            }
            catch (Exception e)
            {
                StartCoroutine(RunToRandomObstacleOrPositionInRange());
                debugoutput = "no stamina pickup in range, PANIC!";
            }

            


        }
        
        Move(targetposition);
        
        yield return new WaitForSeconds(2);
        combatActions[FindCombatActionIndex(collectabletype)].recentlyexecuted = true;
        
    }

    private IEnumerator CreateAmmo()
    {
        debugoutput = "Get ready for some snowballs!";
        if (playerSmallSnowballCount < 6)
        {
            CreateSmallSnowball();
        }
        else
        {
            StartCoroutine(RunToRandomObstacleOrPositionInRange());
        }
        // else //I just dont like the large snowballs
        // {
        //     if (!holdingLargeSnowball)
        //     {
        //         StartCoroutine(CreateLargeSnowball());
        //         yield return new WaitForSeconds(2);
        //     }
        // }
        yield return new WaitForSeconds(1);

            //yield return null;
        combatActions[FindCombatActionIndex("snowball")].recentlyexecuted = true;
    } 

    #endregion
    

    #region EventListeners
    private void OnEnemyEntersFOV(GameObject obj)
    {
        if (playerSmallSnowballCount > 0)
        {
            combatActions[FindCombatActionIndex("attack")].weight += 15;
        }
        else
        {
            combatActions[FindCombatActionIndex("hide")].weight += 15;
            combatActions[FindCombatActionIndex("snowball")].weight += 10-playerSmallSnowballCount;
        }

        if (holdingLargeSnowball)
        {
            combatActions[FindCombatActionIndex("attack")].weight += 5;
        }
         
    }
    
    private void OnEnemyExitsFOV(GameObject obj)
    {
        if (playerSmallSnowballCount == 0)
        {
            foreach (var enemy in enemiesInHearingRange)
            {
                float distancetoenemy = Vector3.Distance(this.transform.position, enemy.transform.position);
                if ( distancetoenemy < 10)
                {
                    combatActions[FindCombatActionIndex("hide")].weight += (10 - (int) distancetoenemy) * 2;
                }
                else
                {
                    combatActions[FindCombatActionIndex("hide")].weight += 2;
                }
            }
        }
    }

    private void OnExecutedCombatAction()
    {
        if (healthpickups.Count != 0)
        {
            combatActions[FindCombatActionIndex("health")].weight += (100 / 5 - playerHealth / 5);
        }
        
        if (staminapickups.Count != 0)
        {
            combatActions[FindCombatActionIndex("stamina")].weight += (10 - playerStamina);
        }

        if (playerSmallSnowballCount > 4)
        {
            combatActions[FindCombatActionIndex("snowball")].weight = 6 - playerSmallSnowballCount;
        }
        else
        {
            combatActions[FindCombatActionIndex("snowball")].weight += 8 - playerSmallSnowballCount;
        }
        
        foreach (var combatAction in combatActions)
        {
            if (combatAction.weight < 0)
            {
                combatAction.weight = 0;
            } else if (combatAction.weight > 50)
            {
                combatAction.weight = 50;
            }
        }
    }
    
    private void OnSnowballEntersFOV(GameObject obj)
    {
        combatActions[FindCombatActionIndex("dodge")].weight += 10;
    }

    private void OnHeardEnemy(GameObject obj)
    {
        combatActions[FindCombatActionIndex("snowball")].weight += 6 - playerSmallSnowballCount * 2;
        if (playerSmallSnowballCount == 0)
        {
            foreach (var enemy in enemiesInHearingRange)
            {
                float distancetoenemy = Vector3.Distance(this.transform.position, enemy.transform.position);
                if ( distancetoenemy < 10)
                {
                    combatActions[FindCombatActionIndex("hide")].weight += (10 - (int) distancetoenemy) * 2;
                }
                else
                {
                    combatActions[FindCombatActionIndex("hide")].weight += 2;
                }
            }
        }
        else
        {
            combatActions[FindCombatActionIndex("attack")].weight += playerSmallSnowballCount * 2;
            if (holdingLargeSnowball)
            {
                combatActions[FindCombatActionIndex("attack")].weight += 10;
            }
        }
    }
    
    private void OnCollectableLeavesRange(GameObject arg1, string arg2)
    {
        if (arg2 == "health")
        {
            if (healthpickups.Contains(arg1))
            {
                healthpickups.Remove(arg1);
            }
        } else if (arg2 == "stamina")
        {
            if (staminapickups.Contains(arg1))
            {
                staminapickups.Remove(arg1);
            }
        }
    }

    private void OnCollectableEntersRange(GameObject arg1, string arg2)
    {
        if (arg2 == "health")
        {
            if (!healthpickups.Contains(arg1))
            {
                healthpickups.Add(arg1);
            }
        } else if (arg2 == "stamina")
        {
            if (!staminapickups.Contains(arg1))
            {
                staminapickups.Add(arg1);
            }
        }
    }


    #endregion


    #region CalculationMethods
    
    private Vector3 DetermineDodgeDirection(GameObject dodgeObject)
    {
        transform.LookAt(dodgeObject.transform);
        Vector3 flightDirection = transform.forward - dodgeObject.transform.forward;
        if (flightDirection.x == 0 && flightDirection.z == 0)
        {
            if (Random.Range(0,2) == 0)
            {
                return transform.position + Vector3.left;
            }
            else
            {
                return transform.position + Vector3.right;
            }
        }
        else
        {
            return Vector3.zero;
        }
    }
    

    #endregion
    
    






}

/* Created by Umberto Falkenhagen for Game Programming Elective SS 21 (Game Lab) */