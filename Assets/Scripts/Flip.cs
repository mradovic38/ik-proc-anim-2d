using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flip : MonoBehaviour
{
    
    public bool FacingRight;

    [SerializeField] private Transform frontFootTarget;
    [SerializeField] private Transform backFootTarget;

    private Rigidbody2D _rb;
    private ProceduralWalking _walkScript;
    private float _inputHorizontal, _inputVertical;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _walkScript = GetComponent<ProceduralWalking>();
    }

    // Update is called once per frame
    void Update()
    {
        _inputHorizontal = Input.GetAxisRaw("Horizontal");


        if(_inputHorizontal > 0 && !FacingRight)
        {
            doFlip();
        }

        else if (_inputHorizontal < 0 && FacingRight)
        {
            doFlip();
        }

    }

    public void doFlip()
    {
        Vector3 _currentScale = gameObject.transform.localScale;
        _currentScale.x *= -1;
        gameObject.transform.localScale = _currentScale;
        FacingRight = !FacingRight;

        /* Vector2 _temp = frontFootTarget.position;
         frontFootTarget.position = backFootTarget.position;
         backFootTarget.position = _temp;

         Vector3 _frontFootLocalScale = frontFootTarget.localScale;
         Vector3 _backFootLocalScale = backFootTarget.localScale;
         _frontFootLocalScale.x *= -1;
         _backFootLocalScale.x *= -1;
         frontFootTarget.localScale = _frontFootLocalScale;
         backFootTarget.localScale = _backFootLocalScale;*/

        _walkScript.DoFlip = true;
    }
}
