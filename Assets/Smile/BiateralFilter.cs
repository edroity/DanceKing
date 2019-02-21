using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BiateralFilter : MonoBehaviour
{
    public enum BlurType
    {
        GaussianBlur = 0,
        BilateralColorFilter = 1,
        BilateralNormalFilter = 2,
    }

    Material filterMaterial = null;

    [Range (1, 4)]
    public int BlurRadius = 4;
    public BlurType blurType = BlurType.GaussianBlur;
    [Range (0, 0.2f)]
    public float bilaterFilterStrength = 0.15f;

    private void Awake()
    {
        filterMaterial = GetComponent<RawImage> ().material;
    }

    private void Update ()
    {
        var blurPass = (int)blurType;
        filterMaterial.SetFloat ("_BilaterFilterFactor", 1.0f - bilaterFilterStrength);
        filterMaterial.SetVector ("_BlurRadius", new Vector4 (BlurRadius, 0, 0, 0));
    }
}
