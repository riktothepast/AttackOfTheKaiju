using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.TileMapping.Unity;
using Prime31.ZestKit;


public class UltramanScript : EnemyScript
{
    bool shouldSearchWhereToMove = false;
    int moveThisTurn;
    public override void Start()
    {
        board = GameObject.FindGameObjectWithTag("Map").GetComponent<TileMap>();
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }


    public override void ActivateUpdate()
    {
        if (moveThisTurn == 0)
        {
            shouldSearchWhereToMove = true;
            moveThisTurn++;
        }
        else
            moveThisTurn = 0;
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
                if (!foundSmaller)
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
            AttackPlayer();
        }
    }

    void AttackPlayer()
    {
        if (((int)Mathf.Floor(transform.position.x) - 1 > 0)){
        if(board.logicMap[(int)Mathf.Floor(transform.position.x) -1, (int)Mathf.Floor(transform.position.y)] == 1)
        {
            transform.ZKpositionTo(transform.position + new Vector3(Random.Range(Random.Range(-0.5f, -0.3f), Random.Range(0.3f, 0.5f)), Random.Range(Random.Range(-0.5f, -0.3f), Random.Range(0.3f, 0.5f)), 0), 0.1f)
            .setLoops(LoopType.PingPong)
            .setCompletionHandler(t =>
            {
                Player.DoDamage(damage);
            }).start();
        }
        }

        if (((int)Mathf.Floor(transform.position.x) + 1 < board.Columns - 1)){
        if(board.logicMap[(int)Mathf.Floor(transform.position.x) +1, (int)Mathf.Floor(transform.position.y)] == 1)
        {
            transform.ZKpositionTo(transform.position + new Vector3(Random.Range(Random.Range(-0.5f, -0.3f), Random.Range(0.3f, 0.5f)), Random.Range(Random.Range(-0.5f, -0.3f), Random.Range(0.3f, 0.5f)), 0), 0.1f)
            .setLoops(LoopType.PingPong)
            .setCompletionHandler(t =>
            {
                Player.DoDamage(damage);
            }).start();
        }
        }

        if (((int)Mathf.Floor(transform.position.y) - 1 > 0)){
        if(board.logicMap[(int)Mathf.Floor(transform.position.x) , (int)Mathf.Floor(transform.position.y)-1] == 1)
        {
           transform.ZKpositionTo(transform.position + new Vector3(Random.Range(Random.Range(-0.5f, -0.3f), Random.Range(0.3f, 0.5f)), Random.Range(Random.Range(-0.5f, -0.3f), Random.Range(0.3f, 0.5f)), 0), 0.1f)
            .setLoops(LoopType.PingPong)
            .setCompletionHandler(t =>
            {
                Player.DoDamage(damage);
            }).start();
        }
        }

        if (((int)Mathf.Floor(transform.position.y) + 1 <board.Rows - 1)){
        if(board.logicMap[(int)Mathf.Floor(transform.position.x) , (int)Mathf.Floor(transform.position.y)+1] == 1)
        {
            transform.ZKpositionTo(transform.position + new Vector3(Random.Range(Random.Range(-0.5f, -0.3f), Random.Range(0.3f, 0.5f)), Random.Range(Random.Range(-0.5f, -0.3f), Random.Range(0.3f, 0.5f)), 0), 0.1f)
            .setLoops(LoopType.PingPong)
            .setCompletionHandler(t =>
            {
                Player.DoDamage(damage);
            }).start();
        }
        }
               
            
        
    }

    List<Vector2> WhereCanIGo()
    {
        /*
         * This fella moves like the kaiju.
         *          []
         *        [] U []
         *          [] 
         */
        List<Vector2> posibleDestinations = new List<Vector2>();
            if (transform.position.x + 1 < board.Columns)
            {
                if (board.logicMap[(int)Mathf.Floor(transform.position.x) + 1, (int)Mathf.Floor(transform.position.y)] == 0)
                {
                    posibleDestinations.Add(new Vector2(transform.position.x + 1, transform.position.y));
                }
            }
            if (transform.position.x - 1 > 0)
            {

                if (board.logicMap[(int)Mathf.Floor(transform.position.x) - 1, (int)Mathf.Floor(transform.position.y)] == 0)
                {
                    posibleDestinations.Add(new Vector2(transform.position.x - 1, transform.position.y ));
                }
            }
            if (transform.position.y + 1 < board.Rows)
            {
                if (board.logicMap[(int)Mathf.Floor(transform.position.x), (int)Mathf.Floor(transform.position.y) + 1] == 0)
                {
                    posibleDestinations.Add(new Vector2(transform.position.x, transform.position.y + 1));
                }
            }
        if (transform.position.y - 1 > 0)
        {
                if (board.logicMap[(int)Mathf.Floor(transform.position.x), (int)Mathf.Floor(transform.position.y) - 1] == 0)
                {
                    posibleDestinations.Add(new Vector2(transform.position.x, transform.position.y - 1));
                }
        }
        
        return posibleDestinations;
    }

    public override void DoDamage(int val)
    {
        base.DoDamage(val);
        if (lifePoints <= 0)
        {
            transform.ZKpositionTo(transform.position + new Vector3(Random.Range(Random.Range(-0.5f, -0.3f), Random.Range(0.3f, 0.5f)), Random.Range(Random.Range(-0.5f, -0.3f), Random.Range(0.3f, 0.5f)), 0), 0.1f)
            .setLoops(LoopType.PingPong)
            .setCompletionHandler(t =>
            {
                if (lifePoints <= 0)
                {
                    manager.UpdateEnemyCount(this.gameObject);
                    GameObject.FindGameObjectWithTag("Map").GetComponent<TileMap>().logicMap[(int)Mathf.Floor(transform.position.x), (int)Mathf.Floor(transform.position.y)] = 0;
                    Destroy(this.gameObject);
                }
            }).start();
        }
    }
}
