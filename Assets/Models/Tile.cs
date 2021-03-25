using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
  public enum TileType { Empty, Floor };

  TileType type = TileType.Empty;
  Action<Tile> tileTypeChanged;
  LooseObject looseObject;
  InstalledObject installedObject;
  World world;
  int x;
  int y;

  public Tile(World world, int x, int y)
  {
    this.world = world;
    this.x = x;
    this.y = y;
  }

  public TileType Type
  {
    get
    {
      return type;
    }
    set
    {
      TileType oldType = type;
      type = value;
      // Callback to update tile GO
      if (tileTypeChanged != null && oldType != type) tileTypeChanged(this);
    }
  }

  public int X
  {
    get
    {
      return x;
    }
  }

  public int Y
  {
    get
    {
      return y;
    }
  }

  public void RegisterTileTypeChangedCallback(Action<Tile> callback)
  {
    tileTypeChanged += callback;
  }
  public void UnregisterTileTypeChangedCallback(Action<Tile> callback)
  {
    tileTypeChanged -= callback;
  }
}
