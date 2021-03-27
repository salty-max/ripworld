//=======================================================================
// Copyright Maxime "jellycat" Blanc 2021.
//=======================================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
  public GameObject boxCursorPrefab;
  bool buildModeIsObjects = false;
  TileType buildModeTile = TileType.Floor;
  string buildModeObjectType;
  Vector3 lastFramePosition;
  Vector3 dragStartPosition;
  Vector3 currFramePosition;
  List<GameObject> dragPreviewGameObjects;

  void Start()
  {
    dragPreviewGameObjects = new List<GameObject>();
  }

  void Update()
  {
    currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    currFramePosition.z = 0;

    // UpdateCursor();
    UpdateDrag();
    UpdateCameraMovement();

    // Save the mouse position from this frame
    lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    lastFramePosition.z = 0;
  }

  // void UpdateCursor()
  // {
  //   // Update the box cursor position
  // Tile tileHovered = WorldController.Instance.GetTileAtWorldCoord(currFramePosition);
  //   if (tileHovered != null)
  //   {
  //     boxCursor.SetActive(true);
  //     Vector3 cursorPosition = new Vector3(tileHovered.X, tileHovered.Y, 0);
  //     boxCursor.transform.position = cursorPosition;
  //   }
  //   else
  //   {
  //     boxCursor.SetActive(false);
  //   }
  // }

  void UpdateDrag()
  {
    // If over UI element, return
    if (EventSystem.current.IsPointerOverGameObject())
    {
      return;
    }

    // Start drag
    if (Input.GetMouseButtonDown(0))
    {
      dragStartPosition = currFramePosition;
    }

    int start_x = Mathf.FloorToInt(dragStartPosition.x);
    int end_x = Mathf.FloorToInt(currFramePosition.x);

    if (end_x < start_x)
    {
      int tmp = end_x;
      end_x = start_x;
      start_x = tmp;
    }

    int start_y = Mathf.FloorToInt(dragStartPosition.y);
    int end_y = Mathf.FloorToInt(currFramePosition.y);

    if (end_y < start_y)
    {
      int tmp = end_y;
      end_y = start_y;
      start_y = tmp;
    }

    // Clean up old drag previews
    foreach (GameObject go in dragPreviewGameObjects)
    {
      dragPreviewGameObjects.Remove(go);
      SimplePool.Despawn(go);
    }

    if (Input.GetMouseButton(0))
    {
      // Display a preview of the drag area
      for (int x = start_x; x <= end_x; x++)
      {
        for (int y = start_y; y <= end_y; y++)
        {
          Tile t = WorldController.Instance.World.GetTileAt(x, y);
          if (t != null)
          {
            // Display the building hint on top of this tile position
            GameObject go = SimplePool.Spawn(boxCursorPrefab, new Vector3(x, y, 0), Quaternion.identity);
            dragPreviewGameObjects.Add(go);
          }
        }
      }
    }

    // End drag
    if (Input.GetMouseButtonUp(0))
    {
      for (int x = start_x; x <= end_x; x++)
      {
        for (int y = start_y; y <= end_y; y++)
        {
          Tile tile = WorldController.Instance.World.GetTileAt(x, y);
          if (tile != null)
          {
            if (buildModeIsObjects)
            {
              // Create the InstalledObject and assign it to the Tile.
              WorldController.Instance.World.PlaceInstalledObject(buildModeObjectType, tile);
            }
            else
            {
              // Tile-changing mode.
              tile.Type = buildModeTile;
            }
          }
        }
      }
    }
  }

  void UpdateCameraMovement()
  {
    // Camera drag
    if (Input.GetMouseButton(1) || Input.GetMouseButton(2)) // Right or middle mouse button
    {
      Vector3 diff = lastFramePosition - currFramePosition;
      Camera.main.transform.Translate(diff);
    }

    // Camera zoom
    Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");
    Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 3f, 25f);
  }

  /// <summary>
  /// Set left click mode to build walls.
  /// </summary>
  public void SetMode_BuildInstalledObject(string objectType)
  {
    // Wall is not a Tile but an InstalledObject.
    buildModeIsObjects = true;
    buildModeObjectType = objectType;
  }

  /// <summary>
  /// Set left click mode to build floors.
  /// </summary>
  public void SetMode_BuildFloor()
  {
    buildModeIsObjects = false;
    buildModeTile = TileType.Floor;
  }

  /// <summary>
  /// Set left click mode to bulldoze.
  /// </summary>
  public void SetMode_Bulldoze()
  {
    buildModeIsObjects = false;
    buildModeTile = TileType.Empty;
  }
}
