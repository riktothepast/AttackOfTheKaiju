using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.TileMapping.Unity;

public class TankScript : EnemyScript {
    bool shouldSearchWhereToMove = false;
    public TankTurrent tankTurret;

	// Use this for initialization
	public override void Start () 
    {
        board = GameObject.FindGameObjectWithTag("Map").GetComponent<TileMap>();
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
	}

    public override void ActivateUpdate()
    {
        shouldSearchWhereToMove = true;
    }

    public override void Update()
    {
        if (shouldSearchWhereToMove && !shouldMove)
        {
            shouldSearchWhereToMove = false;
            List<Vector2> posibleDestinations = WhereCanIGo();
            if (posibleDestinations.Count > 0)
            {
                board.logicMap[(int)transform.position.x, (int)transform.position.y] = 0;
                float distance = Vector2.Distance(Player.transform.position, transform.position);
                bool foundSmaller = false;
                for (int x = 0; x < posibleDestinations.Count; x++)
                {
                    if (Vector2.Distance(Player.transform.position, posibleDestinations[x]) <= distance)
                    {
                        whereToMove = posibleDestinations[x];
                        foundSmaller = true;
                    }
                }
                if(!foundSmaller)
                    whereToMove = posibleDestinations[Random.Range(0, posibleDestinations.Count)];
                board.logicMap[(int)whereToMove.x, (int)whereToMove.y] = 3;
                shouldMove = true;
            }
        }
        if (shouldMove)
        {
            UpdatePosition();
        }

    }

    public override void UpdatePosition()
    {
        transform.position = Vector3.Lerp(transform.position, whereToMove, movementSpeed * Time.deltaTime);
      

        if (Vector3.Distance(transform.position, whereToMove) < 0.009f)
        {
            shouldMove = false;
            canMove = true;
            transform.position = whereToMove;
            tankTurret.RotateTurret();
        }
    }

    List<Vector2> WhereCanIGo()
    {
        /*
         * This fella can only move in diagonals
         *    []{X}[]
         *  {X}  T  {X}
         *    []{X}[]
         */
        List<Vector2> posibleDestinations = new List<Vector2>();
        if (transform.position.y + 1 < board.Rows)
        {   // no buildings at the sides and diagonal is free too.
            if(transform.position.x + 1< board.Columns)
            {
                if (board.logicMap[(int)Mathf.Floor(transform.position.x), (int)Mathf.Floor(transform.position.y)+1] == 0
                    && board.logicMap[(int)Mathf.Floor(transform.position.x)+1, (int)Mathf.Floor(transform.position.y)] == 0
                    && board.logicMap[(int)Mathf.Floor(transform.position.x)+1, (int)Mathf.Floor(transform.position.y)+1] == 0)
                {
                    posibleDestinations.Add( new Vector2(transform.position.x +1 , transform.position.y+1));
                }
            }
            if(transform.position.x - 1 > 0)
            {
                
                if (board.logicMap[(int)Mathf.Floor(transform.position.x), (int)Mathf.Floor(transform.position.y) + 1] == 0
                   && board.logicMap[(int)Mathf.Floor(transform.position.x) - 1, (int)Mathf.Floor(transform.position.y)] == 0
                   && board.logicMap[(int)Mathf.Floor(transform.position.x) - 1, (int)Mathf.Floor(transform.position.y) + 1] == 0)
                {
                    posibleDestinations.Add(new Vector2(transform.position.x - 1, transform.position.y + 1));
                }
            }
        }
        if (transform.position.y - 1 > 0)
        {
            if (transform.position.x - 1 > 0)
            {
                if (board.logicMap[(int)Mathf.Floor(transform.position.x), (int)Mathf.Floor(transform.position.y) - 1] == 0
                           && board.logicMap[(int)Mathf.Floor(transform.position.x) - 1, (int)Mathf.Floor(transform.position.y)] == 0
                           && board.logicMap[(int)Mathf.Floor(transform.position.x) - 1, (int)Mathf.Floor(transform.position.y) - 1] == 0)
                {
                    posibleDestinations.Add(new Vector2(transform.position.x - 1, transform.position.y - 1));
                }
            }
            if (transform.position.x + 1 < board.Columns)
            {
                if (board.logicMap[(int)Mathf.Floor(transform.position.x), (int)Mathf.Floor(transform.position.y) - 1] == 0
                   && board.logicMap[(int)Mathf.Floor(transform.position.x) + 1, (int)Mathf.Floor(transform.position.y)] == 0
                   && board.logicMap[(int)Mathf.Floor(transform.position.x) + 1, (int)Mathf.Floor(transform.position.y) - 1] == 0)
                {
                    posibleDestinations.Add(new Vector2(transform.position.x + 1, transform.position.y - 1));
                }
            }
        }
        return posibleDestinations;
    }
}
