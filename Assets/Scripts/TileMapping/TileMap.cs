namespace Game.TileMapping.Unity
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// Provides a component for tile mapping.
    /// </summary>

    public class TileMap : MonoBehaviour
    {

        /// <summary>
        /// Gets or sets the number of rows of tiles.
        /// </summary>
        public int Rows;

        /// <summary>
        /// Gets or sets the number of columns of tiles.
        /// </summary>
        public int Columns;

        /// <summary>
        /// Gets or sets the value of the tile width.
        /// </summary>
        public float TileWidth = 16f;

        /// <summary>
        /// Gets or sets the value of the tile height.
        /// </summary>
        public float TileHeight = 16f;
        public bool isBGTileMap;
        public List<GameObject> backgrounds;
        public string nameForBgObject;
        List<GameObject> bg;
        GameObject player;
        public GameObject tile;
        public int[,] logicMap;
        /// <summary>
        /// Used by editor components or game logic to indicate a tile location.
        /// </summary>
        /// <remarks>This will be hidden from the inspector window. See <see cref="HideInInspector"/></remarks>
        [HideInInspector]
        public Vector3
            MarkerPosition;
        [HideInInspector]
        public Vector3
            startSelection;
        [HideInInspector]
        public Vector3
            endSelection;
        [HideInInspector]
        public bool
            selectionStarted;

        public GameObject tileToPlace;
        public Sprite tile0, tile1;
        /// <summary>
        /// Initializes a new instance of the <see cref="TileMap"/> class.
        /// </summary>
        public TileMap()
        {
            this.Columns = 20;
            this.Rows = 10;
        }

        void Awake()
        {
            Invoke("SearchForPlayer", 0.5f);
            logicMap = new int[Columns, Rows];
            GenerateEmptyBoard();
        }

        void GenerateEmptyBoard()
        {
            int spriteType = 0;
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    GameObject tempTile = (GameObject)Instantiate(tileToPlace);
                    tempTile.transform.position = new Vector2(x+0.5f, y+0.5f);

                    tempTile.transform.parent = this.transform;

                    if (spriteType % 2 == 0)
                    {
                        tempTile.GetComponent<SpriteRenderer>().sprite = tile0;
                    }
                    else
                    {
                        tempTile.GetComponent<SpriteRenderer>().sprite = tile1;
                    }
                    spriteType++;
                }
                spriteType++;
            }
        }

        void SearchForPlayer()
        {
            player = (GameObject.FindGameObjectWithTag("Player"));

            bg = new List<GameObject>();
        }


        /// <summary>
        /// When the game object is selected this will draw the grid
        /// </summary>
        /// <remarks>Only called when in the Unity editor.</remarks>
        private void OnDrawGizmosSelected()
        {

            // store map width, height and position
            var mapWidth = this.Columns * this.TileWidth;
            var mapHeight = this.Rows * this.TileHeight;
            var position = this.transform.position;

            
            // draw layer border
            Gizmos.color = Color.white;
            Gizmos.DrawLine(position, position + new Vector3(mapWidth, 0, 0));
            Gizmos.DrawLine(position, position + new Vector3(0, mapHeight, 0));
            Gizmos.DrawLine(position + new Vector3(mapWidth, 0, 0), position + new Vector3(mapWidth, mapHeight, 0));
            Gizmos.DrawLine(position + new Vector3(0, mapHeight, 0), position + new Vector3(mapWidth, mapHeight, 0));

            // draw tile cells
            Gizmos.color = Color.grey;
            for (float i = 1; i < this.Columns; i++)
            {
                Gizmos.DrawLine(position + new Vector3(i * this.TileWidth, 0, 0), position + new Vector3(i * this.TileWidth, mapHeight, 0));
            }
            
            for (float i = 1; i < this.Rows; i++)
            {
                Gizmos.DrawLine(position + new Vector3(0, i * this.TileHeight, 0), position + new Vector3(mapWidth, i * this.TileHeight, 0));
            }

            // Draw marker position
            Gizmos.color = Color.red;    
            Gizmos.DrawWireCube(this.MarkerPosition, new Vector3(this.TileWidth, this.TileHeight, 1) * 1.1f);
                        
            if (selectionStarted)
            {
                Gizmos.color = Color.green;
                Vector3 LeftBound = new Vector3(startSelection.x, endSelection.y, 0);
                Gizmos.DrawLine(startSelection, LeftBound);
                LeftBound = new Vector3(endSelection.x, startSelection.y, 0);
                Gizmos.DrawLine(startSelection, LeftBound);
                LeftBound = new Vector3(endSelection.x, startSelection.y, 0);
                Gizmos.DrawLine(endSelection, LeftBound);
                LeftBound = new Vector3(startSelection.x, endSelection.y, 0);
                Gizmos.DrawLine(endSelection, LeftBound);
            }
        }
    }
}
