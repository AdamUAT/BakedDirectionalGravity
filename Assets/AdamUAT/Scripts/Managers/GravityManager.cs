using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
    public enum RegularShapes { Sphere, Cube, Box}
    public enum CoveragePresets { Standard, Minimal, Extra}

    //The active nearby gravityPoints
    private List<GravityData> gravityPoints = new List<GravityData>();

    /// <summary>
    /// Loads a point of gravity data to the list.
    /// </summary>
    public void AddGravityPoint(GravityData gravityPoint)
    {
        gravityPoints.Add(gravityPoint);
    }

    /// <summary>
    /// Loads multiple points of gravity data to the list.
    /// </summary>
    public void AddGravityPoints(List<GravityData> _gravityPoints)
    {
        foreach(GravityData gravityPoint in _gravityPoints)
        {
            gravityPoints.Add(gravityPoint);
        }
    }

    /// <summary>
    /// Unloads a point of gravity data to the list.
    /// </summary>
    public void RemoveGravityPoint(GravityData gravityPoint)
    {
        gravityPoints.Remove(gravityPoint);
    }

    /// <summary>
    /// Unloads multiple points of gravity data to the list.
    /// </summary>
    public void RemoveGravityPoints(List<GravityData> _gravityPoints)
    {
        foreach(GravityData gravityPoint in _gravityPoints)
        {
            gravityPoints.Remove(gravityPoint);
        }
    }

    public void GetGravityDirectionFromPoint(Vector3 position)
    {

    }
}

[System.Serializable]
public class GravityData
{
    [Tooltip("The position of this point of gravity in world space.")]
    public Vector3 Position;
    [Tooltip("The direction of this point of gravity in world space. Magnitude is normalized.")]
    public Vector3 Direction;
}
