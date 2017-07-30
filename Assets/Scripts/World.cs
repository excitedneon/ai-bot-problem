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
    public enum WorldTile {
        Floor, Wall, Generator, LightRed, LightYellow, Door, Detector, WireHorizontal, WireVertical, WireOmni
    }

    public class PathDesc {
        public Vector2 start;
        public WorldTile target;

        public PathDesc(Vector2 start, WorldTile target) {
            this.start = start;
            this.target = target;
        }
        
        public override int GetHashCode() {
            return start.GetHashCode() * (int)target;
        }
    }

    public class World : MonoBehaviour {
        private static string[] levels = new string[]{
            "#######\n" +
            "# b x #\n" +
            "#######\n",

            "#############\n" +
            "# b     y x #\n" +
            "#############\n",

            "###########\n" +
            "# s---#   #\n" +
            "# b b dyx #\n" +
            "#     #   #\n" +
            "###########\n",

            "############\n" +
            "#   #      #\n" +
            "#yb d  r x #\n" +
            "#   #--+   #\n" +
            "#####  |   #\n" +
            "#      s   #\n" +
            "# b    y x #\n" +
            "#          #\n" +
            "############\n",

            "##############\n" +
            "#        #   #\n" +
            "# b    syd r #\n" +
            "#      +-#   #\n" +
            "#####  | #####\n" +
            "#   #--+  #vvv\n" +
            "# b d  y  #vvv\n" +
            "#   #     #vvv\n" +
            "##### #####vvv\n" +
            "vvvv#     #vvv\n" +
            "vvvv##### #vvv\n" +
            "vvvv#xr   #vvv\n" +
            "vvvv# x   #vvv\n" +
            "vvvv#######vvv\n"
        };

        public GameObject[] tilePrefabs;
        public bool[] tileSolid;
        public bool[] tilePowered;
        public GameObject botPrefab;

        public ConnectionBoard connectionBoard;

        private int currentLevel = 0;
        private Dictionary<PathDesc, List<Vector2>> cachedPaths = new Dictionary<PathDesc, List<Vector2>>();
        private WorldTile[] grid;
        private bool[] powerGrid;
        private List<Bot> bots;
        private Dictionary<int, Door> doors = new Dictionary<int, Door>();
        private Dictionary<int, Detector> detectors = new Dictionary<int, Detector>();
        private int width;
        private int height;
        private int generatorCount;
        private int generatorsOn = 0;
        private float winTime = -1;

        void Start() {
            Load(levels[currentLevel]);
        }

        void Update() {
            if (winTime < 0 && generatorCount > 0 && generatorsOn == generatorCount) {
                winTime = Time.time;
            }
            if (winTime > 0 && Time.time - winTime > .6f) {
                Load(levels[++currentLevel]);
                winTime = -1;
            }
            if (Input.GetButtonDown("Restart")) {
                Load(levels[currentLevel]);
            }
        }

        private void Load(string level) {
            // Clear the world
            for (int i = 0; i < transform.childCount; i++) {
                Destroy(transform.GetChild(i).gameObject);
            }
            connectionBoard.Reset();

            string[] rows = level.Split('\n');
            height = rows.Length;
            width = rows[0].Length;
            grid = new WorldTile[width * height];
            powerGrid = new bool[width * height];
            doors.Clear();
            detectors.Clear();
            bots = new List<Bot>();
            generatorCount = 0;
            generatorsOn = 0;
            int x = 0;
            int y = 0;
            foreach (string row in rows) {
                foreach (char cell in row.ToCharArray()) {
                    switch (cell) {
                        case 'b':
                            // Spawn bot
                            GameObject spawnedBot = Instantiate(botPrefab, transform);
                            spawnedBot.transform.localPosition = new Vector3(x, y);
                            Bot bot = spawnedBot.GetComponent<Bot>();
                            bot.hostWorld = this;
                            connectionBoard.AddBot(bot);
                            grid[x + y * width] = WorldTile.Floor;
                            break;
                        default:
                            grid[x + y * width] = WorldTile.Floor;
                            break;
                        case '#':
                            grid[x + y * width] = WorldTile.Wall;
                            break;
                        case 'x':
                            grid[x + y * width] = WorldTile.Generator;
                            generatorCount++;
                            break;
                        case 'r':
                            grid[x + y * width] = WorldTile.LightRed;
                            break;
                        case 'y':
                            grid[x + y * width] = WorldTile.LightYellow;
                            break;
                        case 's':
                            grid[x + y * width] = WorldTile.Detector;
                            break;
                        case 'd':
                            grid[x + y * width] = WorldTile.Door;
                            break;
                        case '-':
                            grid[x + y * width] = WorldTile.WireHorizontal;
                            break;
                        case '|':
                            grid[x + y * width] = WorldTile.WireVertical;
                            break;
                        case '+':
                            grid[x + y * width] = WorldTile.WireOmni;
                            break;
                    }
                    // This check makes the void tiles not appear graphically (while still being technically floors)
                    if (cell != 'v') {
                        GameObject tile = Instantiate(tilePrefabs[(int)grid[x + y * width]]);
                        tile.transform.parent = transform;
                        tile.transform.localPosition = new Vector3(x, y);
                        if (cell == 'd') {
                            doors[x + y * width] = tile.GetComponent<Door>();
                        }
                        if (cell == 's') {
                            detectors[x + y * width] = tile.GetComponent<Detector>();
                        }
                    }
                    x++;
                }
                x = 0;
                y++;
            }
        }

        public bool IsSpaceFree(int x, int y, Bot from) {
            if (x < 0 || x >= width || y < 0 || y >= height) {
                return false;
            }
            foreach (Bot bot in bots) {
                if (bot != from &&
                    Mathf.RoundToInt(bot.transform.localPosition.x) == x &&
                    Mathf.RoundToInt(bot.transform.localPosition.y) == y) {
                    return false;
                }
            }
            int index = x + y * width;
            if (doors.ContainsKey(index)) {
                return doors[index].toggle;
            }
            return !tileSolid[(int)grid[index]];
        }

        public void EnterTile(Vector2 position) {
            int x = Mathf.RoundToInt(position.x);
            int y = Mathf.RoundToInt(position.y);
            if (x < 0 || x >= width || y < 0 || y >= height) {
                return;
            }
            int index = x + y * width;
            if (detectors.ContainsKey(index)) {
                detectors[index].toggle = true;
                SpreadPower(new Vector2(x, y), true);
            }
            if (grid[index] == WorldTile.Generator) {
                generatorsOn++;
            }
        }

        public void ExitTile(Vector2 position) {
            int x = Mathf.RoundToInt(position.x);
            int y = Mathf.RoundToInt(position.y);
            if (x < 0 || x >= width || y < 0 || y >= height) {
                return;
            }
            int index = x + y * width;
            if (detectors.ContainsKey(index)) {
                detectors[index].toggle = false;
                SpreadPower(new Vector2(x, y), false);
            }
            if (grid[index] == WorldTile.Generator) {
                generatorsOn--;
            }
        }

        public void SpreadPower(Vector2 pos, bool powerOn) {
            powerGrid[(int)pos.x + (int)pos.y * width] = powerOn;
            foreach (Vector2 neighbor in GetNeighborPositions(pos, true)) {
                int index = (int)neighbor.x + (int)neighbor.y * width;
                WorldTile tile = grid[index];
                if (tile == WorldTile.Door) {
                    doors[index].toggle = powerOn;
                }
                if (tilePowered[(int)tile] && powerGrid[index] != powerOn) {
                    SpreadPower(neighbor, powerOn);
                }
            }
        }

        public List<Vector2> GetNeighborPositions(Vector2 origin, bool allowDiagonals) {
            List<Vector2> positions = new List<Vector2>();
            origin.x = Mathf.Round(origin.x);
            origin.y = Mathf.Round(origin.y);
            for (int i = 0; i < 9; i++) {
                int x = (i % 3) - 1;
                int y = (i / 3) - 1;
                bool centre = x == 0 && y == 0;
                bool diagonal = x != 0 && y != 0;
                if (!centre && (allowDiagonals || !diagonal)) {
                    positions.Add(new Vector2(x, y) + origin);
                }
            }
            return positions;
        }

        public List<Vector2> GetPathBetween(Vector2 start, WorldTile target, Bot from, float scanDistance = float.MaxValue) {
            // Ensure the start point is ints
            start.x = Mathf.Round(start.x);
            start.y = Mathf.Round(start.y);

            // If we're at the target, just return the starting point
            if (grid[(int)start.x + (int)start.y * width] == target) {
                List<Vector2> path = new List<Vector2>();
                path.Add(start);
                return path;
            }

            PathDesc desc = new PathDesc(start, target);
            if (!cachedPaths.ContainsKey(desc)) {
                // Calculate the path, it's not in the cache yet.
                // Note: This is pretty much copied and pasted from Wikipedia's "A* search algorithm" pseudocode example, but in C#.
                List<Vector2> closed = new List<Vector2>();
                List<Vector2> open = new List<Vector2>();
                open.Add(start);
                Dictionary<Vector2, Vector2> cameFrom = new Dictionary<Vector2, Vector2>();
                Dictionary<Vector2, float> scores = new Dictionary<Vector2, float>();
                scores[start] = 0;

                List<Vector2> path = null;
                while (open.Count > 0) {
                    Vector2 current = new Vector2();
                    float lowestScore = float.MaxValue;
                    foreach (Vector2 node in open) {
                        float score = scores[node];
                        if (score < lowestScore) {
                            lowestScore = score;
                            current = node;
                        }
                    }
                    if (grid[(int)current.x + (int)current.y * width] == target) {
                        // Reconstruct path
                        path = new List<Vector2>();
                        path.Add(current);
                        while (cameFrom.ContainsKey(current)) {
                            current = cameFrom[current];
                            path.Add(current);
                        }
                        path.Reverse();
                        break;
                    }

                    open.Remove(current);
                    closed.Add(current);

                    foreach (Vector2 neighbor in GetNeighborPositions(current, false)) {
                        bool outOfBounds = neighbor.x < 0 || neighbor.x >= width || neighbor.y < 0 || neighbor.y >= height;
                        bool wall = outOfBounds || !IsSpaceFree((int)neighbor.x, (int)neighbor.y, from);
                        bool tooFar = (neighbor - start).magnitude > scanDistance;
                        if (closed.Contains(neighbor) || wall || tooFar) {
                            continue;
                        }
                        if (!open.Contains(neighbor)) {
                            open.Add(neighbor);
                            scores[neighbor] = float.MaxValue;
                        }

                        float tentativeScore = scores[current] + 1;
                        if (tentativeScore >= scores[neighbor]) {
                            continue;
                        }

                        cameFrom[neighbor] = current;
                        scores[neighbor] = tentativeScore;
                    }
                }
                cachedPaths[desc] = path;
            }
            return cachedPaths[desc];
        }
    }
}