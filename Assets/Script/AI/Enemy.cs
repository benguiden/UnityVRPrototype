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

    [Header("Behaviours")]
    public PathFollower pathFollowing;

    [HideInInspector]
    public bool isAlive = true;
    #endregion

    #region Private Variables
    //UI
    private RectTransform uiObject;
    private Image uiImage;
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
        uiObject = ((GameObject)Instantiate (EnemyManager.main.uiPrefab)).GetComponent<RectTransform> ();
        uiImage = uiObject.GetComponent<Image> ();
        uiImage.color = new Color (1f, 1f, 1f, 0f);
        uiObject.SetParent (EnemyManager.main.uiParent);
        uiObject.localScale = new Vector3 (1f, 1f, 1f);
        uiObject.anchoredPosition3D = Vector3.zero;
        uiObject.gameObject.SetActive (false);
        StartCoroutine (UpdateUIPosition ());
    }

    private IEnumerator UpdateUIPosition() {
        yield return new WaitForSeconds (Random.Range (0f, 1f / EnemyManager.main.updateUIRate));
        while (uiObject != null) {
            if ((Vector3.Distance(transform.position, Camera.main.transform.position) <= EnemyManager.main.uiActivateDistance) && (isAlive)) {
                if (!uiObject.gameObject.activeSelf)
                    uiObject.gameObject.SetActive (true);
                Vector3 newUIPosition = EnemyManager.main.canvas.worldCamera.WorldToScreenPoint (transform.position + new Vector3 (0f, 2f, 0f));
                newUIPosition *= 1280f / Screen.width;
                float newAlpha = 1f;
                float displacement = ((Vector2)newUIPosition - uiObject.anchoredPosition).magnitude;
                if (displacement > 0f)
                    newAlpha = 1f / displacement;
                newAlpha = Mathf.Clamp01 (newAlpha * EnemyManager.main.uiAlphaAmount);
                uiImage.color = new Color (1f, 1f, 1f, newAlpha);
                uiObject.anchoredPosition = Vector2.Lerp (uiObject.anchoredPosition, (Vector2)newUIPosition, 1f - EnemyManager.main.uiUpdateSmoothness);
            } else {
                if (uiObject.gameObject.activeSelf)
                    uiObject.gameObject.SetActive (false);
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
            }
        }
    }
    #endregion

}
