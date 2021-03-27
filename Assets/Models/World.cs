//=======================================================================
// Copyright Maxime "jellycat" Blanc 2021.
//=======================================================================

using UnityEngine;
using System.Collections.Generic;

public class World
{

  // A two-dimensional array to hold our tile data.
  Tile[,] tiles;
  Dictionary<string, InstalledObject> installedObjectsPrototypes;

  // The tile width of the world.
  public int Width { get; protected set; }

  // The tile height of the world
  public int Height { get; protected set; }

  /// <summary>
  /// Initializes a new instance of the <see cref="World"/> class.
  /// </summary>
  /// <param name="width">Width in tiles.</param>
  /// <param name="height">Height in tiles.</param>
  public World(int width = 100, int height = 100)
  {
    Width = width;
    Height = height;

    tiles = new Tile[Width, Height];

    for (int x = 0; x < Width; x++)
    {
      for (int y = 0; y < Height; y++)
      {
        tiles[x, y] = new Tile(this, x, y);
      }
    }

    Debug.Log("World created with " + (Width * Height) + " tiles.");

    CreateInstalledObjectPrototypes();
  }

  void CreateInstalledObjectPrototypes()
  {
    installedObjectsPrototypes = new Dictionary<string, InstalledObject>();
    installedObjectsPrototypes.Add("Wall", InstalledObject.CreatePrototype("Wall", 0, 1, 1));
  }

  public void PlaceInstalledObject(string objectType, Tile tile)
  {

  }

  /// <summary>
  /// A function for testing out the system
  /// </summary>
  public void RandomizeTiles()
  {
    Debug.Log("RandomizeTiles");
    for (int x = 0; x < Width; x++)
    {
      for (int y = 0; y < Height; y++)
      {

        if (Random.Range(0, 2) == 0)
        {
          tiles[x, y].Type = TileType.Empty;
        }
        else
        {
          tiles[x, y].Type = TileType.Floor;
        }

      }
    }
  }

  /// <summary>
  /// Gets the tile data at x and y.
  /// </summary>
  /// <returns>The <see cref="Tile"/>.</returns>
  /// <param name="x">The x coordinate.</param>
  /// <param name="y">The y coordinate.</param>
  public Tile GetTileAt(int x, int y)
  {
    if (x > Width || x < 0 || y > Height || y < 0)
    {
      Debug.LogError("Tile (" + x + "," + y + ") is out of range.");
      return null;
    }
    return tiles[x, y];
  }

}
