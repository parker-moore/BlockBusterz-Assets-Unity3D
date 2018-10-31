using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Advertisements;



public class PlayAd : MonoBehaviour
{

    //*******************************
    //Class Description: public class PlayAd
    //  This class initilizes advertisments and plays will show ad if ads are supported
    //
    //How to use script:
    //  Attach to advertisment button 
    //  
    //*******************************

    //initilize game id
    public string unityAdGameID;
    public int coinsGainedFromAd = 0;
    public string placementID;  //reward video, skippable video, ect.

    //Playad class
    public static PlayAd pa = null;

    void Start()
    {
        if (pa == null)
        {
            pa = this.GetComponent<PlayAd>();
        }
        if (Advertisement.isSupported)
            Advertisement.Initialize(unityAdGameID);

    }

    public void ShowAd()
    {
        ShowOptions options = new ShowOptions();
        options.resultCallback = HandleShowResult;

        Advertisement.Show(placementID,options);
    }


    void HandleShowResult(ShowResult result)
    {
        if (result == ShowResult.Finished)
        {
            Debug.Log("Video completed - Offer a reward to the player");

        }
        else if (result == ShowResult.Skipped)
        {
            Debug.LogWarning("Video was skipped - Do NOT reward the player");

        }
        else if (result == ShowResult.Failed)
        {
            Debug.LogError("Video failed to show");
        }
    }





}