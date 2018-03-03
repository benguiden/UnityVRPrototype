﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassFollow : MonoBehaviour {

    public Transform reference;
    public float smoothness = 0.75f;


    private void Update() {
        float newAngle = -reference.localEulerAngles.y;
        newAngle = Mathf.LerpAngle (transform.localEulerAngles.y, newAngle, Time.deltaTime * 60f * (1f - smoothness));
        transform.localEulerAngles = new Vector3 (-90f, newAngle, 0f);
    }

}