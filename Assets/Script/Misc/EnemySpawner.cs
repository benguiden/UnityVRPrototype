using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public SpawnType[] spawnTypes;

    #region Spawning Methods
    public void Trigger(int amount) {
        

        for (int a=0; a<amount; a++) {

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
