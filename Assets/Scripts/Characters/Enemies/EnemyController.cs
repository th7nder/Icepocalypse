using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    public GameObject QteButton;

    public int EnemyHealthPointsMax = 100;
    private int _enemyHealthPoints;

    public bool _isUnderAttack = false;
    public bool _isDefending = false;

    private float _animatingTime = 0f;

    public float EnemyRunningSpeed = 5.0f;
    public float EnemyWalkingSpeed = 2.5f;
    public float FightingDeadZone = 0.5f;
    public Transform ViewRangeTransform;
    public Transform PatrolDistanceTransform;

    private GameObject _playerObject;

    private Rigidbody2D _rb;

    private float _patrolDistance;
    private float _patrolRangeA;
    private float _patrolRangeB;
    private float _triggerRange;

    private float _playerEnemyDistance;
    
    private Animator _anim;

    private bool _isHurt;
    private GameObject _qteCircle = null;

    private bool _didQTE = false;

    public float[] QTECirclePositionsArray;
    public float QTECircleInterval;
    private int _qteCircleIterator = 0;
    private float _timeStamp;

    private bool _canHeFight = true;
    private bool _sideFlag = false;

    public Collider2D FistCollider1;
    public Collider2D FistCollider2;

    public GameObject HitFlag;

    enum Facing
    {
        Right,
        Left
    }

    private Facing _facing;

	enum PlayerState 
	{
		Idle,
        Walking,
		Running,
		Attacking
	}

	private PlayerState _playerState = PlayerState.Walking;

	
	void Start ()
	{
        //Time.timeScale = 0.1f;

        HitFlag.SetActive(false);

        FistCollider1.enabled = false;
        FistCollider2.enabled = false;

        _timeStamp = 0f;

        _isHurt = false;

        _anim = GetComponent<Animator>();
        _enemyHealthPoints = EnemyHealthPointsMax;

        _rb = GetComponent<Rigidbody2D>();
        _playerObject = GameObject.FindGameObjectWithTag("Player");

        _patrolDistance = Mathf.Abs(transform.position.x - PatrolDistanceTransform.position.x);
        _patrolRangeA = transform.position.x - _patrolDistance;
        _patrolRangeB = transform.position.x + _patrolDistance;

        //Debug.Log("A: " + _patrolRangeA + " B: " + _patrolRangeB);

        if (ViewRangeTransform.position.x > transform.position.x )
        {
            _facing = Facing.Right;
            //ChangeFacingDirection();
        }
        else
        {
            _facing = Facing.Left;
        }

        _triggerRange = Mathf.Abs( ViewRangeTransform.position.x - transform.position.x );

	}

    public void SetQTETimeStamp()
    {
        _timeStamp = Time.time;
    }

	void Update () 
	{
        var stateInfo = _anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Base Layer.Attack") || stateInfo.IsName("Base Layer.Defend"))
        {
            _sideFlag = true;
        }

        if ( _sideFlag && !stateInfo.IsName("Base Layer.Attack") && !stateInfo.IsName("Base Layer.Defend") )
        {
            Debug.Log("animcaj nie gra");
            _canHeFight = true;
            _sideFlag = false;
            FistCollider1.enabled = false;
            FistCollider2.enabled = false;
            _isDefending = false;
        }

        if (_isHurt)
        {
            //Time.timeScale = 0.3f;

            if (_qteCircle == null)
            {
                //Debug.Log("czas: "+ (Time.time - _timeStamp));
                if ( Time.time - _timeStamp < QTECircleInterval )
                {
                    return;
                }

                Debug.Log("nowe kolko");

                if (_qteCircleIterator < QTECirclePositionsArray.Length)
                {
                    _qteCircle = PutCircle(new Vector3(transform.position.x, transform.position.y + QTECirclePositionsArray[_qteCircleIterator], transform.position.z));
                    _qteCircleIterator++;
                }
                else
                {
                    CancelQTE();
                    Debug.Log("FINISZER.");
                }
            }

            return;

        }

        if ( Input.GetKey(KeyCode.Space) )
        {
            //_anim.SetBool("attack", true);
        }
        else
        {
            //_anim.SetBool("attack", false);
        }

        _playerEnemyDistance = _playerObject.transform.position.x - transform.position.x;

        GetState();

        if(  _playerState == PlayerState.Walking && ( transform.position.x <= _patrolRangeA && _facing == Facing.Left ) ||  _playerState == PlayerState.Walking && ( transform.position.x >= _patrolRangeB && _facing == Facing.Right ) )
        {
            ChangeFacingDirection();
        }
       
	}

    void OnCollisionEnter2D( Collision2D col )
    {
        if ( col.gameObject.tag == "Player" )
        {
	        _rb.velocity = new Vector2(0, _rb.velocity.y);
        }
    }

    void OnTriggerEnter2D( Collider2D col )
    {
        if (col.gameObject.tag == "Wall")
        {
            ChangeFacingDirection();
        }

        if (col.gameObject.tag == "PlayerFist" && _isUnderAttack && !_isDefending )//&& _isUnderAttack && !_isDefending )
        {
            if (!_isDefending)
            {
                Debug.Log("nie broni sie");
            }

            HitFlag.SetActive(!HitFlag.active);

            Debug.Log("enemy health: " + _enemyHealthPoints);
            SetHealth(-10);
            _isUnderAttack = false;

            if (_enemyHealthPoints <= 0)
            {
                Debug.Log("smierc przeciwnika");
                //gameObject.SetActive(false);
                _enemyHealthPoints = EnemyHealthPointsMax;
                Time.timeScale = 1.0f;
                Destroy(gameObject);
            }

            if ( _enemyHealthPoints <= 20 && !_isHurt && !_didQTE)
            {
                _didQTE = true;
                _isHurt = true;
                _playerObject.GetComponent<FightSystem>().IsQTE = true;
                _qteCircle = PutCircle(new Vector3(transform.position.x, transform.position.y + QTECirclePositionsArray[_qteCircleIterator], transform.position.z));
                _qteCircleIterator++;
                _timeStamp = Time.time;
            }
        }
    }

    void SetHealth(int value)
    {
        _enemyHealthPoints += value;
    }

    void ChangeFacingDirection()
    {
        if (_facing == Facing.Right)
        {
            _facing = Facing.Left;
        }
        else
        {
            _facing = Facing.Right;
        }

        transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    void GetState()
    {
        switch( _playerState )
        {
            case PlayerState.Walking:
                Walk(EnemyWalkingSpeed);
                if (Mathf.Abs(_playerEnemyDistance) < _triggerRange)
                {
                    if (_facing == Facing.Left)
                    {
                        if (_playerEnemyDistance < 0 )
                        {
                            _playerState = PlayerState.Running;
                            //Debug.Log("Running");
                        }
                    }
                    else if (_playerEnemyDistance > 0 )
                    {
                        _playerState = PlayerState.Running;
                        //Debug.Log("Running");
                    }
                }

                break;

            case PlayerState.Running:
                Run(EnemyRunningSpeed);
                if (Mathf.Abs(_playerEnemyDistance) <= FightingDeadZone)
                {
                    _playerState = PlayerState.Attacking;
                    //Debug.Log("Attacking , distance: " + Mathf.Abs(_playerEnemyDistance));
                }

                break;

            case PlayerState.Attacking:
                _rb.velocity = new Vector2(0f, _rb.velocity.y);
                Attack();
                if (Mathf.Abs(_playerEnemyDistance) > FightingDeadZone)
                {
                    _playerState = PlayerState.Running;
                    _rb.constraints = RigidbodyConstraints2D.None;
                    _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                    Time.timeScale = 1.0f;
                    //Debug.Log("Running");
                }
                break;
        }
    }

    void Walk( float speed )
    {
        int _directionSpecifier = 1;

        if (_facing == Facing.Left)
        {
            _directionSpecifier *= -1;
        }

        _rb.velocity = new Vector2(speed*_directionSpecifier, _rb.velocity.y);
    }

    void Run(float speed)
    {
        if (_playerEnemyDistance>0)
        {
            _rb.velocity = new Vector2(speed, _rb.velocity.y);
            if ( _facing == Facing.Left )
            {
                ChangeFacingDirection();
            }
            //Debug.Log("Running left");
        }
        else if (_playerEnemyDistance<0)
        {
            _rb.velocity = new Vector2(-speed, _rb.velocity.y);
            if ( _facing == Facing.Right )
            {
                ChangeFacingDirection();
            }
            //Debug.Log("Running left");
        }
    }

    GameObject PutCircle( Vector3 vector )
    {
        GameObject circle = Instantiate(QteButton, vector, Quaternion.identity) as GameObject;
        circle.GetComponent<QTECircleScript>().FatherObject = gameObject;
        circle.GetComponent<QTECircleScript>()._qteType = "Enemy";
        return circle;
    }

    public void CancelQTE()
    {
        _isHurt = false;
        _playerObject.GetComponent<FightSystem>().CancelQTE();
        _qteCircleIterator = 0;
    }

    void Attack()
    {

        float x = Random.Range(0f, 3.0f);

        if (_isDefending)
        {
            //_anim.SetBool("defend", true);
        }

        if ( x < 1.0f && _canHeFight )
        {
            _canHeFight = false;
            FistCollider1.enabled = true;
            FistCollider2.enabled = true;
            _anim.SetBool("attack", true);
        }
    }

    public void Defend()
    {
        if ( _playerState == PlayerState.Attacking )
        {
            _isUnderAttack = true;
            float x = Random.Range(0f, 2.0f);

            if (!_canHeFight)
            {
               // Debug.Log("pownien muc");
            }

            if (x <= 1.0f)
            {
                _canHeFight = false;
                //_anim.SetBool("defend", true);
                //_animatingTime = Time.time;
                _isDefending = true;
                Debug.Log("defend");
            }
        }
        
    }

}
