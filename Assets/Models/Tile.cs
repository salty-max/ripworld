//=======================================================================
// Copyright Maxime "jellycat" Blanc 2021.
//=======================================================================

using System;

public class Tile
{

  // TileType is the base type of the tile.
  public enum TileType { Empty, Floor };

  private TileType _type = TileType.Empty;
  public TileType Type
  {
    get { return _type; }
    set
    {
      TileType oldType = _type;
      _type = value;

      // Call the callback.
      if (tileTypeChanged != null && oldType != _type)
        tileTypeChanged(this);
    }
  }

  // LooseObject is something like a drill or a stack of metal sitting on the floor
  LooseObject looseObject;

  // InstalledObject is something like a wall, door, or sofa.
  InstalledObject installedObject;

  // The world containing the tile
  World world;
  public int X { get; protected set; }
  public int Y { get; protected set; }

  // The function we callback any time our type changes
  Action<Tile> tileTypeChanged;

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
    tileTypeChanged += callback;
  }

  /// <summary>
  /// Unregister a callback.
  /// </summary>
  public void UnegisterTileTypeChangedCallback(Action<Tile> callback)
  {
    tileTypeChanged -= callback;
  }

}
