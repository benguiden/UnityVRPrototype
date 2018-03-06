using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienParagraph : MonoBehaviour {

    public float updateTime = 0.25f;
    public int lineWidth = 8;
    public int lineCount = 8;
    public bool binary = false;

    private char[] letters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

    private string[] lines;

    private TextMesh textMesh;

    private void Awake() {
        textMesh = GetComponent<TextMesh> ();
    }

    private void Start() {
        lines = new string[lineCount];
        for (int i=0; i<lineCount; i++) {
            lines[i] = GenerateLine ();
        }

        UpdateTextMesh (lines);

        StartCoroutine (IUpdate ());
    }

    private IEnumerator IUpdate() {
        while (true) {
            if (gameObject.activeSelf) {

                for (int i=1; i<lineCount; i++) {
                    lines[i - 1] = lines[i];
                    if (i == lineCount - 1)
                        lines[i] = GenerateLine ();
                }
                UpdateTextMesh (lines);

            }
            yield return new WaitForSeconds (updateTime);
        }
    }

    private void UpdateTextMesh(string[] textLines) {
        string newText = "";
        for (int i=0; i<textLines.Length; i++) {
            newText += textLines[i];
            if (i < textLines.Length - 1)
                newText += "\n";
        }
        textMesh.text = newText;
    }

    private string GenerateLine() {
        string newStr = "";

        for (int i = 0; i < lineWidth; i++) {
            if (binary) {
                if (Random.value < 0.5) {
                    newStr += "0";
                } else {
                    newStr += "1 ";
                }
            } else {
                if (Random.value < 0.25)
                    newStr += " ";
                else
                    newStr += letters[Random.Range (0, letters.Length)];
            }
        }

        return newStr;
    }
}
