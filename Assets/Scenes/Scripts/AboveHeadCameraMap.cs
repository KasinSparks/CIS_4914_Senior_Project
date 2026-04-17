using UnityEngine;

public class AboveHeadCameraMap : MonoBehaviour
{
    public Transform aboveView;
    public Transform mapNodes; //to rotate for above view
    private Vector3 savedPosition;
    private Quaternion savedRotation;

    private bool isAboveView = false;

    public void ToggleCameraPosition()
    {
        if (!isAboveView)
        {
            //need to save positions instead of storing at start since the camera moves throughout the map
            savedPosition = transform.position;
            savedRotation = transform.rotation;
            transform.position = aboveView.position;
            transform.rotation = aboveView.rotation;
            RotateChildren(-90f); //this is so the orientation of map nodes is consitent from the camera, vert vs horizontal
            isAboveView = true;
        }
        else
        {
            transform.position = savedPosition;
            transform.rotation = savedRotation;
            RotateChildren(90f);
            isAboveView = false;
        }
    }

    void RotateChildren(float yRotation)
    {
        foreach (Transform child in mapNodes)
        {
            child.Rotate(0f, yRotation, 0f, Space.Self);
        }
    }
}