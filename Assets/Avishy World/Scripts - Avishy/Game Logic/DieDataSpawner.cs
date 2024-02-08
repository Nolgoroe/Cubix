using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieDataSpawner : MonoBehaviour
{
    public DieData CreateNewDieData(DiceSO _diceSO)
    {
        DieData data = new DieData();

        data.dieType = _diceSO.dieType;
        data.element = _diceSO.element;
        data.colorType = _diceSO.colorType;
        data.material = _diceSO.dieMaterial;

        List<DieFaceValue> tmpFaceValues = new List<DieFaceValue>();

        for (int i = 0; i < _diceSO.resouceDataList.Count; i++)
        {
            // for now we do the resources as random.
            ResourceData resourceData = SetDiceResourcesRandom();

            DieFaceValue faceValue = new DieFaceValue(resourceData, _diceSO.buffDataList[i]);
            tmpFaceValues.Add(faceValue);
        }

        data.facesValues = tmpFaceValues;
        data.towerPrefabConnected = _diceSO.towerPrefab;
        data.diePrefab = _diceSO.diePrefab;
        data.dieIcon = _diceSO.icon;

        Player.Instance.AddDieData(data);

        return data;
    }

    private ResourceData SetDiceResourcesRandom() //temp?
    {
        ResourceData resourceData = new ResourceData();

        int randomResource = Random.Range(0, System.Enum.GetValues(typeof(ResourceType)).Length - 1);
        resourceData.Type = (ResourceType)randomResource;
        resourceData.Value = Random.Range(1, 10); //temp
        resourceData.Icon = Helpers.ReturnIconByType(resourceData.Type);

        return resourceData;
    }

}
