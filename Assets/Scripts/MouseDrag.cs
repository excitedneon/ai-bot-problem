using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIBotProblem {
    public class MouseDrag : MonoBehaviour {
        public float speed = 1;

        void Update() {
            if (Input.GetButton("Interact")) {
                Vector3 movement = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * speed;
                transform.localPosition = transform.localPosition + movement;
            }
        }
    }
}