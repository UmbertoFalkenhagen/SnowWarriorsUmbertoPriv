using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreateSnowballsState : IAIState
{
    
    public IAIState DoState(UmbertoAINew npc)
    {
        npc.debugoutput = "Creating snowballs";
        npc.CreateSmallSnowball();
        
        if ((npc.enemiesInSight.Count != 0 || npc.enemiesInHearingRange.Count != 0) && npc.playerSmallSnowballCount > 0)
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
        }
    }
}
