using UnityEngine;

[System.Serializable]
public struct Clamp
{
    public float min;
    public float max;
}

public class TouchCameraControl : MonoBehaviour
{
    public float TouchSensitivity_x = 0.1f;
    public float TouchSensitivity_y = 0.01f;
    public float PinchSensitivity = 1.0f;

    public Clamp yRotationClamp;
    public Clamp yTranslateClamp;
    public Clamp zoomClamp;

    public void Update()
    {
        if (Input.touchCount == 1)
        {
            float x = Input.touches[0].deltaPosition.x * TouchSensitivity_x;
            float yRotation = Mathf.Clamp(transform.rotation.eulerAngles.y + x, yRotationClamp.min, yRotationClamp.max);
            transform.rotation = Quaternion.Euler(new Vector3(0.0f, yRotation, 0.0f));
        }
        else if (Input.touchCount == 2)
        {
            float y = Input.touches[0].deltaPosition.y * -TouchSensitivity_y * Time.unscaledDeltaTime;
            float yPos = Mathf.Clamp(transform.position.y + y, yTranslateClamp.min, yTranslateClamp.max);
            transform.position = new Vector3(0.0f, yPos, 0.0f);

            Vector2 touchZeroPrevPos = Input.touches[0].position - Input.touches[0].deltaPosition;
            Vector2 touchOnePrevPos = Input.touches[1].position - Input.touches[1].deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (Input.touches[0].position - Input.touches[1].position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            float zoom = Camera.main.fieldOfView + deltaMagnitudeDiff * PinchSensitivity * Time.unscaledDeltaTime;
            Camera.main.fieldOfView = Mathf.Clamp(zoom, zoomClamp.min, zoomClamp.max);
        }
    }
}
