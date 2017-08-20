using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.Events;

public class Control : MonoBehaviour {

    #region Delegates
    public delegate void TeamChangeEventHandler(CellBehaviour sender, Cell.enmTeam previous, Cell.enmTeam current);
    public delegate void EnteredCellEditMode(EditCell sender);
    public delegate void PanelValueChanged(LevelEditorCore.PCPanelAttribute attribute);
    public delegate void EditModeChanged(LevelEditorCore.Mode mode);
    public delegate void NewSelectionForDownload(SaveFileInfo sender);

    public delegate void EnteredUpgradeMode(Upgrade_Manager sender);
    public delegate void QuitUpgradeMode(Upgrade_Manager sender);
    #endregion

    public static List<CellBehaviour> cells = new List<CellBehaviour>();

    public static bool isPaused = false;
    public static bool canPause = true;

    private float time;

    private bool isInGame = false;
    public static PlaySceneState levelState = PlaySceneState.NONE;
    public enum PlaySceneState {
        NONE,
        CAMPAIGN,
        CUSTOM,
        PREVIEW
    }
    public static ProfileManager pM;


    public static int DebugSceneIndex = 0;


    #region Prefab
    public GameObject profileVis;
    #endregion

    #region Post-Game data
    private static bool isWinner = true;
    private static bool domination = false;
    private static string gameTime;
    private static int totalCoinsAwarded;
    #endregion

