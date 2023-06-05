using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float normalMoveSpeed;
    [SerializeField] float fastMoveSpeed;
    [SerializeField] float rotSpeed;
    [SerializeField] float zoomSpeed;
    [SerializeField] float snappiness;

    Transform worldCameraTransform;
    Camera worldCamera;

    float moveSpeed;

    Vector3 zoomAmount;

    Vector3 newPos;
    Quaternion newRot;
    Vector3 newZoom;

    Vector3 dragPosStart;
    Vector3 dragPosCurrent;

    Vector3 rotPosStart;
    Vector3 rotPosCurrent;

    Plane plane;

    private void Awake()
    {
        worldCameraTransform = transform.Find("Camera");
        worldCamera = worldCameraTransform.GetComponent<Camera>();

        plane = new Plane(Vector3.up, Vector3.zero);

        zoomAmount = new Vector3(0, -zoomSpeed, zoomSpeed);

        newPos = transform.position;
        newRot = transform.rotation;
        newZoom = worldCameraTransform.localPosition;
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            moveSpeed = fastMoveSpeed;
        else
            moveSpeed = normalMoveSpeed;

        HandleMouseInput();
        HandleKeyboardInput();

        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * snappiness);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * snappiness);
        worldCameraTransform.localPosition = Vector3.Lerp(worldCameraTransform.localPosition, newZoom, Time.deltaTime * snappiness);
    }

    void HandleMouseInput()
    {
        GetMovementMouse();
        GetRotationMouse();
        GetZoomMouse();
    }

    void HandleKeyboardInput()
    {
        GetMovementKeyboard();
        GetRotationKeyboard();
        GetZoomKeyboard();
    }

    void GetMovementMouse()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = worldCamera.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out float entry))
            {
                dragPosStart = ray.GetPoint(entry);
            }
        }

        if (Input.GetMouseButton(1))
        {
            Ray ray = worldCamera.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out float entry))
            {
                dragPosCurrent = ray.GetPoint(entry);
                newPos = transform.position + dragPosStart - dragPosCurrent;
            }
        }
    }

    void GetMovementKeyboard()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPos += (transform.forward * moveSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPos += (-transform.forward * moveSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPos += (transform.right * moveSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPos += (-transform.right * moveSpeed);
        }
    }

    void GetRotationMouse()
    {
        if (Input.GetMouseButtonDown(2))
        {
            rotPosStart = Input.mousePosition;
        }
        if (Input.GetMouseButton(2))
        {
            rotPosCurrent = Input.mousePosition;

            Vector3 difference = rotPosStart - rotPosCurrent;

            rotPosStart = rotPosCurrent;

            newRot *= Quaternion.Euler(Vector3.up * (-difference.x / 5));
        }
    }

    void GetRotationKeyboard()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            newRot *= Quaternion.Euler(Vector3.up * rotSpeed);
        }
        if (Input.GetKey(KeyCode.E))
        {
            newRot *= Quaternion.Euler(Vector3.up * -rotSpeed);
        }
    }

    void GetZoomMouse()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            newZoom += Input.mouseScrollDelta.y * zoomAmount * 20f;
        }
    }

    void GetZoomKeyboard()
    {
        if (Input.GetKey(KeyCode.R))
        {
            newZoom += zoomAmount;
        }
        if (Input.GetKey(KeyCode.F))
        {
            newZoom -= zoomAmount;
        }
    }

    public void PositionCamera(Vector2 pointOnPlane)
    {
        newPos = transform.position = new Vector3(pointOnPlane.x, transform.position.y, pointOnPlane.y);
    }

}
