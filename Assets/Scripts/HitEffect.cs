using PrimeTween;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [SerializeField]
    private new SkinnedMeshRenderer renderer;

    [SerializeField]
    private float duration = 0.25f;

    [SerializeField]
    private Color color = Color.red;

    private bool isPlaying = false;

    // private Material sharedMaterial;

    private void Start()
    {
        isPlaying = false;
        // sharedMaterial = renderer.sharedMaterial;
    }

    public void Play()
    {
        if (isPlaying)
        {
            return;
        }
        isPlaying = true;
        Tween
            .Custom(
                Color.white,
                color,
                duration / 2f,
                onValueChange: val =>
                {
                    renderer.material.color = val;
                },
                cycles: 2,
                cycleMode: CycleMode.Yoyo
            )
            .OnComplete(() =>
            {
                isPlaying = false;
                // renderer.sharedMaterial = sharedMaterial;
            });
    }
}
