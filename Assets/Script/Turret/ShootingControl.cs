using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingControl : MonoBehaviour {

    #region Public Variables
    [Header("Bullet")]
	public Object bulletPrefab;
    public Vector3 bulletOffset;
    public Transform bulletParent;

    [Header ("UI")]
    public Transform recticle;
    public AnimationCurve recticleCurve;
    public float recticleTime = 0.2f;
    #endregion

    #region Private Variables
    private float recticleTimeT = 0;
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

        if (recticleTimeT < recticleTime) {
            float newScale = recticleCurve.Evaluate (recticleTimeT / recticleTime);
            recticle.localScale = new Vector3 (newScale, newScale, 1f);
            recticleTimeT += Time.deltaTime;
        }else if (recticleTimeT > recticleTime) {
            recticleTimeT = recticleTime;
            float newScale = recticleCurve.Evaluate (recticleTimeT / recticleTime);
            recticle.localScale = new Vector3 (newScale, newScale, 1f);
        }
    }
    #endregion


    #region Shooting Methods
    public void Shoot(){
        TurretBullet bullet = ((GameObject)Instantiate(bulletPrefab)).GetComponent<TurretBullet>();
        bullet.transform.position = transform.position + transform.TransformDirection(bulletOffset);
        bullet.Inialise(transform.forward, bulletParent);
        UIManager.main.ReloadBulletUI ();
        recticleTimeT = 0f;
    }
    #endregion

}
