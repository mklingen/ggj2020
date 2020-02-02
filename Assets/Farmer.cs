using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmer : MonoBehaviour
{
    public LineRenderer Laser = null;
    private Fence _fenceToRepair = null;
    public float RepairRadius = 1.0f;
    private Rigidbody _body = null;
    private Vector3 _right;
    public GameObject Exclaimer = null;
    private Vector3 _startPosition;

    private AudioSource farmerAudio;
    public AudioClip repairSound;
    public AudioClip angrySound;
    public AudioClip fallSound;
    void Awake()
    {
        farmerAudio = GetComponent<AudioSource>();

    }

    // Start is called before the first frame update
    void Start()
    {
        _startPosition = transform.position;
        Exclaimer.SetActive(false);
        _right = Camera.main.transform.right;
        _sprite = GetComponentsInChildren<SpriteRenderer>()[1];
        _body = GetComponentInChildren<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
    }

    void SearchFences()
    {
        Fence closestFence = null;
        float closestDist = float.MaxValue;
        foreach (var fence in Fence.Fences)
        {
            if (!fence.isActiveAndEnabled)
            {
                float dist = (fence.transform.position - transform.position).sqrMagnitude;
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestFence = fence;
                }
            }
        }

        if (closestFence != null)
        {
            farmerAudio.clip = angrySound;
            farmerAudio.Play();
            Exclaimer.SetActive(true);
        }
        _fenceToRepair = closestFence;
    }


    public float Kp = 0.1f;
    public float RepairSpeed = 1.5f;
    private float _repairCounter = 0.0f;
    private SpriteRenderer _sprite = null;
    private Animator _animator = null;

    void RepairFence()
    {
        if (_fenceToRepair != null)
        {
            Laser.enabled = true;
            Laser.SetPosition(0, transform.position + Vector3.up * 0.5f);
            Laser.SetPosition(1, _fenceToRepair.transform.position + Random.insideUnitSphere * 0.15f);
        }
        _repairCounter += Time.deltaTime;
        if (_repairCounter > RepairSpeed)
        {
            _fenceToRepair.gameObject.SetActive(true);
            farmerAudio.clip = repairSound;
            farmerAudio.Play();
        }
    }

    void GoToFence()
    {
        Laser.enabled = false;
        var delta = (_fenceToRepair.transform.position - transform.position);
        if (delta.magnitude < RepairRadius)
        {
            Exclaimer.SetActive(false);
            RepairFence();
        }
        else
        {
            Exclaimer.SetActive(Mathf.Sin(Time.time * 10.0f) > 0);
            _body.AddForce(delta * Kp);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(transform.position.x) < 5.1 &&
            Mathf.Abs(transform.position.z) < 5.1)
        {
            if (_fenceToRepair == null || _fenceToRepair.isActiveAndEnabled)
            {
                Laser.enabled = false;
                SearchFences();
                _repairCounter = 0.0f;
            }

            if (_fenceToRepair != null)
            {
                GoToFence();
            }

            float dotRight = Vector3.Dot(_body.velocity, _right);
            _sprite.flipX = dotRight > 0 ? true : false;
        }
        else
        {
            Fall();
        }

        _animator.speed = _body.velocity.magnitude / 0.5f;
       
    }

    private bool _isFalling = false;
    public void Fall()
    {
        if (!_isFalling) { 
            // turn on gravity and play falling sound
            _body.useGravity = true;
            farmerAudio.PlayOneShot(fallSound);
            _isFalling = true;
            // turn off position constraints
            _body.constraints = RigidbodyConstraints.FreezeRotation;
        }
        if (transform.position.y < -10)
        {
            // once he's fallen far enough, reset him
            transform.position = _startPosition;
            _body.velocity = Vector3.zero;
            _body.useGravity = false;
            // turn on y position freezing
            _body.constraints |= RigidbodyConstraints.FreezePositionY;
            _isFalling = false;
        }
    }
}
