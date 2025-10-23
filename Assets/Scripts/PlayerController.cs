using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 500f;

    Quaternion targetRotation;
    CameraController cameraController;
    private void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float moveAmovent = Mathf.Abs(h) + Mathf.Abs(v);
        var moveInput = (new Vector3(h, 0, v)).normalized;
        var moveDir = cameraController.planerRotation * moveInput;

        if (moveAmovent > 0) 
        {
            transform.position += moveDir * moveSpeed * Time.deltaTime;
            targetRotation = Quaternion.LookRotation(moveDir);
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
