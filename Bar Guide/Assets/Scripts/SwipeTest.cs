using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeTest : MonoBehaviour
{
    GUIBehaviour gui;
    GUIBehaviour_XML guiXML;

    Vector2 StartPos;
    int SwipeID = -1;
    float minMovement = 20.0f;
    
    
    // Use this for initialization
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
        foreach (var T in Input.touches)
        {
            var P = T.position;
            if (T.phase == TouchPhase.Began && SwipeID == -1)
            {
                SwipeID = T.fingerId;
                StartPos = P;
            }
            else if (T.fingerId == SwipeID)
            {
                var delta = P - StartPos;
                if (T.phase == TouchPhase.Moved && delta.magnitude > minMovement)
                {
                    SwipeID = -1;
                    if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    {
                        if (delta.x > 0)
                        {
                            Debug.Log("Swipe Right Found");
                        }
                        else
                        {
                            Debug.Log("Swipe Left Found");
                        }
                    }
                    else
                    {
                        if (delta.y > 0)
                        {

                            Debug.Log("Swipe Up Found");
                        }
                        else
                        {

                            Debug.Log("Swipe Down Found");
                        }
                    }
                }
                else if (T.phase == TouchPhase.Canceled || T.phase == TouchPhase.Ended)
                    SwipeID = -1;
            }
        }
    }
}