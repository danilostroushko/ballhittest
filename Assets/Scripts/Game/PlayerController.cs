using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public event Action<float> OnHit;
    public event Action OnMinFill;

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private SphereCollider sphereCollider;

    [SerializeField] private MeshRenderer hitMeshRenderer;
    [SerializeField] private ParticleSystem hitBoom;
    [SerializeField] private LineRenderer playerLine;
    [SerializeField] private TargetItem target;

    public float offsetZ = 3f;
    public float maxSize = 1f;
    public float minSize = 0.2f;
    public float currentSize = 1f;

    public float fillStep = 0.1f;
    public float minFillStep = 0.05f;
    public float maxFill = 1f;
    public float fillSpeed = 0.1f;
    public float fillMult = 1f;
    public float hitFillSize = 0f;

    public Gradient sphereColor;
    public Gradient hitSphereColor;

    public Vector3 GetCenter => transform.position - sphereCollider.center;
    public float GetRadius => sphereCollider.radius;

    private bool isTap = false;
    private bool isMinFill = false;
    public bool isMoveHit = false;

    private void Start()
    {
        hitMeshRenderer.gameObject.SetActive(false);
        hitMeshRenderer.material.color = hitSphereColor.Evaluate(0);
        hitFillSize = 0f;

        meshRenderer.material.color = sphereColor.Evaluate(currentSize.RangeToPercent(minSize, maxSize));
    }

    public void Init(float totalWidth, float totalHeight)
    {
        transform.position = new Vector3(0, 0, -offsetZ);
        target.transform.position = new Vector3(0, 0, totalHeight + offsetZ);

        playerLine.widthMultiplier = sphereCollider.radius * 2f;
        Vector3 middlePoint = transform.position.WithZ(Vector3.Distance(transform.position, target.transform.position) / 2f);
        playerLine.SetPosition(0, transform.position);
        playerLine.SetPosition(1, middlePoint.WithY(0.001f));
        playerLine.SetPosition(2, target.transform.position.WithY(0.002f));
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) { return; }

        if (isMinFill || isMoveHit) { return; }

        if (Input.GetMouseButtonDown(0))
        {
            isTap = true;
        }

        if (Input.GetMouseButton(0) && hitFillSize < maxFill && isTap)
        {
            if (currentSize <= minSize) { isMinFill = true; OnMinFill?.Invoke(); return; }

            float size = fillStep * fillSpeed * Time.deltaTime;
            hitFillSize += size * fillMult;
            if (hitFillSize >= minFillStep)
            {
                currentSize -= size;

                SetHitSize(hitFillSize);
                SetSize(currentSize);

                playerLine.widthMultiplier = sphereCollider.radius * 2f;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isTap = false;
            if (hitFillSize >= minFillStep)
            {
                OnHit?.Invoke(hitFillSize);
                hitFillSize = 0f;
            }
        }
    }

    private void SetSize(float size)
    {
        meshRenderer.transform.localScale = Vector3.one * size;
        sphereCollider.center = new Vector3 (0, size, 0);
        sphereCollider.radius = size;

        meshRenderer.material.color = sphereColor.Evaluate(currentSize.RangeToPercent(minSize, maxSize));
    }

    private void SetHitSize(float size)
    {
        hitMeshRenderer.gameObject.SetActive(true);
        hitMeshRenderer.transform.localScale = Vector3.one * size;
        hitMeshRenderer.material.color = hitSphereColor.Evaluate(hitFillSize.RangeToPercent(minSize, maxSize));
    }

    public float MoveHitSphere(Vector3 target, float speed = 0.1f)
    {
        isMoveHit = true;

        float dist = Vector3.Distance(hitMeshRenderer.transform.position, target);
        float duration = speed * dist;
        if (hitMeshRenderer.transform.position.z < target.z)
        {
            hitMeshRenderer.transform.DOMoveZ(target.z, duration)
                .SetEase(Ease.Linear)
                .SetDelay(0.1f)
                .OnComplete(() =>
                {
                    hitMeshRenderer.gameObject.SetActive(false);
                    hitMeshRenderer.transform.localPosition = new Vector3(0, 0, sphereCollider.radius + 1f);

                    hitBoom.transform.position = target.WithY(0);
                    hitBoom.gameObject.SetActive(true);
                    hitBoom.Play();
                });
            return duration;
        }
        else
        {
            return 0;
        }
    }

    public float JumpTo(float z, bool isTarget)
    {
        if (isTarget)
        {
            target.OpenDoors();
        }

        float _offset = isTarget ? 0 : offsetZ;
        float jumpDist = 0.5f;
        float speed = 0.25f;
        float dist = Mathf.Abs(transform.position.z - z);
        float targetZ = Mathf.FloorToInt(dist - _offset);
        float duration = dist * speed;
        int numJumps = Mathf.FloorToInt(targetZ / jumpDist);

        if (targetZ >= _offset)
        {
            transform.DOMoveZ(transform.position.z + targetZ, duration).SetEase(Ease.Linear);
            meshRenderer.transform.DOLocalJump(Vector3.zero, 1, numJumps, duration).SetEase(Ease.Linear);
            return duration;
        }
        else
        {
            return 0;
        }
    }
}