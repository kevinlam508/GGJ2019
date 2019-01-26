using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSwitcher : MonoBehaviour
{
	private Image display;
	private RectTransform rect;

	[SerializeField] Sprite[] sprites;

    // Start is called before the first frame update
    void Start()
    {
    	display = GetComponent<Image>();
    	rect = GetComponent<RectTransform>();
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

    public void SetPosition(float x, float y){
    	rect.anchoredPosition = new Vector3(x, y);
    }
}
