using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    private Vector3 defaultCameraPosition;
    private Quaternion defaultCameraRotation;

    private Transform focusTarget;

    public float moveSpeed = 2, scrollSpeed = 5;
    public float heightMin = 5, heightMax = 20;

    public float borderMovePadding = 40;

    private int screenHeight;

	void Start () {

        defaultCameraPosition = transform.position;
        
        defaultCameraRotation = transform.rotation;

        screenHeight = Screen.height;
    }
	
	void Update () {
        float horDirection = Input.GetAxis("Horizontal");
        float vertDirection = Input.GetAxis("Vertical");

        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");

        #region Horizontal Movement
        if (horDirection < 0)
        {
            transform.position = new Vector3(transform.position.x - moveSpeed * Time.deltaTime, transform.position.y, transform.position.z);
        }else if (horDirection > 0)
        {
            transform.position = new Vector3(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y, transform.position.z);
        }
        #endregion

        #region Vertical Movement
        if (vertDirection < 0)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y - moveSpeed * Time.deltaTime, heightMin, heightMax), transform.position.z);
        }
        else if (vertDirection > 0)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y + moveSpeed * Time.deltaTime, heightMin, heightMax), transform.position.z);
        }
        #endregion
    }

    public void LateUpdate()
    {
        if (focusTarget != null)
        {
            transform.position = Vector3.Lerp(transform.position, focusTarget.position, Time.deltaTime * 5);
        }
    }

    public void setFocusTarget(Transform _newTarget)
    {
        focusTarget = _newTarget;
    }
}
