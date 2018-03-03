using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    #region Public Variables
    public static UIManager main;

    [Header ("Bullets")]
    public RectTransform[] bulletMasks;
    public float bulletMaskHeight;
    public float reloadBulletTime = 0.25f;
    public AnimationCurve reloadBulletCurve;
    #endregion

    #region Private Variables
    //Bullets
    private int bulletUIIndex = 0;
    private Coroutine[] bulletCoroutines;
    #endregion

    #region Mono Methods
    private void Awake() {
        main = this;

        bulletCoroutines = new Coroutine[bulletMasks.Length];
    }
    #endregion

    #region Public Methods
    public void ReloadBulletUI() {
        if (bulletCoroutines[bulletUIIndex] != null)
            StopCoroutine (bulletCoroutines[bulletUIIndex]);

        bulletCoroutines[bulletUIIndex] = StartCoroutine (IReload (bulletUIIndex));

        bulletUIIndex = (bulletUIIndex + 1) % bulletMasks.Length;
    }
    #endregion

    #region Coroutines
    private IEnumerator IReload(int index) {
        float time = 0f;
        bulletMasks[index].sizeDelta = new Vector2 (bulletMasks[index].sizeDelta.x, bulletMaskHeight);
        while (time < reloadBulletTime / 2f) {
            float newHeight = reloadBulletCurve.Evaluate (time / (reloadBulletTime / 2f)) * bulletMaskHeight;
            bulletMasks[index].sizeDelta = new Vector2 (bulletMasks[index].sizeDelta.x, newHeight);

            time += Time.deltaTime;
            yield return null;
        }

        time = 0f;

        bulletMasks[index].localScale = new Vector3 (0f, 1f, 1f);
        bulletMasks[index].sizeDelta = new Vector2 (bulletMasks[index].sizeDelta.x, bulletMaskHeight);

        while (time < reloadBulletTime / 2f) {
            float newScale = 1f - reloadBulletCurve.Evaluate (time / (reloadBulletTime / 2f));
            bulletMasks[index].localScale = new Vector3 (newScale, 1f, 1f);

            time += Time.deltaTime;
            yield return null;
        }

        bulletMasks[index].localScale = new Vector3 (1f, 1f, 1f);
    }
    #endregion

}
