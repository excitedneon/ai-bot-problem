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

using System.Collections.Generic;
using UnityEngine;

namespace AIBotProblem {
    public class SeekingAI : AIBase {
        private List<Vector2> path = null;
        private WorldTile seekingFor;
        private float distance;

        public SeekingAI(float direction, bool moving, Bot host, WorldTile seekingFor, float distance = 2.9f) : base(direction, moving, host) {
            this.seekingFor = seekingFor;
            this.distance = distance;
        }

        public override void InitAI() {
            path = host.hostWorld.GetPathBetween(host.transform.localPosition, seekingFor, host, distance);
        }

        public override void UpdateAI() {
            if (path == null) {
                return;
            }
            Vector2 currentPos = host.transform.localPosition;
            currentPos.x = Mathf.Round(currentPos.x);
            currentPos.y = Mathf.Round(currentPos.y);
            while (path.Count > 0 && path[0] != currentPos) {
                path.RemoveAt(0);
            }
            if (path.Count > 1) {
                Vector2 delta = path[1] - currentPos;
                if (delta.y < 0) {
                    direction = 270;
                }
                if (delta.y > 0) {
                    direction = 90;
                }
                if (delta.x < 0) {
                    direction = 180;
                }
                if (delta.x > 0) {
                    direction = 0;
                }
                Move();
            }
        }
    }
}
