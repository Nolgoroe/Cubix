using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dice Creation", menuName = "ScriptableObjects/Dice")]
public class DiceSO : ScriptableObject
{
    public TowerBaseParent towerPrefab;
    public Material dieMaterial;
}
