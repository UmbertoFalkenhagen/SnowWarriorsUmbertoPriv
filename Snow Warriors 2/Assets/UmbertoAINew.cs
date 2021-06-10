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
    
    //State properties
    public AttackState attackState = new AttackState();
    public CreateSnowballsState createSnowballsState = new CreateSnowballsState();
    public GetCollectableState getCollectableState = new GetCollectableState();
    public WanderState wanderState = new WanderState();

    private void OnEnable()
    {
        currentState = wanderState;
    }

    private void Update()
    {
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
        // if (createsnowballs)
        // {
        //     StartCoroutine(CreateSmallSnowball());
        // }
    }
    
    
}
