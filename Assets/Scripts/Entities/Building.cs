using UnityEngine;
using System.Collections;
using Game.TileMapping.Unity;

public class Building : MonoBehaviour {

    SpriteRenderer render;
    public Sprite damageSprite;
    public int LifePoints = 2;
    GameManager manager;
	// Use this for initialization
	void Start () {
        render = GetComponent<SpriteRenderer>();
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
	}


    public void DoDamage(int val)
    {
        LifePoints -= val;

        if (LifePoints <= 0)
        {
            manager.UpdateBuildingCount(gameObject);
            GameObject.FindGameObjectWithTag("Map").GetComponent<TileMap>().logicMap[(int)Mathf.Floor(transform.position.x), (int)Mathf.Floor(transform.position.y)] = 0;
            render.sprite = damageSprite;
            transform.tag = "Tile";
            render.sortingOrder = -2;
        }
    }
}
