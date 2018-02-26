using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    #region Public Visible Variables
    [Header("Physics")]
    public float mass = 1f;
    public Vector3 velocity;
    public Vector3 acceleration;
    public float maxSpeed = 10f;

    [Header("Behaviours")]
    public PathFollower pathFollowing;
    #endregion

    #region Private Variables
    
    #endregion

    #region Mono Methods
    private void Awake() {
        pathFollowing.SetEnemy(this);

        AwakeBehaviours();
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

    #region Messages
    private void OnTriggerEnter(Collider c) {
        Debug.Log(c.gameObject.name);
        if (c.gameObject.tag == "Projectile") {
            Destroy(gameObject);
        }
    }
    #endregion

}
