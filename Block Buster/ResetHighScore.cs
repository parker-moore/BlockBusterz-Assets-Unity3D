using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class ResetHighScore : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameManager myGM = (GameManager)target;

        if (GUILayout.Button("Reset Purchases"))
        {
            // if button pressed, then call funciton in script
            PlayerPrefManager.SetHighscore(0);
            PlayerPrefManager.SetIsProductUnlocked("BTN_Orange ball", false);
            PlayerPrefManager.SetIsProductUnlocked("BTN_Blue ball", false);
            PlayerPrefManager.SetIsProductUnlocked("BTN_Red ball", false);
            PlayerPrefManager.SetIsProductUnlocked("BTN_Green ball", false);
            PlayerPrefManager.SetIsProductUnlocked("BTN_Pink ball", false);
            PlayerPrefManager.SetIsProductUnlocked("BTN_Yellow ball", false);
            PlayerPrefManager.SetIsProductUnlocked("BTN_dark red ball", false);
            PlayerPrefManager.SetIsProductUnlocked("BTN_Purple ball", false);

            PlayerPrefManager.SetHighscore(0);

        }

        if (GUILayout.Button("Output coins"))
        {
            // if button pressed, then call function in script
            Debug.Log(PlayerPrefManager.GetCoins());
        }
        
    }
}
