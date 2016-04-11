using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

public class DragRigidbody : MonoBehaviour
{
    [SerializeField]
    float spring = 100.0f;
    [SerializeField]
    float damper = 5.0f;
    [SerializeField]
    float drag = 4.0f;
    [SerializeField]
    float angularDrag = 5.0f;
    [SerializeField]
    float springDistance = 0.2f;
    [SerializeField]
    float distanceInFrontOfCamera = 3f;

    SpringJoint springJoint = null;
    readonly Dictionary<Rigidbody, Renderer[]> allRigidBodies = new Dictionary<Rigidbody, Renderer[]>();
    readonly List<Rigidbody> visibleRigidbodies = new List<Rigidbody>();

    void Start()
    {
        // Grab all the rigidbodies in the scene
        Rigidbody[] allBodies = GameObject.FindObjectsOfType<Rigidbody>();
        foreach(Rigidbody body in allBodies)
        {
            // Make sure this rigid body is movable
            if(body.isKinematic == false)
            {
                allRigidBodies.Add(body, body.GetComponentsInChildren<Renderer>());
            }
        }

        // Create a joint controller
        var go = new GameObject("Rigidbody dragger");
        var newBody = go.AddComponent<Rigidbody>();
        springJoint = go.AddComponent<SpringJoint>();
        springJoint.autoConfigureConnectedAnchor = false;
        newBody.isKinematic = true;

        // Position the joint controller
        go.transform.SetParent(Camera.main.transform);
        go.transform.localRotation = Quaternion.identity;
        go.transform.localPosition = Vector3.forward * distanceInFrontOfCamera;
    }

    void Update()
    {
        // Make sure the user pressed the mouse down
        if (!GetInput(true))
            return;

        // Check if an object is on-screen
        Rigidbody detectedRigidbody = null;
        foreach (KeyValuePair<Rigidbody, Renderer[]> bodyPair in allRigidBodies)
        {
            // Check if any renderer is visible
            foreach (Renderer renderer in bodyPair.Value)
            {
                if (renderer.isVisible)
                {
                    detectedRigidbody = bodyPair.Key;
                    break;
                }
            }
            if (detectedRigidbody != null)
            {
                break;
            }
        }

        if (detectedRigidbody != null)
        {
            springJoint.spring = spring;
            springJoint.damper = damper;
            springJoint.maxDistance = springDistance;
            springJoint.connectedBody = detectedRigidbody;
            springJoint.anchor = Vector3.zero;
            springJoint.connectedAnchor = Vector3.zero;

            StartCoroutine(DragObject());
        }
    }

    Rigidbody GetVisibleRigidbody()
    {
        // Collect all the rigidbodies that are visible into a single list
        visibleRigidbodies.Clear();
        foreach (KeyValuePair<Rigidbody, Renderer[]> bodyPair in allRigidBodies)
        {
            // Check if any renderer is visible
            foreach (Renderer renderer in bodyPair.Value)
            {
                if (renderer.isVisible)
                {
                    visibleRigidbodies.Add(bodyPair.Key);
                }
            }
        }

        // Search for the closest one
        Rigidbody closestBody = null;
        float closestDistance = float.MaxValue;
        float currentDistance = float.MaxValue;
        foreach(Rigidbody body in visibleRigidbodies)
        {
            currentDistance = (body.position - springJoint.transform.position).magnitude;
            if(currentDistance < closestDistance)
            {
                closestBody = body;
                closestDistance = currentDistance;
            }
        }
        return closestBody;
    }

    IEnumerator DragObject()
    {
        var oldDrag = springJoint.connectedBody.drag;
        var oldAngularDrag = springJoint.connectedBody.angularDrag;
        springJoint.connectedBody.drag = drag;
        springJoint.connectedBody.angularDrag = angularDrag;

        while (GetInput(false))
        {
            yield return null;
        }

        if (springJoint.connectedBody)
        {
            springJoint.connectedBody.drag = oldDrag;
            springJoint.connectedBody.angularDrag = oldAngularDrag;
            springJoint.connectedBody = null;
        }
    }

    bool GetInput(bool onlyOnMouseDown)
    {
        if(onlyOnMouseDown)
        {
            return CrossPlatformInputManager.GetButtonDown("Fire1");
        }
        else
        {
            return CrossPlatformInputManager.GetButton("Fire1");
        }
    }
}
