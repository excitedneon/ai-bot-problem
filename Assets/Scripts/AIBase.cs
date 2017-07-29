using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
