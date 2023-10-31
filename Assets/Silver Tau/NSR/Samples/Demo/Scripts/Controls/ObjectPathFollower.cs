using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SilverTau.NSR.Samples
{
    public class ObjectPathFollower : MonoBehaviour
    {
        [SerializeField] private float speed = 3f;
        [SerializeField] private Transform pathParent;
        private Transform _targetPoint;
        private int _index;

        #region Editor

        private void OnDrawGizmos()
        {
            for (int i = 0; i < pathParent.childCount; i++)
            {
                var from = pathParent.GetChild(i).position;
                var to = pathParent.GetChild((i+1) % pathParent.childCount).position;
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(from, to);
            }
        }

        #endregion

        #region MonoBehaviour

        private void Start () {
            _index = 0;
            _targetPoint = pathParent.GetChild (_index);
        }

        private void Update () {
            transform.position = Vector3.MoveTowards (transform.position, _targetPoint.position, speed * Time.deltaTime);
            if (Vector3.Distance (transform.position, _targetPoint.position) < 0.1f) 
            {
                _index++;
                _index %= pathParent.childCount;
                _targetPoint = pathParent.GetChild (_index);
            }
        }

        #endregion
    }
}
