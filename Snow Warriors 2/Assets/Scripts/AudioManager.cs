using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    
    [Header("Audio Files")]
    public AudioClip dashSfx;
    public AudioClip formsnowSfx;
    public AudioClip healthSfx;
    public AudioClip staminaSfx;
    public AudioClip throwSfx;

    [Header("Background Track")] 
    public AudioSource musicsrc;
    public AudioSource footstepsrc;
    
    [System.Serializable]
    
    public class Taunt
    {
        public string name;
        public AudioClip Audiofile;
        [Range(0.0f, 1.0f)] public int volume;
        public Player PlayerSelect;
        
    }

    [System.Serializable]
    public class Footstep
    {
        public string name;
        public AudioClip Audiofile;
        [Range(0.00f, 1.00f)] public int volume;
        public Player PlayerSelect;
        
    }

    [Header("Audio Classes")] 
    public Taunt[] Taunts;

    [Header("Master Volume")]  
    [Range(0f, 1f)] public float master;
    
    public enum Player
    {
        Player1,
        Player2,
        Player3,
        Player4,
        Player5
    }

    private void Start()
    {
        musicsrc.Play();
    }

    public void PlayFootstep(bool sw)
    {
        switch (sw)
        {
            case true:
                footstepsrc.Play();
                break;
            
            case false:
                footstepsrc.Stop();
                break;
        }
        footstepsrc.Play();
    }
    public void PlayTaunt(GameObject player)
    {
        AudioSource.PlayClipAtPoint(Taunts[Random.Range(0, Taunts.Length)].Audiofile, player.transform.position, master);
    }
    public void PlayHealth(GameObject player)
    {
        AudioSource.PlayClipAtPoint(healthSfx, player.transform.position, master);
    }
    
    public void PlayStamina(GameObject player)
    {
        AudioSource.PlayClipAtPoint(staminaSfx, player.transform.position, master);
    }
    
    public void PlayDodge(GameObject player)
    {
        AudioSource.PlayClipAtPoint(dashSfx, player.transform.position, master);
    }
    
    public void PlayThrow(GameObject player)
    {
        AudioSource.PlayClipAtPoint(dashSfx, player.transform.position, master);
    }
    
}

//Programmed by Johnny Broderick (401460) as part of the 'SnowWarriors' AI development project.