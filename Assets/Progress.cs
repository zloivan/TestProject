using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Progress : MonoBehaviour
{


    [SerializeField] private Image progressBar;
    [SerializeField][Range(0f,1f)] private float progress;
    [SerializeField] private float finishTime;

    private void Awake()
    {
        if (finishTime <= 0)
        {
            finishTime = float.Epsilon;
        }
    }

    // Update is called once per frame
    // private void Update()
    // {
    //     
    //     progressBar.fillAmount = progress;
    // }

    [ContextMenu("AnimateProgress")]
    public void UpdateProgress()
    {
        StartCoroutine(DisplayProgressRoutine(finishTime));
    }

    private IEnumerator DisplayProgressRoutine(float f)
    {
        var timeElapsed = 0f;
        while (timeElapsed < finishTime)
        {
            progressBar.fillAmount = Mathf.Lerp(0, 1f, timeElapsed / finishTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        progressBar.fillAmount = 1f;
    }
}
