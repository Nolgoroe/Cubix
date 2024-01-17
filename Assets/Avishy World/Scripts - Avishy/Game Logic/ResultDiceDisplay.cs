using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultDiceDisplay : MonoBehaviour
{
    [SerializeField] private List<DieFace> faces;
    [SerializeField] private DieFace _currentTopFace;

    private void Start()
    {
        //can clone the faces value in an init funciton instead of this.
    }

    public void InitDiceDisplay(Die towerDie)
    {
        faces = new List<DieFace>();
        faces.AddRange(GetComponentsInChildren<DieFace>());

        foreach (DieFace face in faces)
        {
            face.DisplayBuff();
        }
    }
    private void Update()
    {
        Vector3 diretion = Camera.main.transform.position - transform.position;

        Debug.DrawLine(transform.position, diretion);

        transform.LookAt(Camera.main.transform);
    }

    [ContextMenu("Now")]
    public void GetCameraFacingValue()
    {
        Vector3 diretion = Camera.main.transform.position - transform.position;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, diretion, out hit))
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

    private void OnEnable()
    {
        GetCameraFacingValue();
    }
}
