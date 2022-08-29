using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ConstraintProvider : MonoBehaviour
{
    [SerializeField] private List<OverrideTransform> _overriderTransforms;

    private void Awake()
    {
        if (_overriderTransforms == null)
        {
            _overriderTransforms = GetComponentsInChildren<OverrideTransform>().ToList();
        }
    }

    private void OnValidate()
    {
        if (_overriderTransforms == null || _overriderTransforms.Count == 0)
        {
            _overriderTransforms = GetComponentsInChildren<OverrideTransform>().ToList();
        }
    }

    private List<Transform> _trackPosition = new List<Transform>();
    
    [Button]
    public void UpdateConstrains(List<Transform> activeNodes)
    {
        
        if (activeNodes.Count > _overriderTransforms.Count)
        {
            activeNodes.RemoveRange(_overriderTransforms.Count,activeNodes.Count - _overriderTransforms.Count);
        }

        _trackPosition = activeNodes;
        
        
        // Queue<Transform> que = new Queue<Transform>(activeNodes);
        // for (int i = _overriderTransforms.Count - que.Count; i < _overriderTransforms.Count; i++)
        // {
        //     var data = _overriderTransforms[i].data;
        //     data.sourceObject = que.Dequeue();
        //     _overriderTransforms[i].data = data;
        // }
    }

    private void Update()
    {
        if (_trackPosition == null || _trackPosition.Count == 0)
        {
            return;
        }

        int index = 0;
        
        for (int i = _overriderTransforms.Count - _trackPosition.Count; i < _overriderTransforms.Count; i++)
        {
            _overriderTransforms[i].transform.localPosition = _trackPosition[index].localPosition;
            _overriderTransforms[i].transform.localRotation = _trackPosition[index].localRotation;
            index++;
        }
    }
}
