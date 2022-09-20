using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player;
    public Transform orientation;

    // Update is called once per frame
    void Update()
    {
        this.transform.position = player.transform.position;
        this.transform.rotation = orientation.rotation;
    }
}
