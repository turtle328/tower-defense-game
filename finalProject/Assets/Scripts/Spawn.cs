using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.IO;
using System;

public class Spawn : MonoBehaviour
{
    public GameObject monsterPrefab;
    // good way to change difficulty
    public int goldScale = 5;
    public int goldPerRound = 10;
    // seconds between each spawn
    public float interval = 3;
    public int wave = 0;
    public int maxWaveSize;
    private LevelManager lm;
    private CustomQueue<string> monsters;
    private string[] waves;
    private string[][] monsterInfo;
    readonly string wavesFile = "waves.txt";
    readonly string monsterFile = "monsterinfo.txt";

    const int MONSTER_TYPE = 0;
    const int NUM_OF_MONSTER = 1;

    const int MONSTER_NAME = 0;
    const int MONSTER_SCORE = 1;
    const int MONSTER_HEALTH = 2;
    const int MONSTER_SPEED = 3;
    const int MONSTER_COLOR = 4;

    // Use this for initialization
    void Start()
    {
        lm = LevelManager.getInstance();
        lm.setGoldScale(goldScale);
        waves = File.ReadAllLines(wavesFile);
        //CalculateTotalGold();
        var monsterInfoLines = File.ReadAllLines(monsterFile);
        monsterInfo = new string[monsterInfoLines.Length][];

        for (int i = 0; i < monsterInfoLines.Length; i++)
            monsterInfo[i] = monsterInfoLines[i].Split(' ');
        monsters = new CustomQueue<string>(maxWaveSize);
        wave = lm.getWave();
        InitWave();
        InvokeRepeating("SpawnNext", interval, interval);
    }

    void IncrementWave()
    {
        if (++wave == waves.Length) lm.win();
        else
        {
            lm.setWave(wave);
            float percentage = 1f - (1f/waves.Length * wave);
            //Debug.Log(percentage);
            percentage = (percentage < 0) ? 0 : percentage;
            int goldThisRound = Convert.ToInt32(goldPerRound * percentage);
            //Debug.Log(goldThisRound);
            lm.addGold(goldThisRound);
            lm.SaveData();
            InitWave();
        }
    }

    void InitWave()
    {
        string[] curWave = waves[wave].Split(':');
        foreach (string monster in curWave)
        {
            string[] info = monster.Split(' ');
            if (info[MONSTER_TYPE] == "interval")
                monsters.Push(info[NUM_OF_MONSTER]);
            else
                for (int i = 0; i < int.Parse(info[NUM_OF_MONSTER]); i++)
                    monsters.Push(info[MONSTER_TYPE]);
        }
    }

    void SpawnNext()
    {
        if (monsters.IsEmpty() && Monster.numMonsters == 0)
            IncrementWave();

        else if (!monsters.IsEmpty())
        {
            string monster = monsters.Pop();
            float seconds;

            if (float.TryParse(monster, out seconds))
            {
                interval = seconds;
                CancelInvoke();
                InvokeRepeating("SpawnNext", 0f, interval);
            }

            else
            {
                GameObject tmp = Instantiate(monsterPrefab, transform.position, Quaternion.identity);
                ModifyMonster(monster, tmp);
            }
        }
    }

    void CalculateTotalGold()
    {
        int total = 0;

        foreach (string wave in waves)
        {
            string[] theWave = wave.Split(':');
            foreach (string monster in theWave)
            {
                string[] info = monster.Split(' ');
                if (info[MONSTER_TYPE] != "interval")
                {
                    total += int.Parse(info[NUM_OF_MONSTER]) * GetAssociatedScore(info[MONSTER_TYPE])  * goldScale;
                }
            }
        }
        int fromRound = 0;
        for (int i = 100; i > 0; i -= 10)
        {
            fromRound += i;
        }
        Debug.Log("Gold from monsters: " + total +
                 "\nGold from rounds: " + fromRound);
        total += fromRound;
        Debug.Log("Total gold that can be earned " + total);
    }

    int GetAssociatedScore(string monsterType)
    {
        switch(monsterType)
        {
            case "weak":
                return 1;
            case "normal":
                return 2;
            case "strong":
                return 3;
            default:
                return 0;
        }
    }

    void ModifyMonster(string monsterType, GameObject monsterInstance)
    {
        Monster monsterScript = monsterInstance.GetComponent<Monster>();
        MonsterHealth health = monsterInstance.GetComponentInChildren<MonsterHealth>();
        NavMeshAgent monsterNMA = monsterInstance.GetComponent<NavMeshAgent>();
        Renderer[] monsterRenderers = monsterInstance.GetComponentsInChildren<Renderer>();

        // get the info
        for (int i = 0; i < monsterInfo.Length; i++)
        {
            if (monsterType == monsterInfo[i][MONSTER_NAME])
            {
                // set the score
                monsterScript.Score = int.Parse(monsterInfo[i][MONSTER_SCORE]);

                // set the health
                health.setHealth(int.Parse(monsterInfo[i][MONSTER_HEALTH]));

                // set the speed
                monsterNMA.speed = int.Parse(monsterInfo[i][MONSTER_SPEED]);

                // set the color
                for (int j = 0; j < monsterRenderers.Length; j++)
                {
                    if (monsterRenderers[j].name != "HealthBar")
                    {
                        Color color;
                        ColorUtility.TryParseHtmlString(monsterInfo[i][MONSTER_COLOR], out color);
                        monsterRenderers[j].material.SetColor("_Color", color);
                    }
                }
            }
        }
    }
}
