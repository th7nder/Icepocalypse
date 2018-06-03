using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Scripts.Variables;
public class ExplosiveBarrelController : MonoBehaviour
{

    #region Variables
    public GameObject Explosion;
    public GameEvent BarrelExplosionEvent;
    #endregion

    #region Collisions
    void OnCollisionEnter2D(Collision2D col)
    {
        //BarrelExplosionEvent.Raise();
        Instantiate(Explosion, col.gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    #endregion
}
