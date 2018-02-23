using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingControl : MonoBehaviour {

    #region Public Variables
    [Header("Bullet")]
	public Object bulletPrefab;
    public Vector3 bulletOffset;
    public Transform bulletParent;
    #endregion

    #region Mono Methods
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine (transform.position, transform.position + transform.forward);
    }

    
    private void Update() {
        //#if UNITY_EDITOR
        if (Input.GetMouseButtonDown (0))
            Shoot ();
        //#endif
    }
    #endregion


    #region Shooting Methods
    public void Shoot(){
        TurretBullet bullet = ((GameObject)Instantiate(bulletPrefab)).GetComponent<TurretBullet>();
        bullet.transform.position = transform.position + transform.TransformDirection(bulletOffset);
        bullet.Inialise(transform.forward, bulletParent);
    }
    #endregion

}
