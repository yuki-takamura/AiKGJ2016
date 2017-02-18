using UnityEngine;
using System.Collections;

public class enemyMove : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {

        this.transform.position = new Vector3(Mathf.Cos(Enemy.angle) * Enemy.rad, 1f,
            Mathf.Sin(Enemy.angle) * Enemy.rad);

        if (Enemy.rad > 0)
            Enemy.rad -= 0.05f;
    }
}
