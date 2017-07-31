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
    public class Tutorial : MonoBehaviour {
        // The tutorial screen is 16x8 characters.
        // Horizontal sample for quicker checking:
        //  "----------------\n"
        private static string[] tutorials = new string[] {
            "The bot is\n" +
            "connected to the\n" +
            "'Bot A' input.\n" +
            "The outputs on\n" +
            "the right\n" +
            "control it.",

            "Success!",

            "The bots don't\n" +
            "see very far.\n" +
            "\n" +
            "But they do know\n" +
            "where the\n" +
            "closest lights\n" +
            "are.\n",

            "The bots can\n" +
            "toggle switches\n" +
            "by standing on\n" +
            "them.",

            "Sometimes you\n" +
            "need more than a\n" +
            "single bot to\n" +
            "get the power\n" +
            "running.",

            "You can press R\n" +
            "to reset the\n" +
            "level.",

            "<color=#6c6>You won!\n" +
            "The game is a\n" +
            "bit short, but\n" +
            "hopefully it was\n" +
            "enjoyable.</color>\n" +
            "\n" +
            "<color=#8e8> - Neon</color>"
        };
        //  "----------------\n"
        
        public TextMesh text;
        public World world;

        private int currentTutorial = -1;

        void Update() {
            currentTutorial = -1;
            if (world.GetCurrentLevel() == 2) {
                currentTutorial = 0;
            }
            if (world.GetCurrentLevel() == 3) {
                currentTutorial = 2;
            }
            if (world.GetCurrentLevel() == 4) {
                currentTutorial = 3;
            }
            if (world.GetCurrentLevel() == 5) {
                currentTutorial = 4;
            }
            if (world.GetCurrentLevel() == 6) {
                currentTutorial = 5;
            }
            if (world.GetCurrentLevel() == 8) {
                currentTutorial = 6;
            }
            if (world.IsWinning() && world.GetCurrentLevel() != 1) {
                currentTutorial = 1;
            }
            UpdateDisplay();
        }

        private void UpdateDisplay() {
            if (currentTutorial < 0 || currentTutorial >= tutorials.Length) {
                text.text = "";
            } else {
                text.text = tutorials[currentTutorial];
            }
        }
    }
}
