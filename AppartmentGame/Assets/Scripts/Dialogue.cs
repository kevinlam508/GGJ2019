using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Exodrifter.Rumor.Engine;

public enum People { EDWARD, LAUREN, MAXINE, LILLI, SARAH, CHARLES, PLAYER };
public enum Background {
    YOUR_ROOM_DAY,
    YOUR_ROOM_NIGHT,
    STRINGLIGHTS_DAY,
    STRINGLIGHTS_NIGHT,
    HALLWAY,
    ROOM_201_CLOSED, 
    ROOM_201_OPEN, 
    ROOM_203_CLOSED, 
    ROOM_203_OPEN, 
    ROOM_205_CLOSED, 
    ROOM_205_OPEN, 
    ROOM_207_CLOSED, 
    ROOM_207_OPEN, 
    SNURGLARS_REGISTER, 
    SNURGLARS_TABLE, 
    ARCADE, 
    MUSEUM, 
    EDWARD_ROOM,
    NIGHT_TRANSITION,
    BLACK,
    ARCADE_GAME
};

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
    [SerializeField] GameObject arcade;
    [SerializeField] GameObject bedroom;

    // progress tracking
    private static int NO_TRINKET = 0;
    private int[] trinketState;
    int numTrinkets = 0;
    [SerializeField] GameObject trinkets;
    [SerializeField] SpriteSwitcher polaroid;

    // minigames
    [SerializeField] GameObject foodJenga;

    // sound mixer
    [SerializeField] SoundMixer soundMixer;

    [SerializeField] AudioSource knocking;
    [SerializeField] AudioSource doorOpen;
    [SerializeField] AudioSource click;
    [SerializeField] AudioSource timer;
    [SerializeField] AudioSource cameraFlash;

    // Start is called before the first frame update
    void Start()
    {
        trinketState = new int[4];
        polaroid.HideSprite();

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

        // set up special
        SetUpSpecialButtons(doors);
        SetUpSpecialButtons(menu);
        SetUpSpecialButtons(arcade);
        SetUpSpecialButtons(bedroom);


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

            // minigame
            rumor.Bindings.Bind("StartingMinigame", StartingMinigame);
            rumor.Bindings.Bind("StartFoodJenga", StartFoodJenga);
            rumor.Bindings.Bind("IsWinMinigame", IsWinMinigame);

            rumor.Bindings.Bind<int, int, float>("ShowPerson", ShowPerson);
            rumor.Bindings.Bind<int>("HidePerson", HidePerson);
            rumor.Bindings.Bind<string>("ShowBackgroundByName",
                ShowBackgroundByName);
            rumor.Bindings.Bind<int>("UnfadePerson", UnfadePerson);
            rumor.Bindings.Bind<int>("FadePerson", FadePerson);
            rumor.Bindings.Bind("ClearPeople", ClearPeople);

            // buttons
            rumor.Bindings.Bind("EnableDialoguebox", EnableDialoguebox);
            rumor.Bindings.Bind("DisableDialoguebox", DisableDialoguebox);
            rumor.Bindings.Bind("EnableDoors", 
                () => { EnableSpecialButtons(doors); });
            rumor.Bindings.Bind("EnableMenu", 
                () => { EnableSpecialButtons(menu); });
            rumor.Bindings.Bind("EnableArcade", 
                () => { EnableSpecialButtons(arcade); });
            rumor.Bindings.Bind("EnableBedroom", 
                () => { EnableSpecialButtons(bedroom); });
            rumor.Bindings.Bind("GetPlayerName", GetPlayerName);
            rumor.Bindings.Bind("SetPlayerName", SetPlayerName);

            // trinkets
            rumor.Bindings.Bind<int, bool>("IsFinished", IsFinished);
            rumor.Bindings.Bind<int, int>("GetTrinket", GetTrinket);

            // sound
            rumor.Bindings.Bind<int>("SwitchToTrack", 
                (i) => {soundMixer.SwitchToTrack((AudioTracks)i); });

            rumor.Bindings.Bind<int>("ShowPolaroid", ShowPolaroid);
            rumor.Bindings.Bind("HidePolaroid", HidePolaroid);
            rumor.Bindings.Bind("Knocking", () => { knocking.Play(); });
            rumor.Bindings.Bind("DoorOpen", () => { doorOpen.Play(); });
            rumor.Bindings.Bind("Timer", () => { timer.Play(); });
            rumor.Bindings.Bind("CameraFlash", () => { cameraFlash.Play(); });

            StartCoroutine(rumor.Start());


#if UNITY_EDITOR
        }
        else {
            DialogueError("No rumor script on " + gameObject + "'s component");
        }
