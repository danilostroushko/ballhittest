using Lean.Pool;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ObstacleItem : MonoBehaviour, IPoolable
{
    [SerializeField] private MeshRenderer meshObject;
    [SerializeField] private Collider meshObjectCollider;

    public List<Color> colors = new List<Color>();
    public Color destroyColor = Color.white;

    public void DestroyItem()
    {
        meshObject.material.DOColor(destroyColor, 0.2f)
            .SetEase(Ease.Linear)
            .OnComplete(() => LeanPool.Despawn(gameObject));
    }

    public void OnSpawn()
    {
        transform.localScale = Vector3.one * Random.Range(0.7f, 1.2f);
        meshObject.material.color = colors.Random();
    }

    public void OnDespawn()
    {

    }
}
