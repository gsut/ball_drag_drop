using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballMove : MonoBehaviour
{
    public LineRenderer mLine;
    private Rigidbody rigitbodyBall;
    private float speed;                     
    private bool isPressed;
    private static bool inGame;
    private Vector3 mouse;
    private Vector3 vBall;
    private Vector3 goLine;
    private float xBall, zBall;
    private float hBall;
    private float distLine;
    private float timeCounter;

    // Start is called before the first frame update
    void Start()
    {
        rigitbodyBall = GetComponent<Rigidbody>();
        inGame = false;
        isPressed = false;
        xBall = transform.position.x;
        zBall = transform.position.z;
        mLine.enabled = false;
        mLine.SetPosition(0, transform.position);  // set line renderer start position to ball position
        mLine.SetPosition(1, transform.position);  // set line renderer end position to ball position
    }



    // Update is called once per frame
    void Update()
    {

        timeCounter += Time.deltaTime;

        // Check every half second (for optimisation)    
        if (timeCounter >= 0.5f)
        {
            if (inGame)
            {
                // get the speed of the ball
                vBall = rigitbodyBall.velocity;   
                hBall = vBall.magnitude;

                if (hBall < 1.5f)  // if the ball is too slow 
                {
                    // stop the ball
                    rigitbodyBall.velocity = Vector3.zero;
                    rigitbodyBall.constraints = RigidbodyConstraints.FreezeAll;

                    hBall = 0;
                    inGame = false;
                    xBall = transform.position.x;
                    zBall = transform.position.z;
                    mLine.SetPosition(0, transform.position); // set line renderer start position to ball position
                }
            }
            else
            {
                hBall = 0;
            }
            timeCounter = 0f;

            // If the ball has gone too far, return it to the center to re-roll.
            if ((transform.position.z < -250) || (transform.position.z > 250) || (transform.position.x < -250) || (transform.position.x > 250))
            {
                // stop the ball
                rigitbodyBall.velocity = Vector3.zero;
                rigitbodyBall.constraints = RigidbodyConstraints.FreezeAll;

                transform.position = new Vector3(0f, 0f, 0f);
                inGame = false;
                mLine.SetPosition(0, transform.position);   // set line renderer start position to ball position
            }

        }


        if (isPressed & (hBall == 0))   // if the mouse is pressed and the speed of the ball is zero
        {
            // calculate where the mouse clicked on the screen (3D)
            Plane plane = new Plane(Vector3.up, 0);
            float distance;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out distance))  // can calculate
            {
                // get click point
                mouse = ray.GetPoint(distance);
                mouse.y = 1f;
                
                distLine = Vector3.Distance(mouse, mLine.GetPosition(0)); // calculate line distince
                if ((distLine < 19) & (mouse.z > -20) & (mouse.z < 20) & (mouse.x > -20) & (mouse.x < 20)) // if mouse click point in ground coordinate
                {
                    transform.position = mouse; // set ball position to mouse click position
                }
            }

            if ((xBall != transform.position.x) & (zBall != transform.position.z)) // if ball moved from first position
            {
                // set line renderer end position to mouse click position
                mLine.SetPosition(1, transform.position);
            }
        }
    }


    void OnMouseDown()
    {
        if (!inGame)
        {
            isPressed = true;
            mLine.enabled = true;
            // reset the ball constraints for moving
            rigitbodyBall.constraints = RigidbodyConstraints.None;
            rigitbodyBall.constraints = RigidbodyConstraints.FreezePositionY;
        }
    }

    void OnMouseUp()
    {
        if (isPressed)
        {
            isPressed = false;
            inGame = true;
            mLine.enabled = false;

            // Set the direction in which the ball will be thrown 
            goLine = new Vector3(xBall - transform.position.x, 1f, zBall - transform.position.z);

            // set the speed at which the ball will be thrown 
            speed = Vector3.Distance(mLine.GetPosition(0), mLine.GetPosition(1)) * 300;

            // hit the ball
            rigitbodyBall.AddForce(goLine * Time.deltaTime * speed, ForceMode.Impulse);
        }
    }
}
