using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodJengaFood : MonoBehaviour
{

	[HideInInspector] public int foodIdx;
	private SpriteRenderer renderer;
	private bool clickable = true;

	void Awake(){
		renderer = GetComponent<SpriteRenderer>();
	}

	public bool Clickable{
		get { return clickable; }
		set { clickable = value; } 
	}

    void OnMouseDown(){
    	if(clickable){
    		transform.parent.parent.parent.gameObject
    			.GetComponent<FoodJengaManager>().Eaten(foodIdx);
    		gameObject.SetActive(false);
    	}
    }

    public void ChangeColor(Color color){
    	renderer.color = color;
    }
}
