using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSwitcher : MonoBehaviour
{
	private Image display;
	private RectTransform rect;
    private float defaultY;

	[SerializeField] Sprite[] sprites;
    [SerializeField] Color fadedColor;

    // Start is called before the first frame update
    void Awake()
    {
    	display = GetComponent<Image>();
    	rect = GetComponent<RectTransform>();
        display.sprite = sprites[0];
        defaultY = rect.anchoredPosition.y;
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

    public void SetPosition(float x){
    	rect.anchoredPosition = new Vector3(x, defaultY);
    }

    public void MakeFaded(){
        display.color = fadedColor;
    }

    public void MakeUnfaded(){
        display.color = Color.white;
    }
}
