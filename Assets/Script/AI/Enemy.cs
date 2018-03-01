using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {
    #region Public Visible Variables
    [Header("Physics")]
    public float mass = 1f;
    public Vector3 velocity;
    public Vector3 acceleration;
    public float maxSpeed = 10f;

    [Header ("Visuals")]
    public Transform torso;

    [Header("Behaviours")]
    public PathFollower pathFollowing;

    [HideInInspector]
    public bool isAlive = true;
    #endregion

    #region Private Variables
    //UI
    private Transform uiObject;
    private SpriteRenderer uiRenderer;
    #endregion

    #region Mono Methods
    private void Awake() {
        pathFollowing.SetEnemy(this);

        AwakeBehaviours();
    }

    private void Start() {
        InitaliseUI ();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + velocity * 2f);

        GizmosBehaviours();
    }

    private void OnValidate() {
        if (mass < float.Epsilon)
            mass = float.Epsilon;
        if (maxSpeed < 0f)
            maxSpeed = 0f;
    }

    private void Update() {
        UpdateBehaviours();
        UpdatePhysics();
        UpdateUIPosition ();
    }
    #endregion

    #region Behaviour Methods
    private void AwakeBehaviours() {
        pathFollowing.Awake();
    }

    private void UpdateBehaviours() {
        pathFollowing.Update(); 
    }

    private void GizmosBehaviours() {
        pathFollowing.OnDrawGizmos();
    }
    #endregion

    #region Physics Methods
    public void AddForwardForce(float forwardForce) {
        acceleration += transform.forward * forwardForce * Time.deltaTime;
    }
    public void AddForce(Vector3 addedForce) {
        acceleration += addedForce * mass * Time.deltaTime;
    }
    public void AddForwardAcceleration(float forwardAcceleration) {
        acceleration += transform.forward * forwardAcceleration * Time.deltaTime;
    }
    public void AddAcceleration(Vector3 addedAcceleration) {
        acceleration += addedAcceleration * Time.deltaTime;
    }

    private void UpdatePhysics() {
        velocity += acceleration;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        acceleration = Vector3.zero;

        transform.position += velocity * Time.deltaTime;
    }
    #endregion

    #region UI Methods
    private void InitaliseUI() {
        uiObject = ((GameObject)Instantiate (EnemyManager.main.uiPrefab)).transform;
        uiRenderer = uiObject.GetComponent<SpriteRenderer> ();
        uiRenderer.color = new Color (1f, 1f, 1f, 0f);
        uiObject.SetParent (EnemyManager.main.uiParent);
        uiObject.position = Camera.main.transform.position;
        uiObject.localEulerAngles = Vector3.zero;
        uiObject.gameObject.SetActive (false);
        StartCoroutine (UpdateUIPosition ());
    }

    private IEnumerator UpdateUIPosition() {
        yield return new WaitForSeconds (Random.Range (0f, 1f / EnemyManager.main.updateUIRate));
        while (uiObject != null) {
            float distanceToEnemy = Vector3.Distance (transform.position + new Vector3 (0f, EnemyManager.main.uiYOffset, 0f), Camera.main.transform.position);
            if ((distanceToEnemy <= EnemyManager.main.uiActivateDistance) && (isAlive)) {
                if (!uiObject.gameObject.activeSelf)
                    uiObject.gameObject.SetActive (true);

                Vector3 futurePosition = transform.position;
                if (EnemyManager.main.anticipatePosition) {
                    float timeToEnemy = distanceToEnemy / EnemyManager.main.bulletSpeedForUI;
                    futurePosition += (velocity * timeToEnemy);
                }

                Vector3 newPosition = ((futurePosition + new Vector3 (0f, EnemyManager.main.uiYOffset, 0f)) - Camera.main.transform.position);
                newPosition = Camera.main.transform.position + (newPosition.normalized * EnemyManager.main.uiCameraDistance);

                uiObject.transform.position = Vector3.Lerp (uiObject.transform.position, newPosition, EnemyManager.main.uiUpdateSmoothness);

                float newAlpha = 1f - (distanceToEnemy / EnemyManager.main.uiActivateDistance);
                newAlpha = Mathf.Lerp (uiRenderer.color.a, newAlpha, 0.25f);

                uiRenderer.color = new Color (1f, 1f, 1f, newAlpha);
                uiObject.LookAt (Camera.main.transform);

            } else {
                if (uiObject.gameObject.activeSelf) {
                    uiObject.gameObject.SetActive (false);
                    uiRenderer.color = new Color (1f, 1f, 1f, 0f);
                }
            }
            
            yield return new WaitForSeconds (1f / EnemyManager.main.updateUIRate);
        }
    }
    #endregion


    #region Messages
    private void OnTriggerEnter(Collider c) {
        if (enabled) {
            if (c.gameObject.tag == "Projectile") {
                isAlive = false;
                Vector3 forcePosition = new Vector3(c.transform.position.x, 0f, c.transform.position.z);
                GetComponent<BreakUp>().Activate(forcePosition, c.gameObject.GetComponent<TurretBullet>().breakForce);
                uiObject.gameObject.SetActive (false);
                SpawnDestroyVFX (c.transform.position);
                Destroy (c.gameObject);
            }
        }
    }

    private void SpawnDestroyVFX(Vector3 spawnPosition) {
        for (int i=0; i< EnemyManager.main.destroyVFX.Length; i++){
            Transform vfxTransform = ((GameObject)Instantiate (EnemyManager.main.destroyVFX[i])).transform;
            if ((i == 0) && (torso != null)) {
                vfxTransform.SetParent (torso);
                vfxTransform.localPosition = Vector3.zero;
            } else {
                vfxTransform.SetParent (transform);
                vfxTransform.position = spawnPosition;
            }
        }
    }
    #endregion

}
