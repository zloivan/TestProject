using System.Collections;
using System.Collections.Generic;
using RopePhysics_3;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ConstraintHelper : Editor
{
    [MenuItem("Tools/TEST")]
    private static void TODO()
    {
        Transform constraintParent = null;
        
        foreach (var VARIABLE in  Selection.objects)         
        {
            //mylogs Probably remove this later
            if (Debug.isDebugBuild) Debug.Log($"<color=purple>{(VARIABLE as GameObject)?.name}</color>");
            var node = (VARIABLE as GameObject)?.GetComponent<RopeNode>();

            if (node != null)
            {
                if (constraintParent == null)
                {
                    var parent = new GameObject("ConstraintGameContainer");
                    parent.transform.SetParent(node.transform.parent);
                    
                    constraintParent = parent.GetComponent<Transform>();
                }

                var newConstraint = new GameObject($"constraind_{node.name}");
                newConstraint.transform.SetParent(constraintParent);
                var  oT =  newConstraint.AddComponent<OverrideTransform>();
                //oT.
            }

        }
        
    }

    [MenuItem("Tools/Order Separate")]
    private static void OrderTransformsSeperatly()
    {
        var parent = (Selection.objects[0] as GameObject)?.transform.parent;
        
        for (int i = Selection.objects.Length - 1; i >= 1; i--)
        {
            var child = (Selection.objects[i] as GameObject)?.transform;
            

            if (child != null && parent != null)
            {
                child.SetParent(parent);
            }
        }
        
    }

    [MenuItem("Tools/Order In")]
    private static void OrderTransformsParently()
    {
        for (int i = Selection.objects.Length - 1; i >= 1; i--)
        {
            var child = (Selection.objects[i] as GameObject)?.transform;
            var parent = (Selection.objects[i - 1] as GameObject)?.transform;

            if (child != null && parent != null)
            {
                child.SetParent(parent);
            }
        }
    }
    
    
}
