
/* HouseLight.cs */

using UnityEngine;
using System.Collections;

public class HouseLight : MonoBehaviour {
	private float num;
	private float rad;

	// Use this for initialization
	void Start () {
		num = 1.0f;
		rad = 45.0f;
	}

    // Update is called once per frame
    void Update()
    {
        if (Timer.countStop == false)
        {
            Quaternion q = Quaternion.Euler(rad, num, num);
            transform.rotation = q;

            if (Input.GetKey(KeyCode.LeftArrow))
                num -= 1;
            if (Input.GetKey(KeyCode.RightArrow))
                num += 1;
            if (Input.GetKey(KeyCode.UpArrow) && rad > 20)
                rad--;
            if (Input.GetKey(KeyCode.DownArrow) && rad < 45)
                rad++;
        }
    }
}
