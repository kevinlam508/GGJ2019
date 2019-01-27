using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSwitcher2 : MonoBehaviour
{
	private SpriteRenderer display;
    private float defaultY;

	[SerializeField] Sprite[] sprites;
    [SerializeField] Color fadedColor;

    // Start is called before the first frame update
    void Awake()
    {
    	display = GetComponent<SpriteRenderer>();
        display.sprite = sprites[0];
    }

    public void ShowSprite(int i){
#if UNITY_EDITOR
	    if(i > sprites.Length && i < 0){
	        Debug.Log(gameObject + " does not have a sprite " + i);
	        UnityEditor.EditorApplication.isPlaying = false;
	    }
#endif

    	display.enabled = true;
	    display.sprite = sprites[i];
    }

    public void HideSprite(){
    	display.enabled = false;
    }

    public bool IsVisible{
        get { return display.enabled; }
    }
}
