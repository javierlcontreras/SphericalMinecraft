using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplestCharacterContoller : MonoBehaviour
{
    Transform characterTransform;
    // Start is called before the first frame update
    void Start()
    {
        characterTransform = this.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {   
        Vector3 mouse = Input.mousePosition;
        float width = mouse.x;
        float height = mouse.y;

        Vector3 dir = Vector3.zero;
        if (Input.GetKey("w")) dir += Vector3.forward;
        if (Input.GetKey("s")) dir += Vector3.back;
        if (Input.GetKey("a")) dir += Vector3.left;
        if (Input.GetKey("d")) dir += Vector3.right;
        if (Input.GetKey("space")) dir += Vector3.up;
        if (Input.GetKey(KeyCode.LeftShift)) dir += Vector3.down;
        dir = Vector3.Normalize(dir);

        characterTransform.Translate(Time.deltaTime*dir*10);
    }
}
