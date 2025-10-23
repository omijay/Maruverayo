using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform followTarget;
    [SerializeField] float distance = 5.0f;
    [SerializeField] Vector2 framingOffset;
    [SerializeField] bool inverteX;
    [SerializeField] bool inverteY;

    [SerializeField] float RSpeed = 2.0f;
    float rotationY;
    float rotationX;
    [SerializeField] float RMaxValue = 45f;
    [SerializeField] float RMinValue = 5f;
    float invertxVal;
    float invertyVal;
    void Start()
    {
        
    }

    
    void Update()
    {
        invertxVal= (inverteX)? -1 : 1;
        invertyVal= (inverteY)? -1 : 1;
        rotationY += Input.GetAxis("Mouse X") * RSpeed * invertxVal;
        rotationX += Input.GetAxis("Mouse Y") * RSpeed * invertyVal;
        rotationX = Mathf.Clamp(rotationX, RMinValue, RMaxValue);
        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
        var FPosition = followTarget.position + new Vector3(framingOffset.x, framingOffset.y);
        transform.position = FPosition - targetRotation * new Vector3(0, 0, distance);
        transform.rotation = targetRotation;

    }
}
