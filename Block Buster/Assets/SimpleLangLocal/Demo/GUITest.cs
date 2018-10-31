using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUITest : MonoBehaviour
{

    public GameObject highlight;
    public GameObject language;

    public void SetEN()
    {

        if (PlayerPrefManager.GetSoundFX() == true && gameObject.GetComponent<AudioSource>())
            gameObject.GetComponent<AudioSource>().Play();

        LocalizationManager.instance.SetLang("EN");
    }

    public void SetES()
    {

        if (PlayerPrefManager.GetSoundFX() == true && gameObject.GetComponent<AudioSource>())
            gameObject.GetComponent<AudioSource>().Play();

        LocalizationManager.instance.SetLang("ES");
        language.GetComponent<Language>().LangSwitch();

    }

    public void SetFR()
    {

        if (PlayerPrefManager.GetSoundFX() == true && gameObject.GetComponent<AudioSource>())
            gameObject.GetComponent<AudioSource>().Play();

        LocalizationManager.instance.SetLang("FR");
        language.GetComponent<Language>().LangSwitch();

    }

    public void SetCN()
    {
        if (PlayerPrefManager.GetSoundFX() == true && gameObject.GetComponent<AudioSource>())
            gameObject.GetComponent<AudioSource>().Play();

        LocalizationManager.instance.SetLang("CN");
        language.GetComponent<Language>().LangSwitch();

    }

    public void SetJP()
    {
        if (PlayerPrefManager.GetSoundFX() == true && gameObject.GetComponent<AudioSource>())
            gameObject.GetComponent<AudioSource>().Play();

        LocalizationManager.instance.SetLang("JP");
        language.GetComponent<Language>().LangSwitch();

    }

    public void SetIT()
    {
        if (PlayerPrefManager.GetSoundFX() == true && gameObject.GetComponent<AudioSource>())
            gameObject.GetComponent<AudioSource>().Play();

        LocalizationManager.instance.SetLang("IT");
        language.GetComponent<Language>().LangSwitch();

    }

    public void SetGM()
    {
        if (PlayerPrefManager.GetSoundFX() == true && gameObject.GetComponent<AudioSource>())
            gameObject.GetComponent<AudioSource>().Play();

        LocalizationManager.instance.SetLang("GM");
        language.GetComponent<Language>().LangSwitch();

    }

    public void SetDU()
    {
        if (PlayerPrefManager.GetSoundFX() == true && gameObject.GetComponent<AudioSource>())
            gameObject.GetComponent<AudioSource>().Play();

        LocalizationManager.instance.SetLang("DU");
        language.GetComponent<Language>().LangSwitch();

    }




    public void SetHighlight()
    {
        PlayerPrefs.SetFloat("X", gameObject.transform.position.x);
        PlayerPrefs.SetFloat("Y", gameObject.transform.position.y);
        PlayerPrefs.SetFloat("Z", gameObject.transform.position.z);

        highlight.transform.position = new Vector3(PlayerPrefs.GetFloat("X"), PlayerPrefs.GetFloat("Y"), PlayerPrefs.GetFloat("Z"));
    }
}
