using log4net.Util;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemieScript))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        
        EnemieScript fov = (EnemieScript)target;
        Vector3 position = fov.transform.position + new Vector3(0, 1, 0);
        Handles.color = Color.white;
        Handles.DrawWireArc(position, Vector3.up, Vector3.forward, 360, fov.fovRadius);

        Handles.color = Color.blue;
        Handles.DrawWireArc(position, Vector3.up, Vector3.forward, 360, fov.radius360vision);

        Vector3 viewAngle01 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.angle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(position, position + viewAngle01 * fov.fovRadius);
        Handles.DrawLine(position, position + viewAngle02 * fov.fovRadius);

    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}