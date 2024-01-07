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
    upLeft,  
    upRight,  
    downLeft,  
    downRight,  
    upLeftRight,  
    upLeftDown,   
    upRightDown,   
    downLeftRight,  
    downLeftUp,
    downRightUp,
    upRightLeftDown,
    None
}
public class ToolEnemyPathCell : ToolGridCell
{
    [SerializeField] PathSides pathMeshSides;

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
        if (currentX - 1 > 0)
        {
            if (gameGridCellsArray[currentX - 1, currentY].ReturnTypeOfCell() == TypeOfCell.enemyPath)
            {
                connectLeft = true;
            }
        }

        //check down
        if (currentY - 1 > 0)
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
            pathSides = PathSides.upRightLeftDown;
        }
        else if(connectUp & connectLeft && connectDown)
        {
            pathSides = PathSides.upLeftDown;
        }
        else if(connectUp & connectRight && connectDown)
        {
            pathSides = PathSides.upRightDown;
        }
        else if(connectUp & connectLeft && connectRight)
        {
            pathSides = PathSides.upLeftRight;
        }
        else if(connectDown & connectLeft && connectRight)
        {
            pathSides = PathSides.downLeftRight;
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
            pathSides = PathSides.downLeft;
        }
        else if(connectDown & connectRight)
        {
            pathSides = PathSides.downRight;
        }
        else if(connectUp & connectLeft)
        {
            pathSides = PathSides.upLeft;
        }
        else if(connectUp & connectRight)
        {
            pathSides = PathSides.upRight;
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

        return pathSides;
    }

    public void DecideOnPathMesh()
    {
        //This function will change to change the mesh of this object instead of spawining a new one and passing on data.


        pathMeshSides = GetConnectedPathSides();

        GameObject toSpawn = ToolReferencerObject.Instance.levelCreationToolSO.ReturnPrefabByPathSides(pathMeshSides); // this will go away when we do mesh.

        if (toSpawn) // this will go away when we do mesh.
        {
            GameObject spawned = Instantiate(toSpawn, transform.position, Quaternion.identity, transform.parent); // this will go away when we do mesh.

            ToolEnemyPathCell enemyPathCell; // this will go away when we do mesh.
            spawned.TryGetComponent<ToolEnemyPathCell>(out enemyPathCell); // this will go away when we do mesh.

            if (enemyPathCell)
            {
                enemyPathCell.CopyOtherGridCell(this); // this will go away when we do mesh.
            }

        }

        gameObject.SetActive(false); //in the future we won't need this since we'll just swap the mesh
    }
}
