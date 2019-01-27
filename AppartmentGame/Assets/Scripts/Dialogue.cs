using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Exodrifter.Rumor.Engine;

public enum People { EDWARD, LAUREN, MAXINE, LILLI, SARAH, CHARLES, PLAYER };

public class Dialogue : MonoBehaviour
{

    // rumor script for the dialogue
    [SerializeField] Text textBox;
    [SerializeField] GameObject dialogueBox;
    [SerializeField] TextAsset rumorScript;

    // dialogue handling
	private Rumor rumor;

    // people sprites
    private People currentPerson;
    private float screenWidth;
    [SerializeField] SpriteSwitcher[] peopleModels;

    // backgrounds
    [SerializeField] SpriteSwitcher backgrounds;

    // choices
    private bool useButtons = true;
    [SerializeField] GameObject choices;
    private Text[] choiceText;

    // special choices
    [SerializeField] GameObject doors;
    [SerializeField] GameObject menu;

    // Start is called before the first frame update
    void Start()
    {
        screenWidth = ((RectTransform)textBox.gameObject
            .GetComponent<Transform>().parent
            .parent).rect.width;

        // hide people
        foreach(SpriteSwitcher s in peopleModels)
            s.HideSprite();

        // setup and hide choices
        choiceText = new Text[choices.transform.childCount];
        for(int i = 0; i < choiceText.Length; ++i){
            int temp = i;
            choices.transform.GetChild(i).GetComponent<Button>().onClick
                .AddListener(() => { 
                    Choose(temp); 
                    DisableChoices();
                    });
            choiceText[i] = choices.transform.GetChild(i)
                .GetChild(0).GetComponent<Text>();
        }
        DisableChoices();

        // set up doors
        for(int i = 0; i < doors.transform.childCount; ++i){
            int temp = i;
            doors.transform.GetChild(i).GetComponent<Button>().onClick
                .AddListener(() => { 
                    Choose(temp); 
                    DisableDoors();
                    });
        }
        DisableDoors();

        // set up menu
        for(int i = 0; i < menu.transform.childCount; ++i){
            int temp = i;
            menu.transform.GetChild(i).GetComponent<Button>().onClick
                .AddListener(() => { 
                    Choose(temp); 
                    DisableMenu();
                    });
        }
        DisableDoors();

#if UNITY_EDITOR
        if(rumorScript != null){
#endif
            // set up script
            rumor = new Rumor(rumorScript.text);

            // subscribe events
            rumor.State.OnSetDialog += OnSetDialogue;
            rumor.State.OnAddDialog += OnAddDialogue;
            rumor.State.OnClear += OnClear;
            rumor.State.OnAddChoice += OnAddChoice;
            rumor.State.OnRemoveChoice += OnRemoveChoice;

            // bind functions
            rumor.Bindings.Bind("GetEdwardVal", GetEdwardVal);
            rumor.Bindings.Bind("GetLaurenVal", GetLaurenVal);
            rumor.Bindings.Bind("GetLilliVal", GetLilliVal);
            rumor.Bindings.Bind("GetMaxineVal", GetMaxineVal);
            rumor.Bindings.Bind("GetSarahVal", GetSarahVal);
            rumor.Bindings.Bind("GetCharlesVal", GetCharlesVal);
            rumor.Bindings.Bind("StartingMinigame", StartingMinigame);
            rumor.Bindings.Bind<People, int, float>("ShowPerson", ShowPerson);
            rumor.Bindings.Bind<People>("HidePerson", HidePerson);
            rumor.Bindings.Bind<int>("ShowBackground", ShowBackground);
            rumor.Bindings.Bind<People>("UnfadePerson", UnfadePerson);
            rumor.Bindings.Bind<People>("FadePerson", FadePerson);
            rumor.Bindings.Bind("ClearPeople", ClearPeople);
            rumor.Bindings.Bind("EnableDoors", EnableDoors);

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

    // events
    void OnSetDialogue(object speaker, string text){
    	textBox.text = text;
        if(speaker is People){
            People person = (People)speaker;
            currentPerson = person;
            for(int i = 0; i < peopleModels.Length; ++i){
                if(peopleModels[i].IsVisible && i != (int)person){
                    FadePerson((People)i);
                }
            }
            UnfadePerson(person);
        }
        else {
            for(int i = 0; i < peopleModels.Length; ++i){
                if(peopleModels[i].IsVisible){
                    FadePerson((People)i);
                }
            }
        }
    }

    void OnAddDialogue(object speaker, string text){
        if(speaker is People && ((People)speaker) == currentPerson){
            textBox.text += text;
        }
    }

    void OnClear(ClearType type){
        textBox.text = "";
    }

    void OnAddChoice(int num, string text){
        if(useButtons){
            if(!choices.activeSelf)
                EnableChoices();

            choiceText[num].text = text;
        }
    }

    void OnRemoveChoice(int num, string text){
    }

    void EnableChoices(){
        choices.SetActive(true);
    }

    void DisableChoices(){
        choices.SetActive(false);
    }

    void EnableDialoguebox(){
        dialogueBox.SetActive(true);
    }

    void DisableDialoguebox(){
        dialogueBox.SetActive(false);
    }

    void EnableDoors(){
        ClearPeople();
        DisableDialoguebox();
        doors.SetActive(true);
        useButtons = false; // using special buttons
    }

    void DisableDoors(){
        EnableDialoguebox();
        doors.SetActive(false);
        useButtons = true; // stop special buttons
    }

    void EnableMenu(){
        ClearPeople();
        DisableDialoguebox();
        menu.SetActive(true);
        useButtons = false; // using special buttons
    }

    void DisableMenu(){
        EnableDialoguebox();
        menu.SetActive(false);
        useButtons = true; // stop special buttons
    }

    public void Choose(int n){
        rumor.Choose(n);
    }

    // script controls
    People GetEdwardVal(){
        return People.EDWARD;
    }
    People GetLaurenVal(){
        return People.LAUREN;
    }
    People GetMaxineVal(){
        return People.MAXINE;
    }
    People GetLilliVal(){
        return People.LILLI;
    }
    People GetSarahVal(){
        return People.SARAH;
    }
    People GetCharlesVal(){
        return People.CHARLES;
    }

    void StartingMinigame(){
        GameState.state = State.MINIGAME;
    }

    void ShowPerson(People person, int sprite, float relPos){
#if UNITY_EDITOR
        if((int)person > peopleModels.Length || (int)person < 0){
            DialogueError("Person " + (int)person + " no loaded");
        }
#endif

        peopleModels[(int)person].ShowSprite(sprite);
        peopleModels[(int)person].SetPosition(relPos * screenWidth);
    }

    void HidePerson(People person){
#if UNITY_EDITOR
        if((int)person > peopleModels.Length || (int)person < 0){
            DialogueError("Person " + (int)person + " no loaded");
        }
#endif
        peopleModels[(int)person].HideSprite();
    }

    void FadePerson(People person){
#if UNITY_EDITOR
        if((int)person > peopleModels.Length || (int)person < 0){
            DialogueError("Person " + (int)person + " no loaded");
        }
#endif
        peopleModels[(int)person].MakeFaded();
    }

    void UnfadePerson(People person){
#if UNITY_EDITOR
        if((int)person > peopleModels.Length || (int)person < 0){
            DialogueError("Person " + (int)person + " no loaded");
        }
#endif
        peopleModels[(int)person].MakeUnfaded();
    }

    void ShowBackground(int background){
        backgrounds.ShowSprite(background);     
    }

    void ClearPeople(){
        for(int i = 0; i < peopleModels.Length; ++i)
            HidePerson((People)i);
    }

#if UNITY_EDITOR
    void DialogueError(string msg){
        Debug.Log(msg);
        UnityEditor.EditorApplication.isPlaying = false;
    }
#endif
}
