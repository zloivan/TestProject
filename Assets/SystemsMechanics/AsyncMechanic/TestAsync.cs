using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestAsync : MonoBehaviour
{
    [SerializeField] private TMP_Text _progressValue;
    [SerializeField] private Button _start;
    [SerializeField] private Button _cancel;

    
    private CancellationTokenSource _cts;

    
    private void Awake()
    {
        _start.onClick.AddListener(StartAsync);
        _cancel.onClick.AddListener(Cancel);
        
    }

    [Button]
    public async void StartAsync()
    {
        _cts = new CancellationTokenSource();
        _cts.Token.Register(() =>
        {
            print("Message from callback");
        });

        try
        {
            await SomeAsyncMethod(100, _cts.Token, new Progress<int>(i => _progressValue.text = i.ToString()));
        }
        catch (OperationCanceledException e)
        {
            Debug.Log($"{e}");
        }
        finally
        {
            _cts.Dispose();
        }
       
    }

    private async UniTask SomeAsyncMethod(int coutTo, CancellationToken cToken, IProgress<int> progress)
    {
        print("Before Start");

        for (var i = 0; i < coutTo; i++)
        {
            await UniTask.Delay(100, cancellationToken: cToken);

            progress.Report(i * 100 / coutTo);
            if (_cts.IsCancellationRequested)
            {
                return;
            }
        }

        print("After Done");
    }

    [Button]
    private void Cancel()
    {
        _cts.Cancel();
    }
}