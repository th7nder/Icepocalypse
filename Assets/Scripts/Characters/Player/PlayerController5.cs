﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;


public class PlayerController5 : MonoBehaviour
{
    public UnityEvent TubeStopperDestroy;
    public float JumpForce = 18;
    public float WallReflectionForce = 17;
    public float MoveSpeed = 10;
    public float _wallTimer = 0;
    public bool FacingRight = true;
    float _moveDirection = 0;
    int counter = 0;

    private bool _tubeIgnore = false;
    private bool _hasBounced = false;


    public enum PlayerState
    {
        Inert,
        Grounded,
        Attacking,
        WallClimbing,
        HandBarring,
        WallHugging,
        ObstacleClimbing_L,
        ObstacleClimbing_R,
        TubeSliding,
        TubeStopped,
        Sloping
    }

    public PlayerState CurrentState = PlayerState.Inert;

    public LayerMask Ground;
    public LayerMask Wall;
    public LayerMask Tube;
    public LayerMask Obstacle_R;
    public LayerMask Obstacle_L;
    public LayerMask HandBar;
    public LayerMask Stopper;
    public LayerMask Slope;



    private Rigidbody2D _rigidbody;
    private Collider2D _colliderWhole;
    private Collider2D _colliderBody;
    private Collider2D _colliderLegs;
    


