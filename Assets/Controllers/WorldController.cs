//=======================================================================
// Copyright Maxime "jellycat" Blanc 2021.
//=======================================================================

using UnityEngine;
using System.Collections.Generic;

public class WorldController : MonoBehaviour
{
  public static WorldController Instance { get; protected set; }

  // The only tile sprite right now
  public Sprite floorSprite;

  // The world and tile data
  public World World { get; protected set; }

  Dictionary<Tile, GameObject> tileGameObjectMap;

  void Start()
  {
    if (Instance != null)
    {
      Debug.LogError("WorldController - There should never be multiple world controllers.");
    }
    Instance = this;

    // Create a world with Empty tiles
    World = new World();

    // Instantiate dictionary that tracks which GameObject is rendering which Tile data
    tileGameObjectMap = new Dictionary<Tile, GameObject>();

    // Create a GameObject for each tile, so they show visually. (and redunt reduntantly)
    for (int x = 0; x < World.Width; x++)
    {
      for (int y = 0; y < World.Height; y++)
      {
        // Get the tile data
        Tile tile_data = World.GetTileAt(x, y);

        // This creates a new GameObject and adds it to our scene.
        GameObject tile_go = new GameObject();

        // Add Tile/GameObject to the dictionary
        tileGameObjectMap.Add(tile_data, tile_go);

        tile_go.name = "Tile_" + x + "_" + y;
        tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
        tile_go.transform.SetParent(this.transform, true);

        // Add a sprite renderer, but don't bother setting a sprite
        // because all the tiles are empty right now.
        tile_go.AddComponent<SpriteRenderer>();

        // Register the callback so the GameObject is updated when the Tile type is changed
        tile_data.RegisterTileTypeChangedCallback(OnTileTypeChanged);
      }
    }

    // Shake things up, for testing.
    World.RandomizeTiles();
  }

  void Update() { }

  void DestroyAllTileGameObjects()
  {
    // This function might get called when changing floors/levels.
    // Destroy all visual **GameObjects** but not the tile data!

    foreach (KeyValuePair<Tile, GameObject> pair in tileGameObjectMap)
    {
      Tile tile_data = pair.Key;
      GameObject tile_go = pair.Value;

      // Remove the pair from the map
      tileGameObjectMap.Remove(tile_data);
      // Unregister the callback
      tile_data.UnregisterTileTypeChangedCallback(OnTileTypeChanged);
      // Destroy the visual GO
      Destroy(tile_go);
    }
  }

  // This function is called whenever a tile's type gets changed.
  void OnTileTypeChanged(Tile tile_data)
  {
    if (!tileGameObjectMap.ContainsKey(tile_data))
    {
      Debug.LogError("tileGameObjectMap doesn't contain the tile_data -- did you forget to add the tile to the dictionary? Or maybe to unregister a callback?");
      return;
    }

    GameObject tile_go = tileGameObjectMap[tile_data];

    if (tile_go == null)
    {
      Debug.LogError("tileGameObjectMap's returned GameObject is null -- did you forget to add the tile to the dictionary? Or maybe to unregister a callback?");
      return;
    }

    if (tile_data.Type == TileType.Floor)
    {
      tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
    }
    else if (tile_data.Type == TileType.Empty)
    {
      tile_go.GetComponent<SpriteRenderer>().sprite = null;
    }
    else
    {
      Debug.LogError("OnTileTypeChanged - Unrecognized tile type.");
    }
  }

  /// <summary>
  /// Get Tile at given world coordinates.
  /// </summary>
  /// <returns>The <see cref="Tile"/>.</returns>
  /// <param name="coord">The world coordinates to get Tile from</param>
  public Tile GetTileAtWorldCoord(Vector3 coord)
  {
    int x = Mathf.FloorToInt(coord.x);
    int y = Mathf.FloorToInt(coord.y);

    return World.GetTileAt(x, y);
  }

}