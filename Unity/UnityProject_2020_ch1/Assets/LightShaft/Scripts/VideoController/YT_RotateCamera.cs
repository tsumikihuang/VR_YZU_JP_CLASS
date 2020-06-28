using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YT_RotateCamera : MonoBehaviour {

    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    void Update()
    {
        if (Input.GetMouseButton(0))
            {
            yaw += speedH * Input.GetAxis("Mouse X");
            pitch -= speedV * Input.GetAxis("Mouse Y");
            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }
        if ( Input.touchCount == 1)
            {
            yaw +=  Input.touches[0].deltaPosition.x;
            pitch -= Input.touches[0].deltaPosition.y;
            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }
    }
}
