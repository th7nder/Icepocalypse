using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Scripts.Variables;

public class FreezingController : MonoBehaviour {
    public float FreezeTime = 30.0f;
    public float WarmMultiplier = 2.0f;
    public Camera MainCamera;

    public GameEvent FreezeEvent;

    private FrostEffect frostEffect;
    float _freezeLevel;
    bool _freezed;
    bool _inWarmArea;

	void Start()
	{
        _freezeLevel = 0;
        _freezed = false;
        _inWarmArea = false;
        frostEffect = MainCamera.GetComponent<FrostEffect>();
	}

	void Update()
	{
        if (_freezed)
            return;
        

        if (_freezeLevel > FreezeTime)
        {
            _freezed = true;
            Freeze();
        } 
        else if (_inWarmArea)
        {
            if (_freezeLevel > 0)
            {
                _freezeLevel -= Time.deltaTime * WarmMultiplier;
            }
            else
            {
                _freezeLevel = 0;
            }
                
        } 
        else 
        {
            _freezeLevel += Time.deltaTime;
        }

        frostEffect.FrostAmount = _freezeLevel / FreezeTime;

    }

    void Freeze()
    {
        FreezeEvent.Raise();
    }

    public void OnWarmAreaEnter()
    {
        _inWarmArea = true;
    }

	public void OnWarmAreaExit()
    {
        _inWarmArea = false;
    }







}
