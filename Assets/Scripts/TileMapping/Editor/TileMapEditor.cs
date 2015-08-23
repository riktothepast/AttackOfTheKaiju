namespace Game.Unity.Editors.Editor
{
		using System;
		using System.Collections.Generic;
		using Game.TileMapping.Unity;
		using UnityEditor;
		using MiniJSON;
		using UnityEngine;
        using System.IO;

		/// <summary>
		/// Provides a editor for the <see cref="TileMap"/> component
		/// </summary>
		[CustomEditor(typeof(TileMap))]
		public class TileMapEditor : Editor
		{

				public enum StateEditor
				{
						Single,
						Mutli
				}

				public enum SubMultiState
				{
						Draw,
						Erase
				}

				/// <summary>
				/// Holds the location of the mouse hit location
				/// </summary>
				private Vector3 mouseHitPos;
				private Vector2 startPosSelection;
				private Vector2 endPosSelection;
				public GameObject tilePrefab;
				List<GameObject> prefabs;
                List<Texture2D> textures = new List<Texture2D> ();
				int currentPrefab = 0;
				public bool isEditing, replaceTiles;
				public bool overrideBottom;
				public SubMultiState mutliToolState;
				public StateEditor state;

                private int tileSetIndex;
                private int selectedtileIndex;
                private Vector2 tileScrollPosition;
                private List<string> levelList = new List<string>();
                private List<GameObject> activeTilesetPrefabs = new List<GameObject>();
                private List<Texture2D> activeTilesetPreviews = new List<Texture2D>();


                public Vector2 lastTileSize = new Vector2(12, 12);
                public Vector2 newTileSize = new Vector2(12, 12);

				enum MapMode
				{
						Normal,
						Waypoints,
						Portals,
                        WeaponSpawners
				}

				MapMode enabledMode;

				/// <summary>
				/// Lets the Editor handle an event in the scene view.
				/// </summary>
				private void OnSceneGUI ()
				{
						// if UpdateHitPosition return true we should update the scene views so that the marker will update in real time
						if (this.UpdateHitPosition ()) {
								SceneView.RepaintAll ();
						}

						// Calculate the location of the marker based on the location of the mouse
						this.RecalculateMarkerPosition ();

						this.CalculateEndSelection ();



						// get a reference to the current event
						Event current = Event.current;

						// if the mouse is positioned over the layer allow drawing actions to occur
						if (isEditing) {
								switch (state) {
								case StateEditor.Single:

										if (IsMouseOnLayer ()) {
												var map = (TileMap)target;
												map.selectionStarted = false;
												// if mouse down or mouse drag event occurred
												if (current.type == EventType.MouseDown || current.type == EventType.MouseDrag) {
														if (current.button == 1) {
																// if right mouse button is pressed then we erase blocks
																this.Erase ();
																current.Use ();
														} else if (current.button == 0) {
																// if left mouse button is pressed then we draw blocks
																this.Draw ();
																current.Use ();
														}
												}
										}
										break;
								case StateEditor.Mutli:

										if (this.IsMouseOnLayer ()) {
												// if mouse down or mouse drag event occurred
												if (current.rawType == EventType.mouseDown || current.rawType == EventType.mouseDrag) {
														var map = (TileMap)this.target;
														if (map.selectionStarted == false) {
																if (current.button == 0) {
																		// if left mouse button is pressed then we draw blocks
																		mutliToolState = SubMultiState.Draw;
	  																	map.selectionStarted = true;
																		this.CalculateStartSelection ();
																		this.startPosSelection = this.GetTilePositionFromLocation (map.startSelection);
																		current.Use ();
																}
																if (current.button == 1) {
																		// if left mouse button is pressed then we draw blocks
																		mutliToolState = SubMultiState.Erase;
																		map.selectionStarted = true;
																		this.CalculateStartSelection ();
																		this.startPosSelection = this.GetTilePositionFromLocation (map.startSelection);
																		current.Use ();
																}

														} else {
																switch (mutliToolState) {
																case SubMultiState.Draw:

																		if (current.button == 0) {
																				// if left mouse button is pressed then we draw blocks
																				map.selectionStarted = false;
																				endPosSelection = this.GetTilePositionFromLocation (map.endSelection);
																				this.SubRenderTiles ();
																				current.Use ();
																		}
																		if (current.button == 1) {
																				// if right mouse button is pressed then we erase blocks
																				map.selectionStarted = false;
																				current.Use ();
																		}

																		break;
																case SubMultiState.Erase:
																		if (current.button == 0) {
																				// if left mouse button is pressed then we draw blocks
																				map.selectionStarted = false;
																				endPosSelection = this.GetTilePositionFromLocation (map.endSelection);
																				this.SubRemoveTiles ();
																				current.Use ();
																		}
																		if (current.button == 1) {
																				// if right mouse button is pressed then we erase blocks
																				map.selectionStarted = false;
																				current.Use ();
																		}

																		break;
																}
														}





												}
										}
										break;
								}

								if (Event.current.type == EventType.keyDown && Event.current.keyCode == (KeyCode.D) && currentPrefab < prefabs.Count - 1) {
										currentPrefab++;
										var map = (TileMap)this.target;
										map.tile = prefabs [currentPrefab];

								}

								if (Event.current.type == EventType.keyDown && Event.current.keyCode == (KeyCode.A) && currentPrefab > 0) {
										currentPrefab--;
										var map = (TileMap)this.target;
										map.tile = prefabs [currentPrefab];

								}
						}


						// draw a UI tip in scene view informing user how to draw & erase tiles
						if (isEditing) {
								Handles.BeginGUI ();
								GUI.Label (new Rect (10, Screen.height - 90, 100, 100), "LMB: Draw");
								GUI.Label (new Rect (10, Screen.height - 105, 100, 100), "RMB: Erase");
								GUI.Label (new Rect (10, Screen.height - 75, 300, 100), "Mouse Position: " + this.mouseHitPos);
								GUI.Label (new Rect (10, Screen.height - 55, 300, 100), "MousePositionInTiles: " + this.GetTilePositionFromMouseLocation ());

								GUI.Label (new Rect (10, 45, 300, 100), "Start Selection: " + startPosSelection);
								GUI.Label (new Rect (10, 65, 300, 100), "End Selection: " + endPosSelection);
								Handles.EndGUI ();
						} else {
								var map = (TileMap)this.target;
								map.selectionStarted = false;
						}
				}

				public override void OnInspectorGUI ()
				{
						DrawDefaultInspector ();
						if (GUILayout.Button ("Clean Map")) {
								cleanMap ();
						}

						if (GUILayout.Button ("Generate Backgrounds")) {
								var map = (TileMap)this.target;
								var backgrounds = new GameObject (map.nameForBgObject);
								backgrounds.tag = "Backgrounds";
								backgrounds.transform.parent = map.transform;
								for (int i = 0; i < map.backgrounds.Count; i++) {
										var newBG = Instantiate (map.backgrounds [i]) as GameObject;
										newBG.transform.parent = backgrounds.transform;
										newBG.transform.position = backgrounds.transform.position;
										newBG.GetComponent<Renderer> ().sortingOrder = (i * -1) - 1;
								}
						}

						if (GUILayout.Button ("Delete Backgrounds")) {
								var map = (TileMap)this.target;
								foreach (Transform child in map.transform) {
										if (child.gameObject.tag == "Backgrounds") {
												DestroyImmediate (map.transform.Find ("Backgrounds").gameObject);
										}
								}
						}
                        lastTileSize = EditorGUILayout.Vector2Field("Last Tile Size", lastTileSize);
                        newTileSize = EditorGUILayout.Vector2Field("New Tile Size", newTileSize);
                        if (GUILayout.Button("Re align map"))
                        {
                            ReAlignMap();
                        }

						enabledMode = (MapMode)EditorGUILayout.EnumPopup (enabledMode);
						isEditing = EditorGUILayout.Toggle ("Is Editing", isEditing);

						replaceTiles = EditorGUILayout.Toggle ("Should Replace Tiles", replaceTiles);

						if (isEditing) {
								OnEnable ();
								state = (StateEditor)EditorGUILayout.EnumPopup ("Editing Mode", state);
								if (state == StateEditor.Mutli) {
										overrideBottom = EditorGUILayout.Toggle ("Override Bottom", overrideBottom);
								}
						}

                        // Tilesets
                        DrawTilesetsPopup();
                        

				}

        private void DrawTilesetsPopup()
        {

            string[] fis = Directory.GetDirectories(Application.dataPath + "/Resources/Prefabs/Tiles/", "*", SearchOption.AllDirectories);
            string[] separators = new string[2]{".","/Prefabs/Tiles/"};
            foreach( string item in fis)
            {
                string[] names = item.Split(separators, System.StringSplitOptions.None);
                levelList.Add(names[1]);
            }

            EditorGUI.BeginChangeCheck();
            tileSetIndex = EditorGUILayout.Popup(tileSetIndex, levelList.ToArray());
            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.UnloadUnusedAssetsImmediate();
                activeTilesetPrefabs.Clear();
                activeTilesetPreviews.Clear();
                string[] prefabs = Directory.GetFiles(Application.dataPath + "/Resources/Prefabs/Tiles/" + levelList[tileSetIndex] +"/", "*.prefab", SearchOption.TopDirectoryOnly);
                string[] pfseparators = new string[2]{".prefab","/Resources/"};
                foreach(string pf in prefabs)
                {
                    string[] names = pf.Split(pfseparators, System.StringSplitOptions.None);
                    activeTilesetPrefabs.Add((GameObject) Resources.Load( names[1] ));
                }
                AllocateTextures();
            }
            DrawTilesetsBox();


        }    
            

        private void DrawTilesetsBox()
        {
            if(AssetPreview.IsLoadingAssetPreviews())
            {
                this.Repaint();
            }
            GUILayout.BeginVertical(GUI.skin.box);
            Rect boxRect = GUILayoutUtility.GetRect(Screen.width - 50, 200);
            GUILayout.EndVertical();

            int numRows = 0;
            var map = (TileMap) target;
            int numCols = (int)((Screen.width - 50 ) / (32 + 4));
            if (activeTilesetPrefabs.Count > 0)
            {
                numRows = activeTilesetPrefabs.Count / numCols;
                if (numRows * numCols < activeTilesetPrefabs.Count)
                {
                    numRows += 1;
                }
            }

//            var boxRect = GUILayoutUtility.GetRect(Screen.width - 50, 32);;

            tileScrollPosition = GUI.BeginScrollView( boxRect, tileScrollPosition, new Rect(0, 0, boxRect.width - 50,  4 + numRows * (32 + 4)) );
            
            float x = 4;
            float y = 4;
            int count = 0;
            
            for (int row = 0; row < numRows; ++row)
            {
                x = 4;
                GUILayout.BeginHorizontal();
                
                
                for (int col = 0; col < numCols; ++col)
                {
                    int index = col + row * numCols;
                    if (index >= activeTilesetPrefabs.Count)
                    {
                        break;
                    }
                    
//                    Texture2D p = CreateTexture( (AssetPreview.GetAssetPreview( activeTilesetPrefabs[index] )) ); 
                    activeTilesetPreviews.Add( textures[index] );

//                    // double check the cache exists, sometimes null in its doesnt create the preview right away
                    if (activeTilesetPreviews[index] == null)
                    {
                        continue;
                    }
                    Rect r = new Rect(x, y, 32, 32);
                    x += (32 + 4);
                    
                    Color saved = GUI.color;    
                    
                    GUI.color = new Color( Color.white.r,Color.white.g,Color.white.b,Color.white.a );
                    
                    if (GUI.Button( r, new GUIContent(activeTilesetPreviews[index], "" + activeTilesetPrefabs[index].name ) , GUIStyle.none ))
                    {
                        if (Event.current.button == 0)
                        {
                            map.tile = activeTilesetPrefabs[index];
                            selectedtileIndex = index;
                        }

                    }
                    
                    if ( selectedtileIndex == count )
                    {
                        GUIStyle style = new GUIStyle( GUI.skin.box );
                        GUI.color = new Color( 0,Color.white.g,0,Color.white.a * 2f/3f ); 
                        Rect r2 = new Rect(r.x-3, r.y-3, r.width+6, r.height+6);
                        GUI.Box(r2, activeTilesetPreviews[index], style);
                    }
                    
                    GUI.color = saved;
                    GUI.backgroundColor =saved;  
                    count++;
                }
                GUILayout.EndHorizontal();
                y += (32 + 4);
                
            }       
            
            GUI.EndScrollView();
        }

				private void cleanMap ()
				{
						var map = (TileMap)this.target;
						var children = new List<GameObject> ();
						foreach (Transform child in map.transform)
								children.Add (child.gameObject);
						children.ForEach (child => DestroyImmediate (child));
				}

                private void ReAlignMap()
                {
                    var map = (TileMap)this.target;
                    var children = new List<GameObject>();
                    foreach (Transform child in map.transform)
                        children.Add(child.gameObject);
                    foreach (GameObject c in children)
                    {
                        c.transform.position = new Vector3(c.transform.position.x / lastTileSize.x, c.transform.position.y / lastTileSize.y, c.transform.position.z);
                        c.transform.position = new Vector3(c.transform.position.x * newTileSize.x, c.transform.position.y * newTileSize.y, c.transform.position.z);

                    }
                }

				void getPrefabList ()
				{
						UnityEngine.Object[] pref = Resources.LoadAll ("Prefabs/Tiles", typeof(GameObject));
						foreach (UnityEngine.Object fab in pref) {
								var gameObj = (GameObject)fab;
								prefabs.Add (gameObj);
						}

				}

				/// <summary>
				/// When the <see cref="GameObject"/> is selected set the current tool to the view tool.
				/// </summary>
				private void OnEnable ()
				{
						if (isEditing) {
								Tools.current = Tool.View;
								Tools.viewTool = ViewTool.FPS;
//								prefabs = new List<GameObject> ();
//								getPrefabList ();
						}
				}

				/// <summary>
				/// Draws a block at the pre-calculated mouse hit position
				/// </summary>
				private void Draw ()
				{

						if (enabledMode == MapMode.Normal) {
								// get reference to the TileMap component
								var map = (TileMap)this.target;
								var newTile = false;

								var tilePos = this.GetTilePositionFromMouseLocation ();

								var actualPos = new Vector3 (tilePos.x * map.TileWidth, tilePos.y * map.TileHeight, 0);

								actualPos.x += map.TileWidth / 2;
								actualPos.y += map.TileHeight / 2;

								foreach (Transform child in map.transform) {
										if (child.position == actualPos) {
												if (replaceTiles) {
														DestroyImmediate (child.gameObject);
												} else {
														return;
												}
										}
								}
								var tile = PrefabUtility.InstantiatePrefab (map.tile) as GameObject;
								var prefab = PrefabUtility.FindPrefabRoot (tile);
								PrefabUtility.SetPropertyModifications (tile, PrefabUtility.GetPropertyModifications (prefab));
								newTile = true;

								// set the tiles position on the tile map
								var tilePositionInLocalSpace = new Vector3 ((tilePos.x * map.TileWidth) + (map.TileWidth / 2), (tilePos.y * map.TileHeight) + (map.TileHeight / 2));
								tile.transform.position = map.transform.position + tilePositionInLocalSpace;

								// set the tiles parent to the game object for organizational purposes
								tile.transform.parent = map.transform;

								//check wether the tilemap is foreground or background
								if (map.isBGTileMap) {
										tile.GetComponent<Renderer> ().sortingLayerName = "BGTiles";
								}

								// give the tile a name that represents it's location within the tile map
								if (newTile) {
//										if (tile.GetComponent<SpriteRenderer> ()) {
//												tiles.Add (new TileObject (tile.name, tilePos.x, tilePos.y, tile.GetComponent<SpriteRenderer> ().sprite.name));
//										} else {
//												tiles.Add (new TileObject (tile.name, tilePos.x, tilePos.y));
//										}
								}

						}
				}

				public List<Vector3> ResolveRectSize (Vector3 orgin, Vector3 end)
				{

						List<Vector3> tilesInRect = new List<Vector3> ();

						if (orgin.x < end.x && orgin.y < end.y) {

								float rectWidth = end.x - orgin.x;
								float rectHeight = end.y - orgin.y;

								for (int j = 0; j <= rectHeight; j++) {

										for (int i = 0; i <= rectWidth; i++) {
												Vector3 temp = orgin;
												temp.x += i;
												temp.y += j;
												tilesInRect.Add (temp);
										}
								}
						} else if (orgin.x < end.x && orgin.y > end.y) {

								float rectWidth = end.x - orgin.x;
								float rectHeight = orgin.y - end.y;

								Vector3 newOrgin = new Vector3 (orgin.x, end.y);

								for (int j = 0; j <= rectHeight; j++) {

										for (int i = 0; i <= rectWidth; i++) {
												Vector3 temp = newOrgin;
												temp.x += i;
												temp.y += j;
												tilesInRect.Add (temp);
										}
								}
						} else if (orgin.x > end.x && orgin.y > end.y) {

								float rectWidth = orgin.x - end.x;
								float rectHeight = orgin.y - end.y;

								for (int j = 0; j <= rectHeight; j++) {

										for (int i = 0; i <= rectWidth; i++) {
												Vector3 temp = end;
												temp.x += i;
												temp.y += j;
												tilesInRect.Add (temp);
										}
								}
						} else if (orgin.x > end.x && orgin.y < end.y) {

								float rectWidth = orgin.x - end.x;
								float rectHeight = end.y - orgin.y;

								Vector3 newOrgin = new Vector3 (end.x, orgin.y);

								for (int j = 0; j <= rectHeight; j++) {

										for (int i = 0; i <= rectWidth; i++) {
												Vector3 temp = newOrgin;
												temp.x += i;
												temp.y += j;
												tilesInRect.Add (temp);
										}
								}
						}
						return tilesInRect;
				}

				public void SubRenderTiles ()
				{


						if (enabledMode == MapMode.Normal) {
								// get reference to the TileMap component

								Debug.Log ("Started Sub Render");
								List<Vector3> tilesInRect = ResolveRectSize (startPosSelection, endPosSelection);
								bool tileSkip = false;

								for (int i = 0; i < tilesInRect.Count; i++) {


										var map = (TileMap)this.target;
										var newTile = false;

										var tilePos = tilesInRect [i];

										var actualPos = new Vector3 (tilePos.x * map.TileWidth, tilePos.y * map.TileHeight, 0);

										actualPos.x += map.TileWidth / 2;
										actualPos.y += map.TileHeight / 2;


										if (overrideBottom == false) {
												foreach (Transform child in map.transform) {
														if (child.position == actualPos) {
																if (replaceTiles) {
																		DestroyImmediate (child.gameObject);
																} else {
																		tileSkip = true;
																}
														}
												}
										}
										if (tileSkip == false) {
												var tile = PrefabUtility.InstantiatePrefab (map.tile) as GameObject;
												var prefab = PrefabUtility.FindPrefabRoot (tile);
												PrefabUtility.SetPropertyModifications (tile, PrefabUtility.GetPropertyModifications (prefab));
												newTile = true;

												// set the tiles position on the tile map
												var tilePositionInLocalSpace = new Vector3 ((tilePos.x * map.TileWidth) + (map.TileWidth / 2), (tilePos.y * map.TileHeight) + (map.TileHeight / 2));
												tile.transform.position = map.transform.position + tilePositionInLocalSpace;

												// set the tiles parent to the game object for organizational purposes
												tile.transform.parent = map.transform;

												//check wether the tilemap is foreground or background
												if (map.isBGTileMap) {
														tile.GetComponent<Renderer> ().sortingLayerName = "BGTiles";
												}

												// give the tile a name that represents it's location within the tile map
												if (newTile) {
//														if (tile.GetComponent<SpriteRenderer> ()) {
//																tiles.Add (new TileObject (tile.name, tilePos.x, tilePos.y, tile.GetComponent<SpriteRenderer> ().sprite.name));
//														} else {
//																tiles.Add (new TileObject (tile.name, tilePos.x, tilePos.y));
//														}
												}
										}
										tileSkip = false;
								}

						}
				}

				public void SubRemoveTiles ()
				{
						if (enabledMode == MapMode.Normal) {
								// get reference to the TileMap component
								var map = (TileMap)this.target;

								Debug.Log ("Started Sub Render");
								List<Vector3> tilesInRect = ResolveRectSize (startPosSelection, endPosSelection);

								for (int k = 0; k < tilesInRect.Count; k++) {
										// Calculate the position of the mouse over the tile layer
										var tilePos = tilesInRect [k];

										var actualPos = new Vector3 (tilePos.x * map.TileWidth, tilePos.y * map.TileHeight, 0);

										actualPos.x += map.TileWidth / 2;
										actualPos.y += map.TileHeight / 2;

										foreach (Transform tile in map.transform) {

                                            if (Vector2.Distance(tile.position, actualPos) < 1)
                                            {
//														for (int i = 0; i < tiles.Count; i++) {
//																if (tiles [i].Position == new Vector2 (tilePos.x, tilePos.y)) {
//																		tiles.RemoveAt (i);
//																}
//														}
														DestroyImmediate (tile.gameObject);
												}
										}

								}
						}
				}

				/// <summary>
				/// Erases a block at the pre-calculated mouse hit position
				/// </summary>
				private void Erase ()
				{

						if (enabledMode == MapMode.Normal) {
								// get reference to the TileMap component
								var map = (TileMap)this.target;

								// Calculate the position of the mouse over the tile layer
								var tilePos = this.GetTilePositionFromMouseLocation ();

								var actualPos = new Vector3 (tilePos.x * map.TileWidth, tilePos.y * map.TileHeight, 0);

								actualPos.x += map.TileWidth / 2;
								actualPos.y += map.TileHeight / 2;

								foreach (Transform tile in map.transform) {
                                            if (Vector2.Distance(tile.position, actualPos) < 1)
                                            {
//												for (int i = 0; i < tiles.Count; i++) {
//														if (tiles [i].Position == tilePos) {
//																tiles.RemoveAt (i);
//														}
//												}
												DestroyImmediate (tile.gameObject);
										}
								}

						}

				}

				private Vector2 GetTilePositionFromLocation (Vector3 location)
				{
						// get reference to the tile map component
						var map = (TileMap)this.target;

						// calculate column and row location from mouse hit location
						var pos = new Vector3 (location.x / map.TileWidth, location.y / map.TileHeight, map.transform.position.z);

						// round the numbers to the nearest whole number using 5 decimal place precision
						pos = new Vector3 ((int)Math.Round (pos.x, 5, MidpointRounding.ToEven), (int)Math.Round (pos.y, 5, MidpointRounding.ToEven), 0);

						// do a check to ensure that the row and column are with the bounds of the tile map
						var col = (int)pos.x;
						var row = (int)pos.y;
						if (row < 0) {
								row = 0;
						}

						if (row > map.Rows - 1) {
								row = map.Rows - 1;
						}

						if (col < 0) {
								col = 0;
						}

						if (col > map.Columns - 1) {
								col = map.Columns - 1;
						}

						// return the column and row values
						return new Vector2 (col, row);
				}

				/// <summary>
				/// Calculates the location in tile coordinates (Column/Row) of the mouse position
				/// </summary>
				/// <returns>Returns a <see cref="Vector2"/> type representing the Column and Row where the mouse of positioned over.</returns>
				private Vector2 GetTilePositionFromMouseLocation ()
				{
						// get reference to the tile map component
						var map = (TileMap)this.target;

						// calculate column and row location from mouse hit location
						var pos = new Vector3 (this.mouseHitPos.x / map.TileWidth, this.mouseHitPos.y / map.TileHeight, map.transform.position.z);

						// round the numbers to the nearest whole number using 5 decimal place precision
						pos = new Vector3 ((int)Math.Round (pos.x, 5, MidpointRounding.ToEven), (int)Math.Round (pos.y, 5, MidpointRounding.ToEven), 0);

						// do a check to ensure that the row and column are with the bounds of the tile map
						var col = (int)pos.x;
						var row = (int)pos.y;
						if (row < 0) {
								row = 0;
						}

						if (row > map.Rows - 1) {
								row = map.Rows - 1;
						}

						if (col < 0) {
								col = 0;
						}

						if (col > map.Columns - 1) {
								col = map.Columns - 1;
						}

						// return the column and row values
						return new Vector2 (col, row);
				}

				/// <summary>
				/// Returns true or false depending if the mouse is positioned over the tile map.
				/// </summary>
				/// <returns>Will return true if the mouse is positioned over the tile map.</returns>
				private bool IsMouseOnLayer ()
				{
						// get reference to the tile map component
						var map = (TileMap)this.target;

						// return true or false depending if the mouse is positioned over the map
						return this.mouseHitPos.x > 0 && this.mouseHitPos.x < (map.Columns * map.TileWidth) &&
								this.mouseHitPos.y > 0 && this.mouseHitPos.y < (map.Rows * map.TileHeight);
				}

				/// <summary>
				/// Recalculates the position of the marker based on the location of the mouse pointer.
				/// </summary>
				private void RecalculateMarkerPosition ()
				{
						// get reference to the tile map component
						var map = (TileMap)this.target;

						// store the tile location (Column/Row) based on the current location of the mouse pointer
						var tilepos = this.GetTilePositionFromMouseLocation ();

						// store the tile position in world space
						var pos = new Vector3 (tilepos.x * map.TileWidth, tilepos.y * map.TileHeight, 0);

						// set the TileMap.MarkerPosition value
						map.MarkerPosition = map.transform.position + new Vector3 (pos.x + (map.TileWidth / 2), pos.y + (map.TileHeight / 2), 0);
				}

				private void CalculateStartSelection ()
				{
						// get reference to the tile map component
						var map = (TileMap)this.target;

						// store the tile location (Column/Row) based on the current location of the mouse pointer
						var tilepos = this.GetTilePositionFromMouseLocation ();

						// store the tile position in world space
						var pos = new Vector3 (tilepos.x * map.TileWidth, tilepos.y * map.TileHeight, 0);

						// set the TileMap.MarkerPosition value
						map.startSelection = map.transform.position + new Vector3 (pos.x + (map.TileWidth / 2), pos.y + (map.TileHeight / 2), 0);
				}

				private void CalculateEndSelection ()
				{
						// get reference to the tile map component
						var map = (TileMap)this.target;

						// store the tile location (Column/Row) based on the current location of the mouse pointer
						var tilepos = this.GetTilePositionFromMouseLocation ();

						// store the tile position in world space
						var pos = new Vector3 (tilepos.x * map.TileWidth, tilepos.y * map.TileHeight, 0);

						// set the TileMap.MarkerPosition value
						map.endSelection = map.transform.position + new Vector3 (pos.x + (map.TileWidth / 2), pos.y + (map.TileHeight / 2), 0);
				}

				/// <summary>
				/// Calculates the position of the mouse over the tile map in local space coordinates.
				/// </summary>
				/// <returns>Returns true if the mouse is over the tile map.</returns>
				private bool UpdateHitPosition ()
				{
						// get reference to the tile map component
						var map = (TileMap)this.target;

						// build a plane object that
						var p = new Plane (map.transform.TransformDirection (Vector3.forward), map.transform.position);

						// build a ray type from the current mouse position
						var ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);

						// stores the hit location
						var hit = new Vector3 ();

						// stores the distance to the hit location
						float dist;

						// cast a ray to determine what location it intersects with the plane
						if (p.Raycast (ray, out dist)) {
								// the ray hits the plane so we calculate the hit location in world space
								hit = ray.origin + (ray.direction.normalized * dist);
						}

						// convert the hit location from world space to local space
						var value = map.transform.InverseTransformPoint (hit);

						// if the value is different then the current mouse hit location set the
						// new mouse hit location and return true indicating a successful hit test
						if (value != this.mouseHitPos) {
								this.mouseHitPos = value;
								return true;
						}

						// return false if the hit test failed
						return false;
				}

        public void AllocateTextures ()
        {
            textures.Clear ();
            for (int i = 0; i < activeTilesetPrefabs.Count; i++) {
                Texture2D p = CreateTexture ((AssetPreview.GetAssetPreview (activeTilesetPrefabs [i])));
                textures.Add (p);
            }
            EditorUtility.SetDirty (this);
            
        }

        public  Texture2D CreateTexture(Texture2D texture)
        {
            if ( texture == null )
            {
                return texture;
            }
            
            Texture2D rtexture = new Texture2D((int)texture.width, (int)texture.height);
            var pixels = texture.GetPixels(0, (int)0, (int)texture.width, (int)texture.height);
            
            rtexture.SetPixels(pixels);
            rtexture.hideFlags = texture.hideFlags;
            rtexture.filterMode = texture.filterMode;
            rtexture.name = texture.name;
            
            rtexture.Apply();
            return rtexture;
        }

		}

		public class TileObject
		{
				string type;
				Vector2 position;
				string image;

				public string Name { get { return type; } }

				public string Image { get { return image; } set { image = value; } }

				public Vector2 Position { get { return position; } }

				public TileObject (string name, float x, float y, string image)
				{
						this.type = name;
						this.image = image;
						this.position = new Vector2 (x, y);
				}

				public TileObject (string name, float x, float y)
				{
						this.type = name;
						this.position = new Vector2 (x, y);
				}
		}


}