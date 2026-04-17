using UnityEngine;

public class AboveHeadCamera : MonoBehaviour
{
    public Transform normalView;
    public Transform aboveView;

    private bool view = true; //if view is true, currently in normal view, if false its birds eye view

    void Start() //moves to normal view on start because it is slightly misaligned from the camera and the coords are weird (easy fix)
    {
        transform.position = normalView.position;
        transform.rotation = normalView.rotation;
    }
    public void ToggleCameraPosition()
    {
        if (view == false)
        {
            transform.position = normalView.position;
            transform.rotation = normalView.rotation;
        }
        else
        {
            transform.position = aboveView.position;
            transform.rotation = aboveView.rotation;
        }
        view = !view;
    }
}
