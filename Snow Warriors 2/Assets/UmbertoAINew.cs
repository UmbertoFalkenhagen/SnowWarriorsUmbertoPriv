using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UmbertoAINew : Player
{
    private string currentStateName;
    private IAIState currentState;
    
    public List<GameObject> healthpickups;
    public List<GameObject> staminapickups;
    public bool createsnowballs = false;

    private EnviScanner _enviScanner;
    
    //State properties
    public List<IAIState> actionList = new List<IAIState>();
    public AttackState attackState = new AttackState(2);
    public CreateSnowballsState createSnowballsState = new CreateSnowballsState(3);
    public GetCollectableState getCollectableState = new GetCollectableState(1);
    public WanderState wanderState = new WanderState(0);

    private void Start()
    {
        actionList = new List<IAIState>();
        actionList.Add(attackState);
        actionList.Add(createSnowballsState);
        actionList.Add(getCollectableState);
        actionList.Add(wanderState);
        actionList.Sort(delegate(IAIState state1, IAIState state2)
        {
            return state1.actionstateweight.CompareTo(state2.actionstateweight);
            
        });
        Debug.Log(actionList[0].ToString());
        Debug.Log(actionList[1].ToString());
        Debug.Log(actionList[2].ToString());
        Debug.Log(actionList[3].ToString());
        
    }

    private void OnEnable()
    {
        currentState = wanderState;


        _enviScanner = GetComponentInChildren<EnviScanner>();
        _enviScanner.OnEnemyEntersFOV += OnEnemyEntersFOV;
        _enviScanner.OnEnemyExitsFOV += OnEnemyExitsFOV;
        _enviScanner.OnHeardEnemy += OnHeardEnemy;
        _enviScanner.OnSnowballEntersFOV += OnSnowballEntersFOV;
    }

    private void Update()
    {
        actionList.Sort(delegate(IAIState state1, IAIState state2)
        {
            return state1.actionstateweight.CompareTo(state2.actionstateweight);
            
        });
        healthpickups = new List<GameObject>();
        staminapickups = new List<GameObject>();
        foreach (var collectable in collectablesInRange)
        {
            if (collectable != null)
            {
                if (collectable.GetComponent<HealthPickUp>() != null)
                {
                    if (!healthpickups.Contains(collectable))
                    {
                        healthpickups.Add(collectable);
                    }
                } else if (collectable.GetComponent<StaminaPickUp>() != null)
                {
                    if (!staminapickups.Contains(collectable))
                    {
                        staminapickups.Add(collectable);
                    }
                }
            }
        }
        currentState = currentState.DoState(this);
        currentStateName = currentState.ToString();
        OnExecutedCombatAction();
        Debug.Log(FindActionIndex("AttackState"));

    }
    
    private int FindActionIndex(string _actiontype)
    {
        for (int i = 0; i < actionList.Count-1; i++)
        {
            if (actionList[i].ToString() == _actiontype)
            {
                Debug.Log("Found action at index " + i );
                return i;
            }
        }

        Debug.Log("Couldn't find the Index of given action.");
        return 0;
    }

    #region EventListeners

    private void OnEnemyEntersFOV(GameObject obj)
    {
        if (playerSmallSnowballCount > 0)
        {
            actionList[FindActionIndex("AttackState")].actionstateweight += 15;
        }
        else
        {
            actionList[FindActionIndex("WanderState")].actionstateweight += 15;
            actionList[FindActionIndex("CreateSnowballState")].actionstateweight += 10-playerSmallSnowballCount;
        }

        if (holdingLargeSnowball)
        {
            actionList[FindActionIndex("AttackState")].actionstateweight += 5;
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
                    actionList[FindActionIndex("WanderState")].actionstateweight  += (10 - (int) distancetoenemy) * 2;
                }
                else
                {
                    actionList[FindActionIndex("WanderState")].actionstateweight  += 2;
                }
            }
        }
    }

    public void OnExecutedCombatAction()
    {
        if (healthpickups.Count != 0)
        {
            actionList[FindActionIndex("GetCollectableState")].actionstateweight += (100 / 5 - playerHealth / 5);
        }
        
        if (staminapickups.Count != 0)
        {
            actionList[FindActionIndex("GetCollectableState")].actionstateweight += (10 - playerStamina);
        }

        if (playerSmallSnowballCount > 4)
        {
            actionList[FindActionIndex("CreateSnowballState")].actionstateweight = 6 - playerSmallSnowballCount;
        }
        else
        {
            actionList[FindActionIndex("CreateSnowballState")].actionstateweight += 8 - playerSmallSnowballCount;
        }
        
        foreach (var action in actionList)
        {
            if (action.actionstateweight < 0)
            {
                action.actionstateweight = 0;
            } else if (action.actionstateweight > 50)
            {
                action.actionstateweight = 50;
            }
        }
    }
    
    private void OnSnowballEntersFOV(GameObject obj)
    {
        actionList[FindActionIndex("WanderState")].actionstateweight += 10;
    }

    private void OnHeardEnemy(GameObject obj)
    {
        actionList[FindActionIndex("CreateSnowballState")].actionstateweight += 6 - playerSmallSnowballCount * 2;
        if (playerSmallSnowballCount == 0)
        {
            foreach (var enemy in enemiesInHearingRange)
            {
                float distancetoenemy = Vector3.Distance(this.transform.position, enemy.transform.position);
                if ( distancetoenemy < 10)
                {
                    actionList[FindActionIndex("WanderState")].actionstateweight  += (10 - (int) distancetoenemy) * 2;
                }
                else
                {
                    actionList[FindActionIndex("WanderState")].actionstateweight  += 2;
                }
            }
        }
        else
        {
            actionList[FindActionIndex("AttackState")].actionstateweight += playerSmallSnowballCount * 2;
            if (holdingLargeSnowball)
            {
                actionList[FindActionIndex("AttackState")].actionstateweight += 10;
            }
        }
    }

    #endregion
    
    
}
