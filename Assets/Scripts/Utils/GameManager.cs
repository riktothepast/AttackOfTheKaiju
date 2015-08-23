using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Game.TileMapping.Unity;

public class GameManager : MonoBehaviour {

    public int buidingsToDestroy = 5;
    public int maxEnemies = 2;
    TileMap board;
    public List<GameObject> buildings;
    public List<GameObject> enemies;
    public GameObject playerToSpawn;
    PlayerScript player;
    public Text buildingsText, enemiesText;
    int builingCount;
	// Use this for initialization
	void Start () {
        board = GameObject.FindGameObjectWithTag("Map").GetComponent<TileMap>();
        GenerateNewBuildingsAtRandomPositions(buidingsToDestroy);
        GenerateNewEnemies(maxEnemies);
        SpawnPlayer();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetToUpdateEntities()
    {
        for (int x = 0; x < enemies.Count; x++)
        {
            enemies[x].GetComponent<EnemyScript>().ActivateUpdate();
        }
    }

    void GenerateNewBuildingsAtRandomPositions(int builings)
    {
        for (int x = 0; x < builings; x++)
        {
            Vector2 newPos = GetEmptyPosition(0, board.Columns, 0, board.Rows);
            board.logicMap[(int)newPos.x, (int)newPos.y] = 2;
            GameObject building = (GameObject)Instantiate(buildings[Random.Range(0, buildings.Count)], new Vector2((int)newPos.x + 0.5f, (int)newPos.y + 0.5f), transform.rotation);
            building.GetComponent<SpriteRenderer>().sortingOrder = board.Rows - (int)Mathf.Ceil(newPos.y);
            buildings.Add(building);
        }
    }

    void GenerateNewEnemies(int maxEnemies)
    {
        for (int x = 0; x < maxEnemies; x++)
        {
            Vector2 newPos = GetEmptyPosition(0, board.Columns, board.Rows-2, board.Rows);
            board.logicMap[(int)newPos.x, (int)newPos.y] = 2;
            GameObject enemy = (GameObject)Instantiate(enemies[Random.Range(0, enemies.Count)], new Vector2((int)newPos.x + 0.5f, (int)newPos.y + 0.5f), transform.rotation);
            enemies.Add(enemy);
        }
    }

    Vector2 GetEmptyPosition(int initialX, int finalX, int initialY, int finalY)
    {
        int newPosX = Random.Range(initialX, finalX);
        int newPosY = Random.Range(initialY, finalY);
        if (board.logicMap[newPosX, newPosY] != 0)
            return GetEmptyPosition(initialX, finalX, initialY, finalY);

        return new Vector2(newPosX, newPosY);
    }

    void SpawnPlayer()
    {
        Vector2 newPos = GetEmptyPosition(0, board.Columns, 0, 1);
        Instantiate(playerToSpawn, new Vector2((int)newPos.x + 0.5f, (int)newPos.y + 0.5f), transform.rotation);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
    }

    public Vector2 PixelsToTiles(Vector2 position)
    {
        return new Vector2(Mathf.Floor(position.x), Mathf.Floor(position.y));
    }

    public bool CanPlayerMoveThere(int x, int y)
    {
        /*
         * Player can only move one tile this way:
         *          []
         *        [] P []
         *          [] 
         */
        if (x < 0 || x > board.Columns || y < 0 || y > board.Rows)
            return false;
        if (board.logicMap[x, y] != 0)
            return false;

        if (Mathf.Floor(player.transform.position.y) == y)
        {
            if (Mathf.Floor(player.transform.position.x) + 1 == x)
            { 
                return true;
            }else if (Mathf.Floor(player.transform.position.x) - 1 == x)
            {
                return true;
            }
        }
        else if (Mathf.Floor(player.transform.position.x) == x)
        {
            if (Mathf.Floor(player.transform.position.y) + 1 == y)
            {
                return true;
            }
            else if (Mathf.Floor(player.transform.position.y) - 1 == y)
            {
                return true;
            }
        }

        return false;
    }

    public bool CanPlayerAttackThere(int x, int y)
    {
        if (x < 0 || x > board.Columns || y < 0 || y > board.Rows)
            return false;
        if (board.logicMap[x, y] == 0)
            return false;

        if (Mathf.Floor(player.transform.position.y) == y)
        {
            if (Mathf.Floor(player.transform.position.x) + 1 == x)
            {
                return true;
            }
            else if (Mathf.Floor(player.transform.position.x) - 1 == x)
            {
                return true;
            }
        }
        else if (Mathf.Floor(player.transform.position.x) == x)
        {
            if (Mathf.Floor(player.transform.position.y) + 1 == y)
            {
                return true;
            }
            else if (Mathf.Floor(player.transform.position.y) - 1 == y)
            {
                return true;
            }
        }

        return false;
    }

    public void AttackItem(int posX, int posY)
    {
        for (int x = 0; x < buildings.Count; x++)
        {
            if ((int)Mathf.Floor(buildings[x].transform.position.x) == (int)posX && (int)Mathf.Floor(buildings[x].transform.position.y) == (int)posY)
            {
                buildings[x].GetComponent<Building>().DoDamage(player.damage);
                SetToUpdateEntities();
                break;
            }
        }
    }

    public void UpdateBuildingCount(GameObject obj)
    {
        builingCount++;
        buildings.Remove(obj);
        buildingsText.text = "Buildings: \n" + builingCount.ToString("D2");
    }

}
