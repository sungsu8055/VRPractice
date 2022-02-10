using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPPquad : MonoBehaviour
{
    OVRPassthroughLayer passthroughLayer;
    public MeshFilter projectionObject;
    OVRInput.Controller controllerHand;

    void Start()
    {
        passthroughLayer = GetComponent<OVRPassthroughLayer>();
        passthroughLayer.AddSurfaceGeometry(projectionObject.gameObject, false);
        if (GetComponent<GrabObjects>())
        {
            GetComponent<GrabObjects>().GrabbedObjectDelegate += Grab;
            GetComponent<GrabObjects>().ReleasedObjectDelegate += Release;
        }
    }

    public void Grab(OVRInput.Controller grabHand)
    {
        passthroughLayer.RemoveSurfaceGeometry(projectionObject.gameObject);
        controllerHand = grabHand;
    }

    public void Release()
    {
        controllerHand = OVRInput.Controller.None;
        passthroughLayer.AddSurfaceGeometry(projectionObject.gameObject, false);
    }
}
