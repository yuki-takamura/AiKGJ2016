
/* Timer.cs */

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    private float time = 30;
    public static bool countStop = false;

	// Use this for initialization
	void Start () {

        GetComponent<Text>().text = ((int)time).ToString();

    }
	
	// Update is called once per frame
	void Update () {

        time -= Time.deltaTime;

        if (time < 0)
        {
            time = 0;
            countStop = true;
        }
        GetComponent<Text>().text = ((int)time).ToString();

	}
}
