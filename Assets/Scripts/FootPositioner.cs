using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FootPositioner : MonoBehaviour
{
    public Transform Target;

    [SerializeField] private Transform centerOfMass;
    [SerializeField] private FootPositioner otherFoot;
    [SerializeField] private bool isBalanced;

    public float Lerp;   // used to lerp the foot from its current position to target position
    [SerializeField] private Vector3 startPos; // the start position of a step
    [SerializeField] private Vector3 endPos; // the end position of a step
    [SerializeField] float overShootFactor = 0.8f; // how far should we anticipate a step
    [SerializeField] float stepSpeed = 3f; // how fast the foot moves
    public float FootDisplacementOnX = 0.25f; // the foot's displacement from body center on the X axis

    private Vector3 _midPos;

    private bool _facingRight;

    // Start is called before the first frame update
    void Awake()
    {
        startPos = endPos = _midPos = Target.position;
        _facingRight = GetComponent<Flip>().FacingRight;
    }

    // Update is called once per frame
    void Update()
    {
        FootDisplacementOnX *= _facingRight ? 1: -1;

        UpdateBalance();

        // this foot can only move when: (1) the other foot finishes moving, (2) the other foot made the last step
        bool _thisFootCanMove = otherFoot.Lerp > 1 && Lerp > otherFoot.Lerp;

        // if the body is not balanced AND this foot has finished its previous step (we don't want to calculate new steps in the process of moving a foot)
        if (!isBalanced && Lerp > 1 && _thisFootCanMove)
        {
            CalculateNewStep();
        }

        float _easedLerp = EaseInOutCubic(Lerp);

        if (_facingRight)
        {
            // a lerping method that draws an arc using startPos, midPos, and endPos
            Target.position = Vector3.Lerp(
                Vector3.Lerp(startPos, _midPos, _easedLerp),
                Vector3.Lerp(_midPos, endPos, _easedLerp),
            _easedLerp
                );
        }
        else 
        {
            otherFoot.Target.position = Vector3.Lerp(
                Vector3.Lerp(startPos, _midPos, _easedLerp),
                Vector3.Lerp(_midPos, endPos, _easedLerp),
            _easedLerp
                );

        }
        Lerp += Time.deltaTime * stepSpeed;
    }

    private void UpdateBalance()
    {

        // if center of mass is between two feet, the body is balanced
        isBalanced = IsFloatInRange(centerOfMass.position.x, Target.position.x - FootDisplacementOnX, otherFoot.Target.position.x - otherFoot.FootDisplacementOnX);
    }

    private void CalculateNewStep()
    {
        startPos = Target.position;

        // this will make the foot start moving to its target position starting from next frame
        Lerp = 0;

        // find where the foot should land without considering overshoot
        RaycastHit2D _ray = Physics2D.Raycast(centerOfMass.transform.position + new Vector3(FootDisplacementOnX, 0, 0), Vector2.down, 10);

        // consider the overshoot factor
        Vector3 _posDiff = ((Vector3)_ray.point - Target.position) * (1 + overShootFactor);

        // find end target position
        endPos = Target.position + _posDiff;

        float _stepSize = Vector3.Distance(startPos, endPos);
        _midPos = startPos + _posDiff / 2f + new Vector3(0, _stepSize * 0.1f);
    }





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
    private float EaseInOutCubic(float x)
    {
        return 1f / (1 + Mathf.Exp(-10 * (x - 0.5f)));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(endPos, 0.1f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(Target.position, 0.1f);
    }
}
