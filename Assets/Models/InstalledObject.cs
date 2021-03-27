//=======================================================================
// Copyright Maxime "jellycat" Blanc 2021.
//=======================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Installed objects are things like walls, doors, and furniture.

public class InstalledObject
{
  // This represents the BASE tile of the object -- but in practice, 
  // large objects may actually occupy multiple tiles.
  Tile tile;
  // This object type will be queried by the visual system to know what sprite to render for this object.
  string objectType;
  // This is a multiplier. so a value of 2 means you move twice as slowly (i.e. at half speed).
  // Tile types and other environmental effects may be combined.
  // E.g. a "rough" tile (cost of 2) with a table (cost of 3) that is on fire (cost of 3)
  // would have a total movement cost of (2+3+3 = 8), so movement through this tile would be at 1/8th normal speed.
  // SPECIAL: If movementCost = 0, then this tile is impassable. (e.g. a wall).
  float movementCost;
  // For example, a sofa might be 3x2 (actual graphics only appear to cover the 3x1 area, but the extra space is for leg room).
  int width;
  int height;

  // TODO: Implement larger objects
  // TODO: Implement object rotation

  protected InstalledObject() { }

  static public InstalledObject CreatePrototype(string objectType, float movementCost = 1f, int width = 1, int height = 1)
  {
    InstalledObject obj = new InstalledObject();
    obj.objectType = objectType;
    obj.movementCost = movementCost;
    obj.width = width;
    obj.height = height;

    return obj;
  }

  static public InstalledObject PlaceInstance(InstalledObject proto, Tile tile)
  {
    InstalledObject obj = new InstalledObject();
    obj.objectType = proto.objectType;
    obj.movementCost = proto.movementCost;
    obj.width = proto.width;
    obj.height = proto.height;
    obj.tile = tile;

    // FIXME: This assumes the object is 1x1.
    if (!tile.PlaceObject(obj))
    {
      // For some reason, cannot place the object in the tile.
      // (Probably already occupied)
      // Do NOT return the newly instatiated object. (It will be garbage collected)
      return null;
    }

    return obj;
  }
}
