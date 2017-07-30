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
    public class Detector : Toggleable {
        void Update() {
            if (toggle) {
                Vector3 scale = transform.localScale;
                scale = Vector3.Lerp(scale, new Vector3(1.25f, 1.25f, 1.25f), 0.4f);
                transform.localScale = scale;
            } else {
                Vector3 scale = transform.localScale;
                scale = Vector3.Lerp(scale, new Vector3(1f, 1f, 1f), 0.6f);
                transform.localScale = scale;
            }
        }
    }
}
