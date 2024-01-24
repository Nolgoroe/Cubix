using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DieData : MonoBehaviour
{
    public DieType DieType;
    public DieElement element;
    public Material material;
    public List<DieFaceValue> facesValue;
    public TowerBaseParent towerPrefabConnected;

    public int currentFaceindex;

}
