using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager main;

    public float shieldRadius = 10f;

    public int wave;

    [Header("Shield")]
    public float shieldHP = 25f;
    public Transform shieldIndicator;
    public Vector2 shieldIndicatorRange = new Vector2 (-90f, 90f);
    public float shieldIndicatorSmoothness = 0.85f;

    private float maxShieldHP;
    private float shieldHPAngle;

    private void Awake() {
        main = this;
        maxShieldHP = shieldHP;
        shieldHPAngle = shieldIndicatorRange.y;
    }

    private void Update() {
        shieldIndicator.localEulerAngles = new Vector3 (0f, 0f, Mathf.LerpAngle (shieldIndicator.localEulerAngles.z, shieldHPAngle, Time.deltaTime * 60f * (1f - shieldIndicatorSmoothness)));

        if (Input.GetKeyDown (KeyCode.Space)) {
            TakeDamage (1f);
        }
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
