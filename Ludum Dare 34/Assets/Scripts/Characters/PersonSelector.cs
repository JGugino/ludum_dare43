using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonSelector : MonoBehaviour {

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                string otherTag = hit.collider.tag;

                if (otherTag == "Towns Person")
                {
                    if (CameraManager.instance.getFocusTarget() != null)
                    {
                        CameraManager.instance.removeTarget();
                    }

                    CameraManager.instance.setFocusTarget(hit.transform);

                    hit.collider.GetComponent<TownPersonController>().isControlling = true;

                    if (UIController.instance.infoBoxObject.activeSelf)
                    {
                        UIController.instance.updateInfoBoxText(CameraManager.instance.getFocusTarget().GetComponent<TownPersonController>());
                    }
                }
            }
        }
    }
}
