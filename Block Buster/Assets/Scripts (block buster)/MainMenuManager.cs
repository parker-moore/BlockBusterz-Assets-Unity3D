using UnityEngine;
using System.Collections;
using UnityEngine.UI; // include UI namespace since references UI Buttons directly
using UnityEngine.EventSystems; // include EventSystems namespace so can set initial input for controller support
using UnityEngine.SceneManagement; // include so we can load new scenes

public class MainMenuManager : MonoBehaviour
{
    //*******************************
    //How to use script:
    //  Attach to MainMenuUI, share game screenUI, settingsUI, and advertismentUI
    //  set up all public variables
    //*******************************

    //MainMenu UI
    public GameObject mainMenuUI;
    public GameObject settingsUI;
    public GameObject advertismentUI;
    public GameObject shareUI;
    public GameObject leaderboardUI;
    public GameObject musicButton;
    public GameObject sfxButton;
    public GameObject playButton;
    public GameObject adsButton;
    public GameObject UI_Language;
    public GameObject[] SFX_Images;
    public GameObject shopUI;
    public GameObject purchaseNoAdsButton;

    //MainMenu handler
    public int shareUIAppearsEvery_Game = 2; //determines how often shareUI appears after 'x' games
    public int advertismentUIAppearsEvery_Game = 3; //determines how often advertismentUI appears after 'x' games

    //audio clips
    public AudioClip touchButtonSound; //place SFX for touching button here
    public AudioSource gameMusic; //place game background music here 
    public AudioSource menuMusic;

    public static MainMenuManager instance = null;

    bool musicOn;
    bool sfxOn;

    void Start()
    {
        if (instance == null)
            instance = this.GetComponent<MainMenuManager>();

        if (PlayerPrefManager.GetNoAdsPurchased() == true)
            purchaseNoAdsButton.SetActive(false);

        //has player gotten rid of ads?
        HideAdButton();

        if (PlayerPrefManager.GetSoundFX() == true)
        {
            SFX_Images[0].SetActive(false); // SFX off image
            SFX_Images[1].SetActive(true); // SFX on image
        }
        else
        {
            SFX_Images[0].SetActive(true); // SFX off image
            SFX_Images[1].SetActive(false); // SFX on image
        }
    }

    public void ToggleSFX()
    {
        if (PlayerPrefManager.GetSoundFX() == false)
        {
            //if soundFX is on and button sound is set
            if (PlayerPrefManager.GetSoundFX() == true && touchButtonSound)
                GetComponent<AudioSource>().PlayOneShot(touchButtonSound);


            //turn off SFX
            PlayerPrefManager.SetSoundFX(true);
            sfxOn = false;
            Debug.Log("SFX on");

            SFX_Images[0].SetActive(false);
            SFX_Images[1].SetActive(true);


        }
        else
        {
            //turn on SFX
            PlayerPrefManager.SetSoundFX(false);
            sfxOn = true;
            Debug.Log("SFX off");

            SFX_Images[0].SetActive(true);
            SFX_Images[1].SetActive(false);
        }

    }

    public void ToggleSettings()
    {
        if (settingsUI.activeSelf == true)
        {
            settingsUI.SetActive(false);
        }
        else
        {
            settingsUI.SetActive(true);
        }

        if (PlayerPrefManager.GetSoundFX() == true)
        {
            sfxButton.GetComponent<Animator>().SetBool("sfxOn", true);
        }
        if (PlayerPrefManager.GetBackgroundMusic() == true)
        {
            if (musicButton.GetComponent<Animator>().isActiveAndEnabled)
                musicButton.GetComponent<Animator>().SetBool("sfxOn", true);
        }
    }

    public void ToggleShop()
    {
        if (shopUI.activeSelf == true)
        {
            shopUI.SetActive(false);
            mainMenuUI.SetActive(true);
        }
        else
        {
            mainMenuUI.SetActive(false);
            shopUI.SetActive(true);
        }
    }

    /// <summary>
    /// MenuHandler handles what menu panel (advertisment, share screen, or mainmenu) appears
    /// once a game ends
    /// </summary>
    public void MenuHandler()
    {
        /*if (playButton.GetComponent<Animator>().GetBool("EnterScreen") == true)
            playButton.GetComponent<SceneTransition>().ExitScreen();
        else
            playButton.GetComponent<SceneTransition>().EnterScreen();
            */

        if (mainMenuUI.activeSelf == true)
            mainMenuUI.SetActive(false);
        else
            mainMenuUI.SetActive(true);

    }

    public void ToggleLanguage()
    {
        if (UI_Language && UI_Language.activeSelf == false)
        {
            UI_Language.SetActive(true);
            mainMenuUI.SetActive(false);
        }
        else
        {
            UI_Language.SetActive(false);
            mainMenuUI.SetActive(true);
        }
    }

    public void HideAdButton()
    {
        if (PlayerPrefManager.GetBool("benedev.mansnothot.noads") == true)
        {
            adsButton.SetActive(false);
        }
    }








}
