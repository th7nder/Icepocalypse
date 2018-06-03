using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour {

    #region Variables
    public float _explosionTimer = 0;
    private AudioSource _source;
    public AudioClip _explosionSound;
    private SpriteRenderer _spriteRenderer;
    #endregion
    
    #region Start&Update
    // Use this for initialization
    void Start ()
    {
        _source = GetComponent<AudioSource>();
        _source.PlayOneShot(_source.clip);
        _spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        _explosionTimer += Time.deltaTime;
        if (_explosionTimer >= 10) Destroy(gameObject);
        else if (_explosionTimer >= 1.14) _spriteRenderer.enabled = false;
        
	}
    #endregion
}
