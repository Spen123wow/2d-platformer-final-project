using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform player;

    private void Update()
    {
        transform.position = new Vector3(player.position.x, VerticalMin(), transform.position.z);
    }

    private float VerticalMin()
    {
        float yMin = -4f;
        if (player.position.y <= -4f)
        {
            return yMin;
        }
        else
        {
            return player.position.y;
        }

    }
}
