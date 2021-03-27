//=======================================================================
// Copyright Maxime "jellycat" Blanc 2021.
//=======================================================================

using System;
using UnityEngine;

// TileType is the base type of the tile.
public enum TileType { Empty, Floor };

public class Tile
{

  private TileType _type = TileType.Empty;
  public TileType Type
  {
    get { return _type; }
    set
    {
      TileType oldType = _type;
      _type = value;

      // Call the callback.
      if (cbTileTypeChanged != null && oldType != _type)
        cbTileTypeChanged(this);
    }
  }

  // LooseObject is something like a drill or a stack of metal sitting on the floor
  LooseObject looseObject;

  // InstalledObject is something like a wall, door, or sofa.
  public InstalledObject InstalledObject { get; protected set; }

  // The world containing the tile
  World world;
  public int X { get; protected set; }
  public int Y { get; protected set; }

  // The function we callback any time our type changes
  Action<Tile> cbTileTypeChanged;

  /// <summary>
  /// Initializes a new instance of the <see cref="Tile"/> class.
  /// </summary>
  /// <param name="world">A World instance.</param>
  /// <param name="x">The x coordinate.</param>
  /// <param name="y">The y coordinate.</param>
  public Tile(World world, int x, int y)
  {
    this.world = world;
    this.X = x;
    this.Y = y;
  }

  /// <summary>
  /// Register a function to be called back when tile type changes.
  /// </summary>
  public void RegisterTileTypeChangedCallback(Action<Tile> callback)
  {
    cbTileTypeChanged += callback;
  }

  /// <summary>
  /// Unregister a callback.
  /// </summary>
  public void UnregisterTileTypeChangedCallback(Action<Tile> callback)
  {
    cbTileTypeChanged -= callback;
  }

  public bool PlaceObject(InstalledObject objInstance)
  {
    if (objInstance == null)
    {
      // Uninstalling whatever was here before.
      InstalledObject = null;
      return true;
    }

    if (InstalledObject != null)
    {
      Debug.LogError("Trying to assign a installed object to a tile that already has one.");
      return false;
    }

    InstalledObject = objInstance;
    return true;
  }
}
