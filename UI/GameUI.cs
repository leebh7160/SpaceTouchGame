using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;


enum FADEWHERE
{
    StageToMain, MainToStage, PauseToMain, PauseToStage, ClearToMain, StageToStage, MainToDev, DevToMain, EndToDev
}

public class GameUI : MonoBehaviour
{
    //===========================참조
    private Indicator indicator;
    private KeyClass keyClass;

    //===========================상황에 따른 UI
    private Transform playUI;                               //게임 진행 중 플레이어 UI
    private TextMeshProUGUI playBoostText;                      //플레이어 부스터 UI
    private TextMeshProUGUI playShootText;                      //플레이어 사격 UI
    private Button playRefrashBtn;                          //스테이지 재시작 버튼
    private Button playPauseBtn;                            //스테이지 재시작 버튼
    private Transform playTriggerUp;                          //플레이어 트리거 실행
    private Transform playTriggerDown;                          //플레이어 트리거 실행

    private Transform playQuestOBJ;                         //플레이어 퀘스트 UI
    private TextMeshProUGUI playQuestText;                  //플레이어 퀘스트 Text

    private Slider playHPSlider;                             //플레이어 HP 확인 바 오브젝트
    private Image playHitAlert;
 //===============================


    private Transform clearUI;                              //스테이지 클리어 시 UI
    private Button clearNextStageBtn;                       //스테이지 이동

    private Transform pauseUI;                              //일시정지 누를 시 UI
    private GameObject pauseText;                           //일시정지 글씨
    private GameObject gameoverText;                        //게임오버 글씨
    private Button pauseMainBtn;                            //메인화면 이동
    private Button pauseRefrashBtn;                         //스테이지 재시작 버튼
    private Button pauseStageBtn;                           //스테이지 돌아가기 버튼

    private Transform mainStageUI;                          //게임 실행 시 메인 UI
    private Button mainStartGameBtn;                        //게임 시작 버튼
    private Button mainOptionBtn;                           //게임 옵션 버튼
    private Button mainDevBtn;                              //제작자 소개 버튼
    private Button mainQuitBtn;                             //게임 끄기 버튼

    private Transform optionUI;                             //옵션 버튼 UI
    private Slider optionSound;
    private Button optionSave;
    private Button optionCancle;
    private Image optionWrong;
    private Dropdown optionDropdown;
    private int resolutionNum = 0;

    private Transform stageUI;                              //스테이지 버튼 UI
    private Transform stageFirst;
    private Transform stageSecond;
    private List<GameObject> stageButtonList;                   //스테이지 리스트
    [SerializeField] private GameObject stageBtn_Pre;       //스테이지 버튼 프리팹
    private Button stageMainBtn;                            //메인화면 돌아가기 버튼
    private Button stageStageBtn_Next;                      //다음 스테이지 버튼
    private Button stageStageBtn_Back;                      //이전 스테이지 버튼

    private Image FadeGraphic;

    private DataSave dataSave;
    private DataXMLStream dataXMLStream;

    private Transform ScreenEventObj;
    private Animator ScreenEventAni;
    private AudioSource ScreenEventSound;

    private Transform developeUI;
    private Button developeMainBtn;
    private Image devEndImage;

    private Transform refrashUI;
    private Button refrashBtn;
    //===========================세팅 변수
    [SerializeField] private int stageNum = 0;
    [SerializeField] private int clickstage = 0;

    //===========================KeyObject
    [Header("KeyObject")]
    [Space]
    [SerializeField]
    [Tooltip("누를 키의 이름")] private List<string> key_Name_List;
    [SerializeField]
    [Tooltip("키와 대응하는 키보드위치")] private List<KeyCode> key_Code_List;

    private Transform key_Setting_Alert;
    private Button moveKeyBtn;
    private Button shootKeyBtn;
    private bool iskeyChange = false;
    private GameObject btnObject;
    //===========================KeyObject^^

    private KeyCode movekey;
    private KeyCode shootkey;

