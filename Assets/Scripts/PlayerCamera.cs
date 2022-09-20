using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float xRotation, yRotation, sensitivity;
    public float mousePitch = 0f;
    public Transform orientation;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
        // float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;
        //
        // yRotation += (mouseX);
        // xRotation -= (mouseY);
        // xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        //
        // this.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        // orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        //
        // float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        // float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        // yRotation += mouseX;
        // xRotation -= mouseY;
        // xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        //
        // orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        // this.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        
        var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        mousePitch -= mouseDelta.y * sensitivity;
        
        mousePitch = Mathf.Clamp(mousePitch, -90f, 90f);
        
        orientation.Rotate(Vector3.up * (mouseDelta.x * sensitivity));
        
        this.transform.localRotation = Quaternion.Euler(Vector3.right * mousePitch + Vector3.up * (mouseDelta.x * sensitivity));

    }
}
