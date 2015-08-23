using UnityEngine;
using System.Collections;
using Game.TileMapping.Unity;

public class TouchScript : MonoBehaviour {

    public LayerMask tilesMask;
    TileMap board;
    PlayerScript pScript;
    GameManager manager;

	// Use this for initialization
	void Start () {
        board = GameObject.FindGameObjectWithTag("Map").GetComponent<TileMap>();
        pScript = GetComponent<PlayerScript>();
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
        if (pScript.canMove)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit;
                hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1, tilesMask);

                if (hit)
                {
                    if (hit.collider.CompareTag("Building"))
                    {
                        if (manager.CanPlayerAttackThere((int)Mathf.Floor(hit.collider.transform.position.x), (int)Mathf.Floor(hit.collider.transform.position.y)))
                            hit.collider.GetComponent<Building>().DoDamage(pScript.damage);
                        else
                            Debug.Log("cant attack there");
                        return;
                    }
                    if (hit.collider.CompareTag("Tile"))
                    {
                        if (manager.CanPlayerMoveThere((int)Mathf.Floor(hit.collider.transform.position.x), (int)Mathf.Floor(hit.collider.transform.position.y)))
                            pScript.SetToMove(hit.collider.transform.position);
                        else
                            Debug.Log("cant move there");
                    }
                }
                return;
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (manager.CanPlayerMoveThere((int)Mathf.Floor(transform.position.x), (int)Mathf.Floor(transform.position.y) + 1))
                    pScript.SetToMove(new Vector2((transform.position.x), (transform.position.y) + 1));
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                if (manager.CanPlayerMoveThere((int)Mathf.Floor(transform.position.x), (int)Mathf.Floor(transform.position.y) - 1))
                    pScript.SetToMove(new Vector2((transform.position.x), (transform.position.y) - 1));
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (manager.CanPlayerMoveThere((int)Mathf.Floor(transform.position.x) - 1, (int)Mathf.Floor(transform.position.y)))
                    pScript.SetToMove(new Vector2((transform.position.x) - 1, (transform.position.y)));
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                if (manager.CanPlayerMoveThere((int)Mathf.Floor(transform.position.x) + 1, (int)Mathf.Floor(transform.position.y)))
                    pScript.SetToMove(new Vector2((transform.position.x) + 1, (transform.position.y)));
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (manager.CanPlayerAttackThere((int)Mathf.Floor(transform.position.x), (int)Mathf.Floor(transform.position.y) + 1))
                    manager.AttackItem((int)Mathf.Floor(transform.position.x), (int)Mathf.Floor(transform.position.y) + 1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (manager.CanPlayerAttackThere((int)Mathf.Floor(transform.position.x), (int)Mathf.Floor(transform.position.y) - 1))
                    manager.AttackItem((int)Mathf.Floor(transform.position.x), (int)Mathf.Floor(transform.position.y) - 1);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (manager.CanPlayerAttackThere((int)Mathf.Floor(transform.position.x) - 1, (int)Mathf.Floor(transform.position.y)))
                    manager.AttackItem((int)Mathf.Floor(transform.position.x) - 1, (int)Mathf.Floor(transform.position.y));
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (manager.CanPlayerAttackThere((int)Mathf.Floor(transform.position.x) + 1, (int)Mathf.Floor(transform.position.y)))
                    manager.AttackItem((int)Mathf.Floor(transform.position.x) + 1, (int)Mathf.Floor(transform.position.y));
            }
        }
	}


}
