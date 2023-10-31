using UnityEngine;

namespace SilverTau.NSR.Samples
{
    public class RotateObject : MonoBehaviour
    {
        [SerializeField] private Vector3 rotateAmount = new Vector3(90, 180, 45);
        [SerializeField] private float speed = 0.3f;
 
        private void Update () 
        {
            transform.Rotate(rotateAmount * (Time.deltaTime * speed));
        }
    }
}
