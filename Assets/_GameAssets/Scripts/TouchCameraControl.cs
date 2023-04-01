using FasilkomUI.Tutorial;
using UnityEngine;

namespace FasilkomUI
{
    [System.Serializable]
    public struct Clamp
    {
        public float min;
        public float max;
    }

    public class TouchCameraControl : MonoBehaviour
    {
        public static TouchCameraControl Instance { get; private set; }

        [SerializeField] Camera[] m_cameras;

        public float TouchSensitivity_x = 0.1f;
        public float TouchSensitivity_y = 0.01f;
        public float PinchSensitivity = 1.0f;

        public Clamp yRotationClamp;
        public Clamp yTranslateClamp;
        public Clamp zoomClamp;

        [Header("Editor Only")]
        public float MouseSensitivity_x = 1.0f;
        public float MouseSensitivity_y = 0.01f;
        public float ScrollSensitivity = 5.0f;
        Vector3 m_lastMousePosition;

        #region Unity's Callback
        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (UITutorial.Instance && UITutorial.Instance.gameObject.activeSelf)
                return;

            if (UITextProcessing.Instance.IsUIDictionaryActive)
                return;

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
                UpdateAllCamerasZoom(zoom);
            }

#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                m_lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                float x = (Input.mousePosition.x - m_lastMousePosition.x) * MouseSensitivity_x;
                float yRotation = Mathf.Clamp(transform.rotation.eulerAngles.y + x, yRotationClamp.min, yRotationClamp.max);
                transform.rotation = Quaternion.Euler(new Vector3(0.0f, yRotation, 0.0f));
            }
            else if (Input.GetMouseButton(1))
            {
                float y = (Input.mousePosition.y - m_lastMousePosition.y) * MouseSensitivity_y;
                float yPos = Mathf.Clamp(transform.position.y + y, yTranslateClamp.min, yTranslateClamp.max);
                transform.position = new Vector3(0.0f, yPos, 0.0f);
            } else
            {
                float scrollDeltaY = -Input.mouseScrollDelta.y;
                float zoom = Camera.main.fieldOfView + scrollDeltaY;
                foreach (Camera camera in m_cameras)
                    camera.fieldOfView = Mathf.Clamp(zoom, zoomClamp.min, zoomClamp.max);
            }
#endif
        }
        #endregion

        public float GenerateCameraZoomPercentage()
        {
            return (Camera.main.fieldOfView - zoomClamp.min) / (zoomClamp.max - zoomClamp.min);
        }

        public void UpdateAllCamerasZoom(float zoom)
        {
            foreach (Camera camera in m_cameras)
                camera.fieldOfView = Mathf.Clamp(zoom, zoomClamp.min, zoomClamp.max);
        }
    }
}