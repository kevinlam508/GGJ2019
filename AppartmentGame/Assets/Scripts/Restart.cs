using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)){
        	GameState.state = State.DIALOGUE;
        	GameState.res = MinigameResult.LOSE;
        	SceneManager.LoadScene(0);
        }
        if(Input.GetKeyDown(KeyCode.Escape)){
        	Application.Quit();
        }
    }
}
