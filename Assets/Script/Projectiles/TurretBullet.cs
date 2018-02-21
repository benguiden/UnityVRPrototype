using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBullet : MonoBehaviour {

    public float lifeSpan = 10f;
    public float speed = 20f;

    public void Inialise(Vector3 bulletDirection){
        transform.forward = bulletDirection;
    }

}
