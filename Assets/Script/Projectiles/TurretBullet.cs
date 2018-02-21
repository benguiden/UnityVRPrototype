using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBullet : MonoBehaviour {

    public float lifeSpan = 10f;
    public float speed = 20f;

    #region Mono Methods
    private void Update() {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
    #endregion

    #region Bullet Methods
    public void Inialise(Vector3 bulletDirection, Transform bulletParent){
        transform.forward = bulletDirection;

        if (bulletParent != null)
            transform.parent = bulletParent;

        StartCoroutine (KillAfterTime (lifeSpan));
    }

    private IEnumerator KillAfterTime(float timeToKill) {
        yield return new WaitForSeconds (timeToKill);
        gameObject.SetActive (false);
        ObjectDisposer.main.DisposeOf (this.gameObject);
    }
    #endregion


}
