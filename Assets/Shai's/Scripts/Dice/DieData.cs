using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]//serializing just to view data in scene
public class DieData 
{
    public DieType dieType;
    public DieElement element;
    public CellTypeColor colorType;
    public Material material;
    public List<DieFaceValue> facesValues;
    public TowerBaseParent towerPrefabConnected;
    public GameObject diePrefab;
    public Sprite dieIcon;
}
