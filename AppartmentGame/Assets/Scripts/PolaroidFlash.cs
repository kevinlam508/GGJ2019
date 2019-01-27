using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolaroidFlash : MonoBehaviour
{
	private Image flash;
	private AudioSource sound;

	[SerializeField] float flashIncTime = .1f;
	[SerializeField] float flashDecTime = .5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Flash(){

    }

    IEnumerator WhiteScreen(){
    	Color c = Color.white;
    	c.a = 0;

    	yield break;
    }
}
