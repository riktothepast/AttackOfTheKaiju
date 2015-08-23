using UnityEngine;
using System.Collections;
using Game.TileMapping.Unity;
using Prime31.ZestKit;

public class Building : MonoBehaviour {

    SpriteRenderer render;
    public GameObject damageSprite;
    public int LifePoints = 2;
    GameManager manager;
	// Use this for initialization
	void Start () {
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
	}


    public void DoDamage(int val)
    {
        LifePoints -= val;
        transform.ZKpositionTo(transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0), 0.1f)
        .setLoops(LoopType.PingPong)
        .setCompletionHandler(t => { 
        if (LifePoints <= 0)
        {
            manager.UpdateBuildingCount(gameObject);
            GameObject.FindGameObjectWithTag("Map").GetComponent<TileMap>().logicMap[(int)Mathf.Floor(transform.position.x), (int)Mathf.Floor(transform.position.y)] = 0;
            Instantiate(damageSprite,transform.position,transform.rotation);
            Destroy(this.gameObject);
        }
        }).start();

    }
}
