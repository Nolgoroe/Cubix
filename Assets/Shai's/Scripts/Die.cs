using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum DieElement { Water, Fire, Poison }

public class Die : MonoBehaviour
{
    public UnityEvent OnRollStart;
    public UnityEvent OnRollEnd;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private List<DieFace> faces;
    [SerializeField] private DieElement element;

    private bool _isMoving;

    public bool IsMoving { get { return _isMoving; } }
    public Rigidbody RB { get { return _rb; } }


    private void Start()
    {
        OnRollEnd.AddListener(_rb.ResetCenterOfMass);
    }

    private void LateUpdate()
    {
        CheckState();
    }

    private void CheckState()
    {
        if (_isMoving && Mathf.Approximately(_rb.velocity.magnitude, 0))
        {
            _isMoving = false;
            OnRollEnd.Invoke();
        }
        else if (!_isMoving && !Mathf.Approximately(_rb.velocity.magnitude, 0))
        {
            _isMoving = true;
            OnRollStart.Invoke();
        }
    }

}

