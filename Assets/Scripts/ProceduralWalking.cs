using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ProceduralWalking : MonoBehaviour
{
    [Header("Balance")]
    [SerializeField] private Transform centerOfMass;
    public bool IsBalanced;

    [Header("Targets")]
    [SerializeField] private Transform frontFootTarget;
    [SerializeField] private Transform backFootTarget;

    [Header("Foot Positioning")]
    [SerializeField] private float stepSpeed = 10f;
    [SerializeField] private float lerpDifference = 0.02f;
    [SerializeField] private float absoluteFootDisplacementOnX = 0.25f;
    [SerializeField] private float overShootFactor = 0.6f;

    [Header("Flipping")]
    public bool DoFlip;


    private float _lerpFront;
    private float _lerpBack;
    private Vector2 _startPosFront, _startPosBack, _endPosFront, _endPosBack, _midPosFront, _midPosBack;
    private bool _frontFootCanMove, _backFootCanMove;
    private float _easedLerpFront, _easedLerpBack;

    private Flip _flip;
    private Rigidbody2D _rb;
    
    // Start is called before the first frame update
    void Awake()
    {
        _flip = GetComponent<Flip>();
        _rb = GetComponent<Rigidbody2D>();

        
    }

    private void Start()
    {
        _startPosFront = _midPosFront = _endPosFront = frontFootTarget.position;
        _startPosBack = _midPosBack = _endPosBack = backFootTarget.position;
        _lerpFront = 0;
        _lerpBack = lerpDifference;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBalance();

        if (DoFlip)
        {
            // rescale

            Vector3 _frontFootLocalScale = frontFootTarget.localScale;
            Vector3 _backFootLocalScale = backFootTarget.localScale;
            _frontFootLocalScale.x *= -1;
            _backFootLocalScale.x *= -1;
            frontFootTarget.localScale = _frontFootLocalScale;
            backFootTarget.localScale = _backFootLocalScale;

            // change foot displacement
            absoluteFootDisplacementOnX *= -1;

            // move feet

            Vector2 _temp = frontFootTarget.position;
            frontFootTarget.position = backFootTarget.position;
            backFootTarget.position = _temp;

            _temp = _endPosFront;
            _endPosFront = _endPosBack;
            _endPosBack = _temp;

            // reset DoFlip
            DoFlip = false;

        }

        _frontFootCanMove = _lerpBack > 1 && _lerpFront > _lerpBack;
        _backFootCanMove = _lerpFront > 1 && _lerpBack > _lerpFront;

        if (!IsBalanced && _lerpFront > 1 && _frontFootCanMove)
        {
            calculateStep(true);
        }
        else if (!IsBalanced && _lerpBack > 1 && _backFootCanMove)
        {
            calculateStep(false);
        }

        _easedLerpFront = easeInOutCubic(_lerpFront);
        _easedLerpBack = easeInOutCubic(_lerpBack);

        frontFootTarget.position = Vector2.Lerp(
                Vector2.Lerp(_startPosFront, _midPosFront, _easedLerpFront),
                Vector2.Lerp(_midPosFront, _endPosFront, _easedLerpFront),
            _easedLerpFront
                );

        backFootTarget.position = Vector2.Lerp(
                Vector2.Lerp(_startPosBack, _midPosBack, _easedLerpBack),
                Vector2.Lerp(_midPosBack, _endPosBack, _easedLerpBack),
            _easedLerpBack
                );

        _lerpFront += Time.deltaTime * stepSpeed;
        _lerpBack += Time.deltaTime * stepSpeed;
    }


    void UpdateBalance()
    {
        // if center of mass is between two feet, the body is balanced
        IsBalanced = IsFloatInRange(centerOfMass.position.x, frontFootTarget.position.x - absoluteFootDisplacementOnX, backFootTarget.position.x + absoluteFootDisplacementOnX);
    }

    void calculateStep(bool isFront)
    {
        if (isFront)
        {
            _startPosFront = frontFootTarget.position;

            _lerpFront = 0;

            // find where the foot should land without considering overshoot
            RaycastHit2D _ray = Physics2D.Raycast(centerOfMass.transform.position + new Vector3(absoluteFootDisplacementOnX * (isFront? 1:-1), 0, 0), Vector2.down, 10);

            // consider the overshoot factor
            Vector2 _posDiff = ((Vector3)_ray.point - frontFootTarget.position) * (1 + overShootFactor);

            // find end target position
            _endPosFront = ((Vector2)(frontFootTarget.position)) + _posDiff;

            float _stepSize = Vector3.Distance(_startPosFront, _endPosFront);
            _midPosFront = _startPosFront + _posDiff / 2f + new Vector2(0, _stepSize * 0.1f);
        }

        else
        {
            _startPosBack = backFootTarget.position;

            _lerpBack = 0;

            // find where the foot should land without considering overshoot
            RaycastHit2D _ray = Physics2D.Raycast(centerOfMass.transform.position + new Vector3(absoluteFootDisplacementOnX * (isFront ? 1 : -1), 0, 0), Vector2.down, 10);

            // consider the overshoot factor
            Vector2 _posDiff = ((Vector3)_ray.point - backFootTarget.position) * (1 + overShootFactor);

            // find end target position
            _endPosBack = ((Vector2)(backFootTarget.position)) + _posDiff;

            float _stepSize = Vector3.Distance(_startPosBack, _endPosBack);
            _midPosBack = _startPosBack + _posDiff / 2f + new Vector2(0, _stepSize * 0.1f);
        }
        
    }




    #region HELPER FUNCTIONS
    /// <summary>
    /// returns true if "value" is between "bound1" and "bound2"
    /// </summary>
    bool IsFloatInRange(float _value, float _bound1, float _bound2)
    {
        float _minValue = Mathf.Min(_bound1, _bound2);
        float _maxValue = Mathf.Max(_bound1, _bound2);
        return _value > _minValue && _value < _maxValue;
    }

    /// <summary>
    /// Smoothly ease in and ease out the input using sigmoid function
    /// </summary>
    private float easeInOutCubic(float x)
    {
        return 1f / (1 + Mathf.Exp(-10 * (x - 0.5f)));
    }
    #endregion

    #region DRAW GIZMOS
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_endPosFront, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_endPosBack, 0.1f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(frontFootTarget.position, 0.1f);
        Gizmos.DrawSphere(backFootTarget.position, 0.1f);
    }
    #endregion
}
