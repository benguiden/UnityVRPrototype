using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakUp : MonoBehaviour {

    public GameObject[] rigidbodies;
    public GameObject[] toDestroy;
    public GameObject[] toDeactivate;
    public float deactivateRigidbodiesTime = 3f;

    private Coroutine activateCoroutine;

    public void Activate(Vector3 forcePosition, float force) {
        if (activateCoroutine == null)
            activateCoroutine = StartCoroutine(IActivate(forcePosition, force));
    }

    private IEnumerator IActivate(Vector3 forcePosition, float force) {
        Component[] components = GetComponents (typeof (Behaviour));

        foreach (Component component in components) {
            ((Behaviour)component).enabled = false;
        }

        Vector3[] rigibodyPositions = new Vector3[rigidbodies.Length];
        Rigidbody[] rigidbodyComponents = new Rigidbody[rigidbodies.Length];

        for (int i = 0; i < rigidbodies.Length; i++) {
            rigidbodyComponents[i] = rigidbodies[i].AddComponent<Rigidbody> ();
            rigidbodyComponents[i].collisionDetectionMode = CollisionDetectionMode.Discrete;
            Collider objectCollider = rigidbodies[i].GetComponent<Collider> ();
            if (objectCollider != null)
                objectCollider.enabled = true;
            rigibodyPositions[i] = rigidbodies[i].transform.position + new Vector3 (0f, 3f, 0f);
        }

        foreach (GameObject objectToDestroy in toDestroy) {
            Destroy (objectToDestroy);
        }

        yield return null;

        for (int i = 0; i < rigidbodies.Length; i++) {
            rigidbodies[i].transform.position = rigibodyPositions[i];
        }

        foreach (GameObject objectToDeactivate in toDeactivate) {
            objectToDeactivate.SetActive (false);
        }

        yield return null;

        foreach (Rigidbody rigidbodyComponent in rigidbodyComponents) {
            rigidbodyComponent.AddExplosionForce (force, forcePosition, 5f);
        }

        yield return new WaitForSeconds (deactivateRigidbodiesTime);

        foreach (Rigidbody rigidbodyComponent in rigidbodyComponents) {
            rigidbodyComponent.isKinematic = true;
        }
    }

}
