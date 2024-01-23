using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PathSides
{
    up, 
    left,  
    down,  
    right,  
    leftUp, 
    rightUp,  
    leftDown,  
    rightDown,  
    leftRightUp,  
    leftUpDown,   
    rightUpDown,   
    leftRightDown,  
    downLeftUp,
    downRightUp,
    leftRightUpDown,
    None
}
public class ToolEnemyPathCell : ToolGridCell
{

    private PathSides GetConnectedPathSides()
    {
        PathSides pathSides = PathSides.None;

        bool connectUp = false, connectLeft = false, connectDown = false, connectRight = false;

        int currentX = positionXYInGridArray.x;
        int currentY = positionXYInGridArray.y;

        ToolGridCell[,] gameGridCellsArray = ToolReferencerObject.Instance.toolGameGrid.ReturnCellsArray();
        ToolGameGrid gameGrid = ToolReferencerObject.Instance.toolGameGrid;

        #region Check Neighbor Cells
        //check up
        if (currentY + 1 < gameGrid.ReturnGridWidthAndHeight().y)
        {
            if (gameGridCellsArray[currentX, currentY + 1].ReturnTypeOfCell() == TypeOfCell.enemyPath)
            {
                connectUp = true;
            }
        }

        //check left
        if (currentX - 1 > -1)
        {
            if (gameGridCellsArray[currentX - 1, currentY].ReturnTypeOfCell() == TypeOfCell.enemyPath)
            {
                connectLeft = true;
            }
        }

        //check down
        if (currentY - 1 > -1)
        {
            if (gameGridCellsArray[currentX, currentY - 1].ReturnTypeOfCell() == TypeOfCell.enemyPath)
            {
                connectDown = true;
            }
        }

        //check right
        if (currentX + 1 < gameGrid.ReturnGridWidthAndHeight().x)
        {
            if (gameGridCellsArray[currentX + 1, currentY].ReturnTypeOfCell() == TypeOfCell.enemyPath)
            {
                connectRight = true;
            }
        }
        #endregion

        #region Decide on path sides by all options
        if (connectUp & connectRight && connectLeft && connectDown)
        {
            pathSides = PathSides.leftRightUpDown;
        }
        else if(connectUp & connectLeft && connectDown)
        {
            pathSides = PathSides.leftUpDown;
        }
        else if(connectUp & connectRight && connectDown)
        {
            pathSides = PathSides.rightUpDown;
        }
        else if(connectUp & connectLeft && connectRight)
        {
            pathSides = PathSides.leftRightUp;
        }
        else if(connectDown & connectLeft && connectRight)
        {
            pathSides = PathSides.leftRightDown;
        }
        else if(connectDown & connectLeft && connectUp)
        {
            pathSides = PathSides.downLeftUp;
        }
        else if(connectDown & connectRight && connectUp)
        {
            pathSides = PathSides.downRightUp;
        }
        else if(connectDown & connectLeft)
        {
            pathSides = PathSides.leftDown;
        }
        else if(connectDown & connectRight)
        {
            pathSides = PathSides.rightDown;
        }
        else if(connectUp & connectLeft)
        {
            pathSides = PathSides.leftUp;
        }
        else if(connectUp & connectRight)
        {
            pathSides = PathSides.rightUp;
        }
        else if(connectUp)
        {
            pathSides = PathSides.up;
        }
        else if(connectLeft)
        {
            pathSides = PathSides.left;
        }
        else if(connectDown)
        {
            pathSides = PathSides.down;
        }
        else if(connectRight)
        {
            pathSides = PathSides.right;
        }

        #endregion

        pathMeshSides = pathSides;
        return pathSides;
    }






    public void DecideOnPathMesh()
    {
        //This function will change to change the mesh of this object instead of spawining a new one and passing on data.

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null) return;

        pathMeshSides = GetConnectedPathSides();

        Mesh newMesh = ToolReferencerObject.Instance.levelCreationToolSO.ReturnPrefabByPathSides(pathMeshSides); // this will go away when we do mesh.
        meshFilter.mesh = newMesh;

        Quaternion RotationByDir = Quaternion.Euler(0, 0, GetRotationAngle());
        transform.localRotation = RotationByDir;
    }
}
