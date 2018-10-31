using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Class Description: public class Language
 *  Controls what language to display to user and can switch between languages using PlayerPref
 */

public class Language : MonoBehaviour
{
    public GameObject gm;
    string word;
    
    void Awake()
    {
        word = this.gameObject.GetComponent<Text>().text;
        if (LocalizationManager.instance)
            gameObject.GetComponent<Text>().text = LocalizationManager.instance.GetWord(gameObject.GetComponent<Text>().text);
    }

    void Start()
    {

        if(word == null)
            word = this.gameObject.GetComponent<Text>().text;


        if (LocalizationManager.instance)
            gameObject.GetComponent<Text>().text = LocalizationManager.instance.GetWord(word);
    }

    void OnEnable()
    {      

        if (LocalizationManager.instance)
        {
            gameObject.GetComponent<Text>().text = LocalizationManager.instance.GetWord(word);
        }

        if (gm)
            gm.GetComponent<GameManager>().UpdateHighScore(); // updates the high score text to change its language
    }

    public void LangSwitch()
    {
        gameObject.GetComponent<Text>().text = LocalizationManager.instance.GetWord(word);
    }

}
