using System;
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

	// progress tracking
	private int[] eatenCount;
    HashSet<int> finishedIdx;

    [SerializeField] SpriteSwitcher2 lifeVisual;

    public event Action OnEnd;

    // sound
    private AudioSource crunchAudio;
    [SerializeField] AudioClip[] crunch;
    private AudioSource disgustAudio;
    [SerializeField] AudioClip[] disgust;

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
        gameObject.SetActive(false);

    	crunchAudio = GetComponents<AudioSource>()[0];
    	disgustAudio = GetComponents<AudioSource>()[1];
    }

    // Update is called once per frame
    void Update()
    {
    	if(GameState.state == State.MINIGAME){
	    	lifeVisual.ShowSprite(Mathf.Max(lives, 0));
	        if(lives == 0){
	        	GameState.state = State.DIALOGUE;
	        	GameState.res = MinigameResult.LOSE;
	        	DisableClick();
	        	OnEnd();
	        }
	        else if(foods.transform.childCount == finishedIdx.Count){
	        	GameState.state = State.DIALOGUE;
	        	GameState.res = MinigameResult.WIN;
	        	DisableClick();
	        	OnEnd();
	        }
    	}
    }

    public void Eaten(int foodIdx){
    	if(foodIdx != targetIdx){
    		lives--;
    		PlayDisgust();
    	}
    	else {
    		PlayCrunch();
    	}
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
    	targetIdx = UnityEngine.Random.Range(0, foods.transform.childCount);
    	while(foods.transform.childCount != finishedIdx.Count && 
    		foods.transform.GetChild(targetIdx).childCount == 
    		eatenCount[targetIdx]){

    		finishedIdx.Add(targetIdx);
    		targetIdx = UnityEngine.Random.Range(0, foods.transform.childCount);
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
    	PlayDisgust();
    	lives--;
    	EatFood(col.gameObject.GetComponent<FoodJengaFood>().foodIdx);
    	RandomTarget();
    }

    void EatFood(int food){
    	eatenCount[food]++;
    }

    void PlayCrunch(){
    	crunchAudio.clip = crunch[UnityEngine.Random.Range(0, crunch.Length)];
    	crunchAudio.Play();
    }

    void PlayDisgust(){
    	disgustAudio.clip = disgust[UnityEngine.Random.Range(0, disgust.Length)];
    	disgustAudio.Play();
    }
}
