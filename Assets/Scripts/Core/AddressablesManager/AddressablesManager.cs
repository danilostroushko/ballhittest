using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablesManager : MonoBehaviour
{
    private AsyncOperationHandle downloadHandle;

    public IEnumerator Initialize()
    {
        yield break;
    }

    public IEnumerator Load<T>(string name, Action<T> OnCompleted = null, Action<float> OnProgress = null)
    {
        var sizes = Addressables.GetDownloadSizeAsync(name);
        while (sizes.Status == AsyncOperationStatus.None)
        {
            yield return null;
        }

        downloadHandle = Addressables.DownloadDependenciesAsync(name, false);
        while (downloadHandle.Status == AsyncOperationStatus.None)
        {
            OnProgress?.Invoke(downloadHandle.GetDownloadStatus().Percent);
            yield return null;
        }

        OnCompleted?.Invoke((T)downloadHandle.Result);
        Addressables.Release(downloadHandle);
    }

    public void LoadAsset<T>(string name, Action<T> OnCompleted = null, Action<float> OnProgress = null)
    {
        StartCoroutine(LoadAssetCoroutine<T>(name, OnCompleted, OnProgress));
    }

    private IEnumerator LoadAssetCoroutine<T>(string name, Action<T> OnCompleted = null, Action<float> OnProgress = null)
    {
        //Addressables.CheckForCatalogUpdates();
        var downloadHandle = Addressables.LoadAssetAsync<T>(name);
        while (downloadHandle.Status == AsyncOperationStatus.None)
        {
            OnProgress?.Invoke(downloadHandle.GetDownloadStatus().Percent);
            yield return null;
        }

        OnCompleted?.Invoke(downloadHandle.Result);
        Addressables.Release(downloadHandle);
    }
}