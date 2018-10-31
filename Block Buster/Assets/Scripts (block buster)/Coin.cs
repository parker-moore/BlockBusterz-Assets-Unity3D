using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    //*******************************
    //How to use script:
    //  Attach to game currency that is collectable 
    //  if player interacts with this object
    //*******************************

    public AudioClip coinSound;
    public int coinValue = 1;
    public bool soundOn;
    public Animator gemDeath;
    public AnimationClip deathclip;
    public GameObject gemScore;

    bool taken = false;

    SpriteRenderer sr;
    EdgeCollider2D ec;
    AudioSource audiSou;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        ec = GetComponent<EdgeCollider2D>();
        gemScore = GameObject.Find("TXT_plus one"); // text of "+1" that appears above gem count when player obtains a gem
        audiSou = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.tag == "Ball") && (!taken))
        {
            sr.enabled = false; // turn off sprite so animation can finish
            ec.enabled = false; // turn off edge collider so animation can finish
            gemDeath.SetBool("Dead", true);
            taken = true; //set to taken so there is no chance of coin being picked up again
            GameManager.gm.AddCoin(coinValue); //add coin value to collection
            gemScore.GetComponent<BlackBarrierAnimations>().EnableAnimation();


            //PlayerPrefManager.GetSoundFX() == true && 
            if (GetComponent<AudioSource>() && PlayerPrefManager.GetSoundFX() == true)
            {
                audiSou.clip = coinSound;
                audiSou.Play();
                Destroy(gameObject.transform.parent.gameObject, deathclip.length);
            }
            else
                Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
