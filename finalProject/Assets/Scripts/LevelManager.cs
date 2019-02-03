using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Collections;

public struct RoadRow
{
    public RoadRow(int rs, int re)
    {
        RoadStart = rs;
        RoadEnd = re;
    }

    public int RoadStart { get; set; }
    public int RoadEnd { get; set; }
}

public class LevelManager : MonoBehaviour
{
    #region Fields
    private static LevelManager instance;
    // enforce loading behavior
    public static bool loadingData = false;
    // enforce singleton behavior
    private bool theOne = false;
    // good way to check if player dead
    public GameObject buildplace;
    public bool[,] towerPlacedMatrix;
    public NavMeshSurface surface;
    public float skyBoxRotationSpeed = 1;

    private RoadRow[] road = new RoadRow[GROUND_SIZE];
    private Text theGUI;
    private Health castleHp;
    private SaveData data;
    private AudioManager am;
    private const int GROUND_SIZE = 29;
    private int curScore = 0;
    private int curGold = 100;
    private int curWave = 0;
    private int goldScale = 0;
    readonly string file = "road.txt";

    public int Gold { get { return curGold; } set { curGold = value; setGUI(); } }
    #endregion

    public static LevelManager getInstance()
    {
        return instance;
    }

    internal void win()
    {
        GameObject.Find("Canvas").GetComponent<PauseMenu>().Pause();
        GameObject.Find("Canvas").SetActive(false);
        GameObject.Find("YouWin").GetComponent<Canvas>().enabled = true;
        GameObject.Find("YouWin").GetComponentInChildren<Text>().text = "SCORE: " + curScore;
    }

    internal void die()
    {
        GameObject.Find("Canvas").GetComponent<PauseMenu>().Pause();
        GameObject.Find("Canvas").SetActive(false);
        GameObject.Find("DefeatScreen").GetComponent<Canvas>().enabled = true;
        GameObject.Find("ScoreText").GetComponent<Text>().text = "SCORE: " + curScore;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Tower Defense Game"))
        {
            RenderSettings.skybox.SetFloat("_Rotation", Time.time * skyBoxRotationSpeed);
        }
    }

    void Awake()
    {
        if (instance) Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);
            theOne = true;
            instance = this;
        }
    }

    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        //transforms.Dispose();
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (theOne)
        {
            if (am == null) am = AudioManager.instance;
            if (scene == SceneManager.GetSceneByName("MainMenu"))
            {
                am.StopAll();
                am.Play("GameMenu");
            }
            if (scene == SceneManager.GetSceneByName("Tower Defense Game"))
            {
                am.StopAll();
                am.Play("BGM");
                castleHp = GameObject.Find("Castle").GetComponentInChildren<Health>();
                surface = GameObject.Find("NavMesh").GetComponent<NavMeshSurface>();
                CreateMap();

                if (loadingData)
                {
                    loadingData = false;
                    castleHp.setHealth(data.castleHealth);
                }

                else ResetValues();

                theGUI = GameObject.Find("GUIText").GetComponent<Text>();
                setGUI();
            }
        }
    }

    void ResetValues()
    {
        curScore = 0;
        curGold = 100;
        curWave = 0;
    }

    public void CreateMap()
    {
        if (!loadingData) towerPlacedMatrix = new bool[GROUND_SIZE,GROUND_SIZE];
        int roadStart;
        int roadEnd;
        InitRoad();
        for (int i = 0; i < GROUND_SIZE; i++)
        {
            roadStart = road[i].RoadStart;
            roadEnd = road[i].RoadEnd;
            for (int j = 0; j < GROUND_SIZE; j++)
            {
                // if i = x and j = y place tower
                if (j < roadStart || j > roadEnd)
                {
                    GameObject g = Instantiate(buildplace, new Vector3(-14 + j, 0.5f, -14 + i), Quaternion.identity);
                    Buildplace bp = g.GetComponent<Buildplace>();
                    bp.RegisterLocation(i, j);
                    if (towerPlacedMatrix[i, j]) bp.PlaceTower(false);
                }
            }
        }
        surface.BuildNavMesh();
    }

    public void UpdateTowerPlacedMatrix(bool placed, int x, int y)
    {
        towerPlacedMatrix[x,y] = placed;
    }

    public object[] getSaveData()
    {
        return new object[] { curScore, curGold, curWave, castleHp.current(), towerPlacedMatrix};
    }

    public void SaveData()
    {
        SaveSystem.SaveData(this);
    }

    public void LoadData()
    {
        data = SaveSystem.LoadSave();
        if (data != null)
        {
            loadingData = true;
            curScore = data.score;
            curGold = data.gold;
            curWave = data.wave;
            towerPlacedMatrix = data.placedTowers;
            Time.timeScale = 1;
            SceneManager.LoadScene("Tower Defense Game");
            setGUI();
        }
    }

    private void InitRoad()
    {
        string[] roadRows = File.ReadAllLines(file);
        for (int i = 0; i < roadRows.Length; i++)
        {
            string[] roadRow = roadRows[i].Split(' ');
            road[i] = new RoadRow(int.Parse(roadRow[0]), int.Parse(roadRow[1]));
        }
    }

    public void setWave(int wave)
    {
        curWave = wave;
        setGUI();
    }

    public int getWave()
    {
        return curWave;
    }
    
    public void setGoldScale(int scale)
    {
        goldScale = scale;
    }

    public void addScore(int points)
    {
        curScore += points;
        curGold += points * goldScale;
        setGUI();
    }

    public void addGold(int gold)
    {
        curGold += gold;
        setGUI();
    }

    private void setGUI()
    {
        if (theGUI != null)
        {
            theGUI.text = "Score:\t" + curScore +
                          "\n<color=#ffff00ff>Gold:\t" + curGold + "</color>" +
                          "\nWave:\t" + (curWave + 1);
        }
    }
}