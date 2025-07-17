using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 2f;
    [SerializeField] private float maxAcceleration = 35f;

    private Rigidbody2D _rb;
    private Vector2 _direction, _desiredVelocity, _velocity;
    private float _maxSpeedChange;


    //[SerializeField] private float 
    // Start is called before the first frame update
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _direction.x = Input.GetAxisRaw("Horizontal");
        _desiredVelocity = new Vector2(_direction.x, 0f) * Mathf.Max(maxSpeed, 0f);

    }

    void OnWalk(InputValue value)
    {
      /*  _direction.x = value.Get<Vector2>().x;
        _desiredVelocity = new Vector2(_direction.x, 0f) * Mathf.Max(maxSpeed, 0f);*/
    }

    private void FixedUpdate()
    {
        _velocity = _rb.velocity;
        
        _maxSpeedChange = maxAcceleration * Time.deltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);

        _rb.velocity = _velocity;
    }
}
