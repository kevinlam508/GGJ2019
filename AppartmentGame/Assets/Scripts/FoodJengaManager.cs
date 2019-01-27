using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodJengaManager : MonoBehaviour
{
	// all food
	private GameObject foods;

	// target food
	private int targetIdx;
	[SerializeField] Color untargetColor;

	// game logic
	[SerializeField] float clickDelay = 1f;
	private int lives = 3;

	private int[] eatenCount;
    HashSet<int> finishedIdx;

    // Start is called before the first frame update
    void Start()
    {
        foods = transform.GetChild(1).gameObject;
        for(int i = 0; i < foods.transform.childCount; ++i){
        	foreach(Transform t in foods.transform.GetChild(i)){
        		t.gameObject.GetComponent<FoodJengaFood>().foodIdx = i;
        	}
        }
        eatenCount = new int[foods.transform.childCount];
        finishedIdx = new HashSet<int>();
        RandomTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if(lives == 0){
        	GameState.state = State.DIALOGUE;
        	GameState.res = MinigameResult.LOSE;
        	DisableClick();
        }
        else if(foods.transform.childCount == finishedIdx.Count){
        	GameState.state = State.DIALOGUE;
        	GameState.res = MinigameResult.WIN;
        	DisableClick();
        }
    }

    public void Eaten(int foodIdx){
    	EatFood(foodIdx);
    	RandomTarget();
    	StartCoroutine(DisableClicking());
    }

    IEnumerator DisableClicking(){
    	// disable temporarily
        DisableClick();
        yield return new WaitForSeconds(clickDelay);

        // reenable
        foreach(Transform food in foods.transform){
        	foreach(Transform obj in food){
        		obj.gameObject.GetComponent<FoodJengaFood>()
        			.Clickable = true;
        	}
        }
        yield break;
    }

    void DisableClick(){
    	foreach(Transform food in foods.transform){
        	foreach(Transform obj in food){
        		obj.gameObject.GetComponent<FoodJengaFood>()
        			.Clickable = false;
        	}
        }
    }

    void ResetColor(){
        foreach(Transform food in foods.transform){
        	foreach(Transform obj in food){
        		obj.gameObject.GetComponent<FoodJengaFood>()
        			.ChangeColor(untargetColor);
        	}
        }
    }

    void RandomTarget(){
    	// deletes food groups that are empty while picking next target
    	targetIdx = Random.Range(0, foods.transform.childCount);
    	while(foods.transform.childCount != finishedIdx.Count && 
    		foods.transform.GetChild(targetIdx).childCount == 
    		eatenCount[targetIdx]){

    		finishedIdx.Add(targetIdx);
    		targetIdx = Random.Range(0, foods.transform.childCount);
    	}

    	if(foods.transform.childCount != finishedIdx.Count){
	    	// retints foods
	    	ResetColor();
	    	foreach(Transform obj in foods.transform.GetChild(targetIdx)){
	    		obj.gameObject.GetComponent<FoodJengaFood>()
	    			.ChangeColor(Color.white);
	    	}
	    }
    }

    void OnTriggerEnter2D(Collider2D col){
    	lives--;
    	EatFood(col.gameObject.GetComponent<FoodJengaFood>().foodIdx);
    	RandomTarget();
    }

    void EatFood(int food){
    	eatenCount[food]++;
    }
}
