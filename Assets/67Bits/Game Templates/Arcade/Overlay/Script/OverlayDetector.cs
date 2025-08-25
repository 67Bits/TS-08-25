using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements IDropCollector for a monobehavior
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class OverlayDetector : MonoBehaviour
{
    public List<Type> detectedTypes = new List<Type>();
    List<Component> detectedComponents = new List<Component>();
    List<GameObject> detectedGameObjects = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        var components = other.gameObject.GetComponents<Component>();
        foreach (Type type in detectedTypes)
        {
            foreach (var component in components)
            {
                if (component.GetType().IsAssignableFrom(type))
                {
                    AttendDetection(type, component);
                    detectedGameObjects.Add(other.gameObject);
                }
            }
        }
    }

    protected virtual void AttendDetection(Type type, Component component)
    {
    }

    protected virtual void AttendUnDetection(Type type, Component component)
    {
    }

    private void OnTriggerExit(Collider other)
    {
        if (detectedGameObjects.Contains(other.gameObject))
        {

        }
    }
}