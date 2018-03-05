using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager main;

    public float shieldRadius = 10f;

    public AudioSource music;

    [Header("Waves")]
    public int wave;
    public float[] waveTime;

    [Header("Shield")]
    public float shieldHP = 25f;
    public Transform shieldIndicator;
    public Vector2 shieldIndicatorRange = new Vector2 (-90f, 90f);
    public float shieldIndicatorSmoothness = 0.85f;

    private float currentWaveTime;
    private float maxShieldHP;
    private float shieldHPAngle;

    private void Awake() {
        main = this;
        maxShieldHP = shieldHP;
        shieldHPAngle = shieldIndicatorRange.y;
        currentWaveTime = waveTime[0];

        if (music != null) {
            if (!GlobalManager.music)
                music.enabled = false;
        }
    }

    private void Update() {
        shieldIndicator.localEulerAngles = new Vector3 (0f, 0f, Mathf.LerpAngle (shieldIndicator.localEulerAngles.z, shieldHPAngle, Time.deltaTime * 60f * (1f - shieldIndicatorSmoothness)));

        //Wave Update
        currentWaveTime -= Time.deltaTime;
        if (currentWaveTime <= 0f) {
            if (wave < waveTime.Length - 1) {
                wave++;
                currentWaveTime = waveTime[wave];
                UIManager.main.waveIndexText.text = (wave + 1).ToString ();
            } else {
                currentWaveTime = -0.5f;
            }
        }

        UIManager.main.waveTimeText.text = (Mathf.FloorToInt (currentWaveTime) + 1).ToString ();

    }

    public void RestartGame() {
        Debug.Log ("Restarting Game...");
        UnityEngine.SceneManagement.SceneManager.LoadScene (0);
    }

    public void TakeDamage(float damage) {
        shieldHP -= damage;

        shieldHPAngle = Mathf.Lerp (shieldIndicatorRange.x, shieldIndicatorRange.y, shieldHP / maxShieldHP);

        if (shieldHP <= 0f) {
            shieldHP = 0f;
            RestartGame ();
        }
    }

}
