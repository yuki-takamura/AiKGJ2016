
/* Enemy.cs */

using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public GameObject enemy;
    public const float interval = 5;
    private float time;
    float countTime;

    public static float rad;
    public static float angle;
	
	// Update is called once per frame
	void Update () {

        time += Time.deltaTime;
        countTime += Time.deltaTime;

        if (time >= interval && countTime < 30)
        {
            time = 0;
            CreatEnemy();
        }

	}

    void CreatEnemy()
    {
        angle = Random.Range(0, 360);
        rad = Random.Range(20, 40);

        Instantiate(enemy, new Vector3(Mathf.Cos(angle) * rad, 1f, Mathf.Sin(angle) * rad),
            Quaternion.identity);

    }

}