    #region Initializers
    public static Control script;
    private void Awake() {
        if (script == null) {
            script = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (script != this) {
            Destroy(gameObject);
        }

        if (!Directory.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves")) {
            Directory.CreateDirectory(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves");
        }
        if (!Directory.Exists(Application.temporaryCachePath + Path.DirectorySeparatorChar + "Saves")) {
            Directory.CreateDirectory(Application.temporaryCachePath + Path.DirectorySeparatorChar + "Saves");
        }
        if (!Directory.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "Profiles")) {
            Directory.CreateDirectory(Application.persistentDataPath + Path.DirectorySeparatorChar + "Profiles");
        }

    }

    private void Start() {

        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

        string activeScene = SceneManager.GetActiveScene().name;

        if (/*activeScene == 5 || */activeScene == "Level_Player") {
            isInGame = true;
            if (levelState != PlaySceneState.PREVIEW) {
                StartCoroutine(GameState());
            }
            
        }
        if (activeScene == "Profiles") {
            if (pM == null) {
                pM = new ProfileManager(profileVis, GameObject.Find("Content").transform);
                pM.ListProfiles();
            }
        }
        if (ProfileManager.currentProfile == null && activeScene == "Main_Menu") {
            DebugSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene("Profiles");
        }
    }


    private void OnDestroy() {
        SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }
    #endregion


    private void SceneManager_activeSceneChanged(Scene oldS, Scene newS) {

        if (newS.name == "Main_Menu") {
            print(ProfileManager.getCurrentProfile.profileName);
            if (ProfileManager.getCurrentProfile == null) {
                DebugSceneIndex = 0;
                SceneManager.LoadScene("Profiles");
                return;
            }
            GameObject.Find("Profile").GetComponent<TextMeshProUGUI>().SetText("Welcome: " + ProfileManager.getCurrentProfile.profileName);

        }

        if (newS.name == "Level_Editor") {

        }

        if (newS.name == "Level_Select") {
            LevelSelectScript.displayedSaves.Clear();
        }

        if (newS.name == "Level_Player") {
            if (levelState != PlaySceneState.PREVIEW) {
                StartCoroutine(GameState());
            }
        }


        if (newS.name == "Level_Sharing") {

        }

        if (newS.name == "Debug") {

        }


        if (newS.name == "Post_Game") {
            isInGame = false;

            if (isWinner) {
                UI_ReferenceHolder.resultingJudgdement.text = "You Won!";
            }
            else {
                UI_ReferenceHolder.resultingJudgdement.text = "You Lost!";
            }

            if (domination) {
                UI_ReferenceHolder.didDominate.text = "Dominated all cells";
            }
            else {
                UI_ReferenceHolder.didDominate.text = "";
            }
            UI_ReferenceHolder.totalTimeToClear.text = "Fought for:\n" + gameTime;

            UI_ReferenceHolder.totalCoinsAwarded.text = totalCoinsAwarded + "<size=40>coins";
        }

        if (newS.name == "Profiles") {
            pM = new ProfileManager(profileVis, GameObject.Find("Content").transform);
            pM.ListProfiles();
        }
        Time.timeScale = 1;
    }


    private void LateUpdate() {
        if (isInGame) {
            time += Time.deltaTime;
        }

        if (SceneManager.GetActiveScene().buildIndex != 1) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (isPaused) {
                    UnPause();
                }
                else {
                    print(Upgrade_Manager.isUpgrading);
                    if (Upgrade_Manager.isUpgrading) {
                        Upgrade_Manager.isUpgrading = false;
                    }
                    else {
                        if (canPause) {
                            Pause();
                        }
                    }
                }
            }
        }
    }

    public IEnumerator GameState() {
        isInGame = true;

        while ((SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 3) && isInGame) {
            yield return new WaitForSeconds(1);
            int activeAIs = 0;

            for (int i = 0; i < Initialize_AI.AIs.Length; i++) {
                if (Initialize_AI.AIs[i] != null) {
                    if (Initialize_AI.AIs[i].isActive) {
                        activeAIs++;
                    }
                }
            }
            //print(activeAIs);
            if (activeAIs == 0) {
                if (SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 3) {

                    YouWon();

                }

            }
        }

        int playerCells = 0;

        for (int i = 0; i < cells.Count; i++) {
            if (cells[i].cellTeam == Cell.enmTeam.ALLIED) {
                playerCells++;
            }
        }
        //print(playerCells);
        if (playerCells == 0) {
            if (SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 3) {

                GameOver();

            }
        }
    }


    public static void Pause() {
        isPaused = true;
        Time.timeScale = 0;
        UI_ReferenceHolder.menuPanel.SetActive(true);
    }

    public static void UnPause() {
        isPaused = false;
        Time.timeScale = 1;
        UI_ReferenceHolder.menuPanel.SetActive(false);
    }

    public void GameOver() {
        SceneManager.LoadScene("Post_Game");
        isWinner = false;
        gameTime = "Time:\t" + string.Format("{0:00}:{1:00}.{2:00} minutes", (int)time / 60, time % 60, time.ToString().Remove(0, time.ToString().Length - 2));
        isInGame = false;
    }

    public void YouWon() {
        totalCoinsAwarded = 1;
        SceneManager.LoadScene("Post_Game");
        isWinner = true;
        int neutrals = 0;
        foreach (CellBehaviour c in cells) {
            if (c.cellTeam == Cell.enmTeam.NEUTRAL) {
                neutrals++;
            }
        }
        if (neutrals == 0) {
            totalCoinsAwarded += 1;
            domination = true;
        }
        else {
            domination = false;
        }
        gameTime = "Time:\t" + string.Format("{0:00}:{1:00}.{2:00} minutes", (int)time / 60, time % 60, time.ToString().Remove(0, time.ToString().Length - 2));

        //Did we play a campaign level ?
        if (CampaignLevel.currentCampaignLevel != null) {
            ProfileManager.getCurrentProfile.completedCampaignLevels += 1;
            ProfileManager.getCurrentProfile.clearedCampaignLevels[CampaignLevel.currentCampaignLevel] = time;
            CampaignLevel.currentCampaignLevel = null;
            CampaignLevel.current = null;
        }
        else {
            ProfileManager.getCurrentProfile.completedCustomLevels += 1;
            print("Played custom garbage");
        }

        isInGame = false;
        print(ProfileManager.getCurrentProfile.ownedCoins);
        ProfileManager.getCurrentProfile.ownedCoins += totalCoinsAwarded;
        ProfileManager.SerializeChanges();
    }
}

