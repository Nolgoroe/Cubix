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
    private float _reqStagnantTime = 1;
    private float _stagnantTimer;

    public bool IsMoving { get { return _isMoving; } }
    public Rigidbody RB { get { return _rb; } }


    private void Start()
    {
        OnRollStart.AddListener(SetMovingTrue);
    }

    private void LateUpdate()
    {
        CheckState();
    }

    private void CheckState()
    {
        if (_isMoving)
        {
            if (_rb.velocity.magnitude < 0.001f)
            {
                _stagnantTimer += Time.deltaTime;

                if (_stagnantTimer >= _reqStagnantTime)
                {
                    _isMoving = false;
                    OnRollEnd.Invoke();
                    _stagnantTimer = 0;
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

    }
}

