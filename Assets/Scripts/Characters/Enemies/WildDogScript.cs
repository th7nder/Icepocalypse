using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildDogScript : MonoBehaviour
{
    public float EnemyRunningSpeed = 5.0f;
    public float EnemyWalkingSpeed = 2.5f;
    public float FightingDeadZone = 0.5f;
    public Transform ViewRangeTransform;
    public Transform PatrolDistanceTransform;

    private GameObject _playerObject;

    private GameObject _circle = null;

    protected Rigidbody2D _rb;

    private float _patrolDistance;
    private float _patrolRangeA;
    private float _patrolRangeB;
    private float _triggerRange;

    private float _playerEnemyDistance;

    private bool _isClutching;
    private float _time;
    private int _keyPressesIterator;

    public GameObject QTECircle;

    [HideInInspector]
    public bool ClickedTheCircle = false;

    enum Facing
    {
        Right,
        Left
    }

    private Facing _facingD;

    enum PlayerState
    {
        Idle,
        Walking,
        Running,
        Attacking
    }

    private PlayerState _dogState = PlayerState.Walking;

    void Start()
    {
        _isClutching = false;
        _rb = GetComponent<Rigidbody2D>();
        _playerObject = GameObject.FindGameObjectWithTag("Player");
        _patrolDistance = Mathf.Abs(transform.position.x - PatrolDistanceTransform.position.x);
        _patrolRangeA = transform.position.x - _patrolDistance;
        _patrolRangeB = transform.position.x + _patrolDistance;

        //Debug.Log("A: " + _patrolRangeA + " B: " + _patrolRangeB);

        if (ViewRangeTransform.position.x > transform.position.x)
        {
            _facingD = Facing.Right;
        }
        else
        {
            _facingD = Facing.Left;
        }

        _triggerRange = Mathf.Abs(ViewRangeTransform.position.x - transform.position.x);
    }

    void Update()
    {
        _playerEnemyDistance = _playerObject.transform.position.x - transform.position.x;

        if (_dogState != PlayerState.Running && _dogState != PlayerState.Attacking && (transform.position.x <= _patrolRangeA && _facingD == Facing.Left) || (transform.position.x >= _patrolRangeB && _facingD == Facing.Right))
        {
            ChangeFacingDirection();
        }

        GetState();


        if ( !_isClutching )
        {
            _playerEnemyDistance = _playerObject.transform.position.x - transform.position.x;

            if (_dogState != PlayerState.Running && _dogState != PlayerState.Attacking && (transform.position.x <= _patrolRangeA && _facingD == Facing.Left) || (transform.position.x >= _patrolRangeB && _facingD == Facing.Right))
            {
                ChangeFacingDirection();
            }

            GetState();
        }
        else
        {
            if ( _playerObject != null )
            {
                transform.position = new Vector2(_playerObject.transform.position.x, _playerObject.transform.position.y + 0.7f);
                _rb.isKinematic = true;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            
            if ( _playerObject == null )
            {
                Debug.Log("object null");
            }
            //Fight();
        }

    }

    void Fight()
    {
        if ( Time.time - _time >= 5.0f )
        {
            Debug.Log("gracz umiera");
            _playerObject = null;
            //_rb.isKinematic = false;
            //this.GetComponent<BoxCollider2D>().isTrigger = false;
        }

        if ( Input.GetKeyDown( KeyCode.F) && _keyPressesIterator < 20 )
        {
            _keyPressesIterator++;
        }
        else if ( _keyPressesIterator >= 20 )
        {
            //_rb.AddForce(new Vector2(0.5f, 0f));
            //_rb.isKinematic = false;
            //this.GetComponent<BoxCollider2D>().isTrigger = false;
            Time.timeScale = 1.0f;
            Destroy(gameObject);
            _playerObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
            _playerObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            if ( ClickedTheCircle )
            {
                Destroy(gameObject);
                Debug.Log("mamy animacje kontry na psie");
            }
            else
            {
                //_isClutching = true;
                //this.GetComponent<BoxCollider2D>().isTrigger = true;
                // _time = Time.time;
                Debug.Log("pies nas zjada");
            }
        }
    }

    void ChangeFacingDirection()
    {
        if (_facingD == Facing.Right)
        {
            _facingD = Facing.Left;
        }
        else
        {
            _facingD = Facing.Right;
        }

        transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
    }

    void GetState()
    {
        switch (_dogState)
        {
            case PlayerState.Walking:
                Walk(EnemyWalkingSpeed);

                if (Mathf.Abs(_playerEnemyDistance) < _triggerRange)
                {
                    if (_facingD == Facing.Left)
                    {
                        if (_playerEnemyDistance < 0)
                        {
                            _dogState = PlayerState.Running;
                        }
                    }
                    else if (_playerEnemyDistance > 0)
                    {
                        _dogState = PlayerState.Running;
                    }
                }

                break;

            case PlayerState.Running:
                Run(EnemyRunningSpeed);

                if (Mathf.Abs(_playerEnemyDistance) <= FightingDeadZone)
                {
                    _dogState = PlayerState.Attacking;
                }

                break;

            case PlayerState.Attacking:
                Attack();
                break;
        }
    }

    void Walk(float speed)
    {
        int _directionSpecifier = 1;

        if (_facingD == Facing.Left)
        {
            _directionSpecifier = -1;
        }

        _rb.velocity = new Vector2(speed * _directionSpecifier, _rb.velocity.y);
    }

    void Run(float speed)
    {
        if (_playerEnemyDistance > 0)
        {
            _rb.velocity = new Vector2(speed, _rb.velocity.y);
            //Debug.Log("Running left");
        }
        else if (_playerEnemyDistance < 0)
        {
            _rb.velocity = new Vector2(-speed, _rb.velocity.y);
            //Debug.Log("Running left");
        }
    }

    void Attack()
    {
        //Vector3 _jumpVector = new Vector3(_playerObject.transform.position.x - transform.position.x, 0.05f, transform.position.z);
        //_rb.AddForce( _jumpVector, ForceMode2D.Impulse);
        //Time.timeScale = 0.05f;

        if ( ClickedTheCircle == false && _circle == null )
        {
            Vector2 vector = new Vector2(transform.position.x, transform.position.y + 0.5f);

            _circle = Instantiate(QTECircle, vector, Quaternion.identity) as GameObject;
            _circle.GetComponent<QTECircleScript>().FatherObject = gameObject;

            _circle.GetComponent<QTECircleScript>()._qteType = "Dog";
        }

        _rb.velocity = new Vector2( (_playerObject.transform.position.x - transform.position.x)*7.0f, ( _playerObject.transform.position.y + 1.0f - transform.position.y )*10.0f );
        _playerObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        //_rb.velocity = new Vector2(0, _rb.velocity.y);
    }

}
