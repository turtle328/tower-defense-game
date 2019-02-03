using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildplace : MonoBehaviour
{
    public GameObject towerPrefab;
    private GameObject tower;
    private LevelManager lm;
    private int towerCost;
    private bool towerPlaced = false;
    private int locX;
    private int locY;

    private void Start()
    {
        lm = LevelManager.getInstance();
    }

    public void RegisterLocation(int x, int y)
    {
        locX = x;
        locY = y;
    }

    void OnMouseUpAsButton()
    {
        PlaceTower();
    }

    public void PlaceTower(bool goldCheck = true)
    {
        if (lm == null) lm = LevelManager.getInstance();
        // Build tower on Buildplace
        if (!towerPlaced && Time.timeScale != 0)
        {
            tower = Instantiate(towerPrefab);
            towerCost = tower.GetComponent<Tower>().towerCost;
            if (towerCost > lm.Gold && goldCheck)
                Destroy(tower);
            else
            {
                towerPlaced = true;
                if (goldCheck) lm.Gold -= towerCost;
                tower.transform.position = transform.position + Vector3.up;
                lm.UpdateTowerPlacedMatrix(true, locX, locY);
            }
        }
    }

    private void RemoveTower()
    {
        if (towerPlaced)
        {
            towerPlaced = false;
            lm.Gold += towerCost / 2;
            Destroy(tower);
            lm.UpdateTowerPlacedMatrix(false, locX, locY);
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButton(1))
        {
            RemoveTower();
        }
    }
}