using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Variables;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour
{
	public GameObject Player;


	private Vector2 _startPosition;
	private bool _touching = false;
    private PlayerControllerExperimental _playerControllerExperimental;
    private FightSystem _fightSystem;
	
    public void Start() 
    {
        _playerControllerExperimental = Player.GetComponent<PlayerControllerExperimental>();
        _fightSystem = Player.GetComponent<FightSystem>();
    }
	public void OnLeft()
	{
		_playerControllerExperimental.OnLeftDirection();
	}

	public void OnRight()
	{
		_playerControllerExperimental.OnRightDirection();
	}

    public void OnTop() {
        _playerControllerExperimental.OnTopDirection();
    }

    public void OnDown() 
    {
        _playerControllerExperimental.OnDownDirection();
    }

    public void OnAttack()
    {
        _playerControllerExperimental.StopMovement();
        _fightSystem.ClickedAttack = true;
    }

	private void CheckGesture(Vector2 distance, Vector2 position, int fingerId = -1)
	{
        if (EventSystem.current.IsPointerOverGameObject(fingerId))
            return;

        if (position.x > Screen.width / 2)
        {
            OnRight();
        }
        else
        {
            OnLeft();
        }
	}


	private void Update()
	{
	#if UNITY_EDITOR 
		if (Input.GetButtonDown("Fire1"))
		{
			_touching = true;
			_startPosition = Input.mousePosition;
		}

		if (Input.GetButtonUp("Fire1"))
		{
			if (_touching)
			{
				Vector2 distance;
				distance.x = Input.mousePosition.x - _startPosition.x;
				distance.y = Input.mousePosition.y - _startPosition.y;
				CheckGesture(distance, Input.mousePosition);
			}
			_touching = false;
		}
	#else
		if(Input.touchCount != 1) return;
		var touch = Input.touches.First();
		switch (touch.phase)
		{
			case TouchPhase.Began:
				_startPosition = touch.position;
                CheckGesture(_startPosition, _startPosition, touch.fingerId);
				break;
			
			case TouchPhase.Ended:
				//Vector2 distance = touch.position - _startPosition;
				//CheckGesture(distance, touch.position);
				break;
        }
	#endif
	}
}
