using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SilverTau.NSR.Samples
{
    [RequireComponent(typeof(Camera))]
    public class CameraControl : MonoBehaviour
    {
        public bool canRotate;
        [SerializeField] [Range(-2.0f, 2.0f)] private float xRotationSpeed = 0.8f;
        [SerializeField] [Range(-2.0f, 2.0f)] private float yRotationSpeed = 0.8f;

        private Camera _targetCamera;
        
        private void Start()
        {
            
#if !PLATFORM_STANDALONE && !UNITY_EDITOR
            xRotationSpeed /= 2;
            yRotationSpeed /= 2;
#endif
            if (Camera.main)
            {
                _targetCamera = Camera.main;
                return;
            }

            var mainCamera = GameObject.FindWithTag("Main Camera");
            if (!mainCamera) return;
            if (!mainCamera.TryGetComponent<Camera>(out var resultCamera)) return;
            _targetCamera = resultCamera;
            return;
        }

        private void Update()
        {
            if (canRotate)
            {
                RotateCamera();
            }
        }
        
        private void RotateCamera()
        {
            if (!Input.GetMouseButton(0)) return;
            var xDeg = Input.GetAxis("Mouse X") * xRotationSpeed * 100 * -1 * Time.deltaTime;
            var yDeg = Input.GetAxis("Mouse Y") * yRotationSpeed * 100 * -1 * Time.deltaTime;
        
            if(_targetCamera == null) return;
            _targetCamera.transform.Rotate(new Vector3(yDeg, -xDeg,0));
            var z = _targetCamera.transform.eulerAngles.z;
            _targetCamera.transform.Rotate(0, 0, -z);
        }
    }
}