    private void Start()
    {
        StartCoroutine(Init());
        //StageUI_stageLoad();
    }
    private IEnumerator Init()
    {
        //===================================참조
        indicator               = this.GetComponent<Indicator>();
        keyClass                = new KeyClass();
        //===================================참조^^

        FadeGraphic             = this.transform.Find("FadeGrapic").GetComponent<Image>();

        playUI                  = this.transform.Find("PlayUI");
        playBoostText           = playUI.Find("BoostUI").GetChild(1).GetComponent<TextMeshProUGUI>();
        playShootText           = playUI.Find("ShootUI").GetChild(1).GetComponent<TextMeshProUGUI>();
        playRefrashBtn          = playUI.Find("RefrashButton").GetComponent<Button>();
        playPauseBtn            = playUI.Find("PauseButton").GetComponent<Button>();
        playHPSlider            = playUI.Find("HPUI").Find("HPSlider").GetComponent<Slider>();
        playTriggerUp           = this.transform.Find("TriggerUp");
        playTriggerDown         = this.transform.Find("TriggerDown");
        playHitAlert            = this.transform.Find("HitAlert").GetComponent<Image>();
        playRefrashBtn.onClick.AddListener(PlayerUI_Replay);
        playPauseBtn.onClick.AddListener(PlayerUI_Pause);

        playQuestOBJ            = playUI.Find("QuestUI");
        playQuestText           = playQuestOBJ.GetChild(1).GetComponent<TextMeshProUGUI>();

        clearUI                 = this.transform.Find("ClearUI");
        clearNextStageBtn       = clearUI.Find("NextStageBtn").GetComponent<Button>();
        clearNextStageBtn.onClick.AddListener(ClearUI_NextStage);

        pauseUI                 = this.transform.Find("PauseUI");
        pauseText               = pauseUI.Find("PauseText").gameObject;
        gameoverText            = pauseUI.Find("GameOverText").gameObject;
        pauseText.SetActive(false);
        gameoverText.SetActive(false);
        pauseMainBtn            = pauseUI.Find("MainMenuBtn").GetComponent<Button>();
        pauseRefrashBtn         = pauseUI.Find("RefrashBtn").GetComponent<Button>();
        pauseStageBtn           = pauseUI.Find("StageBtn").GetComponent<Button>();
        pauseMainBtn.onClick.AddListener(PauseUI_Main);
        pauseRefrashBtn.onClick.AddListener(PauseUI_Replay);
        pauseStageBtn.onClick.AddListener(PauseUI_Stage);

        mainStageUI             = this.transform.Find("MainStageUI");
        mainStartGameBtn        = mainStageUI.Find("StartBtn").GetComponent<Button>();
        mainOptionBtn           = mainStageUI.Find("OptionBtn").GetComponent<Button>();
        mainDevBtn              = mainStageUI.Find("DevBtn").GetComponent<Button>();
        mainQuitBtn             = mainStageUI.Find("QuitBtn").GetComponent<Button>();
        mainStartGameBtn.onClick.AddListener(MainUI_Start);
        mainOptionBtn.onClick.AddListener(MainUI_Option);
        mainDevBtn.onClick.AddListener(MainUI_DevBtn);
        mainQuitBtn.onClick.AddListener(MainUI_QuitBtn);

        optionUI                = this.transform.Find("OptionUI");
        optionWrong             = optionUI.Find("WrongSave").GetComponent<Image>();
        optionSound             = optionUI.Find("Sound").GetChild(1).GetComponent<Slider>();
        optionSave              = optionUI.Find("Save").GetComponent<Button>();
        optionCancle            = optionUI.Find("Cancel").GetComponent<Button>();
        optionDropdown          = optionUI.Find("DropdownResolution").Find("Dropdown").GetComponent<Dropdown>();
        refrashBtn              = optionUI.Find("RefrashBtn").Find("RefrashBtn").GetComponent<Button>();

        optionSound.onValueChanged.AddListener(delegate { SettingUI_SoundValue(); });
        refrashBtn.onClick.AddListener(delegate { SettingUI_RefrashStage(); });

        optionSave.onClick.AddListener(SettingUI_Save);
        optionCancle.onClick.AddListener(SettingUI_Close);
        SettingUI_Resolution();//해상도 세팅


        stageUI = this.transform.Find("StageUI");
        stageFirst              = stageUI.Find("FirstStageButton");
        stageSecond             = stageUI.Find("SecondStageButton");
        stageButtonList         = new List<GameObject>();
        stageMainBtn            = stageUI.Find("BackMainButton").GetComponent<Button>();
        stageStageBtn_Next      = stageUI.Find("NextStageButton").GetComponent<Button>();
        stageStageBtn_Back      = stageUI.Find("BackStageButton").GetComponent<Button>();
        stageMainBtn.onClick.AddListener(StageUI_MainBtn);
        stageStageBtn_Next.onClick.AddListener(StageUI_NextStage);
        stageStageBtn_Back.onClick.AddListener(StageUI_BackStage);
        stageStageBtn_Back.gameObject.SetActive(false);

        developeUI              = this.transform.Find("GameEndUI");
        developeMainBtn         = developeUI.Find("MainBtn").GetComponent<Button>();
        developeMainBtn.onClick.AddListener(DevUI_Main);

        devEndImage             = this.transform.Find("EndImage").GetComponent<Image>();
       
        dataSave = new DataSave();
        dataXMLStream = GameObject.Find("DataManager").GetComponent<DataXMLStream>();

        ScreenEventObj = GameObject.Find("BrokeSpaceShip").transform;
        ScreenEventAni = ScreenEventObj.GetComponent<Animator>();
        ScreenEventSound = ScreenEventObj.GetComponent<AudioSource>();
        StageUi_ButtonSetting();

        key_Setting_Alert       = optionUI.Find("KeyAlert");
        moveKeyBtn              = optionUI.transform.Find("MoveBtn").Find("Button").GetComponent<Button>();
        shootKeyBtn             = optionUI.transform.Find("ShootBtn").Find("Button").GetComponent<Button>();
        moveKeyBtn.onClick.AddListener(Key_Click_Event);
        shootKeyBtn.onClick.AddListener(Key_Click_Event);
        yield return null;
    }

