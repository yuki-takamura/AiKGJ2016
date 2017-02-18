
/* GameResult.cs */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameResult : MonoBehaviour {

    public GameObject parts;
  

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (Timer.countStop)
        {
            parts.SetActive(true);
        }

	}

    void OnRetry()
    {
        SceneManager.LoadScene("Main");
    }
}
