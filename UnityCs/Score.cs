
/* Score.cs */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Score : MonoBehaviour {

    public static float score = 0;
    Text text;

	// Use this for initialization
	void Start () {

        text = GetComponent<Text>();

	}
	
	// Update is called once per frame
	void Update () {

        text.text = "SCORE : " + score.ToString();

	}

}
