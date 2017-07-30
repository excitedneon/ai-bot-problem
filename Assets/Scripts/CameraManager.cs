/* AI-Bot Problem is a puzzle game where you switch AIs between bots.
 * Copyright (C) 2017  Jens Pitkänen
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using UnityEngine;

namespace AIBotProblem {
    public class CameraManager : MonoBehaviour {
        private static Vector3 position = new Vector3(3, 3, -7);
        private static float zoomLevel = 7;

        public Camera cam;
        public Vector3 offset;


        void Update() {
            zoomLevel = Mathf.Clamp(zoomLevel - Input.GetAxis("Mouse ScrollWheel") * 3, 1, 11);
            position.z = -zoomLevel;
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, position + offset, 0.3f);
        }

        public static float GetZoom() {
            return zoomLevel;
        }
        
        public static void Set(Vector2 position) {
            CameraManager.position = position;
        }

        public static void Move(Vector2 moveAmount) {
            position.x += moveAmount.x;
            position.y += moveAmount.y;
        }
    }
}
