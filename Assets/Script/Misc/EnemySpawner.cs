using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public PathDatabase pathDatabase;
    public int[] pathIndexes;

    public Vector2 spawnGap = new Vector2 (2f, 4f);

    public SpawnType[] spawnTypes;

    #region Mono Methods
    private void Start() {
        StartCoroutine(RespawnAutomatically());
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            Trigger(1);
        }
    }
    #endregion

    #region Spawning Methods
    private IEnumerator RespawnAutomatically() {
        float time = 5f;
        while (enabled) {
            while ((time > 0f) && (enabled)) {
                time -= Time.deltaTime;
                yield return null;
            }
            if (enabled) {
                Trigger(1);
                time = Mathf.Lerp (spawnGap.x, spawnGap.y, Random.value);
            }
        }
    }

    public void Trigger(int amount) {
        SpawnType[] waveSpawns = GetWaveSpawns();

        for (int a=0; a<amount; a++) {
            Object prefabToSpawn = GetPrefabByChance(waveSpawns);
            if (prefabToSpawn!= null) {
                Transform newEnemy = ((GameObject)Instantiate(prefabToSpawn, EnemyManager.main.enemyParent)).transform;
                Vector3 newPosition = transform.position;
                newPosition.x += Random.Range (-1f, 1f);
                newPosition.z += Random.Range (-1f, 1f);
                newEnemy.position = newPosition;
                Enemy newEnemyStats = newEnemy.GetComponent<Enemy> ();
                newEnemyStats.pathFollowing.pathDatabase = pathDatabase;
                newEnemyStats.pathFollowing.pathIndex = (uint)pathIndexes[Random.Range (0, pathIndexes.Length)];
                newEnemyStats.pathFollowing.RefreshPath ();
            }
        }
    }

    private SpawnType[] GetWaveSpawns() {
        List<SpawnType> waveSpawns = new List<SpawnType> ();
        for (int i = 0; i < spawnTypes.Length; i++) {
            if (GameManager.main.wave >= spawnTypes[i].enteringWave) {
                waveSpawns.Add (spawnTypes[i]);
            }
        }
        return waveSpawns.ToArray ();
    }

    private Object GetPrefabByChance(SpawnType[] spawns) {
        if (spawns.Length > 0) {
            float sum = 0f;
            int index = -1;
            for (int i = 0; i < spawns.Length; i++) {
                sum += spawns[i].chance;
            }
            sum = Random.Range(0f, sum);
            while (sum >= 0f) {
                index++;
                sum -= spawns[index].chance;
            }
            return spawns[index].prefab;
        } else {
            return null;
        }
    }
    #endregion

    #region Structs
    [System.Serializable]
    public struct SpawnType{
        public Object prefab;
        public int enteringWave;
        public float chance;
    }
    #endregion

}
