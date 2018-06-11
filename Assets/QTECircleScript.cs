using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTECircleScript : MonoBehaviour {

    public float LifeTime = 1.0f;
    private float _startTime;

    [HideInInspector]
    public string _qteType;

    [SerializeField]
    public GameObject FatherObject;

    private Button _button;

	// Use this for initialization
	void Start ()
    {
        _startTime = Time.time;
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnMouseUp);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (LifeTime == 0f)
            return;

        if ( Time.time - _startTime >= LifeTime )
        {
            if (_qteType == "Enemy")
            {
                FatherObject.GetComponent<EnemyController>().CancelQTE();
            }
            else if (  _qteType == "Dog")
            {
                Time.timeScale = 1.0f;
            }

            Destroy(gameObject);
        }
	}

    void OnMouseUp()
    {
        Debug.Log("klik");

        if ( _qteType == "Enemy" )
        {
            FatherObject.GetComponent<EnemyController>().SetQTETimeStamp();
        }
        else if (_qteType == "Dog")
        {
            FatherObject.GetComponent<WildDogScript>().ClickedTheCircle = true;
            Time.timeScale = 1.0f;
        }

        if (LifeTime != 0f)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<FightSystem>().ClickedTheCircle = true;
            Destroy(gameObject);
        }

    }
}
