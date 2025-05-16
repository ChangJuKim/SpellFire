// ----------------------------------------------------------------------------
// Unite 2017 - Game Architecture with Scriptable Objects
// 
// Author: Ryan Hipple
// Date:   10/04/17
// ----------------------------------------------------------------------------


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardMover : MonoBehaviour
{
    [Serializable]
    public class MoveAxis
    {
        [SerializeField] private KeyCode Positive;
        [SerializeField] private KeyCode Negative;

        public MoveAxis(KeyCode positive, KeyCode negative)
        {
            Positive = positive;
            Negative = negative;
        }

        public static implicit operator float(MoveAxis axis)
        {
            return (Input.GetKey(axis.Positive)
                ? 1.0f : 0.0f) -
                (Input.GetKey(axis.Negative)
                ? 1.0f : 0.0f);
        }
    }

    [SerializeField] private float MoveRate;
    [SerializeField] private MoveAxis Horizontal = new MoveAxis(KeyCode.D, KeyCode.A);
    [SerializeField] private MoveAxis Vertical = new MoveAxis(KeyCode.W, KeyCode.S);

    [SerializeField] private float Boundary;

    private float ScreenWidth;
    private float ScreenHeight;

    private void Start()
    {
        ScreenWidth = Screen.width;
        ScreenHeight = Screen.height;
    }

    private Vector3 checkBoundary()
    {
        Vector3 vector3 = Vector3.zero;
        vector3.x = (Input.mousePosition.x > ScreenWidth - Boundary) ? 1.0f : (Input.mousePosition.x < 0 + Boundary) ? -1.0f : 0.0f;
        vector3.y = (Input.mousePosition.y > ScreenHeight - Boundary) ? 1.0f : (Input.mousePosition.y < 0 + Boundary) ? -1.0f : 0.0f;
        return vector3;
    }

    private void Update()
    {

        Vector3 boundary = checkBoundary();
        boundary.x = Math.Max(-1, Math.Min(1, boundary.x + Horizontal));
        boundary.y = Math.Max(-1, Math.Min(1, boundary.y + Vertical));

        Vector3 moveNormal = boundary.normalized;

        transform.position += moveNormal * Time.deltaTime * MoveRate;
    }

}