using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultDiceDisplay : MonoBehaviour
{
    [SerializeField] private List<DieFace> localFaces;
    [SerializeField] private DieFace _currentTopFace;
    [SerializeField] private Vector3 offsetForRay;

    private void Update()
    {
        Vector3 diretion = Camera.main.transform.position - transform.position;

        Debug.DrawLine(transform.position + offsetForRay, diretion);

        transform.LookAt(Camera.main.transform);
    }

    private void OnEnable()
    {
        GetCameraFacingValue();
    }





    public void InitDiceDisplay(Die towerDie)
    {
        localFaces = new List<DieFace>();
        localFaces.AddRange(GetComponentsInChildren<DieFace>());

        DieFace[] towerDieFaces = towerDie.GetAllFaces();

        for (int i = 0; i < towerDieFaces.Length; i++)
        {
            DieFaceValue faceValue = towerDieFaces[i].GetFaceValue();
            Material mat = towerDieFaces[i].GetComponent<MeshRenderer>().material;
            localFaces[i].ChangeFaceMat(mat);

            BuffData buffData = new BuffData();
            buffData.Type = faceValue.Buff.Type;
            buffData.Value = faceValue.Buff.Value;
            buffData.Icon = faceValue.Buff.Icon;
            localFaces[i].SetBuff(buffData);

        }

        foreach (DieFace face in localFaces)
        {
            face.DisplayBuff();
        }

        transform.LookAt(Camera.main.transform);

        GetCameraFacingValue();
    }

    [ContextMenu("Now")]
    public void GetCameraFacingValue()
    {
        Vector3 diretion = Camera.main.transform.position - transform.position;

        RaycastHit hit;
        if (Physics.Raycast(transform.position + offsetForRay, diretion, out hit))
        {
            if (hit.transform.GetComponent<DieFace>())
            {
                _currentTopFace = hit.transform.GetComponent<DieFace>();
            }

        }
    }

    public void SetFaceData(BuffData buff)
    {
        if (_currentTopFace)
        {
            _currentTopFace.SetBuff(buff);

            _currentTopFace.DisplayBuff();
        }
    }

}
