using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour {

    #region Public Variables
    public static EnemyManager main;

    [Header ("UI")]
    public Canvas canvas;
    public CanvasScaler canvasScaler;
    public Object uiPrefab;
    public Transform uiParent;
    public float uiActivateDistance = 50f;
    public float updateUIRate = 60f;
    [Range (float.Epsilon, 1f)]
    public float uiUpdateSmoothness = 0.85f;
    public float uiAlphaAmount = 8f;
    #endregion

    #region Mono Methods
    private void Awake() {
        main = this;
    }
    #endregion

}
