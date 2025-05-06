using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonExt : Button, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject enableState;
    public GameObject disableState;

    public float duration = 0.1f;
    public float upScale = 1f;
    public float hoverScale = 1.01f;
    public float downScale = 0.97f;

    public Ease hoverEase = Ease.Linear;
    public Ease downEase = Ease.Linear;

    public AnimationCurve hoverScaleEase = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
    public AnimationCurve downScaleEase = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

    public Transform animationObject;

    public override bool IsInteractable()
    {
        if (enableState != null) { enableState.SetActive(interactable); }
        if (disableState != null) { disableState.SetActive(!interactable); }
        return base.IsInteractable();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (!interactable) { return; }
        if (animationObject == null) animationObject = transform;

        animationObject.DOScale(hoverScale, duration).SetEase(hoverEase).SetUpdate(true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (!interactable) { return; }
        if (animationObject == null) animationObject = transform;

        animationObject.DOScale(1f, duration).SetEase(Ease.Linear).SetUpdate(true);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (!interactable) { return; }
        if (animationObject == null) animationObject = transform;

        animationObject.DOScale(downScale, duration).SetEase(downEase).SetUpdate(true);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (!interactable) { return; }
        if (animationObject == null) animationObject = transform;

        animationObject.DOScale(upScale, duration).SetEase(Ease.Linear).SetUpdate(true);
    }
}