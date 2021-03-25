using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
  public Sprite floorSprite;
  World world;
  void Start()
  {
    // Create world with empty tiles
    world = new World();

    // Create a GO for each tile, for visual purpose
    for (int x = 0; x < world.Width; x++)
    {
      for (int y = 0; y < world.Height; y++)
      {
        Tile tile_data = world.GetTileAt(x, y);
        GameObject tile_go = new GameObject();
        tile_go.name = $"Tile_{x}_{y}";
        tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
        tile_go.AddComponent<SpriteRenderer>();
        tile_data.RegisterTileTypeChangedCallback((tile) => { OnTileTypeChanged(tile, tile_go); });
      }
    }

    world.RandomizeTiles();
  }


  void Update()
  { }

  void OnTileTypeChanged(Tile tile_data, GameObject tile_go)
  {
    if (tile_data.Type == Tile.TileType.Floor)
    {
      tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
    }
    else if (tile_data.Type == Tile.TileType.Empty)
    {
      tile_go.GetComponent<SpriteRenderer>().sprite = null;
    }
    else
    {
      Debug.LogError("OnTileTypeChanged - Unrecognized tile type.");
    }
  }
}
