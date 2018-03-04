using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour {

    #region Public Variables
    public static EnemyManager main;

    [Header("References")]
    public Transform enemyParent;

    [Header ("Visuals")]
    public Object[] destroyVFXRobot;
    public Object[] destroyVFXAlien;
    public Material transparentMatRobot, transparentMatAlien;

    [Header ("Audio")]
    public AudioClip[] robotDeathAudio;
    public AudioClip[] alienDeathAudio;

    [Header ("Shield Punching")]
    public Transform shieldTransform;
    public Object shieldVFX;
    public float shieldVFXY, shieldVFXRadius;
    public AudioClip shieldSFX;

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

    #region Public Methods
    public AudioClip GetAudioClip(Enemy.Type enemyType, AudioClipType clipType) {
        AudioClip[] clips = null;
        switch (clipType) {
            case AudioClipType.Death:
                if (enemyType == Enemy.Type.Robot)
                    clips = robotDeathAudio;
                else
                    clips = alienDeathAudio;
                break;
        }

        if (clips != null) {
            if (clips.Length > 0)
                return clips[Random.Range (0, clips.Length)];
            else
                return null;
        } else {
            return null;
        }
        
    }
    #endregion

    public enum AudioClipType{
        Death
    }

}
