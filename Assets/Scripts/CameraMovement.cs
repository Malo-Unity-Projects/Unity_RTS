using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float panSpeed;
    public float movementTime;
    public float panBorderThickness;
    Vector3 pos;

    private float minHeight = 10f;
    private float maxHeight = 20f;
    Vector3 newZoom;
    public Vector3 zoomAmount;

    void Start()
    {
        pos = transform.position;
        newZoom = Camera.main.transform.localPosition;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow)) {
            pos += (transform.forward * panSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            pos += (transform.forward * -panSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            pos += (transform.right * panSpeed);
        }
        if (Input.GetKey(KeyCode.Q) ||Input.GetKey(KeyCode.LeftArrow)) {
            pos += (transform.right * -panSpeed);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f && Camera.main.transform.localPosition.y > minHeight) {
            Debug.Log("cam height: " + Camera.main.transform.localPosition.y + " minHeight: " + minHeight);
            newZoom += zoomAmount;
        } else if (Input.GetAxis("Mouse ScrollWheel") < 0f && Camera.main.transform.localPosition.y < maxHeight) {
            Debug.Log("decrease");
            newZoom -= zoomAmount;
        }

        Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, newZoom, Time.deltaTime * movementTime);
        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * movementTime);
    }
}
