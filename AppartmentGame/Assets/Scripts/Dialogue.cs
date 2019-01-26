using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Exodrifter.Rumor.Engine;

public class Dialogue : MonoBehaviour
{

    // rumor script for the dialogue
    [SerializeField] TextAsset rumorScript;

    // dialogue handling
	private Rumor rumor;
    private Text textBox;
    private object currentSpeaker;

    // people sprites
    private float screenWidth;
    [SerializeField] SpriteSwitcher[] peopleModels;

    // backgrounds
    [SerializeField] SpriteSwitcher backgrounds;

    // Start is called before the first frame update
    void Start()
    {
        textBox = GetComponent<Text>();
        screenWidth = ((RectTransform)GetComponent<RectTransform>().parent
            .parent)
            .rect.width;

        foreach(SpriteSwitcher s in peopleModels)
            s.HideSprite();
            
#if UNITY_EDITOR
        if(rumorScript != null){
#endif
            // set up script
            rumor = new Rumor(rumorScript.text);

            // subscribe events
            rumor.State.OnSetDialog += OnSetDialogue;
            rumor.State.OnAddDialog += OnAddDialogue;
            rumor.State.OnClear += OnClear;

            // bind functions
            rumor.Bindings.Bind("StartingMinigame", StartingMinigame);
            rumor.Bindings.Bind<int, int, float>("ShowPerson", ShowPerson);
            rumor.Bindings.Bind<int>("HidePerson", HidePerson);
            rumor.Bindings.Bind<int>("ShowBackground", ShowBackground);

            // start dialogue
            StartCoroutine(rumor.Start());

#if UNITY_EDITOR
        }
        else {
            DialogueError("No rumor script on " + gameObject + "'s component");
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if(rumor != null && GameState.state == State.DIALOGUE){
            rumor.Update(Time.deltaTime);
            if(Input.GetButtonDown("Submit")){
                rumor.Advance();
            }
        }

        if(Input.GetButtonDown("Cancel")){
            GameState.state = State.DIALOGUE;
        }
    }

    void OnSetDialogue(object speaker, string text){
    	textBox.text = text;
        currentSpeaker = speaker;
    }

    void OnAddDialogue(object speaker, string text){
        if(speaker == currentSpeaker){
            textBox.text += text;
        }
    	
    }

    void OnClear(ClearType type){
        textBox.text = "";
    }

    void StartingMinigame(){
        GameState.state = State.MINIGAME;
    }

    void ShowPerson(int person, int sprite, float relPos){
#if UNITY_EDITOR
        if(person > peopleModels.Length || person < 0){
            DialogueError("Person " + person + " no loaded");
        }
#endif
        peopleModels[person].ShowSprite(sprite);
        peopleModels[person].SetPosition(relPos * screenWidth, 0);
    }

    void HidePerson(int person){
#if UNITY_EDITOR
        if(person > peopleModels.Length || person < 0){
            DialogueError("Person " + person + " no loaded");
        }
#endif
        peopleModels[person].HideSprite();
    }

    void ShowBackground(int background){
        backgrounds.ShowSprite(background);     
    }

#if UNITY_EDITOR
    void DialogueError(string msg){
        Debug.Log(msg);
        UnityEditor.EditorApplication.isPlaying = false;
    }
#endif
}
