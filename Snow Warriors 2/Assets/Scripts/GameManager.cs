using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public List<GameObject> playerprefabs;
    private GameObject[] spawnpoints;
    private GameObject[] activePlayersArray;
    public List<GameObject> activePlayersList;
    public List<Player> scoreList;

    public bool spawnPlayersAtStart;
    public float timescale;
    private static GameObject[] playerpositions;
    private bool trackpositions = true;
    public int trackingSpeed;

    public Canvas RoundEndedUI;
    public TextMeshProUGUI winnerName;

    public float roundTime = 600f;
    private float timeSinceStart;
    private float remainingTime;
    public TextMeshProUGUI timerText;

    //List of positions for interactables
    public List<GameObject> collectablespawns = new List<GameObject>();
    public int healthCount = 0;
    public int staminaCount = 0;
    
    private Transform spawnPos;
    [SerializeField] private GameObject HealthPickup;
    [SerializeField] private GameObject StaminaPickup;
    

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = timescale;
        timeSinceStart = 0;
        spawnpoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        collectablespawns = GameObject.FindGameObjectsWithTag("CollectableSpawn").ToList();

        if (spawnPlayersAtStart)
        {
            int lastspawn = 0;
            foreach (var playerprefab in playerprefabs)
            {
                playerprefab.transform.position = spawnpoints[lastspawn].transform.position;
                //Debug.Log(playerprefab.name + " was spawned at location  of " + spawnpoints[lastspawn].transform.parent.name);
                if (lastspawn < spawnpoints.Length - 1)
                {
                    lastspawn++;
                }
                else
                {
                    lastspawn = 0;
                }
            }
        }

        activePlayersArray = GameObject.FindGameObjectsWithTag("Player");
        activePlayersList = activePlayersArray.ToList();

        //spawn interactable objects
        //startInteractable();
        
    }

    // Update is called once per frame
    void Update()
    {
        scoreList = FindObjectsOfType<Player>().ToList();
        scoreList.Sort(delegate(Player player1, Player player2)
        {
            return player1.playerScore.CompareTo(player2.playerScore);
            
        });
        SpawnCollectables();
        activePlayersArray = GameObject.FindGameObjectsWithTag("Player");
        activePlayersList = activePlayersArray.ToList();
        //Debug.Log("There are " + activePlayers.Length + " active players left");
        if (trackpositions)
        {
            //Debug.Log("Tracking player positions");
            trackpositions = false;
            StartCoroutine(TrackPlayerPositions());
        }
        
        timeSinceStart += Time.deltaTime;
        remainingTime = roundTime - timeSinceStart;
        DisplayTime(remainingTime);
        
        if (activePlayersList.Count == 1)
        {
            Time.timeScale = 0f;
            winnerName.text = activePlayersList[0].name;
            RoundEndedUI.gameObject.SetActive(true);
        }
        
        if (remainingTime <= 0f)
        {
            Time.timeScale = 0f;
            winnerName.text = scoreList[0].gameObject.name;
            RoundEndedUI.gameObject.SetActive(true);
        }
    }
    
    public void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    IEnumerator TrackPlayerPositions()
    {
        playerpositions = GameObject.FindGameObjectsWithTag("Player");
        
        yield return new WaitForSeconds(trackingSpeed);
        
        trackpositions = true;
        
    }

    public GameObject[] GetPlayerPositions()
    {
        return playerpositions;
    }
    
    //public void SortPlayersByScore() {}
    
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Starting a new round");
    }

    public void QuitApplication()
    {
        Application.Quit();
        Debug.Log("Quitting the game...");
    }
    
    /* Created by Umberto Falkenhagen for Game Programming Elective SS 21 (Game Lab) */

    public void SpawnCollectables()
    {
        while (healthCount < 2)
        {
            int randomindex = Random.Range(0, collectablespawns.Count);
            if (collectablespawns[randomindex].gameObject.GetComponentInChildren<HealthPickUp>() == null && collectablespawns[randomindex].gameObject.GetComponentInChildren<StaminaPickUp>() == null)
            {
                Instantiate(HealthPickup, collectablespawns[randomindex].transform);
                healthCount++;
            }
        }
        while (staminaCount < 2)
        {
            int randomindex = Random.Range(0, collectablespawns.Count);
            if (collectablespawns[randomindex].gameObject.GetComponentInChildren<HealthPickUp>() == null && collectablespawns[randomindex].gameObject.GetComponentInChildren<StaminaPickUp>() == null)
            {
                Instantiate(StaminaPickup, collectablespawns[randomindex].transform);
                staminaCount++;
            }
        }
    }
}

/* Created by Nico for Game Programming Elective SS 21 (Game Lab) */
