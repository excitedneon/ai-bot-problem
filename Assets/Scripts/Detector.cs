using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : Toggleable {
    void Update() {
        if (toggle) {
            Vector3 scale = transform.localScale;
            scale = Vector3.Lerp(scale, new Vector3(1.25f, 1.25f, 1.25f), 0.4f);
            transform.localScale = scale;
        } else {
            Vector3 scale = transform.localScale;
            scale = Vector3.Lerp(scale, new Vector3(1f, 1f, 1f), 0.6f);
            transform.localScale = scale;
        }
    }
}
