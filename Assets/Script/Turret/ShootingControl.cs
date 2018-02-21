using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingControl : MonoBehaviour {
	
	public Object bulletPrefab;
    public Vector3 bulletOffset;

    public void OnDrawGizmos() {

    }

	public void Shoot(){
        TurretBullet bullet = ((GameObject)Instantiate(bulletPrefab)).GetComponent<TurretBullet>();
        bullet.transform.position = transform.position + transform.TransformDirection(bulletOffset);
        bullet.Inialise(transform.forward);
    }

}
