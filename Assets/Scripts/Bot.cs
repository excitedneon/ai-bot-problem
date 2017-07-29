using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIBotProblem {
    public class Bot : MonoBehaviour {
        public World hostWorld;
        public AIType aiType;
        public float direction = 0;
        public bool moving = false;
        public GameObject hoverEffect;

        private AIBase AI;
        private float currentDirectionRadians = 0f;
        private float lastUpdateTime = -1;
        private Vector3 targetPosition;
        private AIType lastAIType = AIType.Dead;

        void Start() {
            UpdateAIType();
        }

        void Update() {
            if (Time.time - lastUpdateTime >= Globals.STEP_DELAY) {
                UpdateAIType();
                if (AI != null) {
                    AI.Stop();
                    AI.UpdateAI();
                    direction = AI.direction;
                    moving = AI.moving;

                    if (moving) {
                        Vector3 movement = new Vector3(Mathf.Cos(AI.direction * Mathf.Deg2Rad), Mathf.Sin(AI.direction * Mathf.Deg2Rad));
                        int x = Mathf.RoundToInt(transform.localPosition.x + movement.x);
                        int y = Mathf.RoundToInt(transform.localPosition.y + movement.y);
                        if (hostWorld.IsSpaceFree(x, y, this)) {
                            hostWorld.ExitTile(targetPosition);
                            targetPosition = transform.localPosition + movement;
                            hostWorld.EnterTile(targetPosition);
                        }
                    } 
                }
                lastUpdateTime = Time.time;
            }
            if (moving) {
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, .3f);
            }
            if (Mathf.Abs(currentDirectionRadians - direction) > 1) {
                currentDirectionRadians = Mathf.Lerp(currentDirectionRadians, direction, .3f);
            } else {
                currentDirectionRadians = direction;
            }
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y,
                currentDirectionRadians);
        }

        private void UpdateAIType() {
            if (lastAIType != aiType) {
                switch (aiType) {
                    case AIType.Dead:
                        AI = null;
                        break;
                    case AIType.SeekingWin:
                        AI = new SeekingAI(direction, moving, this, WorldTile.Generator);
                        break;
                    case AIType.SeekingDetector:
                        AI = new SeekingAI(direction, moving, this, WorldTile.Detector);
                        break;
                    case AIType.SeekingYellow:
                        AI = new SeekingAI(direction, moving, this, WorldTile.LightYellow, float.MaxValue);
                        break;
                    case AIType.SeekingRed:
                        AI = new SeekingAI(direction, moving, this, WorldTile.LightRed, float.MaxValue);
                        break;
                }
                if (AI != null) {
                    AI.InitAI();
                }
            }
            lastAIType = aiType;
        }
    }
}
