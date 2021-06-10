using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class Player : MonoBehaviour
{
    //Player values
    public int playerHealth = 100;
    public int playerStamina = 10;
    public int playerScore = 0;
    public int playerSmallSnowballCount = 0;
    public bool holdingLargeSnowball = false;

    //Movement
    private int walkingspeed = 10;
    private int runningspeed = 15;
    private int crouchingspeed = 5;
    private int dashingspeed = 30;
    public int currentspeed = 0;

    private bool canDodge = true;
    private bool canWalk = true;

    public event Action<int> OnChangeMoveState;
    
    
    //Sensories
    public EnviScanner enviScanner;
    public List<GameObject> obstaclesInRange = new List<GameObject>();
    public List<GameObject> collectablesInRange = new List<GameObject>();
    public List<GameObject> enemiesInSight = new List<GameObject>();
    public List<GameObject> enemiesInHearingRange = new List<GameObject>();
    public List<GameObject> snowballsInSight = new List<GameObject>();
    
    
    //Circumstantial attributes
    public GameManager gameManager;
    public bool createLargeSnowball = false;
    public bool throwsnowball = false;
    public NavMeshAgent navMeshAgent;

    public GameObject smallSnowballPrefab;
    public GameObject bigSnowballPrefab;
    public GameObject largeSnowball;

    public String debugoutput; //AI's can print their actions to the HUD
   
    

    public CharacterAnimationManager animationManager;
    //private bool changestate = true;

    [SerializeField]
    public int animationstate = 0;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        enviScanner = GetComponentInChildren<EnviScanner>();
        animationManager = GetComponent<CharacterAnimationManager>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        playerHealth = 100;
        playerStamina = 10;
    }

    #region PlayerActions
    public void Move(Vector3 targetPosition)
    {
        if (gameObject.GetComponent<Rigidbody>() != null)
        {
            var velocity = new Vector3(0, 0, 0);
            velocity = gameObject.GetComponent<Rigidbody>().velocity;
            if (velocity.y >= -5)
            {
                velocity.y = -1;
                gameObject.GetComponent<Rigidbody>().velocity.Set(velocity.x, velocity.y, velocity.z);
            }
        }
        if (canWalk)
        {
            ChangeMoveState(2, 0);
            navMeshAgent.SetDestination(targetPosition);
        }
        else
        {
            Debug.Log(name + " can not move currently...");
        }
        
    }

 

    public void Dodge(Vector3 targetPosition)
    {
        if (!canWalk || !canDodge || !(playerStamina >= 4)) return;
        playerStamina -= 4;
        var amanager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        amanager.PlayDodge(gameObject);
        ChangeMoveState(3,0);
        StartCoroutine(SwitchAnimationState(0, 4));
        navMeshAgent.SetDestination(targetPosition);
        StartCoroutine(ResetSpeed(10));
        StartCoroutine(PauseDodging(10));
    }
    
    
    public void Taunt()
    {
        var amanager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        amanager.PlayTaunt(gameObject);
        currentspeed = 60;
        StartCoroutine(PauseDodging(2));
        StartCoroutine(PauseMovement(2));
        
    }
    public void CreateSmallSnowball()
    {
        if (!holdingLargeSnowball && playerSmallSnowballCount < 6 && animationstate != 2)
        {
            animationstate = 2;
            ChangeMoveState(0, 0);
            StartCoroutine(PauseMovement(1));
            StartCoroutine(SwitchAnimationState(0.99f, 0));
            ChangeMoveState(2, 1);
            playerSmallSnowballCount++;

        }
        else
        {
            if (holdingLargeSnowball)
            {
                Debug.Log("You already hold a large snowball");
            }
            if (playerSmallSnowballCount >= 6)
            {
                Debug.Log("You already hold the max amount of snowballs");
            }
        }

    }

    public void CreateLargeSnowball()
    {
        if (!holdingLargeSnowball)
        {
            ChangeMoveState(0, 0);
            StartCoroutine(PauseMovement(2));
            animationstate = 2;
            RemoveStamina(2);
            StartCoroutine(SwitchAnimationState(2, 0));
            holdingLargeSnowball = true;
            Vector3 instantiatingoffset = new Vector3();
            instantiatingoffset = transform.position;
            instantiatingoffset.z += 1;
            instantiatingoffset.y += 1;
            largeSnowball = Instantiate(bigSnowballPrefab, instantiatingoffset, Quaternion.identity);
            largeSnowball.GetComponent<SnowBall>().throwingPlayer = gameObject;
            largeSnowball.transform.parent = this.transform;
            ChangeMoveState(2, 2);
        }
        else
        {
            if (holdingLargeSnowball)
            {
                Debug.Log("You already hold a large snowball");
            }
        }
        ;
        
    }
    
    public void ThrowSnowball(Vector3 target)
    {
        this.transform.LookAt(target);
        if (!holdingLargeSnowball && playerSmallSnowballCount > 0 && animationstate != 3)
        {
            animationstate = 3;
            var amanager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
            amanager.PlayThrow(gameObject);
            GameObject snowball = Instantiate(smallSnowballPrefab, enviScanner.visionOffset.position, Quaternion.identity);
            snowball.GetComponent<SnowBall>().throwingPlayer = this.gameObject;
            snowball.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
            RemoveStamina(1);
            playerSmallSnowballCount--;
            StartCoroutine(SwitchAnimationState(0.99f, 0));
        }
        else if (holdingLargeSnowball)
        {
            var amanager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
            amanager.PlayThrow(gameObject);
            largeSnowball.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
            largeSnowball.transform.SetParent(null);
            holdingLargeSnowball = false;
            RemoveStamina(2);
        }
        else
        {
            Debug.Log("You dont have any snowballs to throw...");
        }
        
    }
    

    #endregion
    
    #region MoveStateManagement

    public IEnumerator ChangeMoveState(int movestate, float delaytime)
    {
        yield return new WaitForSeconds(delaytime);
        if (canWalk)
        {
            switch (movestate)
            {
                case 0: //stopped
                    currentspeed = 0;
                    StartCoroutine(SwitchAnimationState(0, 0));
                    break;
                case 1: //crouching
                    currentspeed = crouchingspeed;
                    StartCoroutine(SwitchAnimationState(0, 1));
                    break;
                case 2: //walking
                    currentspeed = walkingspeed;
                    StartCoroutine(SwitchAnimationState(0, 1));
                    break;
                case 3: //dodging
                    currentspeed = dashingspeed;
                    StartCoroutine(SwitchAnimationState(0, 1));
                    break;
            }
            navMeshAgent.speed = currentspeed;
        }
        else
        {
            Debug.Log(name + " can not change its movementspeed currently");
        }
        
    } 
    
    IEnumerator SwitchAnimationState(float delaytime, int _animationstate)
    {
        yield return new WaitForSeconds(delaytime);
        animationstate = _animationstate;
    }

    #endregion
    
    #region PlayerStatManagment

    public void AddHealth(int amount)
    {
        if (playerHealth + amount > 100)
        {
            playerHealth = 100;
        }
        else
        {
            playerHealth += amount;
        }
    }

    public void RemoveHealth(int amount)
    {
        if (playerHealth - amount <= 0)
        {
            StartCoroutine(KillPlayer(1));
        }
        else
        {
            playerHealth -= amount;
        }
        
    }
    
    public void AddStamina(int amount)
    {
        if (playerStamina + amount > 10)
        {
            playerStamina = 10;
        }
        else
        {
            playerStamina += amount;
        }
    }
    
    public void RemoveStamina(int amount)
    {
        if (playerStamina - amount < 0)
        {
            Debug.Log("No stamina left");
        }
        else
        {
            playerStamina -= amount;
        }
        
    }
    
    IEnumerator KillPlayer(int delaytime)
    {
        animationstate = 4;
        yield return new WaitForSeconds(delaytime);
        this.gameObject.SetActive(false);
    }
    
    IEnumerator PauseMovement(float _duration)
    {
        canWalk = false;
        yield return new WaitForSeconds(_duration);
        canWalk = true;

    }

    IEnumerator ResetSpeed(float _duration)
    {
        yield return new WaitForSeconds(_duration);
        ChangeMoveState(2, 0);
       
    }
    
    IEnumerator PauseDodging(float _duration)
    {
        canDodge = false;
        yield return new WaitForSeconds(_duration);
        canDodge = true;
    }

        #endregion

    #region EnvironmentListManagement

    public void UpdateEnvironmentLists(List<GameObject> _obstaclesInRange, List<GameObject> _collectablesInRange,
        List<GameObject> _enemiesInSight, List<GameObject> _enemiesInHearingRange, List<GameObject> _snowballsInSight)
    {
        //obstaclesInRange.Clear();
        obstaclesInRange = _obstaclesInRange;
        //collectablesInRange.Clear();
        collectablesInRange = _collectablesInRange;
        //enemiesInSight.Clear();
        enemiesInSight = _enemiesInSight;
        //enemiesInHearingRange.Clear();
        enemiesInHearingRange = _enemiesInHearingRange;
        //snowballsInSight.Clear();
        snowballsInSight = _snowballsInSight;
        RemoveNullObjects();
    }

    public void RemoveNullObjects()
    {
        for (int i = collectablesInRange.Count-1; i > -1; i--)
        {
            if (collectablesInRange[i] == null)
            {
                collectablesInRange.RemoveAt(i);
            }
        }
        for (int i = obstaclesInRange.Count-1; i > -1; i--)
        {
            if (obstaclesInRange[i] == null)
            {
                obstaclesInRange.RemoveAt(i);
            }
        }
        for (int i = enemiesInSight.Count-1; i > -1; i--)
        {
            if (enemiesInSight[i] == null)
            {
                enemiesInSight.RemoveAt(i);
            }
        }
        for (int i = enemiesInHearingRange.Count-1; i > -1; i--)
        {
            if (enemiesInHearingRange[i] == null)
            {
                enemiesInHearingRange.RemoveAt(i);
            }
        }
        for (int i = snowballsInSight.Count-1; i > -1; i--)
        {
            if (snowballsInSight[i] == null)
            {
                snowballsInSight.RemoveAt(i);
            }
        }
    }

    #endregion

}


/* Created by Umberto Falkenhagen for Game Programming Elective SS 21 (Game Lab) */