using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundryScript : MonoBehaviour
{
    /*
     * Class Description: public class BoundryScript
     *  This class controls the width and height of colliders set surrounding black border (top, bottom, left, right) 
     *  correctly (as determined by programmer) resizing for different screen resolutions
     */

    public bool left, right, top, bottom;
    public GameObject barrierImage, canvas;
    private Camera cam = null;
    private float barrierImageHeight, barrierImageWidth;
    private Animator anim;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();

        if (cam == null)
            cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        Vector3 tmpPos = cam.ViewportToWorldPoint(transform.position);


        //adjust collider boundries compared to a canvas of 750 x 1334 (iphone 6 Tall)
        if (Screen.height != 1334)
        {
            barrierImageHeight = barrierImage.GetComponent<RectTransform>().rect.height * ((float)Screen.height / 1334f);
        }
        else
            barrierImageHeight = barrierImage.GetComponent<RectTransform>().rect.height;

        if (Screen.width != 750)
        {
            barrierImageWidth = barrierImage.GetComponent<RectTransform>().rect.width * ((float)canvas.GetComponent<RectTransform>().rect.width / 750f);
        }
        else
            barrierImageWidth = barrierImage.GetComponent<RectTransform>().rect.width;


        float moveOverHeight = Screen.height - barrierImageHeight;
        float moveOverWidth = (float)canvas.GetComponent<RectTransform>().rect.width - barrierImage.GetComponent<RectTransform>().rect.width;

        float moveOverHeightPixToWorld = (moveOverHeight / (float)Screen.height) * (-tmpPos.y * 2);
        float moveOverWidthPixToWorld = (moveOverWidth / (float)canvas.GetComponent<RectTransform>().rect.width) * (-tmpPos.x * 2);

        if (moveOverWidthPixToWorld < 0)
            moveOverWidthPixToWorld = moveOverWidthPixToWorld * -1;
        if (moveOverHeightPixToWorld < 0)
            moveOverHeightPixToWorld = moveOverHeightPixToWorld * -1;
        //*******************************************************************************************


        if (left == true)
            transform.position = new Vector3(tmpPos.x - (transform.localScale.x / 2f) + moveOverWidthPixToWorld / 2, 0, 0);
        else if (right == true)
            transform.position = new Vector3(-tmpPos.x + (transform.localScale.x / 2f) - moveOverWidthPixToWorld / 2, 0, 0);
        else if (bottom)
            transform.position = new Vector3(0, tmpPos.y - (transform.localScale.y / 2f) + moveOverHeightPixToWorld / 2, 0);
        else if (top)
            transform.position = new Vector3(0, -tmpPos.y + (transform.localScale.y / 2f) - moveOverHeightPixToWorld / 2, 0);


        if (left || right)
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(gameObject.GetComponent<BoxCollider2D>().size.x, -tmpPos.y * 2);
        else
        {
            gameObject.transform.localScale = new Vector2(-tmpPos.x * 2, gameObject.GetComponent<BoxCollider2D>().size.y);
        }

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Ball" && bottom)
        {
            other.gameObject.GetComponent<Rigidbody2D>().Sleep();

        }
        else if (other.collider.tag == "Ball")
        {
            barrierImage.GetComponent<Animator>().SetBool("enable", true);
        }

     
    }






}
