using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Ball : MonoBehaviour
{
    /*
     * Class Description: public class Ball
     *  This class interacts with each aspect of 'ball' gameObject 
     *  including: physics, audio, collisions, and what to do during different game states (i.e. start/end)
     * 
     * Where to place:
     *  Attach this scipt should be attached to each 'ball' gameObject
     */

    public GameObject numBallsText; // number of balls player currently controls

    public float speed;         // speed of ball

    public int ballsSpawned = 0,//total balls currently spawned
        ballsCollected,         // total balls collected this game
        numBallsToSpawn;        // number of balls to spawn after a round

    public bool                 // first ball to 'land'
        grounded = true,        // is ball touching the ground
        secondLanded = false,   //is this ball the second ball to touch ground
        justSpawned,            // is this a newly spanwed ball
        ready = true, 
        soundOn;

    public Transform target,
        lookAt;                 // point all balls should move towards

    private Rigidbody2D rigi;   // get RigidBody2D component of obj

    public AudioClip blockClip,
        ballSpawnClip,
        gemClip;

    private AudioSource audioSource;

    void OnEnable()
    {
        transform.position = new Vector3(0, -3.5f, 0);
        grounded = true;
        GameManager.OnClicked += SpeedUp;       // event that speeds up all balls so player doesn't have to wait
        GameManager.OnDeath += DestroyObject;   //load event that destroys all obejcts in scene

        GetComponent<SpriteRenderer>().color = new Color(PlayerPrefManager.GetBallColorR(), PlayerPrefManager.GetBallColorG(), PlayerPrefManager.GetBallColorB()); //determine color of ball
    }

    void OnDisable()
    {
        GameManager.OnClicked -= SpeedUp;
        GameManager.OnDeath -= DestroyObject; //unload event that destroys all obejcts in scene

    }


    void Start()
    {
        if (GameManager.gm.gameRound != 1)
        {
            justSpawned = true;
        }

        secondLanded = false;
        rigi = GetComponent<Rigidbody2D>();
        target = GameManager.gm.target;
        //Physics2D.IgnoreLayerCollision(8, 8, true); // balls do not hit eachother (turned off in Physics2D inspector)   

        numBallsText = GameObject.Find("TXT_NumBalls");
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {

        if ((transform.position.y <= 0) && rigi.velocity.y < 0.05f && rigi.velocity.y > -0.05f && !grounded) //BUG FIX: ball was getting stuck at top or bouncing from side to side with low y velocity
        {
            rigi.AddForce(Vector2.down, ForceMode2D.Impulse);
        }
        else if (transform.position.y > 0 && (rigi.velocity.y < 0.05f && rigi.velocity.y > -0.05f && !grounded))
            rigi.AddForce(Vector2.down, ForceMode2D.Impulse);

        if (secondLanded == true && grounded == true)
        {
            if (target != null)
                transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }

        if (target != null && transform.position == target.position)
        {
            secondLanded = false;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        //If ball hits a block
        if (other.collider.tag == "Block")
        {
            if (PlayerPrefManager.GetSoundFX() == true)
            {
                audioSource.clip = blockClip;
                audioSource.Play();
            }

            GameManager.gm.AddPoints(1);
        }

        //If ball hits the bottom barrier
        if (other.collider.tag == "Bottom Barrier")
        {
            grounded = true;
            GameManager.gm.groundedBalls++;

            if (justSpawned == false)
            {
                if (GameManager.gm.CheckBallCount() == true)
                {
                    numBallsText.GetComponent<MeshRenderer>().enabled = true;
                    numBallsText.GetComponent<TextMesh>().text = "x " + GameManager.gm.ballsCollected;
                }
            }

            //if ball is the first to hit
            if (GameManager.gm.firstLand == false && justSpawned == false)
            {
                target = transform;
                GameManager.gm.target = transform; // store location of first ball to land

                //adjust where number appears so player can see at all times
                if (transform.position.x > 2.2) // approaching right side of screen
                    numBallsText.transform.position = new Vector2(transform.position.x - 0.25f, transform.position.y + 0.5f);
                else if (transform.position.x < -2.2)//approaching left side of screen
                    numBallsText.transform.position = new Vector2(transform.position.x + 0.25f, transform.position.y + 0.5f);
                else
                    numBallsText.transform.position = new Vector2(transform.position.x, transform.position.y + 0.5f);

                GameManager.gm.firstLand = true;
            }
            else //ball is not the first ball the land
            {
                secondLanded = true;
                target = GameManager.gm.target; // this is the first ball landed and all balls will go this to position
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //if ball hits ballSpawn
        if (other.tag == "BallSpawn")
        {
            if (PlayerPrefManager.GetSoundFX() == true)
            {
                audioSource.clip = ballSpawnClip;
                audioSource.Play();
            }
            GameManager.gm.ballsCollected++;
            PlayerPrefManager.SetBool(other.gameObject.name, false);
            other.gameObject.SetActive(false);
        }
    }

    
    //void SpeedUp() - speeds up velocity of ball by multiple of 2
    void SpeedUp() 
    {
        rigi.velocity = rigi.velocity * 2;
    }

    //void DestroyObject() - destroys main ball, if not main ball then simply turns off renderer so ball becomes invisible to player
    void DestroyObject()
    {
        if (gameObject.name != "GO_Main Ball")
            Destroy(gameObject);
        else
            numBallsText.GetComponent<MeshRenderer>().enabled = false;
    }

}



