using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        //private Vector3 offset;

        private void Awake()
        {
            //offset = player.transform.position - Camera.main.transform.position;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            //Camera.main.transform.position += (player.transform.position - Camera.main.transform.position) - offset;
            transform.position = target.position;
        }
    }
}
