using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolPlayerBase : ToolGridCell
{
    void Start()
    {
        ToolGridCell[,] gameGridCellsArray = ToolReferencerObject.Instance.toolGameGrid.ReturnCellsArray();
        ToolGameGrid gameGrid = ToolReferencerObject.Instance.toolGameGrid;

        int currentX = positionXYInGridArray.x;
        int currentY = positionXYInGridArray.y;

        //check up
        if (currentY + 1 < gameGrid.ReturnGridWidthAndHeight().y)
        {
            if (gameGridCellsArray[currentX, currentY + 1].ReturnTypeOfCell() == TypeOfCell.enemyPath)
            {
                transform.LookAt(gameGridCellsArray[currentX, currentY + 1].transform);
                return;
            }
        }

        //check left
        if (currentX - 1 > -1)
        {
            if (gameGridCellsArray[currentX - 1, currentY].ReturnTypeOfCell() == TypeOfCell.enemyPath)
            {
                transform.LookAt(gameGridCellsArray[currentX - 1, currentY].transform);

                return;
            }
        }

        //check down
        if (currentY - 1 > -1)
        {
            if (gameGridCellsArray[currentX, currentY - 1].ReturnTypeOfCell() == TypeOfCell.enemyPath)
            {
                transform.LookAt(gameGridCellsArray[currentX, currentY - 1].transform);

                return;
            }
        }

        //check right
        if (currentX + 1 < gameGrid.ReturnGridWidthAndHeight().x)
        {
            if (gameGridCellsArray[currentX + 1, currentY].ReturnTypeOfCell() == TypeOfCell.enemyPath)
            {
                transform.LookAt(gameGridCellsArray[currentX + 1, currentY].transform);

                return;
            }
        }
    }
}
