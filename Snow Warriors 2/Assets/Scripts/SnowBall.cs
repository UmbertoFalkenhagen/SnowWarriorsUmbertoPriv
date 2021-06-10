using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBall : MonoBehaviour
{
    public GameObject throwingPlayer;

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
                    other.GetComponent<Player>().RemoveHealth(20);
                    throwingPlayer.GetComponent<Player>().playerScore++;
                    Destroy(this.gameObject);
                }
                
            }
        }
        
    }
}

/* Created by Umberto Falkenhagen for Game Programming Elective SS 21 (Game Lab) */