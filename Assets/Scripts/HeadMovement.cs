using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class HeadMovement : MonoBehaviour
{

    [SerializeField] private Transform spine;

    [Header("Head Height")]
    [SerializeField] private float headHeightDifferenceSpeedMultiplier = 0.05f;
    [SerializeField] private float headHeightDifferenceIdle = 0.025f;
    [SerializeField] private float headRiseSpeedMultiplier = 1f;
    [SerializeField] private float headRiseSpeedIdle = 2f;

    [Header("Head Rotation")]
    [SerializeField] private float maxAngleTop = 40f;
    [SerializeField] private float maxAngleBottom = -20f;
    [SerializeField] private float rotationSpeed = 5f;



    private Vector2 _spinePosition;
    private float _riseSpeed;
    private float _startHeadHeight;
    private float _curHeadHeight;
    private float _defaultHeadHeight;
    private float _headRiseSpeed;
    private float _headHeightDifference;
    private Vector2 _mousePosition;
    private Vector2 _mouseDirection;
    private Flip _flip;
    private Rigidbody2D _rb;
    private Quaternion _groundSpineRotation;
    private Quaternion _startSpineRotation;

    void Awake()
    {
        _flip = GetComponent<Flip>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // calculates start head distance from ground
        _defaultHeadHeight = spine.position.y;
        _startHeadHeight = spine.position.y - Physics2D.Raycast(spine.position, Vector2.down, 5).point.y;
        // lerp value
        _riseSpeed = 0f;
        _groundSpineRotation = _startSpineRotation = spine.rotation;
    }

    void Update()
    {
        if (Input.GetAxisRaw("Horizontal")==0 && _rb.velocity.x==0f)
        {
            headCursorLook();
        }
        else
        {
            spine.rotation = Quaternion.Slerp(spine.rotation, new Quaternion(_groundSpineRotation.x, _groundSpineRotation.y, _groundSpineRotation.z * (_flip.FacingRight?1:-1), _groundSpineRotation.w), Time.deltaTime * rotationSpeed);
        }


        _headHeightDifference = headHeightDifferenceIdle + Mathf.Abs(Input.GetAxisRaw("Horizontal")) * headHeightDifferenceSpeedMultiplier;

        _headRiseSpeed = headRiseSpeedIdle + Mathf.Abs(Input.GetAxisRaw("Horizontal")) * headRiseSpeedMultiplier;


        _spinePosition = spine.position;

        // calculate current head distance from ground
        float _bobAmount = Mathf.Sin(Time.time * _headRiseSpeed) * _headHeightDifference;

        RaycastHit2D _hit = Physics2D.Raycast(new Vector2(spine.position.x, spine.position.y - _bobAmount), Vector2.down, 5);

        _groundSpineRotation = Quaternion.Euler(_groundSpineRotation.x, _groundSpineRotation.y, _flip.FacingRight? Vector2.Angle(Vector2.right, _hit.normal): 180 - Vector2.Angle(Vector2.right, _hit.normal));
        _curHeadHeight = _defaultHeadHeight - _bobAmount - _hit.point.y;
        
        Debug.DrawRay(new Vector2(spine.position.x, spine.position.y - _bobAmount), Vector2.down, Color.gray);
        _spinePosition.y = _defaultHeadHeight + (_startHeadHeight - _curHeadHeight) + _bobAmount;


        spine.position = _spinePosition;
    }

    void headCursorLook()
    {
        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        _mouseDirection = (_mousePosition - new Vector2(spine.position.x, spine.position.y)).normalized;
        //Debug.DrawRay(_mousePosition, _mouseDirection);

        spine.right = Vector2.Lerp(spine.right, _startSpineRotation  * _mouseDirection, rotationSpeed*Time.deltaTime); /* * (_flip.FacingRight ? 1 : -1)*/

        Vector3 _eulerDir = spine.localEulerAngles;
        _eulerDir.z = Mathf.Clamp(_eulerDir.z, maxAngleBottom,  maxAngleTop);
        spine.localEulerAngles = _eulerDir;

        if(_mouseDirection.x < -0.1 && _flip.FacingRight)
        {
            _flip.doFlip();
        }
        else if(_mouseDirection.x > 0.1 && !_flip.FacingRight)
        {
            _flip.doFlip();
        }

        
    }

    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Camera.main.ScreenToWorldPoint(Input.mousePosition), .2f);
    }
}
