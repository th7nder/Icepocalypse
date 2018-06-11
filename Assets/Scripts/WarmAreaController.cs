using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Scripts.Variables;

public class WarmAreaController : MonoBehaviour {
    public GameEvent WarmAreaEnter;
    public GameEvent WarmAreaExit;

	public void OnTriggerEnter2D(Collider2D collision)
	{
        WarmAreaEnter.Raise();
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
        WarmAreaExit.Raise();
	}

}
