using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBallBig : SnowBall
{
    public void DropSnowball()
    {
        this.tag = "Obstacle";
        this.GetComponent<CapsuleCollider>().isTrigger = false;
        StartCoroutine(DestroySnowball());
    }

    IEnumerator DestroySnowball()
    {
        yield return new WaitForSeconds(5);
        Destroy(this.gameObject);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (this.CompareTag("Snowball"))
        {
            if (other.CompareTag("Obstacles"))
            {
                Destroy(this.gameObject);
            } else if (other.CompareTag("Player"))
            {
                if (other.gameObject != throwingPlayer.gameObject)
                {
                    other.GetComponent<Player>().RemoveHealth(40);
                    throwingPlayer.GetComponent<Player>().playerScore++;
                    Destroy(this.gameObject);
                }
                
            }
        }
        
    }
    
}

/* Created by Umberto Falkenhagen for Game Programming Elective SS 21 (Game Lab) */