using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDown : MonoBehaviour
{
    
    /*
     * Class Description: publi class MoveDown
     *  After each round, when all flung balls return to ground, blocks at top of screen will move down, in unison, towards ground
     *  This script is also applied to "gems" as once they spawn they will move down the following turn
     */

    public float moveSpeed = 3;
    public Vector2 newPos;  // position where block will move to
    public bool isMoving = false;
    private float distance; // distance between current position and new position
    private GameObject bottomBarrier;

    void OnEnable()
    {
        GameManager.OnDeath += DestroyObject; //load event that destroys all obejcts in scene

        if (gameObject.tag != "Coin") // because coins are generated when a block dies you want them to wait a turn to move down
        {

            if (PlayerPrefManager.GetBool(gameObject.name) == true && PlayerPrefManager.GetBool("gameRunning") == true)
            {
                newPos = new Vector3(PlayerPrefManager.GetXPosition(gameObject.name), PlayerPrefManager.GetYPosition(gameObject.name) - 2.05f, PlayerPrefManager.GetZPosition(gameObject.name));

            }
            else
            {
                SetNewPosition();
                StartCoroutine(WaitOnStart());
            }
        }

        bottomBarrier = GameObject.Find("Bottom Boundry");
    }

    void SetNewPosition()
    {
        newPos = new Vector2(transform.position.x, transform.position.y - 2.05f);
    }

    void OnDisable()
    {
        GameManager.OnDeath -= DestroyObject; //unload event that destroys all obejcts in scene
    }

    void Update()
    {
        distance = Vector2.Distance(transform.position, newPos);

        if (isMoving == true) // dumb to always be calling this HOWEVER having move in Update() created a more fluid looking movement
        {
            Move();
        }

        if (transform.position.y < bottomBarrier.transform.position.y + bottomBarrier.transform.localScale.y) 
        {
            if (gameObject.tag == "Block") // if a block touches ground game is over
            {
                GameManager.gm.GameOver();
            }
            else
            {
                if (gameObject.tag == "Coin") // if coin touches ground simply destroy coin
                    Destroy(gameObject);
                else
                    gameObject.SetActive(false);

            }

        }
    }

    public void SetNewPos()
    {
        newPos = new Vector2(transform.position.x, transform.position.y - 0.75f);
    }

    public void Move()
    {
        if (distance > 0.02f)
            transform.position = Vector2.Lerp(transform.position, newPos, Time.deltaTime * moveSpeed);  // slows down movement over time to create fluid looking start/stop
        else
        {
            GameManager.gm.Ready();
            isMoving = false;
            transform.position = newPos;
            newPos = new Vector2(transform.position.x, transform.position.y - 0.75f);
        }
    }

    void DestroyObject()
    {
        PlayerPrefManager.SetBool(gameObject.name, false);
        gameObject.SetActive(false);
    }

    IEnumerator WaitOnStart()
    {
        if (GameManager.gm.gameRound == 1)
            yield return new WaitForSeconds(0.1f);
        else
            yield return new WaitForSeconds(0.0f);
        isMoving = true;
    }
}
