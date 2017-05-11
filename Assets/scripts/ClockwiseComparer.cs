using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//namespace Hydra.HydraCommon.Utils.Comparers {
public class ClockwiseComparer : IComparer<Vector2> {
    private Vector2 m_Origin;

    #region Properties

    /// <summary>
    /// 	Gets or sets the origin.
    /// </summary>
    /// <value>The origin.</value>
    public Vector2 origin { get { return m_Origin; } set { m_Origin = value; } }

    #endregion

    /// <summary>
    /// 	Initializes a new instance of the ClockwiseComparer class.
    /// </summary>
    /// <param name="origin">Origin.</param>
    public ClockwiseComparer(Vector2 origin) {
        m_Origin = origin;
    }

    #region IComparer Methods

    /// <summary>
    /// 	Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
    /// </summary>
    /// <param name="first">First.</param>
    /// <param name="second">Second.</param>
    public int Compare(Vector2 first, Vector2 second) {
        return IsClockwise(second, first, m_Origin);
    }

    #endregion

    /// <summary>
    /// 	Returns 1 if first comes before second in clockwise order.
    /// 	Returns -1 if second comes before first.
    /// 	Returns 0 if the points are identical.
    /// </summary>
    /// <param name="first">First.</param>
    /// <param name="second">Second.</param>
    /// <param name="origin">Origin.</param>
    public static int IsClockwise(Vector2 first, Vector2 second, Vector2 origin) {
        Vector2 dir1 = first - origin;
        Vector2 dir2 = second - origin;
        float angle1 = Mathf.Atan2(dir1.x, dir1.y) * Mathf.Rad2Deg;
        float angle2 = Mathf.Atan2(dir2.x, dir2.y) * Mathf.Rad2Deg;

        if (angle1 < 0f) angle1 += 360;
        if (angle2 < 0f) angle2 += 360;

        return angle1.CompareTo(angle2);
    }
}


