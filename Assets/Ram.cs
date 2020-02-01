﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ram : MonoBehaviour
{
    // Keep track of the camera so we know which direction is forward and right.
    private Camera _camera = null;

    // Current velocity in units per second.
    private Vector3 _velocity = new Vector3(0, 0, 0);
    // Amount velocity reduces by when player isn't holding down the stick.
    public float Friction = 0.9f;
    // The maximum speed of the character.
    public float Speed = 1.0f;

    // Forward and right vectors of the camera, projected to the plane.
    private Vector3 _fwd = Vector3.zero;
    private Vector3 _right = Vector3.zero;

    // Time that it takes the ram to cool down before ramming again.
    private float _currentCooldown = 0.0f;
    public float CooldownTime = 1.0f;

    // Keeping track of charing up for a ram.
    public float CurrentChargeUp = 0.0f;
    // How fast we charge up for a ram.
    public float ChargeUpRatePerSecond = 0.5f;
    // Charge must be at least this high to ram.
    public float MinimumChargeForMotion = 0.25f;
    // Charge may be at most this high to ram.
    public float MaximumChargeup = 2.0f;

    // Speed that the ram goes when ramming.
    // Multiplied by the charge up.
    public float RammingSpeed = 10.0f;

    // Last directional input we got.
    private Vector3 _lastDirection = Vector3.right;
    // Normalized directional input.
    private Vector3 _lastDirectionNormalized = Vector3.right;
    // Normalized direction we will go just before ramming.
    private Vector3 _directionOnRam = Vector3.right;

    // Axes and button names.
    public string VerticalAxis = "Vertical0";
    public string HorizontalAxis = "Horizontal0";
    public string ChargeUpButton = "Chargeup";

    // Keep track of the height at start so that we can make a procedural
    // bouncing animation.
    private float _heightAtStart = 0.0f;

    // Controls the amplitude and rate of bouncing animations.
    public float BounceWhileWalking = 0.1f;
    public float BounceWhileCharging = 0.2f;
    public float BounceWhileRamming = 0.25f;
    public float BounceRateWhileWalking = 2.0f;
    public float BounceRateWhileCharging = 4.0f;
    public float BounceRateWhileRamming = 3.0f;
    public float MovementDeadband = 0.5f;

    // Keep track of the sprite so we can change its color.
    private SpriteRenderer _sprite = null;

    // Last bounce height for smooting.
    private float _lastYOffset = 0.0f;

    // Ram can be in one of these states.
    public enum MoveState
    {
        // Moving around normally (not ramming).
        Walking,
        // Moving in a straight line quickly toward a target.
        Ramming,
        // Charging up to begin ramming.
        CharingRam,
        // Paused for a moment while we wait to start ramming again.
        Cooldown
    }

    // Current state of the ram.
    public MoveState CurrentState = MoveState.Walking;

    // Start is called before the first frame update
    void Start()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _camera = Camera.main;
        _fwd = _camera.transform.forward;
        _fwd.y = 0.0f;
        _fwd.Normalize();
        _right = _camera.transform.right;
        _right.y = 0.0f;
        _right.Normalize();
        _heightAtStart = transform.position.y;
    }

    // start charging up the ram.
    void StartCharging()
    {
        CurrentState = MoveState.CharingRam;
        CurrentChargeUp = 0.0f;
    }

    // Called every tick while walking.
    void DoWalk()
    {
        if (_lastDirection.magnitude > MovementDeadband)
        {
            _velocity = (_lastDirectionNormalized) * Speed;
        }
        _velocity *= Friction;

        if (Input.GetButton(ChargeUpButton))
        {
            StartCharging();
        }
        _sprite.color = Color.white;
    }

    // Called when the ram hits something.
    private void OnCollisionEnter(Collision collision)
    {
        if (CurrentState == MoveState.Ramming)
        {
            // TODO: do different things depending on what we hit!
            EnterCooldown();
        }
    }

    // Called every tick while the ram is ramming.
    void DoRam()
    {
        _velocity = _directionOnRam * (1.0f + CurrentChargeUp) * RammingSpeed;
        _sprite.color = new Color(1.0f, 0.0f, 0.0f);
    }

    // Start cooling down.
    void EnterCooldown()
    {
        CurrentState = MoveState.Cooldown;
        CurrentChargeUp = 0.0f;
        _currentCooldown = 0.0f;
        _velocity *= 0.0f;
    }

    // Called every tick while the ram is cooling down.
    void DoCooldown()
    {
        if (_currentCooldown >= CooldownTime)
        {
            CurrentState = MoveState.Walking;
        }

        _currentCooldown += Time.deltaTime;

        _sprite.color = new Color(0.5f, 0.5f, 1.0f);
    }

    // Called every tick while the ram is charging up its ram.
    void DoChargeup()
    {
        _velocity *= Friction;
        CurrentChargeUp += Time.deltaTime * ChargeUpRatePerSecond;
        CurrentChargeUp = Mathf.Min(CurrentChargeUp, MaximumChargeup);

        if (CurrentChargeUp < 0.25)
        {
            // Give the player some ability to control the direction the ram will go,
            // but only momentarily.
            _directionOnRam = _lastDirectionNormalized;
        }

        _sprite.color = new Color(CurrentChargeUp, (1.0f - CurrentChargeUp), (1.0f - CurrentChargeUp));

        // Begin ramming when the player releases the chargeup button.
        if (!Input.GetButton(ChargeUpButton))
        {
            if (CurrentChargeUp > MinimumChargeForMotion)
            {
                CurrentState = MoveState.Ramming;
            }
            else
            {
                CurrentState = MoveState.Walking;
                CurrentChargeUp = 0.0f;
            }
        }
    }

    // Procedural bouncing animation.
    void Bounce()
    {
        float rate = 0.0f;
        float amplitude = 0.0f;
        switch (CurrentState)
        {
            case MoveState.Walking:
                {
                    rate = BounceRateWhileWalking;
                    amplitude = BounceWhileWalking;
                    break;
                }
            case MoveState.Ramming:
                {
                    rate = BounceRateWhileRamming;
                    amplitude = BounceWhileRamming;
                    break;
                }
            case MoveState.Cooldown:
                {
                    rate = 0.0f;
                    amplitude = 0.0f;
                    break;
                }
            case MoveState.CharingRam:
                {
                    rate = BounceRateWhileCharging * CurrentChargeUp;
                    amplitude = BounceWhileCharging;
                    break;
                }
        }
        float y = Mathf.Sin(rate * Time.time) * amplitude + _heightAtStart;
        transform.position = new Vector3(transform.position.x, y * 0.5f + _lastYOffset * 0.5f, transform.position.z);
        _lastYOffset = y;
    }

    // Update is called once per frame
    void Update()
    {
        // Get the direction of the input sticks and multiply it by our basis vectors.
        _lastDirection = _fwd * Input.GetAxis(VerticalAxis) + _right * Input.GetAxis(HorizontalAxis);
        if (_lastDirection.magnitude > MovementDeadband)
        {
            // Only normalize when the movement is greater than a deadband to avoid normalizing zero vector.
            _lastDirectionNormalized = _lastDirection.normalized;
        }

        // State machine.
        switch (CurrentState)
        {
            case MoveState.Walking:
                {
                    DoWalk();
                    break;
                }
            case MoveState.Ramming:
                {
                    DoRam();
                    break;
                }
            case MoveState.Cooldown:
                {
                    DoCooldown();
                    break;
                }
            case MoveState.CharingRam:
                {
                    DoChargeup();
                    break;
                }
        }
        transform.position += _velocity * Time.deltaTime;
        Bounce();

        // Draw some debug data (only shown when gizmos are enabled).
        Debug.DrawLine(transform.position, transform.position + _velocity, Color.red);
        Debug.DrawLine(transform.position, transform.position + _lastDirectionNormalized, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + _lastDirection, Color.cyan);
    }
}
