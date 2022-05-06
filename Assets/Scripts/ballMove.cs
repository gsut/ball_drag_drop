using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballMove : MonoBehaviour
{
    private Rigidbody rgPlayer;
    private float Hiz;
    public LineRenderer mLine;
    private bool isPressed;
    public static bool inGame;
    private Vector3 mouse;
    private Vector3 vBall;
    private Vector3 goLine;
    private float xBall, zBall;
    private float hBall;
    private float x, z, h, u;
    private float distLine;

    Vector2 startPos, endPos, direction; // touch start position, touch end position, swipe direction
    float touchTimeStart, touchTimeFinish, timeInterval; // to calculate swipe time to sontrol throw force in Z direction
    private float timeCounter;

    // Start is called before the first frame update
    void Start()
    {
        rgPlayer = GetComponent<Rigidbody>();
        Hiz = 20000;
        inGame = false;
        isPressed = false;
        xBall = transform.position.x;
        zBall = transform.position.z;
        mLine.enabled = false;
        mLine.SetPosition(0, transform.position);
        mLine.SetPosition(1, transform.position);


    }



    // Update is called once per frame
    void Update()
    {

        timeCounter += Time.deltaTime;
        if (timeCounter >= 0.5f)
        {
            if (inGame)
            {
                vBall = rgPlayer.velocity;
                hBall = vBall.magnitude;

                Debug.Log(hBall);
                if (hBall < 1.5f)
                {
                    rgPlayer.velocity = Vector3.zero;
                    rgPlayer.constraints = RigidbodyConstraints.FreezeAll;
                    hBall = 0;
                    inGame = false;
                    xBall = transform.position.x;
                    zBall = transform.position.z;
                    mLine.SetPosition(0, transform.position);
                }
            }
            else
            {
                hBall = 0;
            }
            timeCounter = 0f;

            if ((transform.position.z < -250) || (transform.position.z > 250) || (transform.position.x < -250) || (transform.position.x > 250))
            {
                rgPlayer.velocity = Vector3.zero;
                rgPlayer.constraints = RigidbodyConstraints.FreezeAll;
                transform.position = new Vector3(0f, 0f, 0f);
                inGame = false;
                mLine.SetPosition(0, transform.position);
            }

        }



        if (isPressed & (hBall == 0))
        {
            Plane plane = new Plane(Vector3.up, 0);
            float distance;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out distance))
            {
                mouse = ray.GetPoint(distance);
                mouse.y = 1f;
                distLine = Vector3.Distance(mouse, mLine.GetPosition(0));
                Debug.Log("dist: " + distLine);
                if ((distLine < 19) & (mouse.z > -20) & (mouse.z < 20) & (mouse.x > -20) & (mouse.x < 20))
                {
                    transform.position = mouse;
                }
            }

            if ((xBall != transform.position.x) & (zBall != transform.position.z))
            {
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
            rgPlayer.constraints = RigidbodyConstraints.None;
            rgPlayer.constraints = RigidbodyConstraints.FreezePositionY;
        }
    }

    void OnMouseUp()
    {
        if (isPressed)
        {
            isPressed = false;
            inGame = true;
            mLine.enabled = false;
            // ilk atis burada yapilmali
            goLine = new Vector3(xBall - transform.position.x, 1f, zBall - transform.position.z);
            Hiz = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(transform.position.x - xBall), 2) + Mathf.Pow(Mathf.Abs(transform.position.z - zBall), 2)) * 300;
            rgPlayer.AddForce(goLine * Time.deltaTime * Hiz, ForceMode.Impulse);
            //rgPlayer.velocity = goLine * Time.deltaTime * u;
        }
    }
}
