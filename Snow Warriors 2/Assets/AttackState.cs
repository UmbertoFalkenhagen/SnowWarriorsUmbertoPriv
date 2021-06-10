using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AttackState : IAIState
{
    public IAIState DoState(UmbertoAINew npc)
    {
        npc.createsnowballs = false;
        npc.debugoutput = "Attacking enemies";
        if (npc.enemiesInSight.Count != 0)
        {
            if (npc.enemiesInSight[0].GetComponent<Rigidbody>().velocity.x > 0 ||npc.enemiesInSight[0].GetComponent<Rigidbody>().velocity.z > 0)
            {
                npc.debugoutput = "attacking enemy in sight";
                npc.ThrowSnowball(npc.enemiesInSight[0].transform.position + npc.enemiesInSight[0].transform.forward);
            }
            else
            {
                npc.debugoutput = "attacking enemy in sight";
                npc.ThrowSnowball(npc.enemiesInSight[0].transform.position);
            }
        } else if (npc.enemiesInHearingRange.Count != 0)
        {
            if (npc.enemiesInHearingRange[0].GetComponent<Rigidbody>().velocity.x > 0 ||npc.enemiesInHearingRange[0].GetComponent<Rigidbody>().velocity.z > 0)
            {
                npc.debugoutput = "attacking enemy in hearing range";
                npc.ThrowSnowball(npc.enemiesInSight[0].transform.position + npc.enemiesInSight[0].transform.forward);
            }
            else
            {
                npc.debugoutput = "attacking enemy in hearing range";
                npc.ThrowSnowball(npc.enemiesInSight[0].transform.position);
            }
        }
        else
        {
            Wander(npc);
        }
        
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
    
    private void Wander(UmbertoAINew npc)
    {
        Vector3 targetposition = Vector3.zero;
        switch (Random.Range(1, 5))
        {
            case 1:
                targetposition = npc.transform.position;
                targetposition.x += 5;
                break;
            case 2:
                targetposition = npc.transform.position;
                targetposition.x -= 5;
                break;
            case 3:
                targetposition = npc.transform.position;
                targetposition.z -= 5;
                break;
            case 4:
                targetposition = npc.transform.position;
                targetposition.z += 5;
                break;
        }

        //npc.debugoutput = "Wandering around";
        npc.Move(targetposition);
    }
}
