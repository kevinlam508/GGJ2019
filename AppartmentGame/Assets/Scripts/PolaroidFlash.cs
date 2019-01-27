using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolaroidFlash : MonoBehaviour
{
	private Image flash;

	[SerializeField] float flashIncTime = .1f;
	[SerializeField] float flashDecTime = .5f;

    // Start is called before the first frame update
    void Awake()
    {
        flash = transform.GetChild(0).GetComponent<Image>();
        flash.color = Color.clear;
        flash.gameObject.SetActive(false);
    }

    public void Flash(){
    	StartCoroutine(WhiteScreen());
    }

    IEnumerator WhiteScreen(){
        flash.gameObject.SetActive(true);
    	Color c = Color.white;
    	c.a = 0;
    	while(c.a < 1){
    		c.a += (1.0f / flashIncTime * Time.deltaTime);
    		flash.color = c;
    		yield return null;
    	}
		while(c.a > 0){
    		c.a -= (1.0f / flashDecTime * Time.deltaTime);
    		flash.color = c;
    		yield return null;
    	}
        flash.gameObject.SetActive(false);

    	yield break;
    }
}
