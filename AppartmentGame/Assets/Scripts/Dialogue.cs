using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Exodrifter.Rumor.Engine;

public enum People { EDWARD, LAUREN, MAXINE, LILLI, SARAH, CHARLES, PLAYER };

public class Dialogue : MonoBehaviour
{
    // player name
    private bool playerNameSet = false;
    private string playerName = "";
    [SerializeField] InputField nameField;

    // rumor script for the dialogue
    [SerializeField] Text textBox;
    [SerializeField] GameObject dialogueBox;
    [SerializeField] TextAsset rumorScript;

    // dialogue handling
	private Rumor rumor;

    // people sprites
    private int currentPeople;
    private float screenWidth;
    [SerializeField] SpriteSwitcher[] peopleModels;
    private Dictionary<int, People> flagToPerson;

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
        SetUpSpecialButtons(doors);

        // set up menu
        SetUpSpecialButtons(menu);

#if UNITY_EDITOR
        if(rumorScript != null){
#endif
            InitPeopleDictionary();

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
            rumor.Bindings.Bind<int, int, float>("ShowPerson", ShowPerson);
            rumor.Bindings.Bind<int>("HidePerson", HidePerson);
            rumor.Bindings.Bind<int>("ShowBackground", ShowBackground);
            rumor.Bindings.Bind<int>("UnfadePerson", UnfadePerson);
            rumor.Bindings.Bind<int>("FadePerson", FadePerson);
            rumor.Bindings.Bind("ClearPeople", ClearPeople);
            rumor.Bindings.Bind("EnableDoors", 
                () => { EnableSpecialButtons(doors); });
            rumor.Bindings.Bind("EnableMenu", 
                () => { EnableSpecialButtons(menu); });
            rumor.Bindings.Bind("GetPlayerName", GetPlayerName);
            rumor.Bindings.Bind("SetPlayerName", SetPlayerName);

            StartCoroutine(rumor.Start());

#if UNITY_EDITOR
        }
        else {
            DialogueError("No rumor script on " + gameObject + "'s component");
        }
#endif
    }

    void InitPeopleDictionary(){
        flagToPerson = new Dictionary<int, People>();
        for(int i = 0; i < (int)People.PLAYER; ++i){
            flagToPerson.Add(1 << i, (People)i);
        }
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
        if(speaker is int){
            int flag = (int)speaker;
            currentPeople = flag;
            for(int i = 0; i < peopleModels.Length; ++i){
                if(peopleModels[i].IsVisible && ((1 << i) & flag) == 0){
                    FadePerson(1 << i);
                }
            }
            UnfadePerson(flag);
        }
        else {
            for(int i = 0; i < peopleModels.Length; ++i){
                if(peopleModels[i].IsVisible){
                    FadePerson(1 << i);
                }
            }
        }
    }

    void OnAddDialogue(object speaker, string text){
        if(speaker is int){
            if(((1 << ((int)speaker)) & currentPeople) != 0){
                textBox.text += text;
            }
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

    void EnableSpecialButtons(GameObject buttonParent){
        ClearPeople();
        DisableDialoguebox();
        buttonParent.SetActive(true);
        useButtons = false; // using special buttons
    }

    void DisableSpecialButton(GameObject buttonParent){
        EnableDialoguebox();
        buttonParent.SetActive(false);
        useButtons = true; // stop special buttons
    }

    void SetUpSpecialButtons(GameObject buttonParent){
        for(int i = 0; i < buttonParent.transform.childCount; ++i){
            int temp = i;
            buttonParent.transform.GetChild(i).GetComponent<Button>().onClick
                .AddListener(() => { 
                    Choose(temp); 
                    DisableSpecialButton(buttonParent);
                    });
        }
        DisableSpecialButton(buttonParent);
    }

    public void Choose(int n){
        rumor.Choose(n);
    }

    // script controls
    int GetEdwardVal(){
        return 1 << (int)People.EDWARD;
    }
    int GetLaurenVal(){
        return 1 << (int)People.LAUREN;
    }
    int GetMaxineVal(){
        return 1 << (int)People.MAXINE;
    }
    int GetLilliVal(){
        return 1 << (int)People.LILLI;
    }
    int GetSarahVal(){
        return 1 << (int)People.SARAH;
    }
    int GetCharlesVal(){
        return 1 << (int)People.CHARLES;
    }

    string GetPlayerName(){
        return playerName;
    }
    void SetPlayerName(){
        playerName = nameField.text;
        nameField.gameObject.SetActive(false);
    }


    void StartingMinigame(){
        GameState.state = State.MINIGAME;
    }

    void ShowPerson(int person, int sprite, float relPos){
#if UNITY_EDITOR
        if(person > (1 << peopleModels.Length) || person < 0){
            DialogueError("Person flag error");
        }
#endif

        peopleModels[(int)flagToPerson[person]].ShowSprite(sprite);
        peopleModels[(int)flagToPerson[person]].SetPosition(relPos * screenWidth);
    }

    void HidePerson(int person){
#if UNITY_EDITOR
        if(person > (1 << peopleModels.Length) || person < 0){
            DialogueError("Person flag error");
        }
#endif
        peopleModels[(int)flagToPerson[person]].HideSprite();
    }

    void FadePerson(int people){
#if UNITY_EDITOR
        if(people > (1 << peopleModels.Length) || people < 0){
            DialogueError("Person flag error");
        }
#endif
        for(int i = 0; i < (int)People.PLAYER; ++i){
            int currentFlag = (1 << i) & people;
            if(currentFlag != 0){
                People current = flagToPerson[currentFlag];
                if(peopleModels[(int)current].IsVisible){
                    peopleModels[(int)current].MakeFaded();
                }
            }
        }
    }

    void UnfadePerson(int people){
#if UNITY_EDITOR
        if(people > (1 << peopleModels.Length) || people < 0){
            DialogueError("Person flag error");
        }
#endif
        for(int i = 0; i < (int)People.PLAYER; ++i){
            int currentFlag = (1 << i) & people;
            if(currentFlag != 0){
                People current = flagToPerson[currentFlag];
                if(peopleModels[(int)current].IsVisible){
                    peopleModels[(int)current].MakeUnfaded();
                }
            }
        }
    }

    void ShowBackground(int background){
        backgrounds.ShowSprite(background);     
    }

    void ClearPeople(){
        for(int i = 0; i < peopleModels.Length; ++i)
            HidePerson(1 << i);
    }

#if UNITY_EDITOR
    void DialogueError(string msg){
        Debug.Log(msg);
        UnityEditor.EditorApplication.isPlaying = false;
    }
#endif
}
