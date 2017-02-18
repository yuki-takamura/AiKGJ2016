
/* MoveCamera.cs */

using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour {

    private float num;
    private float angle_X;

	// Use this for initialization
	void Start () {

        num = 0f;
        angle_X = 30.0f;
	
	}
	
	// Update is called once per frame
	void Update () {
	
        if(Timer.countStop  == false)
        {
            Quaternion q = Quaternion.Euler(angle_X, num, 0);
            transform.rotation = q;

            if (Input.GetKey(KeyCode.LeftArrow))
                num -= 1;
            if (Input.GetKey(KeyCode.RightArrow))
                num += 1;
        }

	}
}
