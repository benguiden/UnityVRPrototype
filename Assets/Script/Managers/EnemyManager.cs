using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour {

    #region Public Variables
    public static EnemyManager main;

    [Header ("UI")]
    public Object uiPrefab;
    public Transform uiParent;
    public float uiActivateDistance = 50f;
    public float updateUIRate = 60f;
    [Range (float.Epsilon, 1f)]
    public float uiUpdateSmoothness = 0.25f;
    public float uiAlphaAmount = 8f;
    public float uiYOffset = 0.5f;
    public float uiCameraDistance = 2f;
    public bool anticipatePosition = true;
    public float bulletSpeedForUI;
    #endregion

    #region Mono Methods
    private void Awake() {
        main = this;
    }
    #endregion

}
