using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIBotProblem {
    public class SfxPlayer : MonoBehaviour {
        public AudioClip[] clips;
        public AudioSource player;

        public void Play() {
            player.PlayOneShot(clips[Random.Range(0, clips.Length)]);
        }
    }
}
