using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buildings SO", menuName = "ScriptableObjects/Buildings")]
public class BuildingsSO : ScriptableObject
{
    public enum Dir
    {
        Down, Up, Left, Right
    }

    public string buildingName;
    public GameObject buildingPrefab;
    public GameObject visualPrefab;
    public int width;
    public int height;

    public int GetRotationAngle(Dir dir)
    {
        switch (dir)
        {
            case Dir.Down:  return 0;
            case Dir.Up:    return 180;
            case Dir.Left:  return 90;
            case Dir.Right: return 270;
            default:    return -1;
        }
    }

    public Vector2Int GetRotationOffset(Dir dir)
    {
        switch (dir)
        {
            case Dir.Down: return new Vector2Int(0, 0);
            case Dir.Up: return new Vector2Int(width, height);
            case Dir.Left: return new Vector2Int(0, width);
            case Dir.Right: return new Vector2Int(height, 0);
            default:    return new Vector2Int(0, 0);
        }
    }

    //this is static since this is a helper function that we want to be able to call without needed a refernece to the class directly.
    public static Dir GetNextDir(Dir dir) 
    {
        switch (dir)
        {
            case Dir.Down:  return Dir.Left;
            case Dir.Up:    return Dir.Right;
            case Dir.Left:  return Dir.Up;
            case Dir.Right: return Dir.Down;
        }

        return Dir.Down;
    }

    public List<Vector2Int> GetGridPositionList( Vector2Int offset, Dir dir)
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();

        switch (dir)
        {
            case Dir.Down:
            case Dir.Up:
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
            case Dir.Left:
            case Dir.Right:
                for (int x = 0; x < height; x++)
                {
                    for (int y = 0; y < width; y++)
                    {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
            default:
                break;
        }

        return gridPositionList;
    }
}
