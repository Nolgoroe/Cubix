using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public enum DieElement { Water, Fire, Poison }

[RequireComponent(typeof(Rigidbody), typeof(MeshCollider))]
public class Die : MonoBehaviour
{
    public UnityEvent OnRollStart;
    public UnityEvent OnRollEnd;
    public bool isLocked;
    [SerializeField] private Camera diceCam;
    [SerializeField] private TMP_Text resText;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private List<DieFace> faces;
    [SerializeField] private DieElement element;

    private bool _isMoving;
    private float _reqStagnantTime = 1;
    private float _stagnantTimer;
    private DieFace _currentTopFace;

    public bool IsMoving { get { return _isMoving; } }
    public Rigidbody RB { get { return _rb; } }


    private void Start()
    {
        OnRollStart.AddListener(SetMovingTrue);
        DisplayResources();//for build purpose
    }

    private void LateUpdate()
    {
        CheckState();
        Debug.ClearDeveloperConsole();//testing
        Debug.Log(_currentTopFace?.DisplayObject.rotation.eulerAngles);//testing
        Debug.Log("cam: " + diceCam.transform.rotation.eulerAngles);//testing
    }

    private void CheckState()
    {
        if (_isMoving)
        {
            if (_rb.velocity.magnitude < 0.001f)
            {
                _stagnantTimer += Time.deltaTime;

                if (_stagnantTimer >= _reqStagnantTime)//if die stands still for more than x seconds apply rndRoll logic
                {
                    _isMoving = false;
                    OnRollEnd.Invoke();
                    _stagnantTimer = 0;
                    GetTopValue();
                    Debug.Log("Roll ended");
                }
            }
            else if(_stagnantTimer > 0)
            {
                _stagnantTimer = 0;
            }
        }
    }

    private void SetMovingTrue()
    {
        Debug.Log("Roll started");
        _isMoving = true;
    }

    public DieFaceValue GetTopValue()
    {
        float lowestAngle = float.MaxValue;
        Vector3 tmpFaceVec;
        float tmpAngle;
        foreach (var face in faces)//find the face with the angle closest to up
        {
            tmpFaceVec = face.transform.position - transform.position;
            tmpAngle = Vector3.Angle(Vector3.up, tmpFaceVec);

            if (tmpAngle < lowestAngle)
            {
                lowestAngle = tmpAngle;
                _currentTopFace = face;
            }
        }
        resText.text = ("D" + faces.Count +": Resource: " +
            _currentTopFace.GetFaceValue().Resource.Value + _currentTopFace.GetFaceValue().Resource.Type.ToString() + 
            ", Buff: " + _currentTopFace.GetFaceValue().Buff.Value + _currentTopFace.GetFaceValue().Buff.Type.ToString());
        return _currentTopFace.GetFaceValue();
    }

    public DieFace[] GetAllFaces()
    {
        return faces.ToArray();
    }

    [ContextMenu("DisplayBuffs")]
    public void DisplayBuffs()
    {
        foreach (var face in faces)
        {
            face.DisplayBuff();
        }
    }

    [ContextMenu("DisplayResources")]
    public void DisplayResources()
    {
        foreach (var face in faces)
        {
            face.DisplayResource();
        }
    }

    private void AdjustRotation()
    {

    }




}

