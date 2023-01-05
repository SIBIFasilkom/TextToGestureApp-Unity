using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchCameraControl : MonoBehaviour
{
    public float TouchSensitivity_x = 10f;
    public float PinchSensitivity = 0.25f;

    private void Start()
    {
        CinemachineCore.GetInputAxis = HandleAxisInputDelegate;
    }

    private float HandleAxisInputDelegate(string axisName)
    {
        switch (axisName)
        {

            case "Mouse X":

                if (Input.touchCount == 1)
                {
                    return Input.touches[0].deltaPosition.x / TouchSensitivity_x;
                }
                else
                {
                    return Input.GetAxis(axisName);
                }

            case "Mouse ScrollWheel":
                if (Input.touchCount == 2)
                {
                    // Store both touches.
                    Touch touchZero = Input.GetTouch(0);
                    Touch touchOne = Input.GetTouch(1);

                    // Find the position in the previous frame of each touch.
                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                    // Find the magnitude of the vector (the distance) between the touches in each frame.
                    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                    // Find the difference in the distances between each frame.
                    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                    return deltaMagnitudeDiff * PinchSensitivity;
                }
                else
                {
                    return Input.GetAxis(axisName);
                }

            default:
                Debug.LogError("Input <" + axisName + "> not recognyzed.", this);
                break;
        }

        return 0f;
    }
}
