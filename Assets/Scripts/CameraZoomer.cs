using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIBotProblem {
    public class CameraZoomer : MonoBehaviour {
        public Camera cam;

        private float zoomLevel = -3;

        void Update() {
            zoomLevel = Mathf.Clamp(zoomLevel + Input.GetAxis("Mouse ScrollWheel"), -5, 1);
            cam.orthographicSize = GetMultiplier();
        }

        public float GetMultiplier() {
            return Mathf.Pow(2, -zoomLevel);
        }
    }
}
