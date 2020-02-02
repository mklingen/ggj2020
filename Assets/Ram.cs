using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ram : MonoBehaviour
{
    public ParticleSystem GrassParticles = null;

    // Keep track of the camera so we know which direction is forward and right.
    private Camera _camera = null;

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
    public string VerticalAxis = "Vertical";
    public string HorizontalAxis = "Horizontal";
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

    // Arrow to draw indicating where we will go.
    public GameObject RamIndicator = null;

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

    // Keep track of the RigidBody so we can work with physics.
    private Rigidbody _body = null;

    // Current state of the ram.
    public MoveState CurrentState = MoveState.Walking;

    // Multiplier on velocity to give to the sheep to send it flying.
    public float SheepHitForce = 2.5f;

    // Multiplier on our velocity to stop ourselves when we hit a sheep.
    public float SheepHitEnergyLoss = 0.25f;

    // Mulitiplier on our own velocity to prevent us from just shoving sheep like normal.
    public float ResistSheepMotion = 0.1f;

    public bool IsTouchingSheep = false;

    private Animator _animator = null;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _body = GetComponentInChildren<Rigidbody>();
        _sprite = GetComponentsInChildren<SpriteRenderer>()[1];
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
            float multiplier = IsTouchingSheep ? ResistSheepMotion : 1.0f;
            _body.velocity = (_lastDirection) * Speed * multiplier;
        }
        _body.velocity *= Friction;

        if (Input.GetButton(ChargeUpButton))
        {
            StartCharging();
        }
        _sprite.color = Color.white;
    }

    private void OnCollisionStay(Collision collision)
    {
        bool isSheep = collision.collider.gameObject.layer == LayerMask.NameToLayer("Sheep");
        bool isSideWall = collision.collider.gameObject.layer == LayerMask.NameToLayer("SideWall");
        // While walking and hitting a sheep, reduce our own velocity by a bit so that the player can't just push sheep around.
        if (CurrentState == MoveState.Walking && isSheep)
        {
            IsTouchingSheep = true;
            collision.collider.GetComponentInChildren<Rigidbody>().velocity *= ResistSheepMotion;
        }

        if (CurrentState == MoveState.Ramming)
        {
            RamThing(collision);
            if (!isSideWall)
            {
                EnterCooldown();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        IsTouchingSheep = false;
    }

    private void RamThing(Collision collision)
    {
        bool isSheep = collision.collider.gameObject.layer == LayerMask.NameToLayer("Sheep");
        bool isFence = collision.collider.gameObject.layer == LayerMask.NameToLayer("Fence");

        if (isFence)
        {
            var fence = collision.collider.gameObject.GetComponentInChildren<Fence>();
            if (fence != null)
            {
                fence.Die();
                //_body.velocity *= 0.0f;
            }
        }

        // Give collisions with sheep extra oomph.
        if (isSheep)
        {
            _body.velocity *= SheepHitEnergyLoss;
            collision.collider.GetComponentInChildren<Rigidbody>().velocity += _body.velocity * SheepHitForce;
            collision.collider.GetComponent<Sheep>().Launch();
        }
    }

    // Called when the ram hits something.
    private void OnCollisionEnter(Collision collision)
    {
        bool isSheep = collision.collider.gameObject.layer == LayerMask.NameToLayer("Sheep");
        bool isFence = collision.collider.gameObject.layer == LayerMask.NameToLayer("Fence");

        // While walking and hitting a sheep, reduce our own velocity by a bit so that the player can't just push sheep around.
        if (CurrentState == MoveState.Walking && isSheep)
        {
            IsTouchingSheep = true;
        }
        if (CurrentState == MoveState.Ramming)
        {
            RamThing(collision);
            EnterCooldown();
        }
    }

    private float _timeRamming = 0.0f;

    // Called every tick while the ram is ramming.
    void DoRam()
    {
        _timeRamming += Time.deltaTime;
        if (_timeRamming > 3.0f)
        {
            EnterCooldown();
        }
        _body.velocity = -1.0f * _directionOnRam * (1.0f + CurrentChargeUp) * RammingSpeed;
        if (_body.velocity.magnitude < 10.0f)
        {
            EnterCooldown();
        }
        _sprite.color = new Color(1.0f, 0.0f, 0.0f);
    }

    // Start cooling down.
    void EnterCooldown()
    {
        CurrentState = MoveState.Cooldown;
        CurrentChargeUp = 0.0f;
        _currentCooldown = 0.0f;
    }

    // Called every tick while the ram is cooling down.
    void DoCooldown()
    {
        if (_currentCooldown >= CooldownTime)
        {
            CurrentState = MoveState.Walking;
            _lastDirectionNormalized = Vector3.zero;
            _lastDirection = Vector3.zero;
        }

        _currentCooldown += Time.deltaTime;

        _sprite.color = new Color(0.5f, 0.5f, 1.0f);
        _body.velocity *= Friction;
    }

    // Called every tick while the ram is charging up its ram.
    void DoChargeup()
    {
        RamIndicator.SetActive(true);
        _body.velocity *= Friction;
        CurrentChargeUp += Time.deltaTime * ChargeUpRatePerSecond;
        CurrentChargeUp = Mathf.Min(CurrentChargeUp, MaximumChargeup);

        // Give the player some ability to control the direction the ram will go.
        if (_lastDirectionNormalized.magnitude > 0.5f)
        {
            _directionOnRam = _lastDirectionNormalized;
        }

        RamIndicator.transform.position = transform.position - _directionOnRam;
        RamIndicator.transform.rotation = Quaternion.Euler(0, -Mathf.Rad2Deg * Mathf.Atan2(_directionOnRam.z, _directionOnRam.x), 0);

        _sprite.color = new Color(CurrentChargeUp, (1.0f - CurrentChargeUp), (1.0f - CurrentChargeUp));

        // Begin ramming when the player releases the chargeup button.
        if (!Input.GetButton(ChargeUpButton))
        {
            if (CurrentChargeUp > MinimumChargeForMotion)
            {
                _timeRamming = 0.0f;
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
        RamIndicator.SetActive(false);
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
                    GrassParticles.Stop();
                    DoWalk();
                    break;
                }
            case MoveState.Ramming:
                {
                    GrassParticles.Play();
                    DoRam();
                    break;
                }
            case MoveState.Cooldown:
                {
                    GrassParticles.Stop();
                    DoCooldown();
                    break;
                }
            case MoveState.CharingRam:
                {
                    GrassParticles.Stop();
                    DoChargeup();
                    break;
                }
        }
        Bounce();

        var dotRight = Vector3.Dot(_right, _lastDirectionNormalized);
        var dotFwd = Vector3.Dot(_fwd, _lastDirectionNormalized);

        if (CurrentState == MoveState.CharingRam || CurrentState == MoveState.Ramming)
        {
            dotRight = -dotRight;
            dotFwd = -dotFwd;
        }

        _animator.SetInteger("Direction", dotFwd < 0 ? 0 : 1);
       if (dotFwd > 0)
        {
            _sprite.flipX = dotRight > 0;
        }
       else
        {
            _sprite.flipX = dotRight < 0;
        }

        _animator.speed = Mathf.Max(_body.velocity.magnitude / 10.0f, 0.1f);

        // Draw some debug data (only shown when gizmos are enabled).
        Debug.DrawLine(transform.position, transform.position + _body.velocity, Color.red);
        Debug.DrawLine(transform.position, transform.position + _lastDirectionNormalized, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + _lastDirection, Color.cyan);
    }
}
