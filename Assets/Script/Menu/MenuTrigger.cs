using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTrigger : MonoBehaviour {

    public MenuButton buttonType;



    private void OnTriggerEnter(Collider c) {
        if ((c.gameObject.tag == "Projectile") && (enabled)) {
            if (buttonType == MenuButton.ToMenu)
                GameManager.main.RestartGame ();
            else
                MenuManager.main.Trigger (buttonType);
        }
    }

    public enum MenuButton{
        Play,
        Exit,
        Music,
        ToMenu,
        CalibrateMic
    }

}
