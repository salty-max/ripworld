//=======================================================================
// Copyright Maxime "jellycat" Blanc 2021.
//=======================================================================

using UnityEngine;

public class WorldController : MonoBehaviour
{

  // The only tile sprite right now
  public Sprite floorSprite;

  // The world and tile data
  World world;

  void Start()
  {
    // Create a world with Empty tiles
    world = new World();

    // Create a GameObject for each tile, so they show visually. (and redunt reduntantly)
    for (int x = 0; x < world.Width; x++)
    {
      for (int y = 0; y < world.Height; y++)
      {
        // Get the tile data
        Tile tile_data = world.GetTileAt(x, y);

        // This creates a new GameObject and adds it to our scene.
        GameObject tile_go = new GameObject();
        tile_go.name = "Tile_" + x + "_" + y;
        tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);

        // Add a sprite renderer, but don't bother setting a sprite
        // because all the tiles are empty right now.
        tile_go.AddComponent<SpriteRenderer>();

        // Use a lambda to create an anonymous function to wrap the callback function
        tile_data.RegisterTileTypeChangedCallback((tile) => { OnTileTypeChanged(tile, tile_go); });
      }
    }

    // Shake things up, for testing.
    world.RandomizeTiles();
  }

  void Update()
  {

  }

  // This function is called whenever a tile's type gets changed.
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