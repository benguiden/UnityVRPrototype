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
    public float despawnTime = 5f;

    [Header ("Behaviours")]
    public Type type;
    public bool stateWalking = true;
    public PathFollower pathFollowing;

    [HideInInspector]
    public bool isAlive = true;
    #endregion

    #region Private Variables
    //UI
    private Transform uiObject;
    private SpriteRenderer uiRenderer;
    private Animator animator;
    private BreakUp breakUp;
    #endregion

    #region Mono Methods
    private void Awake() {
        pathFollowing.SetEnemy(this);
        animator = GetComponent<Animator>();
        AwakeBehaviours();
    }

    private void Start() {
        InitaliseUI ();
        breakUp = GetComponent<BreakUp> ();
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
        if (stateWalking) {
            UpdateBehaviours();
            Vector3 cameraPos = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);
            if (Vector3.Distance(cameraPos, transform.position) <= GameManager.main.shieldRadius) {
                BeginAttack();
            }
        } else {
            AttackBehaviour();
        }
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

    private void AttackBehaviour() {
        
    }

    private void BeginAttack() {
        if (stateWalking) {
            animator.SetBool("isRunning", false);
            animator.SetBool("isPunching", true);
            stateWalking = false;
            velocity = Vector3.zero;
            transform.forward = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z) - transform.position;
        }
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
                TurretBullet bullet = c.gameObject.GetComponent<TurretBullet> ();
                if (!bullet.hit) {
                    isAlive = false;
                    Vector3 forcePosition = new Vector3 (c.transform.position.x, 0f, c.transform.position.z);
                    breakUp.Activate (forcePosition, c.gameObject.GetComponent<TurretBullet> ().breakForce);
                    uiObject.gameObject.SetActive (false);
                    SpawnDestroyVFX (c.transform.position);
                    StartCoroutine (Despawn ());
                    bullet.hit = true;
                    Destroy (c.gameObject);
                }
            }
        }
    }

    private void SpawnDestroyVFX(Vector3 spawnPosition) {
        Object[] vfxList;
        if (type == Type.Robot)
            vfxList = EnemyManager.main.destroyVFXRobot;
        else
            vfxList = EnemyManager.main.destroyVFXAlien;

        for (int i=0; i< vfxList.Length; i++){
            Transform vfxTransform = ((GameObject)Instantiate (vfxList[i])).transform;
            if ((i == 0) && (torso != null)) {
                vfxTransform.SetParent (torso);
                vfxTransform.localPosition = Vector3.zero;
            } else {
                vfxTransform.SetParent (transform);
                vfxTransform.position = spawnPosition;
            }
        }
    }

    private IEnumerator Despawn() {
        yield return new WaitForSeconds (despawnTime);

        if (breakUp.rigidbodies.Length != 0) {
            Renderer[] limbRenderers = new Renderer[breakUp.rigidbodies.Length];
            for (int i = 0; i < breakUp.rigidbodies.Length; i++) {
                limbRenderers[i] = breakUp.rigidbodies[i].GetComponent<Renderer> ();
                if (limbRenderers[i] != null) {
                    if (type == Type.Robot)
                        limbRenderers[i].material = EnemyManager.main.transparentMatRobot;
                    else
                        limbRenderers[i].material = EnemyManager.main.transparentMatAlien;
                }
                yield return null;
            }
            Color limbColour;
            float time = 1f;
            while (time > 0f) {
                time -= Time.deltaTime;
                
                for (int i=0; i<limbRenderers.Length; i++) {
                    if (limbRenderers[i] != null) {
                        limbColour = limbRenderers[i].material.color;
                        limbColour.a = time;
                        limbRenderers[i].material.color = limbColour;
                    }
                }

                yield return null;
            }
            Destroy (gameObject);
        }
    }
    #endregion

    public enum Type
    {
        Robot,
        Alien
    }

}
