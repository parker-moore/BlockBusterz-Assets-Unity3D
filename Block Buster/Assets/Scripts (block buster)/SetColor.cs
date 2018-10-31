using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/*
 * public class SetColor -- 
 * sets the color of ball as well as
 * setting the initial position of 
 * highlighter which boarders chosen 
 * color in shop menu
 */

public class SetColor : MonoBehaviour
{

    public Color ballColor;
    public GameObject highLight;

    void Awake()
    {
        if (PlayerPrefs.HasKey("hlX"))
            highLight.transform.position = new Vector2(PlayerPrefs.GetFloat("hlX"), PlayerPrefs.GetFloat("hlY")); // set position of highlighter to that of currently selected color (saves when player exits game)
    }

    public void SetBallColor()
    {
        if (PlayerPrefManager.GetIsProductUnlocked(gameObject.name) || gameObject.name == "BTN_Black ball")
        {
            PlayerPrefManager.SetBallColor(ballColor.r, ballColor.g, ballColor.b);
            Debug.Log(PlayerPrefManager.GetBallColorR());
            highLight.transform.position = this.gameObject.transform.position;
            PlayerPrefs.SetFloat("hlX", gameObject.transform.position.x);
            PlayerPrefs.SetFloat("hlY", gameObject.transform.position.y);
        }
    }
}
