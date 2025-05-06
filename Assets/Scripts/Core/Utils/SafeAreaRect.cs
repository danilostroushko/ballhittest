using System;
using UnityEngine;

public class SafeAreaRect : MonoBehaviour
{
    public RectTransform Panel { get; private set; }
    Rect LastSafeArea = new Rect(0, 0, 0, 0);
    Vector2Int LastScreenSize = new Vector2Int(0, 0);
    ScreenOrientation LastOrientation = ScreenOrientation.AutoRotation;
    [SerializeField] bool ConformX = true;
    [SerializeField] bool ConformY = true;
    [SerializeField] bool Logging = false;

    public event Action Recalculated;

    void Awake()
    {
        Panel = GetComponent<RectTransform>();

        if (Panel == null)
        {
            Debug.LogWarning("Cannot apply safe area - no RectTransform found on " + name);
        }

        Refresh();
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            Refresh();
        }
    }

    public void Refresh()
    {
        Rect safeArea = Screen.safeArea;

        if (safeArea != LastSafeArea
            || Screen.width != LastScreenSize.x
            || Screen.height != LastScreenSize.y
            || Screen.orientation != LastOrientation)
        {
            LastScreenSize.x = Screen.width;
            LastScreenSize.y = Screen.height;
            LastOrientation = Screen.orientation;

            ApplySafeArea(safeArea);

            Recalculated?.Invoke();
        }
    }

    void ApplySafeArea(Rect r)
    {
        LastSafeArea = r;

        if (!ConformX)
        {
            r.x = 0;
            r.width = Screen.width;
        }

        if (!ConformY)
        {
            r.y = 0;
            r.height = Screen.height;
        }

        if (Screen.width > 0 && Screen.height > 0)
        {
            Vector2 anchorMin = r.position;
            Vector2 anchorMax = r.position + r.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            if (anchorMin.x >= 0 && anchorMin.y >= 0 && anchorMax.x >= 0 && anchorMax.y >= 0)
            {
                Panel.anchorMin = anchorMin;
                Panel.anchorMax = anchorMax;
            }
        }

        if (Logging)
        {
            Debug.LogFormat("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
            name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);
        }
    }
}