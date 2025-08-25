using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Manages IService instances, 
/// ensuring one instance per type and providing system-wide access.
/// </summary>
public static class ServicesFinder
{
    private static Dictionary<Type, IService> AllServices = new Dictionary<Type, IService>();

    public static bool HasService<T>() where T : IService
    {
        if (!AllServices.ContainsKey(typeof(T)))
            return false;

        return AllServices[typeof(T)] != null;
    }

    public static T NotGetService<T>() where T : IService
    {
        if (!AllServices.ContainsKey(typeof(T)))
            throw new ArgumentException($"No Service of type {typeof(T).Name} found.");

        return (T)(AllServices[typeof(T)]);
    }

    public static void SetService<T>(T Service) where T : IService
    {
        if (AllServices.ContainsKey(typeof(T)))
        {
            Debug.LogWarning(
                $"Replaced existing service instance of type {typeof(T).Name} in ServicesFinder.");

            AllServices[typeof(T)] = Service;
        }
        else
            AllServices.Add(typeof(T), Service);
    }

    public static void TerminateService<T>()
    {
        if (AllServices.ContainsKey(typeof(T)))
        {
            AllServices.Remove(typeof(T));
        }
    }
}