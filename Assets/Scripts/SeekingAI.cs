using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIBotProblem {
    public class SeekingAI : AIBase {
        private List<Vector2> path = null;
        private WorldTile seekingFor;
        private float distance;

        public SeekingAI(float direction, bool moving, Bot host, WorldTile seekingFor, float distance = 2) : base(direction, moving, host) {
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
