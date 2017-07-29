using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIBotProblem {
    public class ConnectionBoard : MonoBehaviour {
        public SfxPlayer plugInSfx;
        public SfxPlayer plugOutSfx;
        public GameObject linePrefab;
        public int inputCount = 6;
        public RectTransform[] botButtons;

        private List<Bot> bots;
        private int lastNum = -1;
        private bool lastIn = false;
        private LineRenderer line;
        private LineRenderer[] createdLines;

        public ConnectionBoard() {
            bots = new List<Bot>();
            createdLines = new LineRenderer[inputCount * 2];
            Reset();
        }

        private void Update() {
            for (int i = 0; i < botButtons.Length; i++) {
                if (RectTransformUtility.RectangleContainsScreenPoint(botButtons[i], Input.mousePosition, Camera.main)) {
                    if (i < bots.Count) {
                        bots[i].hoverEffect.SetActive(true);
                    }
                } else {
                    if (i < bots.Count) {
                        bots[i].hoverEffect.SetActive(false);
                    }
                }
            }
            if (line != null) {
                if (Input.GetButton("Interact Secondary")) {
                    if (lastIn) {
                        bots[lastNum].aiType = AIType.Dead;
                    }
                    Destroy(line.gameObject);
                    line = null;
                    lastIn = false;
                    lastNum = -1;
                    plugOutSfx.Play();
                } else {
                    Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    point.z = line.GetPosition(1).z;
                    line.SetPosition(1, point);
                }
            }
        }

        public void Reset() {
            for (int i = 0; i < createdLines.Length; i++) {
                if (createdLines[i] != null) Destroy(createdLines[i].gameObject);
                createdLines[i] = null;
            }
            if (line != null) {
                Destroy(line.gameObject);
            }
            bots.Clear();
            lastNum = -1;
            lastIn = false;
        }

        public void AddBot(Bot bot) {
            bots.Add(bot);
        }

        public void Click(int n) {
            int num = n % inputCount;
            bool isIn = n < inputCount;
            Vector3 position = GetPositionFor(isIn, num);
            if (isIn != lastIn && lastNum != -1) {
                int nIn = isIn ? num : lastNum;
                int nOut = isIn ? lastNum : num;
                if (Connect(nIn, nOut)) {
                    line.SetPosition(1, position);
                    createdLines[nIn] = createdLines[nOut + inputCount] = line;
                    line = null;
                    lastIn = false;
                    lastNum = -1;
                    plugInSfx.Play();
                }
            } else {
                lastIn = isIn;
                lastNum = num;
                if (line == null && createdLines[n] == null) {
                    GameObject newLine = Instantiate(linePrefab);
                    line = newLine.GetComponent<LineRenderer>();
                    line.positionCount = 2;
                    line.SetPosition(0, position);
                    line.SetPosition(1, position);
                    plugInSfx.Play();
                } else if (line == null) {
                    line = createdLines[n];
                    createdLines[n] = null;
                    if (line.GetPosition(0) == position) {
                        line.SetPosition(0, line.GetPosition(1));
                    }
                    int index = GetIndexFor(line.GetPosition(0));
                    createdLines[index] = null;
                    lastIn = index < 6;
                    lastNum = index % 6;
                    if (isIn) {
                        bots[num].aiType = AIType.Dead;
                    } else {
                        bots[lastNum].aiType = AIType.Dead;
                    }
                    plugOutSfx.Play();
                }
            }
        }

        private Vector3 GetPositionFor(bool isIn, int num) {
            return GameObject.Find(name + "/" + (isIn ? "In" : "Out") + num).transform.position;
        }

        private int GetIndexFor(Vector3 pos) {
            for (int i = 0; i < transform.childCount; i++) {
                Transform child = transform.GetChild(i);
                if (pos == child.position) {
                    if (child.name.StartsWith("In")) {
                        return int.Parse(child.name.Substring(2));
                    } else if (child.name.StartsWith("Out")) {
                        return int.Parse(child.name.Substring(3)) + 6;
                    }
                }
            }
            Debug.Log("Couldn't find an input at " + pos + ", something is very wrong!");
            return -1;
        }

        private bool Connect(int nIn, int nOut) {
            if (bots.Count > nIn && createdLines[nIn] == null && createdLines[nOut + inputCount] == null) {
                bots[nIn].aiType = (AIType)(nOut + 1);
                return true;
            }
            return false;
        }
    }
}
