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

namespace AIBotProblem {
    public enum AIType {
        Dead, SeekingWin, SeekingDetector, SeekingYellow, SeekingRed
    }

    public class AIBase {
        public float direction;
        public bool moving;
        public Bot host;

        public AIBase(float direction, bool moving, Bot host) {
            this.direction = direction;
            this.moving = moving;
            this.host = host;
        }
        
        public void Move() {
            moving = true;
        }

        public void Stop() {
            moving = false;
        }

        public virtual void UpdateAI() {}
        public virtual void InitAI() {}
    }
}
