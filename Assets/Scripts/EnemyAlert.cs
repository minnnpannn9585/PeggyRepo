// EnemyAlert.cs
using UnityEngine;
using System.Collections.Generic;

public static class EnemyAlert
{
    private static readonly List<EnemyAlertListener> listeners = new List<EnemyAlertListener>();

    public static void Register(EnemyAlertListener l)
    {
        if (l != null && !listeners.Contains(l)) listeners.Add(l);
    }
    public static void Unregister(EnemyAlertListener l) { listeners.Remove(l); }

    public static void Alert(Vector3 pos)
    {
        foreach (var l in listeners) if (l) l.GoTo(pos);
    }
}