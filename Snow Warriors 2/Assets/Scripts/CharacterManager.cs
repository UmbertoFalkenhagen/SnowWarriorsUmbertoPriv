using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterManager : MonoBehaviour
{
    public PlayerControls _playerControls;

    public Vector2 mouseposition;
    public Vector2 movementinput;
    public Camera mainCamera;
    public float movespeed = 5f;
    public float rotationspeed = 1f;
    private void Awake()
    {
        _playerControls = new PlayerControls();
        _playerControls.IngameControls.Movement.performed += ctx => movementinput = ctx.ReadValue<Vector2>();
        
    }

    private void Update()
    {
        Vector3 targetmovedirection = new Vector3(movementinput.x, 0, movementinput.y);
        MoveTowardsTarget(targetmovedirection);
        TurnInMoveDirection(targetmovedirection);
        
    }

    private void MoveTowardsTarget(Vector3 target)
    {
        float speed = movespeed * Time.deltaTime;
        
        transform.Translate(target * speed);
        
        
        
    }

    private void TurnInMoveDirection(Vector3 target)
    {
        var targetRotation = Quaternion.LookRotation(target);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationspeed);
        
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.Disable();
    }
}