#endif
        // set up food jenga
        foodJenga.GetComponent<FoodJengaManager>().OnEnd += EnableDialoguebox;
        foodJenga.GetComponent<FoodJengaManager>().OnEnd += rumor.Advance;
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
        click.Play();
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
            click.Play();
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
                    click.Play();
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

    // minigames
    void StartingMinigame(){
        GameState.state = State.MINIGAME;
    }

    void StartFoodJenga(){
        GameState.state = State.MINIGAME;
        foodJenga.SetActive(true);
        DisableDialoguebox();
        DisableBackground();
    }

    bool IsWinMinigame(){
        return GameState.res == MinigameResult.WIN;
    }

    void ShowPerson(int person, int sprite, float relPos){
#if UNITY_EDITOR
        if(person > (1 << peopleModels.Length) || person < 0){
            DialogueError("Person flag error");
        }
#endif

        peopleModels[(int)flagToPerson[person]].ShowSprite(sprite);
        peopleModels[(int)flagToPerson[person]].SetPosition((relPos - .5f) * screenWidth);
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

    void ShowBackgroundByName(string name){
        Debug.Log(name);
        HideTrinkets();
        switch(name){
            case "your_room_day":
                ShowBackground(Background.YOUR_ROOM_DAY);
                ShowTrinkets();
                break;
            case "your_room_night":
                ShowBackground(Background.YOUR_ROOM_NIGHT);
                ShowTrinkets();
                break;
            case "stringlights_day":
                ShowBackground(Background.STRINGLIGHTS_DAY);
                break;
            case "stringlights_night":
                ShowBackground(Background.STRINGLIGHTS_NIGHT);
                break;
            case "hallway":
                ShowBackground(Background.HALLWAY);
                break;
            case "201_closed":
                ShowBackground(Background.ROOM_201_CLOSED);
                break;
            case "201_open":
                ShowBackground(Background.ROOM_201_OPEN);
                break;
            case "203_closed":
                ShowBackground(Background.ROOM_203_CLOSED);
                break;
            case "203_open":
                ShowBackground(Background.ROOM_203_OPEN);
                break;
            case "205_closed":
                ShowBackground(Background.ROOM_205_CLOSED);
                break;
            case "205_open":
                ShowBackground(Background.ROOM_205_OPEN);
                break;
            case "207_closed":
                ShowBackground(Background.ROOM_207_CLOSED);
                break;
            case "207_open":
                ShowBackground(Background.ROOM_207_OPEN);
                break;
            case "snurglars_register":
                ShowBackground(Background.SNURGLARS_REGISTER);
                break;
            case "snurglars_table":
                ShowBackground(Background.SNURGLARS_TABLE);
                break;
            case "arcade":
                ShowBackground(Background.ARCADE);
                break;
            case "museum":
                ShowBackground(Background.MUSEUM);
                break;
            case "edward_room":
                ShowBackground(Background.EDWARD_ROOM);
                break;
            case "night_transition":
                ShowBackground(Background.NIGHT_TRANSITION);
                break;
            case "black_background":
                ShowBackground(Background.BLACK);
                break;
            case "arcade_game":
                ShowBackground(Background.ARCADE_GAME);
                break;

        }
    }

    bool IsFinished(int doorNum){
        return trinketState[doorNum] != NO_TRINKET;
    }

    void GetTrinket(int doorNum, int trinket){
        trinketState[doorNum] = trinket;
        numTrinkets++;
    }

    void ShowBackground(Background background){
        EnableBackground();
        backgrounds.ShowSprite((int)background);     
    }

    void DisableBackground(){
        backgrounds.gameObject.SetActive(false);
    }
    void EnableBackground(){
        backgrounds.gameObject.SetActive(true);
    }

    void ShowPolaroid(int idx){
        polaroid.ShowSprite(idx);
    }
    void HidePolaroid(){
        polaroid.HideSprite();
    }

    void ClearPeople(){
        for(int i = 0; i < peopleModels.Length; ++i)
            HidePerson(1 << i);
    }

    void HideTrinkets(){
        trinkets.SetActive(false);
    }

    void ShowTrinkets(){
        trinkets.SetActive(true);
        foreach(Transform t in trinkets.transform){
            t.gameObject.SetActive(false);
        }
        for(int i = 0; i < trinketState.Length; ++i){
            if(trinketState[i] != NO_TRINKET){
                trinkets.transform.GetChild((2 * i) - 1 + trinketState[i])
                    .gameObject.SetActive(true);
            }
        }
    }

#if UNITY_EDITOR
    void DialogueError(string msg){
        Debug.Log(msg);
        UnityEditor.EditorApplication.isPlaying = false;
    }
#endif
}
