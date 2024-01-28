using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieDataSpawner : MonoBehaviour
{
    public DieData CreateNewDieData(DiceSO diceSO)
    {
        DieData data = new DieData();

        data.dieType = diceSO.dieType;
        data.element = diceSO.element;
        data.material = diceSO.dieMaterial;

        List<DieFaceValue> tmpFaceValues = new List<DieFaceValue>();

        for (int i = 0; i < diceSO.resouceDataList.Count; i++)
        {
            // for now we do the resources as random.
            ResourceData resourceData = SetDiceResourcesRandom();

            DieFaceValue faceValue = new DieFaceValue(resourceData, diceSO.buffDataList[i]);
            tmpFaceValues.Add(faceValue);
        }

        data.facesValues = tmpFaceValues;
        data.towerPrefabConnected = diceSO.towerPrefab;
        data.diePrefab = diceSO.diePrefab;
        data.dieIcon = diceSO.icon;

        Player.Instance.AddDieData(data);

        return data;
    }

    private ResourceData SetDiceResourcesRandom()
    {
        ResourceData resourceData = new ResourceData();

        int randomResource = Random.Range(0, System.Enum.GetValues(typeof(ResourceType)).Length);
        resourceData.Type = (ResourceType)randomResource;
        resourceData.Value = Random.Range(1, 10); //temp
        resourceData.Icon = Helpers.ReturnIconByType(resourceData.Type);

        return resourceData;
    }

}
