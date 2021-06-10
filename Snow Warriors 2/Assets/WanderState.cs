using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;
using Random = UnityEngine.Random;

public class WanderState : IAIState
{

    public IAIState DoState(UmbertoAINew npc)
    {
        npc.createsnowballs = false;
        if (npc.enemiesInSight.Count != 0 && npc.enemiesInHearingRange.Count != 0)
        {
            npc.debugoutput = "Retreat to hiding spot";
            FindHidingSpot(npc);
        } else if (npc.snowballsInSight.Count != 0)
        {
            npc.debugoutput = "Dodging from snowballs";
            npc.Dodge(DetermineDodgeDirection(npc.snowballsInSight[0], npc));
        }
        else
        {
            npc.debugoutput = "Wandering around";
            Wander(npc);
        }

        if ((npc.enemiesInSight.Count != 0 || npc.enemiesInHearingRange.Count != 0) && npc.playerSmallSnowballCount > 0)
        {
            return npc.attackState;
        } else if (npc.collectablesInRange.Count > 0 && (npc.playerHealth < 80 || npc.playerStamina < 6))
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

    private void FindHidingSpot(UmbertoAINew npc)
    {
        //npc.debugoutput = "Moving to hiding spot";
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

    private void Wander(UmbertoAINew npc)
    {
        int randomindex;
        randomindex = Random.Range(0, npc.obstaclesInRange.Count - 1);
        
        Vector3 targetposition = Vector3.zero;
        targetposition = npc.obstaclesInRange[randomindex].transform.position;
        /*switch (Random.Range(1, 5))
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
        }*/

        //npc.debugoutput = "Wandering around";
        npc.Move(targetposition);
    }
    
    private Vector3 DetermineDodgeDirection(GameObject dodgeObject, UmbertoAINew npc)
    {
        npc.transform.LookAt(dodgeObject.transform);
        Vector3 flightDirection = npc.transform.forward - dodgeObject.transform.forward;
        if (flightDirection.x == 0 && flightDirection.z == 0)
        {
            if (Random.Range(0,2) == 0)
            {
                return npc.transform.position + Vector3.left;
            }
            else
            {
                return npc.transform.position + Vector3.right;
            }
        }
        else
        {
            return Vector3.zero;
        }
    }
}