    #region 플레이어 UI
    internal void PlayerUI_Init(int boost, int shoot)       //플레이어 UI 세팅
    {
        PlayerUI_SetBoost(boost);
        PlayerUI_SetShoot(shoot);
    }

    internal void PlayerUI_Active(bool active)              //플레이어 UI 실행
    {
        playUI.gameObject.SetActive(active);
    }

    public void PlayerUI_Replay()                           //다시시작 버튼 누름
    {
        if (playUI == null)
            return;
        if (playUI.gameObject == null)
            return;
        if(playUI.gameObject.activeInHierarchy == true)
            StartCoroutine(RestartStage());
    }

    public void PlayerUI_Pause()                            //일시정지 버튼 누름
    {
        if (pauseUI.gameObject.activeInHierarchy == false)
        {
            PauseUI_Active(true);
            PauseUI_TextChanger(false);
        }
        else
            PauseUI_Active(false);
    }

    internal void PlayerUI_Moved(int boost)                 //플레이어 부스터 사용 시 UI
    {
        playBoostText.text = boost.ToString();
    }
    internal void PlayerUI_Shoot(int shoot)                 //플레이어 사격 시 UI
    {
        playShootText.text = shoot.ToString();
    }

    internal void PlayerUI_SetBoost(int boost)              //플레이어 부스터 갱신
    {
        playBoostText.text = boost.ToString();
    }

    internal void PlayerUI_SetShoot(int shoot)              //플레이어 사격 갱신
    {
        playShootText.text = shoot.ToString();
    }

    internal void PlayerUI_Goal()                           //플레이어가 골에 들어갔을 시
    {
        ClearUI_Active(true);
    }

    internal void PlayerUI_Hp(int hp)
    {
        if(hp < 0) hp = 0;
        StartCoroutine(PlayerHPLerp(hp));
    }

    internal void PlayerUI_Hit_Alert()
    {
        playHitAlert.color = new Color(1, 0, 0, 1);
        StartCoroutine(PlayerHitAlert());
    }

    internal void PlayerUI_Trigger(bool active)                        //플레이어가 트리거에 닿았을 시(30스테이지 이벤트)
    {
        playTriggerUp.gameObject.SetActive(active);
        playTriggerDown.gameObject.SetActive(active);
        PlayerUI_Active(false);
    }

    //============================퀘스트
    internal void PlayerUI_QuestActive(bool active)
    {
        playQuestOBJ.gameObject.SetActive(active);
    }

    internal void PlayerUI_QuestNumber(int num, string questtext)
    {
        playQuestText.text = num.ToString() + questtext;
    }
    //============================퀘스트^^

    internal void PlayerUI_Indicator(bool show, Vector2 goalpos)
    {
        indicator.Indicator_goalPos(goalpos);
        indicator.Indicator_Active(show);
    }
    #endregion 플레이어 UI

    #region 메인UI
    private void MainUI_Active(bool active)                 //메인 화면 활성화
    {
        mainStageUI.gameObject.SetActive(active);

        PlayerUI_Active(false);
        ClearUI_Active(false);
        SettingUI_Active(false);
        GameManager.Instance.UI_StageDeactive();
        GameManager.Instance.Player_Pause();
    }
    private void MainUI_Start()                              //시작 버튼 누름(키와 사운드 세이브 및 로딩도 함(미친 바꿔야함))
    {
        //SettingUI_Save();
        SettingUI_KeyLoad();
        StageUI_SecondCheck();
        StartCoroutine(FadeMain(FADEWHERE.MainToStage));
        MainUI_ScreenEvent(false);
        SoundManager.Instant.PlayEffect(ScreenEventSound, optionSound.value);
        SoundManager.Instant.PlayBgm("BGM", optionSound.value);
    }

    private void MainUI_Option()                             //옵션 버튼 누름
    {
        SettingUI_Active(true);
        SettingUI_KeyLoad();
    }

    private void MainUI_DevBtn()
    {
        StageUI_SecondCheck();
        StartCoroutine(FadeMain(FADEWHERE.MainToDev));
        MainUI_ScreenEvent(false);
    }

    private void MainUI_QuitBtn()                           //게임 종료
    {
        Application.Quit();
    }

    private void MainUI_ScreenEvent(bool active)
    {
        if (active == true)
        {
            ScreenEventObj.gameObject.SetActive(active);
        }
        else if (active == false)
        {
            ScreenEventAni.SetTrigger("Break");
        }
    }
    #endregion 메인UI

