using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIView : MonoBehaviour, IDisposable
{
    public event Action<UIView> OnShowEvent;
    public event Action<UIView> OnHideEvent;

    [Header("UIView", order = -1)]
    [SerializeField] internal RectTransform rectTransform;
    [SerializeField] internal CanvasGroup canvasGroup;

    [HideInInspector]
    public UIManager UIManager;

    public ViewType viewType;

    public ViewAnimation viewAnimation;
    private float fadeDuration = 0.2f;
    private Vector3 scaleHide = new Vector3(0.7f, 0.7f, 0.7f);
    private float scaleDuration = 0.2f;

    public virtual void Awake()
    {

    }

    public virtual void Start()
    {
        Show();
    }

    public void Show()
    {
        if (viewAnimation != ViewAnimation.None)
        {
            PlayAnimation(true);
        }
        else
        {
            OnOpen();
        }
    }

    public void Hide()
    {
        if (viewAnimation != ViewAnimation.None)
        {
            PlayAnimation(false);
        }
        else
        {
            OnHide();
        }
    }

    public void FastHide()
    {
        Dispose();
    }

    public virtual void OnOpen()
    {
        OnShowEvent?.Invoke(this);
    }

    public virtual void OnHide()
    {
        OnHideEvent?.Invoke(this);
        Dispose();
    }

    public void ToFront()
    {
        rectTransform.SetAsLastSibling();
    }

    public void ToBack()
    {
        rectTransform.SetAsFirstSibling();
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }

    #region Animation
    private void PlayAnimation(bool show = true)
    {
        TweenCallback EndAnimationCallback = show ? OnOpen : OnHide;
        float fadeStart = show ? 0f : 1f;
        float fadeEnd = show ? 1f : 0f;
        Vector3 scaleStart = show ? scaleHide : Vector3.one;
        Vector3 scaleEnd = show ? Vector3.one : scaleHide;
        Ease ease = show ? Ease.OutBack : Ease.InBack;

        switch (viewAnimation)
        {
            case ViewAnimation.Fade:
                canvasGroup.alpha = fadeStart;
                canvasGroup.DOFade(fadeEnd, fadeDuration).SetUpdate(true).onComplete = EndAnimationCallback;
                break;
            case ViewAnimation.Scale:
                rectTransform.localScale = scaleStart;
                rectTransform.DOScale(scaleEnd, scaleDuration).SetEase(ease).SetUpdate(true).onComplete = EndAnimationCallback;
                break;
            case ViewAnimation.FadeAndScale:
                canvasGroup.alpha = fadeStart;
                rectTransform.localScale = scaleStart;
                canvasGroup.DOFade(fadeEnd, fadeDuration - 0.05f).SetUpdate(true);
                rectTransform.DOScale(scaleEnd, scaleDuration).SetEase(ease).SetUpdate(true).onComplete = EndAnimationCallback;
                break;
            case ViewAnimation.Custom:
                EndAnimationCallback.Invoke();
                break;
        }
    }


    #endregion
}

public enum ViewType
{
    Window,
    Popup,
    Overlay
}

public enum ViewAnimation
{
    None,
    Fade,
    Scale,
    FadeAndScale,
    Custom
}