using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
    public class PathFollower{

        //Change To Job System At Some Point

        #region Public Variables
        [Header ("Path")]
        public PathDatabase pathDatabase;
        public uint pathIndex = 0;
        public Vector3 pathPositionOffset;

        [Header ("Movement")]
        public uint pointIndex;
        public float followSpeed = 5f;
        public float pointChangeDistance = 5f;
        public float turnspeed = 1f;
        #endregion

        #region Private Variables
        private Enemy enemy;
        private AIPath path = null;
        public AIPath Path { get { return path; } }
        #endregion

        #region Constructors
        public PathFollower(Enemy _enemy){

        }
        #endregion

        #region Boid Methods
        public void Awake() {
            RefreshPath ();
        pathPositionOffset = new Vector3 (Random.Range (-2f, 2f), 0f, Random.Range (-2f, 2f));
        }

        public void Update() {
            if (path != null) {
                CheckNextPoint ();
                LookAtPoint ();
                MoveTowardsPoint ();
            }
        }

    public void OnDrawGizmos() {
        if (path != null) {
            if (pointIndex < path.points.Count) {
                /*Gizmos.color = new Color (path.color.r, path.color.g, path.color.b, path.color.a / 2f);
                int lastPoint = (int)pointIndex - 1;
                if (lastPoint < 0)
                    lastPoint = path.points.Count - 1;
                //Gizmos.DrawWireCube (path.points[lastPoint], new Vector3 (2f, 2f, 2f));
                Gizmos.DrawLine (path.points[lastPoint] + pathPositionOffset, targetPosition);*/
                Vector3 targetPosition = GetOffsetTarget ();
                Gizmos.color = path.color;
                Gizmos.DrawWireCube (targetPosition, new Vector3 (2f, 2f, 2f));

                //Vector3 lookTarget = path.points[(int)pointIndex] + pathPositionOffset;
                //lookTarget -= enemy.transform.position;
                //Gizmos.DrawLine (enemy.transform.position, enemy.transform.position + (lookTarget.normalized * 10f));
            }
        }
    }
    #endregion

        #region Movement Methods
        public void RefreshPath() {
            if (pathDatabase == null) {
                Debug.LogWarning ("Warning: Path Database reference on " + enemy.gameObject.name + " set to null.\n");
                return;
            } else {
                if (pathIndex >= pathDatabase.paths.Count) {
                    Debug.LogError ("Error: Path index out of range for " + enemy.gameObject.name + " game object.\n");
                    Debug.Break ();
                    return;
                } else {
                    path = pathDatabase.paths[(int)pathIndex];
                }
            }
        }

        private void CheckNextPoint() {
            if (Vector3.Distance(enemy.transform.position, GetOffsetTarget ()) <= pointChangeDistance) {
                pointIndex = (uint)((pointIndex + 1) % path.points.Count);
            }
        }

    private Vector3 GetOffsetTarget() {
        /*Vector3 pathPosForward = (path.points[(int)pointIndex] - path.points[((int)pointIndex + 1) % path.points.Count]).normalized;
        float offsetAngle = Mathf.Atan2 (pathPosForward.z, pathPosForward.x);
        Vector3 offset = new Vector3 (Mathf.Cos (offsetAngle), 0f, Mathf.Sin (offsetAngle));
        Debug.Log (offset);
        offset = new Vector3 (pathPositionOffset.x * offset.magnitude, 0f, pathPositionOffset.z * offset.magnitude);*/
        return path.points[(int)pointIndex] + pathPositionOffset;
    }

    private void LookAtPoint() {
        Vector3 lookTarget = GetOffsetTarget ();
        lookTarget -= enemy.transform.position;
        lookTarget.Normalize ();
        enemy.transform.rotation = Quaternion.Lerp (enemy.transform.rotation, Quaternion.LookRotation (lookTarget), turnspeed * Time.deltaTime * 60f);
    }

        private void MoveTowardsPoint() {
        enemy.velocity = enemy.transform.forward * followSpeed;
        }
        #endregion

        #region Data Methods
        public void SetEnemy(Enemy _enemy) {
            enemy = _enemy;
        }
        #endregion


    }


