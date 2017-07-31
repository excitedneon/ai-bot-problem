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
    public class ConnectionBoard : MonoBehaviour {
        public SfxPlayer plugInSfx;
        public SfxPlayer plugOutSfx;
        public GameObject linePrefab;
        public int inputCount = 6;
        public World world;

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
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
            if (hit.collider != null) {
                if (hit.collider.name.Equals("IOCollider")) {
                    string name = hit.collider.transform.parent.name;
                    bool isIn = name.StartsWith("In");
                    int num = isIn ? int.Parse(name.Substring(2)) : int.Parse(name.Substring(3));
                    if (isIn && num < bots.Count) {
                        CameraManager.Set(bots[num].transform.localPosition);
                    }
                    if (Input.GetButtonDown("Interact")) {
                        Click(num + (isIn ? 0 : inputCount));
                    }
                }
                if (hit.collider.name.StartsWith("Menu") && Input.GetButtonUp("Interact")) {
                    if (hit.collider.name.EndsWith("0") && world.GetCurrentLevel() == 0) {
                        world.Play();
                    }
                    if (hit.collider.name.EndsWith("1") && world.GetCurrentLevel() == 0) {
                        world.Quit();
                    }
                    if (hit.collider.name.EndsWith("1") && world.GetCurrentLevel() == world.GetLevelCount() - 1) {
                        world.MainMenu();
                    }
                }
            }
            
            if (line != null) {
                if (Input.GetButton("Interact Secondary")) {
                    Destroy(line.gameObject);
                    line = null;
                    lastIn = false;
                    lastNum = -1;
                    plugOutSfx.Play();
                    if (lastIn) {
                        bots[lastNum].aiType = AIType.Dead;
                    }
                } else if (hit.collider != null) {
                    Vector3 point = hit.point;
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
                bool shouldPickupLine = line == null && (!isIn || num < bots.Count);
                if (shouldPickupLine && createdLines[n] == null) {
                    GameObject newLine = Instantiate(linePrefab);
                    line = newLine.GetComponent<LineRenderer>();
                    line.positionCount = 2;
                    line.SetPosition(0, position);
                    line.SetPosition(1, position);
                    plugInSfx.Play();
                } else if (shouldPickupLine) {
                    line = createdLines[n];
                    createdLines[n] = null;
                    if (line.GetPosition(0) == position) {
                        line.SetPosition(0, line.GetPosition(1));
                    }
                    int index = GetIndexFor(line.GetPosition(0));
                    createdLines[index] = null;
                    lastIn = index < inputCount;
                    lastNum = index % inputCount;
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
                        return int.Parse(child.name.Substring(3)) + inputCount;
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