    #region 클리어UI
    internal void ClearUI_Active(bool active)                //클리어 UI 활성
    {
        clearUI.gameObject.SetActive(active);
        GameManager.Instance.Player_Pause();
        //Time.timeScale = 0;

    }
    private void ClearUI_Main()                            //메인화면 버튼 누름
    {
        ClearUI_Active(false);
        GameManager.Instance.UI_MainStage();
        StageUI_SecondCheck();
        StartCoroutine(FadeMain(FADEWHERE.ClearToMain));
        GameManager.Instance.UI_MainStage();
        GameManager.Instance.Player_OtherScreen();
        GameManager.Instance.UI_StageDeactive();
        MainUI_ScreenEvent(true);
        SoundManager.Instant.StopBgm();
    }

    //다음 스테이지 버튼 누름
    //다음 스테이지로 그냥 넘어가는 것이 좋을 것 같다.

    private void ClearUI_NextStage()                      //다음 스테이지 넘어감
    {
        if(clickstage >= stageNum)
            stageNum++;
        StageUI_SecondCheck();
        ClearUI_Active(false);
        PlayerUI_Active(false);
        StartCoroutine(FadeMain(stageNum));
        //StageUI_Active(true);
    }

    internal void ClearUI_TriggerNextStage()                //30스테이지 이후 스테이지 클리어 없이 그냥 넘어감
    {
        stageNum++;
        StageUI_Save(stageNum);
        StartCoroutine(FadeMain(stageNum));
        SoundManager.Instant.StopBgm();
    }

    //===========================게임 종료
    internal void ClearUI_GameEnd()
    {
        SoundManager.Instant.PlayBgm("SPECIAL", 0.3f);
        StartCoroutine(GameEndFadeCorutine());
    }
    //===========================게임 종료^^
    #endregion 클리어UI

    #region 일시정지UI
    private void PauseUI_Active(bool active)                //일시정지 활성화
    {
        pauseRefrashBtn.interactable = true;
        pauseStageBtn.interactable = true;
        pauseMainBtn.interactable = true;
        /*if (pauseUI.gameObject.activeInHierarchy == true)
        {
            pauseUI.gameObject.SetActive(false);
            Time.timeScale = 1;
            GameManager.Instance.Player_Pause(true);
        }
        else
        {
            pauseUI.gameObject.SetActive(true);
            Time.timeScale = 0;
            GameManager.Instance.Player_Pause(false);
        }*/
        if (pauseUI.gameObject.activeInHierarchy == active)
            return;
        pauseUI.gameObject.SetActive(active);
        if (active == true)
        {
            GameManager.Instance.Player_Pause();
            Time.timeScale = 0;
        }
        else
        {
            GameManager.Instance.UI_Refrash_Player();
            Time.timeScale = 1;
        }
    }
    private void PauseUI_Main()                            //메인화면 버튼 누름
    {
        Debug.Log("PauseUI_Main");
        pauseMainBtn.interactable = false;
        PauseUI_Active(false);
        Time.timeScale = 1;
        StartCoroutine(FadeMain(FADEWHERE.PauseToMain));
        GameManager.Instance.UI_MainStage();
        GameManager.Instance.Player_OtherScreen();
        GameManager.Instance.UI_StageDeactive();
        GameManager.Instance.Camera_Reset();
        MainUI_ScreenEvent(true);
        SoundManager.Instant.StopBgm();
    }

    private void PauseUI_Stage()                        //스테이지 버튼 누름
    {
        /*PauseUI_Active(false);
        StageUI_Active(true);*/
        pauseStageBtn.interactable = false;
        Debug.Log("PauseUI_Main");
        PauseUI_Active(false);
        Time.timeScale = 1;
        StartCoroutine(FadeMain(FADEWHERE.PauseToStage));
        GameManager.Instance.UI_MainStage();
        GameManager.Instance.Player_OtherScreen();
        GameManager.Instance.UI_StageDeactive();
        GameManager.Instance.Camera_Reset();
        SoundManager.Instant.StopBgm();
    }

    private void PauseUI_Replay()                          //다시시작 버튼 누름
    {
        pauseRefrashBtn.interactable = false;
        Time.timeScale = 1;
        Debug.Log("이게 왜 눌려");
        StartCoroutine(RestartStage());
    }

    private void PauseUI_TextChanger(bool active)       //true = gameover, false = pause
    {
        if (pauseText.gameObject.activeInHierarchy == true)
            return;

        pauseText.SetActive(!active);
        gameoverText.SetActive(active);
    }

    internal void PauseUI_GameOver(bool active)         //일시정지로 게임오버 나타내기
    {
        PauseUI_TextChanger(active);

        if (active == true)
            PauseUI_Active(true);
    }

    internal void PauseUI_Interactable(bool active) //일시정지 위치 제한
    {
        playPauseBtn.interactable = active;
    }

