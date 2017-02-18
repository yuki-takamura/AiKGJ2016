
/* LightPos.cs 
    collision circle */

using UnityEngine;
using System.Collections;

public class LightPos : MonoBehaviour {

    private float angle = 89.5f;
    private float rad = 10.0f;

    private float speed_X = 0.0174f;
    private float speed_Z = 0.5f;
    

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (Timer.countStop == false)
        {
            this.transform.position = new Vector3(Mathf.Cos(angle) * rad, 0.1f, Mathf.Sin(angle) * rad);

            if (Input.GetKey(KeyCode.LeftArrow))
                angle += speed_X ;
            if (Input.GetKey(KeyCode.RightArrow))
                angle -= speed_X;
            if (Input.GetKey(KeyCode.UpArrow) && rad < 20)
                rad += speed_Z;
            if (Input.GetKey(KeyCode.DownArrow) && rad > 10)
                rad -= speed_Z;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        GameObject.Destroy(other.gameObject);
        Score.score += 100;
    }
}
