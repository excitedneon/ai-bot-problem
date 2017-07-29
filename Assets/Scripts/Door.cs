using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Toggleable {
    public Transform door;

    void Update() {
        if (toggle) {
            Vector3 pos = door.localPosition;
            pos.z = Mathf.Lerp(pos.z, 1.1f, 0.5f);
            door.localPosition = pos;
        } else {
            Vector3 pos = door.localPosition;
            pos.z = Mathf.Lerp(pos.z, 0, 0.5f);
            door.localPosition = pos;
        }
    }
}
