using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour
{
    public static List<Sheep> Sheeps = new List<Sheep>();
    public float RandomMovementSpeed = 1.0f;
    public float RandomMovementChange = 1.0f;
    public float LaunchTime = 0.0f;

    public GameObject cloudTemplate;

    private float RandomMoveCounter = 0;
    private Vector3 _randomMotion = Vector3.zero;
    private Rigidbody _body = null;
    private float _startY = 0.0f;
    Vector3 _right = Vector3.zero;
    private SpriteRenderer _sprite = null;
    private Animator _animator = null;
    public void Die()
    {
        // create a new object where sheep used to be
        GameObject cloud = Instantiate(cloudTemplate);
        cloud.transform.SetPositionAndRotation(transform.position, transform.rotation);
        cloud.GetComponentInChildren<Rigidbody>().velocity = _body.velocity;

        Destroy(this.transform.root.gameObject);
    }

    public void Launch()
    {
        LaunchTime = 1.0f;
        GetComponent<AudioSource>().Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _sprite = GetComponentsInChildren<SpriteRenderer>()[1];
        _right = Camera.main.transform.right;
        _startY = transform.position.y;
        _body = GetComponentInChildren<Rigidbody>();
        Sheeps.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (RandomMoveCounter < 0)
        {
            _randomMotion = Random.insideUnitSphere * RandomMovementSpeed;
            _randomMotion.y = 0;
            RandomMoveCounter = RandomMovementChange;
            _animator.speed = Random.Range(0.5f, 2.5f);
        }
        _body.AddForce(_randomMotion);
        RandomMoveCounter -= Time.deltaTime;

        if (LaunchTime > 0)
        {
            LaunchTime -= Time.deltaTime;
            float y = Mathf.Max(Mathf.Sin(Mathf.PI * (1.0f - LaunchTime)) + _startY, _startY);
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }

        float dotRight = Vector3.Dot(_body.velocity, _right);
        _sprite.flipX = dotRight > 0 ? true : false;

    }
}
