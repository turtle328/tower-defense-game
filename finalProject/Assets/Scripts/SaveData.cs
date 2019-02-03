using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int score;
    public int gold;
    public int wave;
    public int castleHealth;
    public bool[,] placedTowers;

    const int SCORE = 0;
    const int GOLD = 1;
    const int WAVE = 2;
    const int CASTLE_HP = 3;
    const int TOWER_PLACED_MATRIX = 4;

    public SaveData(LevelManager lm)
    {
        object[] saveData = lm.getSaveData();
        score = (int) saveData[SCORE];
        gold = (int) saveData[GOLD];
        wave = (int) saveData[WAVE];
        castleHealth = (int) saveData[CASTLE_HP];
        placedTowers = (bool[,]) saveData[TOWER_PLACED_MATRIX];
    }
}