    #endregion 일시정지UI

    #region 페이드 UI
    private void FadeUI_NextStage()                         //다음 스테이지 이동 시 페이드 실행
    {
        StartCoroutine(FadeStage());
    }
    private void FadeUI_MainStage()                         //메인 스테이지 이동 시 페이드 실행
    {
        //StartCoroutine(FadeMain());
    }
    #endregion 페이드 UI

    #region 옵션 UI
    private void SettingUI_Active(bool active)              //게임 옵션 UI 활성화
    {
        optionUI.gameObject.SetActive(active);
    }

    private void SettingUI_Close()
    {
        SettingUI_Active(false);
    }

    #region 옵션 UI 해상도 설정
    private void SettingUI_Resolution()//해상도 설정
    {
        List<Resolution> tempList   = new List<Resolution>();
        tempList                    = GameManager.Instance.GetResolution();
        SettingUI_Set_Resolution(tempList);
    }

    private void SettingUI_Set_Resolution(List<Resolution> reslist)//해상도 드롭박스 전용 설정
    {
        int optionNum = 0;
        optionDropdown.RefreshShownValue();

        foreach (Resolution item in reslist)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = item + " ";
            optionDropdown.options.Add(option);

            if (item.width == Screen.width && item.height == Screen.height)
                optionDropdown.value = optionNum;
            optionNum++;
        }

