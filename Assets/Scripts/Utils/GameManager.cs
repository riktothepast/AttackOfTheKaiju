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
    public GameObject guardian;
    PlayerScript player;
    public Text buildingsText, enemiesText;
    int builingCount, enemyCount;
    public Canvas gameOver, gameWin;
	// Use this for initialization
	void Start () {
        board = GameObject.FindGameObjectWithTag("Map").GetComponent<TileMap>();
        GenerateNewBuildingsAtRandomPositions(buidingsToDestroy);
        GenerateNewEnemies(maxEnemies);
        SpawnPlayer();
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
            Vector2 newPos = GetEmptyPosition(0, board.Columns, board.Rows/2, board.Rows);
            board.logicMap[(int)newPos.x, (int)newPos.y] = 3;
            GameObject enemy = (GameObject)Instantiate(enemies[Random.Range(0, enemies.Count)], new Vector2((int)newPos.x + 0.5f, (int)newPos.y + 0.5f), transform.rotation);
            enemies.Add(enemy);
        }
    }

    void GenerateGuardian()
    {
        List<Vector2> levelCorners = new List<Vector2>();
        levelCorners.Add(new Vector2(0, 0));
        levelCorners.Add(new Vector2(0, board.Rows-1));
        levelCorners.Add(new Vector2(board.Columns-1, 0));
        levelCorners.Add(new Vector2(board.Columns-1, board.Rows-1));
        Vector2 newPos = levelCorners[Random.Range(0, levelCorners.Count)];
        board.logicMap[(int)newPos.x, (int)newPos.y] = 3;
        GameObject enemy = (GameObject)Instantiate(guardian, new Vector2((int)newPos.x + 0.5f, (int)newPos.y + 0.5f), transform.rotation);
        enemies.Add(enemy);
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
        for (int x = 0; x < enemies.Count; x++)
        {
            if ((int)Mathf.Floor(enemies[x].transform.position.x) == (int)posX && (int)Mathf.Floor(enemies[x].transform.position.y) == (int)posY)
            {
                enemies[x].GetComponent<EnemyScript>().DoDamage(player.damage);
                SetToUpdateEntities();
                break;
            }
        }
    }

    public void UpdateBuildingCount(GameObject obj)
    {
        builingCount++;
        if (builingCount == buidingsToDestroy - 5)
        {
            GenerateGuardian();
        }
        buildings.Remove(obj);
        buildingsText.text = "Buildings: \n" + builingCount.ToString("D2");
        if (builingCount >= buidingsToDestroy)
        {
            GameWon();
        }
    }

    public void UpdateEnemyCount(GameObject obj)
    {
        enemyCount++;
        enemies.Remove(obj);
        enemiesText.text = "Enemies: \n" + enemyCount.ToString("D2");
   
    }

    public void GameOver()
    {
        gameOver.GetComponent<Canvas>().enabled = true;
        Debug.Log("game is over");
    }

    public void GameWon()
    {
        gameWin.GetComponent<Canvas>().enabled = true;
        Debug.Log("game is over");
    }

}
