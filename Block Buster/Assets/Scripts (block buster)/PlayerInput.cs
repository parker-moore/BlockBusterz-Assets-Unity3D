using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/* Class Description:
 * public class PlayerInput --
 * Placed only on main ball, this class
 * is in charge of shooting the ball in the
 * opposite direction of where you drag your finger on the screen
 * with thrust no changing with longer or shorter lines
 */

public class PlayerInput : MonoBehaviour
{

    public GameObject numBallsText, pointer;

    public float speed,         // speed of ball
        waitBetweenBalls,       // time between release of each ball
        dist;                   // distance

    public bool ready = true;

    public Transform target,
        lookAt;                 // point all balls should move towards
    public Vector2 curPos,      // current position of first ball to land
        mousePos,               // current position of mouse on screen
        fingerPos;

    private LineRenderer line;  // get LineRenderer component of obj

    private bool firstTouchSet = false; // has the first touch point been set

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }
    void OnEnable()
    {
        int ballCount;
        if (PlayerPrefManager.GetBool("gameRunning") == true)
            ballCount = PlayerPrefs.GetInt("numBalls");
        else
            ballCount = GameManager.gm.ballsCollected;

        numBallsText.GetComponent<TextMesh>().text = "x " + ballCount;  // displays how many balls user will fire
        numBallsText.transform.position = new Vector2(transform.position.x, transform.position.y + 0.5f);
        numBallsText.GetComponent<MeshRenderer>().enabled = true;
    }

    void Update()
    {
        // first touch of screen
        if (Input.GetButtonDown("Touch") || CrossPlatformInputManager.GetButtonDown("Touch"))
        {
            if (Input.touchCount > 0)
                curPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);    // get initial position of where player touched screen
            else
                curPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);           // get initial position where mouse touched screen (for testing purposes)

            firstTouchSet = true;  
        }

        // When Dragging Finger
        if ((Input.GetButton("Touch") || CrossPlatformInputManager.GetButton("Touch")) && firstTouchSet == true)
        {

            if (line.positionCount == 0)
            {
                line.positionCount = 2;
            }

            if (Input.touchCount > 0) // finger input
            {
                fingerPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                Vector3 difference = fingerPos - curPos;                         // difference between current finger position and first touch point
                Vector2 endPoint = transform.position - (difference * 3.85f);   // where end of line will be drawn

                dist = Vector2.Distance(fingerPos, curPos);  // distance between figer and initialPosition

                line.SetPosition(0, transform.position);

                if (dist > 0.2 && fingerPos.y < curPos.y)
                    line.SetPosition(1, endPoint);  // create line
                else
                    line.SetPosition(1, transform.position);

                float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ + 90);  // BUG FIX: had to set rotation of line to 90 degrees more on z axis because angle was off initially

                if (dist > 0.1f)    // don't draw line unless finger is pulled back
                    pointer.SetActive(true);
            }
            else if (firstTouchSet == true) // mouse input
            {
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 difference = mousePos - curPos; // difference between current finger position and first touch point
                Vector2 endPoint = transform.position - (difference * 3.85f); // where end of line will be drawn

                dist = Vector2.Distance(mousePos, curPos);

                line.SetPosition(0, transform.position);

                if (dist > 0.2 && mousePos.y < curPos.y)
                    line.SetPosition(1, endPoint);
                else
                    line.SetPosition(1, transform.position);

                float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ + 90);

                if (dist > 0.1f)
                    pointer.SetActive(true);
            }
        }

        //when you lift up finger
        if (Input.GetButtonUp("Touch") || CrossPlatformInputManager.GetButtonUp("Touch"))
        {
            line.SetPosition(1, transform.position);    // turn off line
            pointer.SetActive(false);                   // turn off pointing triangle on ball
            firstTouchSet = false;
        }
    }

    /*
     * public bool FlingBalls() --
     * wrapper function which starts coroutine fling() if
     * finger is pulled back in direction of blocks
     */
    public bool FlingBalls()
    {
        if (dist > 0.2f && mousePos.y < curPos.y && gameObject.activeSelf == true)
        {
            StartCoroutine(Fling());
            return true;
        }
        else
            return false;
    }

    /*
     * IEnumerator Fling()
     * 
     */

    IEnumerator Fling()
    {
        int ballCount = GameManager.gm.ballsCollected;
        GameManager.gm.groundedBalls = 0;   // when you fling balls the number of balls on ground becomes 0

        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        for (int i = 0; i < balls.Length; i++)
            balls[i].GetComponent<Ball>().ready = false;

        for (int i = 0; i < balls.Length; i++)
        {
            ballCount--;
            balls[i].transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);
            balls[i].GetComponent<Rigidbody2D>().AddForce(transform.up * speed, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.1f);
            balls[i].GetComponent<Ball>().grounded = false;

            balls[i].GetComponent<Ball>().justSpawned = false;
            numBallsText.GetComponent<TextMesh>().text = "x " + ballCount;  // decrement ball count to show player how many balls are left as they are flinged upwards
        }
        numBallsText.GetComponent<MeshRenderer>().enabled = false;  // after flinging all balls turn off renderer of text displaying number of balls
        GameManager.gm.newRound = true; // start new round
    }
}




