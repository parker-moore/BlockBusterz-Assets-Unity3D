using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    /*
     * Class Description: public class Orbit
     *  Allow triangle pointer at tip of ball to correctly orbit around ball
     */
    public Transform ball;
    public float radius;

    private Transform pivot;

    void Start()
    {
        pivot = ball.transform;
        transform.parent = pivot;
        transform.position += Vector3.up * radius;
    }

    void Update()
    {
        Vector3 ballVector = Camera.main.WorldToScreenPoint(ball.position);
        ballVector = Input.mousePosition - ballVector;
        float angle = Mathf.Atan2(ballVector.y, ballVector.x) * Mathf.Rad2Deg;

        pivot.position = ball.position;
        //pivot.eulerAngles = new Vector3(0, 0, angle - 90);
    }  
}