        Resolution_UI_Addlistener();
    }
    private void Resolution_UI_Addlistener()
    {
        optionDropdown.onValueChanged.AddListener(Resolution_Dropdown_Change);
    }

    //드롭박스 값 변경
    //예시 세팅
    private void Resolution_Dropdown_Change(int num)
    {
        resolutionNum = num;
        GameManager.Instance.Resolution_Change(resolutionNum);
    }

    #endregion 옵션 UI 해상도 설정


    private void SettingUI_SoundValue() //게임 사운드 조정
    {
    }

    private void SettingUI_Save()                            //게임 옵션 저장
    {
        float soundValue = optionSound.value;
        string _boostKey = movekey.ToString();
        string _shootKey = shootkey.ToString();

        GameManager.Instance.UI_OptionSetting(soundValue, movekey, shootkey);
        SettingUI_KeySave(soundValue, _boostKey, _shootKey);
        SettingUI_Active(false);
    }

    //===================================키 세팅=============================

    internal void SettingUI_Key_KeySetting(optionData data)//키 초기화
    {
        key_Code_List[0]    = (KeyCode)System.Enum.Parse(typeof(KeyCode), data.boostKey);
        key_Code_List[1]    = (KeyCode)System.Enum.Parse(typeof(KeyCode), data.shootKey);
        movekey             = key_Code_List[0];
        shootkey            = key_Code_List[1];

        keyClass.Key_Setting_Default(key_Name_List, key_Code_List);
    }

    private void Key_Click_Event()//키 변경 클릭 시 이벤트 실행
    {
        btnObject = EventSystem.current.currentSelectedGameObject;
        iskeyChange = true;
    }

    public void OnGUI()//Gui 이벤트 작동 확인
    {
        if (iskeyChange == true)
        {
            key_Setting_Alert.gameObject.SetActive(true);
            Event keyEvent = Event.current;
            KeyCode tempcode = new KeyCode();

            if (keyEvent.isKey || keyEvent.isMouse)
            {
                tempcode = keyEvent.keyCode;
                if (keyEvent.keyCode == KeyCode.Escape)
                {
                    iskeyChange = false;
                    key_Setting_Alert.gameObject.SetActive(false);
                    return;
                }


                if (Input.GetMouseButtonDown(0))
                    tempcode = KeyCode.Mouse0;
                else if (Input.GetMouseButtonDown(1))
                    tempcode = KeyCode.Mouse1;
                
                if (tempcode == KeyCode.None)
                    return;


                keyClass.Key_List_Setting_Keys(btnObject.transform.parent.name, tempcode);

                StartCoroutine(SettingKeyCode());
                iskeyChange = false;
                key_Setting_Alert.gameObject.SetActive(false);

            }
        }


    }

    private IEnumerator SettingKeyCode()//키 이름 변경
    {
        List<KeyCode> keys  = new List<KeyCode>();
        List<TextMeshProUGUI> tempText = new List<TextMeshProUGUI>();
        keys.AddRange(keyClass.Key_GetCode());

        tempText.Add(moveKeyBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>());
        tempText.Add(shootKeyBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>());

        if(keys.Count < 2)
            keys.Add(KeyCode.None);

        for(int i = 0; i < keys.Count; i++)
        {
            tempText[i].text = keys[i].ToString();
            if (keys[i] == KeyCode.Mouse0)
                tempText[i].text = "LeftButton";
            else if (keys[i] == KeyCode.Mouse1)
                tempText[i].text = "RightButton";

            if (tempText[i].text == "System.Collections.Generic.List`1[UnityEngine.KeyCode]")
                tempText[i].text = "None";
        }

        //변수를 따로 만들어서 하면 하나하나 다 맞추어주어야한다.
        //바꿔야한다고 생각하는데?
        movekey = keys[0];
        shootkey = keys[1];

        yield return null;
    }


    private void SettingUI_KeySave(float soundValue, string boost, string key)                        //옵션 저장
    {
        //dataStream.optionDataSave(soundValue, boost, key);
        //dataXMLStream.CreateXML_Option_Simple(soundValue, boost, key);
        //dataXMLStream.SaveXML_Start(true);
        //dataXMLStream.SaveXML_Option_Save(soundValue, boost, key);
        //dataXMLStream.SaveXML_Option_Simple(soundValue, boost, key);
        dataSave.Data_Save_PlayerPrefs(soundValue, boost, key);
    }
    private void SettingUI_KeyLoad()                           //게임 옵션 불러오기
    {
        //optionData data = dataStream.optionDataLoad();
        //dataXMLStream.CreateXML_Option_Simple(1, "E", "R");
        //optionData data = dataXMLStream.LoadXML_Option_Simple();

        /*dataSave.Data_DeleteAll();
        return;*/

        optionData data = dataSave.Data_Load_PlayerPrefs();

        if (data.shootKey == "")
        {
            data.boostKey = "Mouse0";
            data.shootKey = "E";
            data.soundValue = optionSound.value;
        }

        optionSound.value = data.soundValue;

        SettingUI_Key_KeySetting(data);
        StartCoroutine(SettingKeyCode());

        GameManager.Instance.UI_OptionSetting(optionSound.value, movekey, shootkey);
    }

    private void SettingUI_RefrashStage()                   //스테이지 초기화
    {
        stageNum = 0;
        StageUI_Save(stageNum);
        StageUI_stageLoad();

        StageUI_Active_OpenStage();
    }

    //===================================키 세팅=============================^^
    #endregion 옵션 UI

    #region 스테이지 UI

    private void StageUI_Active(bool active)                //스테이지 UI 활성 비활성
    {
        stageUI.gameObject.SetActive(active);
        StageUI_Active_OpenStage();
    }

    private void StageUI_MainBtn()                          //스테이지에서 메인으로
    {
        StageUI_SecondCheck();
        StartCoroutine(FadeMain(FADEWHERE.StageToMain));
        Time.timeScale = 1;
        GameManager.Instance.Player_OtherScreen();
        GameManager.Instance.Player_Pause();
        MainUI_ScreenEvent(true);
        SoundManager.Instant.StopBgm();
    }

    private void StageUI_NextStage()
    {
        stageFirst.gameObject.SetActive(false);
        stageSecond.gameObject.SetActive(true);
        stageStageBtn_Next.gameObject.SetActive(false);
        stageStageBtn_Back.gameObject.SetActive(true);
        StageUI_Active_OpenStage();
    }

    private void StageUI_BackStage()
    {
        stageFirst.gameObject.SetActive(true);
        stageSecond.gameObject.SetActive(false);
        stageStageBtn_Next.gameObject.SetActive(true);
        stageStageBtn_Back.gameObject.SetActive(false);
        StageUI_Active_OpenStage();
    }

    private void StageUI_Save(int stage)                               //스테이지 저장
    {
        //dataStream.stageDataSave(stage);
        dataXMLStream.CreateXML_Stage_Simple(stage);
    }

    private void StageUI_stageLoad()                                    //스테이지 불러오기
    {
        /*stageData data = dataStream.stageDataLoad();
        stageNum = data.stageValue;*/
        stageNum = dataXMLStream.LoadXML_Stage_Simple();
    }

    private void StageUI_Active_OpenStage()                 //스테이지 UI 열릴 시
    {
        //StageUI_SecondCheck();
        StageUI_Refrash();

        if (stageNum >= 42)
            stageNum = 42;

        if (stageFirst.gameObject.activeInHierarchy == true)
        {
            for (int i = 0; i <= stageNum; i++)
            {
                if (i < 25)
                    stageFirst.GetChild(i).gameObject.SetActive(true);
            }

        }
        if (stageSecond.gameObject.activeInHierarchy == true)
        {
            for (int i = 25; i <= stageNum; i++)
            {
                if(i < 42)
                    stageSecond.GetChild(i - 25).gameObject.SetActive(true);
            }
        }
    }

    private void StageUi_ButtonSetting()                    //스테이지 버튼 세팅
    {
        StageUI_StageBtnSetting();

        for (int i = 0; i < 42; i++)
        {
            string num = (i + 1).ToString();
            Transform parent;

            if (i < 25)
            {
                parent = stageFirst;
            }
            else
            {
                parent = stageSecond;
            }

            GameObject tmpObj = Instantiate(stageBtn_Pre, parent);
            tmpObj.name = num;

            Button tmpBtn = tmpObj.GetComponent<Button>();
            tmpBtn.onClick.AddListener(StageUI_ClickStageNum);

            Image tmpImg = tmpObj.GetComponent<Image>();
            tmpImg.color = StageUI_StageColor(i);

            TextMeshProUGUI tmpText = tmpObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            tmpText.text = num;

            stageButtonList.Add(tmpObj);
            tmpObj.SetActive(true);
            //tmpObj.SetActive(true);
        }
    }

    private void StageUI_StageBtnSetting()//처음 스테이지 UI 나타날 시 다음 스테이지 버튼이 어떻게 되어있으면 될까
    {
        if (stageNum < 25)
        {
            stageStageBtn_Next.gameObject.SetActive(true);
            stageStageBtn_Back.gameObject.SetActive(false);
        }
        else
        {
            stageStageBtn_Next.gameObject.SetActive(false);
            stageStageBtn_Back.gameObject.SetActive(true);
        }
    }

    private void StageUI_ClickStageNum()                     //스테이지 UI 숫자 버튼 눌렀을 때 숫자 가져오기
    {
        clickstage = int.Parse(EventSystem.current.currentSelectedGameObject.name) - 1;
        if (clickstage >= stageNum)
        {
            stageNum = clickstage;
            StageUI_SendStageNum(stageNum);
        }
        else
        {
            StageUI_SendStageNum(clickstage);
        }

        StageUI_Save(stageNum);
    }

    private Color StageUI_StageColor(int i)                 //스테이지 갈 수록 색변형
    {
        Color returnColor;
        if(i == 0 && i < 5)
            return returnColor = new Color(0f, 1f, 0f);
        if (i > 4 && i < 10)
            return returnColor = new Color(0f, 1f, 0.55f);
        if (i > 9 && i < 15)
            return returnColor = new Color(0f, 1f, 1f);
        if (i > 14 && i < 20)
            return returnColor = new Color(0f, 0.55f, 1f);
        if (i > 19 && i < 25)
            return returnColor = new Color(0f, 0f, 1f);
        if (i > 24 && i < 30)
            return returnColor = new Color(0.55f, 0f, 1f);
        if (i > 29 && i < 35)
            return returnColor = new Color(1f, 0f, 1f);
        if (i > 34 && i < 40)
            return returnColor = new Color(1f, 0f, 0.55f);
        if (i > 39 && i < 43)
            return returnColor = new Color(1f, 0f, 0f);
        return returnColor = new Color(0f, 1f, 0f);
    }


    private void StageUI_SendStageNum(int num)              //스테이지 정보 전송
    {
        int tmpnum = num;
        StartCoroutine(FadeMain(tmpnum));
    }

    private void StageUI_SecondCheck()
    {
        if (stageNum == 25)
        {
            stageFirst.gameObject.SetActive(false);
            stageSecond.gameObject.SetActive(true);
        }
    }

    private void StageUI_Refrash()                      //스테이지 UI 버튼 초기화
    {
        for(int i = 0; i < stageFirst.childCount; i++)
        {
            stageFirst.GetChild(i).gameObject.SetActive(false);
        }

        for(int i = 0; i < stageSecond.childCount; i++)
        {
            stageSecond.GetChild(i).gameObject.SetActive(false);
        }
    }

    #endregion 스테이지 UI

    #region 제작자 UI
    private void DevUI_Active(bool active)
    {
        developeUI.gameObject.SetActive(active);
    }
    private void DevUI_Main()                            //메인화면 버튼 누름
    {
        DevUI_Active(false);
        StartCoroutine(FadeMain(FADEWHERE.DevToMain));
        GameManager.Instance.Player_OtherScreen();
        GameManager.Instance.UI_StageDeactive();
        GameManager.Instance.Camera_Reset();
        MainUI_ScreenEvent(true);
        SoundManager.Instant.StopBgm();
    }

    private void DevUI_EndImage(bool active)
    {
        devEndImage.gameObject.SetActive(active);
    }

    #endregion 제작자 UI

    #region 코루틴 실행
    private IEnumerator FadeMain(FADEWHERE fadewhere)                          //메인화면 페이드 코루틴
    {
        float time = 0;
        float speed = 0;
        bool check = false;

        FadeGraphic.gameObject.SetActive(true);
        while (speed < 5)
        {
            time = Mathf.Sin(speed);
            speed += 0.3f;
            FadeGraphic.color = new Color(0, 0, 0, time);

            //=====================================================
            if (speed >= 2 && check == false)
            {
                check = true;
                FadeWhere(fadewhere);
            }
            //=====================================================
            yield return new WaitForSeconds(0.1f);
        }
        check = false;
        FadeGraphic.gameObject.SetActive(false);
        yield return null;
    }

    private IEnumerator FadeMain(int stagenum)         //메인화면 페이드 코루틴
    {
        float time = 0;
        float speed = 0;
        bool check = false;

        FadeGraphic.gameObject.SetActive(true);
        while (speed < 5)
        {
            time = Mathf.Sin(speed);
            speed += 0.3f;
            FadeGraphic.color = new Color(0, 0, 0, time);

            if (speed >= 2 && check == false)
            {
                GameManager.Instance.UI_SelectStage(stagenum);
                GameManager.Instance.UI_Refrash_Player();
                StageUI_Active(false);
                PlayerUI_Active(true);
                check = true;
            }
            yield return new WaitForSeconds(0.1f);
        }
        check = false;
        FadeGraphic.gameObject.SetActive(false);
        yield return null;
    }

    private void FadeWhere(FADEWHERE fadewhere)
    {
        GameManager.Instance.UI_StageDeactive();

        switch (fadewhere)
        {
            case FADEWHERE.StageToMain:
                MainUI_Active(true);
                StageUI_Active(false);
                break;
            case FADEWHERE.MainToStage:
                MainUI_Active(false);
                StageUI_Active(true);
                break;
            case FADEWHERE.PauseToMain:
                MainUI_Active(true);
                break;
            case FADEWHERE.PauseToStage:
                StageUI_Active(true);
                PlayerUI_Active(false);
                break;
            case FADEWHERE.ClearToMain:
                MainUI_Active(true);
                break;
            case FADEWHERE.MainToDev:
                MainUI_Active(false);
                DevUI_Active(true);
                break;
            case FADEWHERE.DevToMain:
                DevUI_Active(false);
                MainUI_Active(true);
                break;
            case FADEWHERE.EndToDev:
                break;
        }
    }

    private IEnumerator GameEndFadeCorutine()                   //게임 끝!!!!!
    {
        float time = 0;
        float speed = 0;
        bool check = false;

        FadeGraphic.gameObject.SetActive(true);
        while (speed < 5)
        {
            time = Mathf.Sin(speed);
            speed += 0.15f;
            FadeGraphic.color = new Color(0, 0, 0, time);

            if (speed >= 2 && check == false)
            {
                check = true;
                GameManager.Instance.UI_StageDeactive();
                DevUI_EndImage(true);
                GameManager.Instance.Camera_Reset();
            }
            yield return new WaitForSeconds(0.1f);
        }
        check = false;


        yield return new WaitForSeconds(5f);
        //=================================================================

        time = 0;
        speed = 0;
        check = false;

        FadeGraphic.gameObject.SetActive(true);
        while (speed < 5)
        {
            time = Mathf.Sin(speed);
            speed += 0.15f;
            FadeGraphic.color = new Color(0, 0, 0, time);

            if (speed >= 2 && check == false)
            {
                check = true;
                DevUI_EndImage(false);
                DevUI_Active(true);
            }
            yield return new WaitForSeconds(0.1f);
        }
        FadeGraphic.gameObject.SetActive(false);
        check = false;
        yield return null;
    }

    private IEnumerator FadeStage()                         //스테이지 페이드 코루틴
    { yield return null;}

    private IEnumerator InitButtonSetting()                 //버튼 객체 찾기 코루틴
    { yield return null; }

    private IEnumerator RestartStage()                      //다시 시작 시 화면 코루틴
    {
        float time = 0;
        float speed = 0;
        bool check = false;

        Debug.Log("위쪽이 문제");
        FadeGraphic.gameObject.SetActive(true);

        while (speed < 5)
        {
            Debug.Log("아래가 문제" + speed);
            time    = Mathf.Sin(speed);
            speed   += 0.3f;

            FadeGraphic.color = new Color(0, 0, 0, time);

            if (speed >= 2 && check == false)
            {
                GameManager.Instance.UI_ReStage();
                //GameManager.Instance.UI_Refrash_Player();
                PauseUI_Active(false);
                check = true;
            }
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("나와야 정상");

        check = false;
        FadeGraphic.gameObject.SetActive(false);
        yield return null;

    }

    private IEnumerator PlayerHPLerp(int hp)
    {
        if (hp == 3)
            playHPSlider.value = 3;

        while (hp < playHPSlider.value)
        {
            yield return new WaitForSeconds(0.05f);
            playHPSlider.value -= 0.2f;

            if (playHPSlider.value < 1f && playHPSlider.value > 0.2f)
            {
                Time.timeScale -= 0.2f;
            }
            else if (playHPSlider.value > 0f && playHPSlider.value <= 0.2f)
            {
                PauseUI_GameOver(true);
            }
        }
    }

    private IEnumerator PlayerHitAlert()
    {
        while (playHitAlert.color.a >= 0)
        {
            yield return new WaitForSeconds(0.05f);
            playHitAlert.color -= new Color(0, 0, 0, 0.1f);
        }
    }

    #endregion 코루틴 실행

}
