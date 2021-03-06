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
  Dictionary<Furniture, GameObject> furnitureGameObjectMap;
  Dictionary<string, Sprite> furnitureSprites;

  void Start()
  {
    furnitureSprites = new Dictionary<string, Sprite>();
    Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Furnitures");

    foreach (Sprite s in sprites)
    {
      furnitureSprites[s.name] = s;
    }

    if (Instance != null)
    {
      Debug.LogError("WorldController - There should never be multiple world controllers.");
    }
    Instance = this;

    // Create a world with Empty tiles
    World = new World();

    World.RegisterFurnitureCreated(OnFurnitureCreated);

    // Instantiate dictionary that tracks which GameObject is rendering which Tile data
    tileGameObjectMap = new Dictionary<Tile, GameObject>();
    furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();

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
      tile_go.GetComponent<SpriteRenderer>().sortingLayerName = "Tiles";
    }
    else if (tile_data.Type == TileType.Empty)
    {
      tile_go.GetComponent<SpriteRenderer>().sprite = null;
      tile_go.GetComponent<SpriteRenderer>().sortingLayerName = "Tiles";
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

  public void OnFurnitureCreated(Furniture obj)
  {
    // Debug.Log("OnFurnitureCreated");
    // Create a visual GameObject linked to this data
    // FIXME: Dost not consider multi-tile objects nor rotated objects

    // This creates a new GameObject and adds it to our scene.
    GameObject obj_go = new GameObject();

    // Add Tile/GameObject to the dictionary
    furnitureGameObjectMap.Add(obj, obj_go);

    obj_go.name = $"{obj.ObjectType}_{obj.Tile.X}_{obj.Tile.Y}";
    obj_go.transform.position = new Vector3(obj.Tile.X, obj.Tile.Y, 0);
    obj_go.transform.SetParent(this.transform, true);

    // FIXME: Assume that the object must be a wall so
    // use the hardcoded reference to the wall sprite.
    obj_go.AddComponent<SpriteRenderer>().sprite = GetSpriteForFurniture(obj);
    obj_go.GetComponent<SpriteRenderer>().sortingLayerName = "Furnitures";

    // Register the callback so the GameObject is updated when object's info changes
    obj.RegisterOnChangedCallback(OnFurnitureChanged);
  }

  Sprite GetSpriteForFurniture(Furniture obj)
  {
    if (!obj.LinksToNeighbour)
    {
      return furnitureSprites[obj.ObjectType];
    }

    // Otherwise, the sprite name is more complicated

    string spriteName = $"{obj.ObjectType}_";

    // Check for neighbours North, East, South, West
    int x = obj.Tile.X;
    int y = obj.Tile.Y;
    Tile tile;

    tile = World.GetTileAt(x, y + 1);
    if (tile != null && tile.Furniture != null && tile.Furniture.ObjectType == obj.ObjectType)
    {
      spriteName += "N";
    }
    tile = World.GetTileAt(x + 1, y);
    if (tile != null && tile.Furniture != null && tile.Furniture.ObjectType == obj.ObjectType)
    {
      spriteName += "E";
    }
    tile = World.GetTileAt(x, y - 1);
    if (tile != null && tile.Furniture != null && tile.Furniture.ObjectType == obj.ObjectType)
    {
      spriteName += "S";
    }
    tile = World.GetTileAt(x - 1, y);
    if (tile != null && tile.Furniture != null && tile.Furniture.ObjectType == obj.ObjectType)
    {
      spriteName += "W";
    }

    // For example, if this object has all four neighbours of the same type,
    // the string will look like : Wall_NESW
    if (!furnitureSprites.ContainsKey(spriteName))
    {
      Debug.LogError($"GetSpriteForFurniture -- No sprites with name {spriteName}");
      return null;
    }
    return furnitureSprites[spriteName];
  }

  void OnFurnitureChanged(Furniture obj)
  {
    Debug.LogError("OnFurnitureChanged -- Not implemented");
  }

}