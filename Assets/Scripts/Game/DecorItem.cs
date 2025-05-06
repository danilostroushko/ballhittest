using Lean.Pool;
using System.Collections.Generic;
using UnityEngine;

public class DecorItem : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshObject;

    public List<Color> colors = new List<Color>();

    public void Start()
    {
        meshObject.material.color = colors.Random();
    }
}