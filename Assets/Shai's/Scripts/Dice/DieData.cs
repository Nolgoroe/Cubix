using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]//serializing just to view data in scene
public class DieData 
{
    public DieType DieType;
    public DieElement element;
    public Material material;
    public List<DieFaceValue> facesValues;
    public TowerBaseParent towerPrefabConnected;
}
