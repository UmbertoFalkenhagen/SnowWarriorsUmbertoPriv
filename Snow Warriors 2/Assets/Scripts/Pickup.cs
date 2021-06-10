using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    //private GameObject player;
    private GameObject gameManager;
    private GameObject audioManager;
    private GameManager gameManagerScript;
    
    private Vector3 startPos;
    private Vector3 endPos;
    public float hoverSpeed = 0.5f;
    public bool testSwitch = false;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager");
        audioManager = GameObject.Find("AudioManager");
    }

    private void Start()
    {
        //set up vector3 for start and end of oscillation
        startPos = transform.position;
        startPos.y = 1f;
        endPos = startPos;
        endPos.y = 2f;
    }
    
    private void Update () 
    {
        //oscillation of the object on the y axis
        float t = Mathf.PingPong(Time.time * hoverSpeed * 2.0f, 1.0f);
        transform.position = Vector3.Lerp (startPos, endPos, t);
        
        //use this switch to test the code that would run in OnTriggerEnter
        if (testSwitch)
        {
            testInteraction();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //find player that triggered the interactable

        //check if the object is a health pickup or stamina
        if (this.name == "HealthPickup(Clone)")
        {
            audioManager.GetComponent<AudioManager>().PlayHealth(gameObject);
            //add hp to player
            //gameManagerScript.removeInteractable(this.gameObject);
            //reduce the total count of HP objects
            //GameManager.healthCount--;
            //Debug.Log("new healthcount" + GameManager.healthCount);
        }
        else
        {
            audioManager.GetComponent<AudioManager>().PlayStamina(gameObject);
            //add stamina to player
            //reduce the total count of stamina objects
            //GameManager.staminaCount--;
            //Debug.Log("new healthcount" + GameManager.healthCount);
        }

        //start coroutine to respawn another interactable
        //gameManager.GetComponent<GameManager>().startInteractable();
        
        //destroy object
        Destroy(this.gameObject);
    }

    //this function is used for testing the code that will go in OnTriggerEnter()
    private void testInteraction()
    {
        //check if the object is a health pickup or stamina
        if (this.name == "HealthPickup(Clone)")
        {
            //add hp to player
            //reduce the total count of HP objects
            //GameManager.healthCount--;
            //Debug.Log("new healthcount" + GameManager.healthCount);
        }
        else
        {
            //add stamina to player
            //reduce the total count of stamina objects
            //GameManager.staminaCount--;
            //Debug.Log("new healthcount" + GameManager.healthCount);
        }
        
        testSwitch = false; //reset

        //start coroutine to respawn another interactable
        //gameManager.GetComponent<GameManager>().startInteractable();
        //gameManagerScript.startInteractable();
        
        //destroy object
        StartCoroutine(destroyObj());
    }

    private IEnumerator destroyObj()
    {
        yield return new WaitForSeconds(.2f);
        Destroy(this.gameObject);
    }
}

/* Created by Niccolo Chiodo for Game Programming Elective SS 21 (Game Lab) */