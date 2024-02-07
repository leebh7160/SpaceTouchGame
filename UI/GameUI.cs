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
    //===========================����
    private Indicator indicator;
    private KeyClass keyClass;

    //===========================��Ȳ�� ���� UI
    private Transform playUI;                               //���� ���� �� �÷��̾� UI
    private TextMeshProUGUI playBoostText;                      //�÷��̾� �ν��� UI
    private TextMeshProUGUI playShootText;                      //�÷��̾� ��� UI
    private Button playRefrashBtn;                          //�������� ����� ��ư
    private Button playPauseBtn;                            //�������� ����� ��ư
    private Transform playTriggerUp;                          //�÷��̾� Ʈ���� ����
    private Transform playTriggerDown;                          //�÷��̾� Ʈ���� ����

    private Transform playQuestOBJ;                         //�÷��̾� ����Ʈ UI
    private TextMeshProUGUI playQuestText;                  //�÷��̾� ����Ʈ Text

    private Slider playHPSlider;                             //�÷��̾� HP Ȯ�� �� ������Ʈ
    private Image playHitAlert;
 //===============================


    private Transform clearUI;                              //�������� Ŭ���� �� UI
    private Button clearNextStageBtn;                       //�������� �̵�

    private Transform pauseUI;                              //�Ͻ����� ���� �� UI
    private GameObject pauseText;                           //�Ͻ����� �۾�
    private GameObject gameoverText;                        //���ӿ��� �۾�
    private Button pauseMainBtn;                            //����ȭ�� �̵�
    private Button pauseRefrashBtn;                         //�������� ����� ��ư
    private Button pauseStageBtn;                           //�������� ���ư��� ��ư

    private Transform mainStageUI;                          //���� ���� �� ���� UI
    private Button mainStartGameBtn;                        //���� ���� ��ư
    private Button mainOptionBtn;                           //���� �ɼ� ��ư
    private Button mainDevBtn;                              //������ �Ұ� ��ư
    private Button mainQuitBtn;                             //���� ���� ��ư

    private Transform optionUI;                             //�ɼ� ��ư UI
    private Slider optionSound;
    private Button optionSave;
    private Button optionCancle;
    private Image optionWrong;
    private Dropdown optionDropdown;
    private int resolutionNum = 0;

    private Transform stageUI;                              //�������� ��ư UI
    private Transform stageFirst;
    private Transform stageSecond;
    private List<GameObject> stageButtonList;                   //�������� ����Ʈ
    [SerializeField] private GameObject stageBtn_Pre;       //�������� ��ư ������
    private Button stageMainBtn;                            //����ȭ�� ���ư��� ��ư
    private Button stageStageBtn_Next;                      //���� �������� ��ư
    private Button stageStageBtn_Back;                      //���� �������� ��ư

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
    //===========================���� ����
    [SerializeField] private int stageNum = 0;
    [SerializeField] private int clickstage = 0;

    //===========================KeyObject
    [Header("KeyObject")]
    [Space]
    [SerializeField]
    [Tooltip("���� Ű�� �̸�")] private List<string> key_Name_List;
    [SerializeField]
    [Tooltip("Ű�� �����ϴ� Ű������ġ")] private List<KeyCode> key_Code_List;

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
        //===================================����
        indicator               = this.GetComponent<Indicator>();
        keyClass                = new KeyClass();
        //===================================����^^

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
        SettingUI_Resolution();//�ػ� ����


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

    #region �÷��̾� UI
    internal void PlayerUI_Init(int boost, int shoot)       //�÷��̾� UI ����
    {
        PlayerUI_SetBoost(boost);
        PlayerUI_SetShoot(shoot);
    }

    internal void PlayerUI_Active(bool active)              //�÷��̾� UI ����
    {
        playUI.gameObject.SetActive(active);
    }

    public void PlayerUI_Replay()                           //�ٽý��� ��ư ����
    {
        if (playUI == null)
            return;
        if (playUI.gameObject == null)
            return;
        if(playUI.gameObject.activeInHierarchy == true)
            StartCoroutine(RestartStage());
    }

    public void PlayerUI_Pause()                            //�Ͻ����� ��ư ����
    {
        if (pauseUI.gameObject.activeInHierarchy == false)
        {
            PauseUI_Active(true);
            PauseUI_TextChanger(false);
        }
        else
            PauseUI_Active(false);
    }

    internal void PlayerUI_Moved(int boost)                 //�÷��̾� �ν��� ��� �� UI
    {
        playBoostText.text = boost.ToString();
    }
    internal void PlayerUI_Shoot(int shoot)                 //�÷��̾� ��� �� UI
    {
        playShootText.text = shoot.ToString();
    }

    internal void PlayerUI_SetBoost(int boost)              //�÷��̾� �ν��� ����
    {
        playBoostText.text = boost.ToString();
    }

    internal void PlayerUI_SetShoot(int shoot)              //�÷��̾� ��� ����
    {
        playShootText.text = shoot.ToString();
    }

    internal void PlayerUI_Goal()                           //�÷��̾ �� ���� ��
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

    internal void PlayerUI_Trigger(bool active)                        //�÷��̾ Ʈ���ſ� ����� ��(30�������� �̺�Ʈ)
    {
        playTriggerUp.gameObject.SetActive(active);
        playTriggerDown.gameObject.SetActive(active);
        PlayerUI_Active(false);
    }

    //============================����Ʈ
    internal void PlayerUI_QuestActive(bool active)
    {
        playQuestOBJ.gameObject.SetActive(active);
    }

    internal void PlayerUI_QuestNumber(int num, string questtext)
    {
        playQuestText.text = num.ToString() + questtext;
    }
    //============================����Ʈ^^

    internal void PlayerUI_Indicator(bool show, Vector2 goalpos)
    {
        indicator.Indicator_goalPos(goalpos);
        indicator.Indicator_Active(show);
    }
    #endregion �÷��̾� UI

    #region ����UI
    private void MainUI_Active(bool active)                 //���� ȭ�� Ȱ��ȭ
    {
        mainStageUI.gameObject.SetActive(active);

        PlayerUI_Active(false);
        ClearUI_Active(false);
        SettingUI_Active(false);
        GameManager.Instance.UI_StageDeactive();
        GameManager.Instance.Player_Pause();
    }
    private void MainUI_Start()                              //���� ��ư ����(Ű�� ���� ���̺� �� �ε��� ��(��ģ �ٲ����))
    {
        //SettingUI_Save();
        SettingUI_KeyLoad();
        StageUI_SecondCheck();
        StartCoroutine(FadeMain(FADEWHERE.MainToStage));
        MainUI_ScreenEvent(false);
        SoundManager.Instant.PlayEffect(ScreenEventSound, optionSound.value);
        SoundManager.Instant.PlayBgm("BGM", optionSound.value);
    }

    private void MainUI_Option()                             //�ɼ� ��ư ����
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

    private void MainUI_QuitBtn()                           //���� ����
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
    #endregion ����UI

    #region Ŭ����UI
    internal void ClearUI_Active(bool active)                //Ŭ���� UI Ȱ��
    {
        clearUI.gameObject.SetActive(active);
        GameManager.Instance.Player_Pause();
        //Time.timeScale = 0;

    }
    private void ClearUI_Main()                            //����ȭ�� ��ư ����
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

    //���� �������� ��ư ����
    //���� ���������� �׳� �Ѿ�� ���� ���� �� ����.

    private void ClearUI_NextStage()                      //���� �������� �Ѿ
    {
        if(clickstage >= stageNum)
            stageNum++;
        StageUI_SecondCheck();
        ClearUI_Active(false);
        PlayerUI_Active(false);
        StartCoroutine(FadeMain(stageNum));
        //StageUI_Active(true);
    }

    internal void ClearUI_TriggerNextStage()                //30�������� ���� �������� Ŭ���� ���� �׳� �Ѿ
    {
        stageNum++;
        StageUI_Save(stageNum);
        StartCoroutine(FadeMain(stageNum));
        SoundManager.Instant.StopBgm();
    }

    //===========================���� ����
    internal void ClearUI_GameEnd()
    {
        SoundManager.Instant.PlayBgm("SPECIAL", 0.3f);
        StartCoroutine(GameEndFadeCorutine());
    }
    //===========================���� ����^^
    #endregion Ŭ����UI

    #region �Ͻ�����UI
    private void PauseUI_Active(bool active)                //�Ͻ����� Ȱ��ȭ
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
    private void PauseUI_Main()                            //����ȭ�� ��ư ����
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

    private void PauseUI_Stage()                        //�������� ��ư ����
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

    private void PauseUI_Replay()                          //�ٽý��� ��ư ����
    {
        pauseRefrashBtn.interactable = false;
        Time.timeScale = 1;
        Debug.Log("�̰� �� ����");
        StartCoroutine(RestartStage());
    }

    private void PauseUI_TextChanger(bool active)       //true = gameover, false = pause
    {
        if (pauseText.gameObject.activeInHierarchy == true)
            return;

        pauseText.SetActive(!active);
        gameoverText.SetActive(active);
    }

    internal void PauseUI_GameOver(bool active)         //�Ͻ������� ���ӿ��� ��Ÿ����
    {
        PauseUI_TextChanger(active);

        if (active == true)
            PauseUI_Active(true);
    }

    internal void PauseUI_Interactable(bool active) //�Ͻ����� ��ġ ����
    {
        playPauseBtn.interactable = active;
    }

    #endregion �Ͻ�����UI

    #region ���̵� UI
    private void FadeUI_NextStage()                         //���� �������� �̵� �� ���̵� ����
    {
        StartCoroutine(FadeStage());
    }
    private void FadeUI_MainStage()                         //���� �������� �̵� �� ���̵� ����
    {
        //StartCoroutine(FadeMain());
    }
    #endregion ���̵� UI

    #region �ɼ� UI
    private void SettingUI_Active(bool active)              //���� �ɼ� UI Ȱ��ȭ
    {
        optionUI.gameObject.SetActive(active);
    }

    private void SettingUI_Close()
    {
        SettingUI_Active(false);
    }

    #region �ɼ� UI �ػ� ����
    private void SettingUI_Resolution()//�ػ� ����
    {
        List<Resolution> tempList   = new List<Resolution>();
        tempList                    = GameManager.Instance.GetResolution();
        SettingUI_Set_Resolution(tempList);
    }

    private void SettingUI_Set_Resolution(List<Resolution> reslist)//�ػ� ��ӹڽ� ���� ����
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

    //��ӹڽ� �� ����
    //���� ����
    private void Resolution_Dropdown_Change(int num)
    {
        resolutionNum = num;
        GameManager.Instance.Resolution_Change(resolutionNum);
    }

    #endregion �ɼ� UI �ػ� ����


    private void SettingUI_SoundValue() //���� ���� ����
    {
    }

    private void SettingUI_Save()                            //���� �ɼ� ����
    {
        float soundValue = optionSound.value;
        string _boostKey = movekey.ToString();
        string _shootKey = shootkey.ToString();

        GameManager.Instance.UI_OptionSetting(soundValue, movekey, shootkey);
        SettingUI_KeySave(soundValue, _boostKey, _shootKey);
        SettingUI_Active(false);
    }

    //===================================Ű ����=============================

    internal void SettingUI_Key_KeySetting(optionData data)//Ű �ʱ�ȭ
    {
        key_Code_List[0]    = (KeyCode)System.Enum.Parse(typeof(KeyCode), data.boostKey);
        key_Code_List[1]    = (KeyCode)System.Enum.Parse(typeof(KeyCode), data.shootKey);
        movekey             = key_Code_List[0];
        shootkey            = key_Code_List[1];

        keyClass.Key_Setting_Default(key_Name_List, key_Code_List);
    }

    private void Key_Click_Event()//Ű ���� Ŭ�� �� �̺�Ʈ ����
    {
        btnObject = EventSystem.current.currentSelectedGameObject;
        iskeyChange = true;
    }

    public void OnGUI()//Gui �̺�Ʈ �۵� Ȯ��
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

    private IEnumerator SettingKeyCode()//Ű �̸� ����
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

        //������ ���� ���� �ϸ� �ϳ��ϳ� �� ���߾��־���Ѵ�.
        //�ٲ���Ѵٰ� �����ϴµ�?
        movekey = keys[0];
        shootkey = keys[1];

        yield return null;
    }


    private void SettingUI_KeySave(float soundValue, string boost, string key)                        //�ɼ� ����
    {
        //dataStream.optionDataSave(soundValue, boost, key);
        //dataXMLStream.CreateXML_Option_Simple(soundValue, boost, key);
        //dataXMLStream.SaveXML_Start(true);
        //dataXMLStream.SaveXML_Option_Save(soundValue, boost, key);
        //dataXMLStream.SaveXML_Option_Simple(soundValue, boost, key);
        dataSave.Data_Save_PlayerPrefs(soundValue, boost, key);
    }
    private void SettingUI_KeyLoad()                           //���� �ɼ� �ҷ�����
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

    private void SettingUI_RefrashStage()                   //�������� �ʱ�ȭ
    {
        stageNum = 0;
        StageUI_Save(stageNum);
        StageUI_stageLoad();

        StageUI_Active_OpenStage();
    }

    //===================================Ű ����=============================^^
    #endregion �ɼ� UI

    #region �������� UI

    private void StageUI_Active(bool active)                //�������� UI Ȱ�� ��Ȱ��
    {
        stageUI.gameObject.SetActive(active);
        StageUI_Active_OpenStage();
    }

    private void StageUI_MainBtn()                          //������������ ��������
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

    private void StageUI_Save(int stage)                               //�������� ����
    {
        //dataStream.stageDataSave(stage);
        dataXMLStream.CreateXML_Stage_Simple(stage);
    }

    private void StageUI_stageLoad()                                    //�������� �ҷ�����
    {
        /*stageData data = dataStream.stageDataLoad();
        stageNum = data.stageValue;*/
        stageNum = dataXMLStream.LoadXML_Stage_Simple();
    }

    private void StageUI_Active_OpenStage()                 //�������� UI ���� ��
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

    private void StageUi_ButtonSetting()                    //�������� ��ư ����
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

    private void StageUI_StageBtnSetting()//ó�� �������� UI ��Ÿ�� �� ���� �������� ��ư�� ��� �Ǿ������� �ɱ�
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

    private void StageUI_ClickStageNum()                     //�������� UI ���� ��ư ������ �� ���� ��������
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

    private Color StageUI_StageColor(int i)                 //�������� �� ���� ������
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


    private void StageUI_SendStageNum(int num)              //�������� ���� ����
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

    private void StageUI_Refrash()                      //�������� UI ��ư �ʱ�ȭ
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

    #endregion �������� UI

    #region ������ UI
    private void DevUI_Active(bool active)
    {
        developeUI.gameObject.SetActive(active);
    }
    private void DevUI_Main()                            //����ȭ�� ��ư ����
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

    #endregion ������ UI

    #region �ڷ�ƾ ����
    private IEnumerator FadeMain(FADEWHERE fadewhere)                          //����ȭ�� ���̵� �ڷ�ƾ
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

    private IEnumerator FadeMain(int stagenum)         //����ȭ�� ���̵� �ڷ�ƾ
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

    private IEnumerator GameEndFadeCorutine()                   //���� ��!!!!!
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

    private IEnumerator FadeStage()                         //�������� ���̵� �ڷ�ƾ
    { yield return null;}

    private IEnumerator InitButtonSetting()                 //��ư ��ü ã�� �ڷ�ƾ
    { yield return null; }

    private IEnumerator RestartStage()                      //�ٽ� ���� �� ȭ�� �ڷ�ƾ
    {
        float time = 0;
        float speed = 0;
        bool check = false;

        Debug.Log("������ ����");
        FadeGraphic.gameObject.SetActive(true);

        while (speed < 5)
        {
            Debug.Log("�Ʒ��� ����" + speed);
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
        Debug.Log("���;� ����");

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

    #endregion �ڷ�ƾ ����

}
