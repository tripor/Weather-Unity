using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextLookCamera : MonoBehaviour
{

    public new Camera camera;


    // Start is called before the first frame update
    void Start()
    {

    }

    void LateUpdate()
    {
        transform.LookAt(camera.transform);
        transform.Rotate(new Vector3(0, 180, 0));
    }
}
