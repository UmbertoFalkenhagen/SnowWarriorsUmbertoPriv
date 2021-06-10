using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class JohnnyAI : Player
{
    [Header("AI Adjustments")]
    public int safetyMaxHealth;
    public int safetyMinHealth;

    private bool check = true;
    public enum AIStates
    {
        Agressive,
        Opportunistic,
        Critical
    }

    private AIStates currentState;

    private string actiontext = "null";
    
    private Vector3 randomDirection;

   
    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(UpdateRandomTarget());
    }


    // Update is called once per frame
    void Update()
    {

      
        //Update Debug
        debugoutput =  currentState.ToString() + ", " + actiontext;
        
        switch (currentState)
        {
            case AIStates.Agressive:
                
                //Checks Player Health
                StartCoroutine(StatusReport());
                
                //Attack
                if (enemiesInSight.Count > 0 && playerSmallSnowballCount >= 1 && playerStamina >= 1)
                {
                    actiontext = "Attacking(Saw Enemy)";
                    gameObject.transform.LookAt(enemiesInSight[0].transform, Vector3.up);
                    ThrowSnowball(enemiesInSight[0].transform.position);
                    Move(enemiesInSight[0].transform.position);
                    
                    enemiesInSight.Clear();
                    enemiesInHearingRange.Clear();
                    collectablesInRange.Clear();
                    
                }
                
                //Attack
                else if (enemiesInHearingRange.Count > 0 && playerSmallSnowballCount >= 1 && playerStamina >= 1)
                {
                    actiontext = "Attacking(Heard Enemy)";
                    gameObject.transform.LookAt(enemiesInHearingRange[0].transform, Vector3.up);
                    ThrowSnowball(enemiesInHearingRange[0].transform.position);
                    Move(enemiesInHearingRange[0].transform.position);
                    
                    enemiesInSight.Clear();
                    enemiesInHearingRange.Clear();
                    collectablesInRange.Clear();
                    
                }
                
                //Create Snowball ---
                else if (playerSmallSnowballCount < 3 && check)
                {
                    actiontext = "Making Snowballs";
                    StartCoroutine(SnowballCheck());
                }
                
                //If Collectable in Range, Move to Collect it
                else if (collectablesInRange.Count > 0)
                { 
                    actiontext = "Getting Item";
                    Move(collectablesInRange[0].transform.position);
                    collectablesInRange.Clear();
                }

                //Move to a Random Area
                else
                {
                    actiontext = "Looking Around";
                    Move(randomDirection);
                }


                break;
            
            
            case AIStates.Opportunistic:
                
                //Checks Player Health
                StartCoroutine(StatusReport());
                
                
                //Attack of Opportunity
                 if (enemiesInSight.Count == 1 && playerSmallSnowballCount >= 1 && playerStamina >= 1)
                {
                    actiontext = "Attacking(Saw Enemy)";
                    gameObject.transform.LookAt(enemiesInSight[0].transform, Vector3.up);
                    ThrowSnowball(enemiesInSight[0].transform.position);
                    
                    enemiesInHearingRange.Clear();
                    collectablesInRange.Clear();
                }
                
                //Attack of Opportunity
                else if (enemiesInHearingRange.Count == 1 && playerSmallSnowballCount >= 1 && playerStamina >= 1)
                {
                    actiontext = "Attacking(Heard Enemy)";
                    gameObject.transform.LookAt(enemiesInHearingRange[0].transform, Vector3.up);
                    ThrowSnowball(enemiesInHearingRange[0].transform.position);
                    
                   enemiesInHearingRange.Clear();
                   collectablesInRange.Clear();
                }
                
                //If Collectable in Range, Move to Collect it
                else if (collectablesInRange.Count > 0)
                { 
                    actiontext = "Getting Item";
                    Move(collectablesInRange[0].transform.position);
                    collectablesInRange.Clear();
                }
                
                //If More Than One Enemy in Range, Move to Flee
                else if (enemiesInHearingRange.Count > 1)
                {
                    actiontext = "Fleeing(Move)";
                    Vector3 enemydirection = gameObject.transform.position - enemiesInHearingRange[0].transform.position;
                    Vector3 fleedirection = transform.position + enemydirection;
                    Move(fleedirection);
                }
                
                //Move to a Random Area
                else
                {
                    actiontext = "Looking Around";
                    Move(randomDirection);
                }
                
                break;
            
            
            case AIStates.Critical:
                
                //Checks Player Health
                StartCoroutine(StatusReport());
                
                //If Health in Range, Dodge to Collect it
                if (collectablesInRange.Count > 0 && playerStamina > 2)
                {
                    foreach (var t in collectablesInRange)
                    {
                        if (t.name == "HealthPickup(Clone)")
                        {
                            actiontext = "Getting Health";
                            Dodge(collectablesInRange[0].transform.position);
                            collectablesInRange.Clear();
                        }
                    }
                }
                
                //If Health in Range, Move to Collect it
                if (collectablesInRange.Count > 0)
                {
                    foreach (var t in collectablesInRange)
                    {
                        if (t.name == "HealthPickup(Clone)")
                        {
                            actiontext = "Getting Health";
                            Move(collectablesInRange[0].transform.position);
                            collectablesInRange.Clear();
                        }
                    }
                }
                
                //If Enemy in Range, Move to Flee
                else if (enemiesInHearingRange.Count > 0)
                {
                    actiontext = "Fleeing(Move)";
                    Vector3 enemydirection = gameObject.transform.position - enemiesInHearingRange[0].transform.position;
                    Vector3 fleedirection = transform.position + enemydirection;
                    Move(fleedirection);
                }

                //Move to a Random Area
                else
                {
                    actiontext = "Looking Around";
                    Move(randomDirection);
                }
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }

    }
    
    
    private IEnumerator StatusReport()
    {
        
        if (playerHealth >= safetyMaxHealth)
        {
            currentState = AIStates.Agressive;
        }
        
        else if (playerHealth >= safetyMinHealth && playerHealth < safetyMaxHealth)
        {
            currentState = AIStates.Opportunistic;
        }
        
        else if (playerHealth < safetyMinHealth)
        {
            currentState = AIStates.Critical;
        }
        
        yield return null;
    }

    private IEnumerator SnowballCheck()
    {
        check = false;
        CreateSmallSnowball();
        yield return new WaitForSeconds(1f);
        check = true;
    }
    

    
    private IEnumerator UpdateRandomTarget()
    {
        for (;;)
        {
            var radius = 40f;
            randomDirection = Random.insideUnitCircle * radius;
            
            yield return new WaitForSeconds(2f);
        }
    }
}
