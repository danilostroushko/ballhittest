using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public event Action<UIView> OnOpenViewEvent;
    public event Action<UIView> OnHideViewEvent;

    [SerializeField] private Transform screensCanvas;
    [SerializeField] private Transform popupsCanvas;

    [SerializeField] private CanvasGroup fadeCanvas;

    private List<UIView> UIViews = new List<UIView>();
    private List<UIView> permanentViews = new List<UIView>();

    private Dictionary<string, UIView> prefabs = new Dictionary<string, UIView>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        gameObject.name = "[UIManager]";
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator Initialize()
    {
        prefabs.Clear();

        var locationsHandle = Addressables.LoadResourceLocationsAsync("UI", typeof(GameObject));
        yield return new WaitUntil(() => locationsHandle.IsDone);

        List<AsyncOperationHandle> locations = new List<AsyncOperationHandle>();

        foreach (IResourceLocation location in locationsHandle.Result)
        {
            AsyncOperationHandle<GameObject> loadAssetHandle = Addressables.LoadAssetAsync<GameObject>(location);
            loadAssetHandle.Completed += obj => { prefabs.Add(location.PrimaryKey, obj.Result.GetComponent<UIView>()); };
            locations.Add(loadAssetHandle);
            yield return new WaitUntil(() => loadAssetHandle.IsDone);
        }

        var group = Addressables.ResourceManager.CreateGenericGroupOperation(locations);
        yield return new WaitUntil(() => group.IsDone);
        Addressables.Release(locationsHandle);

        yield return null;
    }

    private void OnHideView(UIView view)
    {
        view.OnHideEvent -= OnHideView;
        if (UIViews.Contains(view))
        {
            UIViews.Remove(view);
            OnHideViewEvent?.Invoke(view);
        }
    }

    public T Open<T>() where T : UIView
    {
        UIView viewPrefab = prefabs[typeof(T).Name];

        Transform parent = viewPrefab.viewType == ViewType.Window ? screensCanvas : popupsCanvas;
        UIView view = Instantiate(viewPrefab.gameObject, parent, false).GetComponent<UIView>();

        view.UIManager = this;
        view.OnHideEvent += OnHideView;

        UIViews.Add(view);
        OnOpenViewEvent?.Invoke(view);

        return (T)view;
    }

    public T GetWindow<T>() where T : UIView
    {
        UIView view = UIViews.Find(v => v is T);
        return view != null ? (T)view : null;
    }

    public void Hide<T>() where T : UIView
    {
        UIView view = UIViews.Find(w => w is T);
        if (view != null)
        {
            view.Hide();
        }
    }

    public void HideAll(bool fast = false)
    {
        UIViews.ForEach(v => { v.OnHideEvent -= OnHideView; if (fast) { v.FastHide(); } else { v.Hide(); } });
        UIViews.Clear();
    }

    public void HideLast()
    {
        if (!permanentViews.Contains(UIViews.Last()))
        {
            UIViews.Last().Hide();
        }
    }

    public void ShowFade()
    {
        fadeCanvas.DOFade(1f, 0.3f).SetEase(Ease.Linear);
        fadeCanvas.interactable = fadeCanvas.blocksRaycasts = true;
    }

    public void HideFade()
    {
        fadeCanvas.DOFade(0f, 0.3f).SetEase(Ease.Linear);
        fadeCanvas.interactable = fadeCanvas.blocksRaycasts = false;
    }
}