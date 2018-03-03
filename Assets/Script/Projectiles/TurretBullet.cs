using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBullet : MonoBehaviour {

    public float lifeSpan = 10f;
    public float speed = 20f;
    public float spawnTime = 0.25f;
    public float breakForce = 10f;
    public bool hit = false;
    public AudioClip hitClip;

    private new Collider collider;
    private AudioSource audioSource;
    private new Renderer renderer;

    #region Mono Methods
    private void Awake() {
        collider = GetComponent<Collider> ();
        audioSource = GetComponent<AudioSource> ();
        renderer = GetComponent<Renderer> ();
    }

    private void Start() {
        StartCoroutine(SpawnGrow());
    }

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

    #region Visual Methods
    private IEnumerator SpawnGrow() {
        Vector3 originalScale = transform.localScale;
        float time = 0f;
        while (time < spawnTime) {
            float scale = time / spawnTime;
            transform.localScale = originalScale * scale;

            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;
    }
    #endregion

    public void Deactivate() {
        StartCoroutine (IDeactivate ());
    }

    private IEnumerator IDeactivate() {
        collider.enabled = false;
        renderer.enabled = false;
        audioSource.clip = hitClip;
        audioSource.loop = false;
        audioSource.Play ();
        while (audioSource.isPlaying) {
            yield return new WaitForSeconds (0.25f);
        }
        gameObject.SetActive (false);
    }

}
