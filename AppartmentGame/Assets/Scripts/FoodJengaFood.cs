using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodJengaFood : MonoBehaviour
{

	[HideInInspector] public int foodIdx;
	private SpriteRenderer rend;
	private bool clickable = true;

	void Awake(){
		rend = GetComponent<SpriteRenderer>();
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
    	rend.color = color;
    }
}
