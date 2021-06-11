using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GetCollectableState : IAIState
{
    public int actionstateweight { get; set; }
    
    public GetCollectableState(int weight)
    {
        actionstateweight = weight;
    }

    public IAIState DoState(UmbertoAINew npc)
    {
        npc.createsnowballs = false;
        npc.debugoutput = "Grabbing a collectable";
        if (npc.playerHealth < 60)
        {
            if (npc.healthpickups.Count != 0)
            {
                npc.Move(npc.healthpickups[0].transform.position);
            }
            else
            {
                if (npc.enemiesInHearingRange.Count != 0 || npc.enemiesInSight.Count != 0)
                {
                    FindHidingSpot(npc);
                }
                else
                {
                    return npc.wanderState;
                }
            }
        } else if (npc.playerStamina < 6)
        {
            if (npc.staminapickups.Count != 0)
            {
                npc.Move(npc.staminapickups[0].transform.position);
            }
            else
            {
                if (npc.enemiesInHearingRange.Count != 0 || npc.enemiesInSight.Count != 0)
                {
                    FindHidingSpot(npc);
                }
                else
                {
                    return npc.wanderState;
                }
            }
        }
        
        
        return npc.actionList[npc.actionList.Count - 1];
        /*if ((npc.enemiesInSight.Count != 0 || npc.enemiesInHearingRange.Count != 0) && npc.playerSmallSnowballCount > 0)
        {
            return npc.attackState;
        } else if (npc.collectablesInRange.Count > 0 && (npc.playerHealth < 60 || npc.playerStamina < 6))
        {
            return npc.getCollectableState;
        } else if (npc.playerSmallSnowballCount < 6 &&
                   (npc.enemiesInSight.Count == 0 || npc.enemiesInHearingRange.Count == 0))
        {
            return npc.createSnowballsState;
        }
        else
        {
            return npc.wanderState;
        }*/
    }
    
    private void FindHidingSpot(UmbertoAINew npc)
    {
        npc.debugoutput = "Moving to hiding spot";
        Vector3 hidingspot = Vector3.zero;
        foreach (var enemy in npc.enemiesInHearingRange)
        {
            if (Vector3.Distance(npc.transform.position, enemy.transform.position) < 15)
            {
                hidingspot += (enemy.transform.position - (enemy.transform.forward * 15));
            }
        }
        npc.Move(hidingspot);
    }
}
