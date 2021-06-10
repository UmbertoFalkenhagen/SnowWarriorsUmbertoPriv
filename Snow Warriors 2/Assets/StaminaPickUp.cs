using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StaminaPickUp : MonoBehaviour
{
    private GameManager _gameManager;
    private AudioManager _audioManager;
    private List<EnviScanner> scanners;
    private Vector3 startPos;
    private Vector3 endPos;
    public float hoverSpeed = 0.5f;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _audioManager = FindObjectOfType<AudioManager>();
        scanners = FindObjectsOfType<EnviScanner>().ToList();
        //set up vector3 for start and end of oscillation
        startPos = transform.position;
        startPos.y = 1f;
        endPos = startPos;
        endPos.y = 2f;
    }

    private void Update()
    {
        //oscillation of the object on the y axis
        float t = Mathf.PingPong(Time.time * hoverSpeed * 2.0f, 1.0f);
        transform.position = Vector3.Lerp (startPos, endPos, t);    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
                        
            other.GetComponent<Player>().AddStamina(10);
            _audioManager.PlayStamina(other.gameObject);
            Destroy(gameObject);
            _gameManager.staminaCount--;
            //Debug.Log("removed 1 stamina pickup");
            foreach (var scanner in scanners)
            {
                scanner.UpdateObjectsInRange();
                scanner.RemoveObjectFromList(gameObject);
            }
        }
    }
}