    // Use this for initialization
    void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _colliderBody = gameObject.GetComponents<Collider2D>()[2];
        _colliderLegs = gameObject.GetComponents<Collider2D>()[1];
        _colliderWhole = gameObject.GetComponents<Collider2D>()[0];
        Ground = LayerMask.GetMask("Ground");
        Wall = LayerMask.GetMask("Wall");
        Tube = LayerMask.GetMask("Tube");
        Stopper = LayerMask.GetMask("TubeStopper");
        HandBar = LayerMask.GetMask("Handbar");
        Slope = LayerMask.GetMask("Slope");
    }


    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            // if(coll.gameObject.GetComponent<Collider2D> == _colliderBody) { }
            Debug.Log(coll.gameObject.name);
            counter++;
            if (CurrentState != PlayerState.Grounded) onWallImpact();
        }
        else if (coll.gameObject.layer == LayerMask.NameToLayer("Tube"))
        {
            Debug.Log("TubeEnter" + counter);
            counter++;
            onTubeImpact();
        }
        else if (coll.gameObject.layer == LayerMask.NameToLayer("Slope"))
        {
            Debug.Log("SlopeEnter" + counter);
            counter++;
            onSlopeImpact();
        }
        else if (coll.gameObject.layer == LayerMask.NameToLayer("Stopper"))
        {
            Debug.Log("StopperEnter" + counter);
            counter++;
            onTubeStopperImpact();
        }
        else if (coll.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("WallEnter"+counter);
            counter++;
            _rigidbody.gravityScale = 10;
            _wallTimer = 0;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
    


    void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            //Debug.Log("GroundExit" + counter);
            counter++;
            if (CurrentState == PlayerState.WallHugging)
            {
                
                _rigidbody.velocity = new Vector2(0, 0);
                _rigidbody.gravityScale = 0;
            }
        }
        else if (coll.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Debug.Log("WallExit" + counter);
            counter++;
            _rigidbody.gravityScale = 10;
            _wallTimer = 0;
            _hasBounced = false;
        }
        else if (coll.collider.gameObject.layer == LayerMask.NameToLayer("Stopper"))
        {
           // Debug.Log("StopperExit" + counter);
            counter++;
            _tubeIgnore = false;
            _rigidbody.gravityScale = 10;
        }
        else if (coll.collider.gameObject.layer == LayerMask.NameToLayer("Slope"))
        {
            Debug.Log("SlopeExit" + counter + " " + CurrentState);
            counter++;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            _rigidbody.gravityScale = 10;
            
        }

    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P)) Debug.Break();

       


        if (Physics2D.IsTouchingLayers(_colliderLegs, Ground)) CurrentState = PlayerState.Grounded;
        else if (Physics2D.IsTouchingLayers(_colliderBody, Tube) && !_tubeIgnore)
        {
            if (Physics2D.IsTouchingLayers(_colliderBody, Stopper)) CurrentState = PlayerState.TubeStopped;
            else CurrentState = PlayerState.TubeSliding;
        }

        else if (Physics2D.IsTouchingLayers(_colliderBody, Wall)) CurrentState = PlayerState.WallHugging;
        else if (Physics2D.IsTouchingLayers(_colliderBody, HandBar)) CurrentState = PlayerState.HandBarring;
        else if (Physics2D.IsTouchingLayers(_colliderWhole, Slope)) CurrentState = PlayerState.Sloping;
        else CurrentState = PlayerState.Inert;




        switch (CurrentState)
        {


            case PlayerState.Grounded:

                movement();

                if (Input.GetKeyDown(KeyCode.RightArrow)) SetFacingRight(true);
                if (Input.GetKeyDown(KeyCode.LeftArrow)) SetFacingRight(false);
                if (Input.GetKeyDown(KeyCode.DownArrow)) OnKeyDown();
                if (Input.GetKeyDown(KeyCode.Space)) onKeySpace();

               break;


            case PlayerState.Inert:


               break;

            case PlayerState.WallHugging:

               manageWallTimer();

                if ((Input.GetKeyDown(KeyCode.Space))) onKeySpace();

                break;

            case PlayerState.Sloping:

                transform.rotation = Quaternion.Euler(0, 0, -30);

                if ((Input.GetKeyDown(KeyCode.Space))) onKeySpace();

                break;

            case PlayerState.TubeSliding:

                if (_rigidbody.velocity.y <= -3) _rigidbody.velocity = new Vector2(0, -3);

                break;

            case PlayerState.TubeStopped:

                if (Input.GetKeyDown(KeyCode.DownArrow)) OnKeyDown();

                break;




        }
    }

    public void OnKeyDown()
    {
        switch (CurrentState)
        {
            case PlayerState.TubeStopped:
                _rigidbody.gravityScale = 10;
                TubeStopperDestroy.Invoke();
                _tubeIgnore = true;
                break;

            default:
                StopMovement();
                break;
        }
    }


    private void movement()
    {
        _rigidbody.velocity = new Vector2(MoveSpeed * _moveDirection, _rigidbody.velocity.y);
    }

    public void StopMovement()
    {
        _moveDirection = 0;
    }

    public void onKeySpace()
    {
        if (CurrentState == PlayerState.Grounded || CurrentState == PlayerState.Sloping) _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpForce);
        else if (CurrentState == PlayerState.WallHugging && !_hasBounced)
        {
            if (!_hasBounced) _hasBounced = true;
            if (FacingRight) SetFacingRight(false);
            else SetFacingRight(true);
            _rigidbody.velocity = new Vector2(_moveDirection * WallReflectionForce, JumpForce);
        }
    }

    public void SetFacingRight(bool _facingRight)
    {
        if (_facingRight)
        {
            FacingRight = true;
            if (transform.localScale.x < 0) transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            _moveDirection = 1;
        }
        else
        {
            FacingRight = false;
            if (transform.localScale.x > 0) transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            _moveDirection = -1;
        }

    }


    private void manageWallTimer()
    {
        _wallTimer += Time.deltaTime;

        if (_wallTimer >= 0.15)
        {
            _rigidbody.gravityScale = 3;
            if (_rigidbody.velocity.y <= -4) _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, -4);
        }

    }

    private void onSlopeImpact()
    {

        //_rigidbody.constraints = RigidbodyConstraints2D.None;
        SetFacingRight(true);
        _rigidbody.gravityScale = 2;
        



    }

    private void onTubeStopperImpact()
    {
        Debug.Log("onTubeStopImpact");
        _rigidbody.velocity = new Vector2(0, 0);
        _rigidbody.gravityScale = 0;

    }

    private void onTubeImpact()
    {
        _rigidbody.velocity = new Vector2(0, 0);
        _rigidbody.gravityScale = 1;


    }

    private void onWallImpact()
    {
        
        _rigidbody.velocity = new Vector2(0, 0);
        _rigidbody.gravityScale = 0;
        

    }

}



