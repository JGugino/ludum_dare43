using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public static CameraManager instance;

    private Vector3 defaultCameraPosition;

    private Transform focusTarget;

    public Vector3 targetOffset;

    public float moveSpeed = 2, rotateSpeed = 5;

    private bool canHorRotate = false;

    private void Awake()
    {
        instance = this;
    }

    void Start () {

        defaultCameraPosition = transform.position;
    }
	
	void Update () {
        float horDirection = Input.GetAxis("Horizontal");
        float vertDirection = Input.GetAxis("Vertical");

        if (focusTarget == null)
        {
            if (UIController.instance.actionBarObject.activeSelf)
            {
                UIController.instance.toggleActionBar(false);
            }

            if (UIController.instance.infoBoxObject.activeSelf)
            {
                UIController.instance.infoBoxObject.SetActive(false);
            }
        }else if (focusTarget != null)
        {
            if (!UIController.instance.actionBarObject.activeSelf)
            {
                UIController.instance.toggleActionBar(true);
            }
        }

        #region Horizontal Movement
        if (horDirection < 0)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x - moveSpeed * Time.deltaTime, 50, 155), transform.position.y, transform.position.z);
            removeTarget();
        }else if (horDirection > 0)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x + moveSpeed * Time.deltaTime, 50, 155), transform.position.y, transform.position.z);
            removeTarget();
        }
        #endregion

        #region Vertical Movement
        if (vertDirection > 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(transform.position.z + moveSpeed * Time.deltaTime, 15, 150));
            removeTarget();
        }
        else if (vertDirection < 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(transform.position.z - moveSpeed * Time.deltaTime, 15, 150));

            removeTarget();
        }
        #endregion

        if (focusTarget == null && transform.position != defaultCameraPosition)
        {
            transform.position = Vector3.Lerp(new Vector3(transform.position.x, transform.position.y, transform.position.z), new Vector3(transform.position.x, defaultCameraPosition.y, transform.position.z), Time.deltaTime * 5);
        }
    }

    public void LateUpdate()
    {

        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");

        #region Vertical Rotate
        if (scrollWheel < 0)
        {
            transform.RotateAround(transform.position, Vector3.right, rotateSpeed * Time.deltaTime);
        }
        else if (scrollWheel > 0)
        {
            transform.RotateAround(transform.position, Vector3.right, -rotateSpeed * Time.deltaTime);
        }
        #endregion

        #region Left & Right Rotate
        if (canHorRotate)
        {
            if (Input.GetButton("Rotate Left"))
            {
                transform.RotateAround(transform.position, Vector3.up, -rotateSpeed * Time.deltaTime);
            }
            else if (Input.GetButton("Rotate Right"))
            {
                transform.RotateAround(transform.position, Vector3.up, rotateSpeed * Time.deltaTime);
            }
        }
        #endregion

        if (focusTarget != null)
        {
            transform.position =Vector3.Lerp(transform.position, focusTarget.position + targetOffset, Time.deltaTime * 5);
        }
    }

    public void removeTarget()
    {
        if (focusTarget != null)
        {
            focusTarget.GetComponent<TownPersonController>().isControlling = false;

            focusTarget = null;

            transform.position = Vector3.Lerp(new Vector3(transform.position.x, transform.position.y, transform.position.z), new Vector3(transform.position.x, defaultCameraPosition.y, transform.position.z), Time.deltaTime * 5);
        }
    }

    public Transform getFocusTarget()
    {
        return focusTarget;
    }

    public void setFocusTarget(Transform _newTarget)
    {
        focusTarget = _newTarget;
    }
}
