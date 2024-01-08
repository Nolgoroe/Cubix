using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static int gameSpeed = 1;
    public int gameSpeedTemp = 1;

    private void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        gameSpeed = gameSpeedTemp;
    }
}
