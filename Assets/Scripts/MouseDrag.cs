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
    public class MouseDrag : MonoBehaviour {
        public CameraZoomer cameraZoomer;
        public float speed = 1;

        void Update() {
            if (Input.GetButton("Interact")) {
                Vector3 movement = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * speed * cameraZoomer.GetMultiplier();
                transform.localPosition = transform.localPosition + movement;
            }
        }
    }
}