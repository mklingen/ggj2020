using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ram : MonoBehaviour
{
    private Camera _camera = null;
    private Vector3 _velocity = new Vector3(0, 0, 0);
    public float Friction = 0.9f;
    public float Speed = 1.0f;

    private Vector3 _fwd = Vector3.zero;
    private Vector3 _right = Vector3.zero;

    private float _currentCooldown = 0.0f;
    public float CooldownTime = 1.0f;

    public float CurrentChargeUp = 0.0f;
    public float ChargeUpRatePerSecond = 0.5f;
    public float MinimumChargeForMotion = 0.25f;
    public float MaximumChargeup = 2.0f;

    public float RammingSpeed = 10.0f;

    private Vector3 _lastDirection = Vector3.zero;
    private Vector3 _lastDirectionNormalized = Vector3.zero;
    private Vector3 _directionOnRam = Vector3.zero;

    public string VerticalAxis = "Vertical0";
    public string HorizontalAxis = "Horizontal0";
    public string ChargeUpButton = "Chargeup";

    private float _heightAtStart = 0.0f;

    public float BounceWhileWalking = 0.1f;
    public float BounceWhileCharging = 0.2f;
    public float BounceWhileRamming = 0.25f;
    public float BounceRateWhileWalking = 2.0f;
    public float BounceRateWhileCharging = 4.0f;
    public float BounceRateWhileRamming = 3.0f;

    private SpriteRenderer _sprite = null;


    private float _lastYOffset = 0.0f;

    public enum MoveState
    {
        Walking,
        Ramming,
        CharingRam,
        Cooldown
    }

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

    void StartCharging()
    {
        CurrentState = MoveState.CharingRam;
        CurrentChargeUp = 0.0f;
    }

    void DoWalk()
    {
        if (_lastDirection.magnitude > 0.5f)
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

    private void OnCollisionEnter(Collision collision)
    {
        if (CurrentState == MoveState.Ramming)
        {
            // TODO: do different things depending on what we hit!
            EnterCooldown();
        }
    }

    void DoRam()
    {
        _velocity = _directionOnRam * CurrentChargeUp * RammingSpeed;
        _sprite.color = new Color(1.0f, 0.0f, 0.0f);
    }

    void EnterCooldown()
    {
        CurrentState = MoveState.Cooldown;
        CurrentChargeUp = 0.0f;
        _currentCooldown = 0.0f;
        _velocity *= 0.0f;
    }

    void DoCooldown()
    {
        if (_currentCooldown >= CooldownTime)
        {
            CurrentState = MoveState.Walking;
        }

        _currentCooldown += Time.deltaTime;

        _sprite.color = new Color(0.5f, 0.5f, 1.0f);
    }

    void DoChargeup()
    {
        _velocity *= Friction;
        CurrentChargeUp += Time.deltaTime * ChargeUpRatePerSecond;
        CurrentChargeUp = Mathf.Min(CurrentChargeUp, MaximumChargeup);

        _sprite.color = new Color(CurrentChargeUp, (1.0f - CurrentChargeUp), (1.0f - CurrentChargeUp));
        if (!Input.GetButton(ChargeUpButton))
        {
            if (CurrentChargeUp > MinimumChargeForMotion)
            {
                _directionOnRam = _lastDirectionNormalized;
                CurrentState = MoveState.Ramming;
            }
            else
            {
                CurrentState = MoveState.Walking;
                CurrentChargeUp = 0.0f;
            }
        }
    }

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
        _lastDirection = _fwd * Input.GetAxis(VerticalAxis) + _right * Input.GetAxis(HorizontalAxis);
        if (_lastDirection.magnitude > 1.0f)
        {
            _lastDirectionNormalized = _lastDirection.normalized;
        }
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
        Debug.DrawLine(transform.position, transform.position + _velocity, Color.red);
        Debug.DrawLine(transform.position, transform.position + _lastDirectionNormalized, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + _lastDirection, Color.cyan);
    }
}
