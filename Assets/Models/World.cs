//=======================================================================
// Copyright Maxime "jellycat" Blanc 2021.
//=======================================================================

using UnityEngine;
using System;
using System.Collections.Generic;

public class World
{

  // A two-dimensional array to hold our tile data.
  Tile[,] tiles;
  Dictionary<string, Furniture> furnituresPrototypes;

  // The tile width of the world.
  public int Width { get; protected set; }

  // The tile height of the world
  public int Height { get; protected set; }

  Action<Furniture> cbFurnitureCreated;

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

    CreateFurniturePrototypes();
  }

  void CreateFurniturePrototypes()
  {
    furnituresPrototypes = new Dictionary<string, Furniture>();
    furnituresPrototypes.Add("Wall", Furniture.CreatePrototype("Wall", 0, 1, 1, true));
  }

  /// <summary>
  /// Creates an Furniture instance based on a prototype
  /// </summary>
  /// <param name="objectType">The Furniture type</param>
  /// <param name="tile">The Tile to placed the Furniture on</param>
  public void PlaceFurniture(string objectType, Tile tile)
  {
    // Debug.Log("PlaceFurniture");
    // TODO: This function assumes 1x1 tiles -- Fix later
    if (!furnituresPrototypes.ContainsKey(objectType))
    {
      Debug.LogError($"furniturePrototypes doesn't contain a prototype for key {objectType}");
      return;
    }

    Furniture obj = Furniture.PlaceInstance(furnituresPrototypes[objectType], tile);

    if (obj == null)
    {
      // Failed to place object -- most likely something was already there.
      return;
    }

    if (cbFurnitureCreated != null)
    {
      cbFurnitureCreated(obj);
    }
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

        if (UnityEngine.Random.Range(0, 2) == 0)
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

  /// <summary>
  /// Register a function to be called back when an installe object is created.
  /// </summary>
  public void RegisterFurnitureCreated(Action<Furniture> callback)
  {
    cbFurnitureCreated += callback;
  }

  /// <summary>
  /// Unregister a callback.
  /// </summary>
  public void UnregisterFurnitureCreated(Action<Furniture> callback)
  {
    cbFurnitureCreated -= callback;
  }

}
