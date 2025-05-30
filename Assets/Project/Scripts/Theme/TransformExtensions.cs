using UnityEngine;
using DG.Tweening;

public static class TransformExtensions
{
    // Reset transform to default values
    public static void ResetTransform(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    // Set world position with optional animation
    public static void SetWorldPosition(this Transform transform, Vector3 position, float duration = 0f)
    {
        if (duration > 0f)
            transform.DOMove(position, duration);
        else
            transform.position = position;
    }

    // Set local position with optional animation
    public static void SetLocalPosition(this Transform transform, Vector3 position, float duration = 0f)
    {
        if (duration > 0f)
            transform.DOLocalMove(position, duration);
        else
            transform.localPosition = position;
    }

    // Smooth scale animation
    public static Tween ScaleTo(this Transform transform, Vector3 scale, float duration, Ease ease = Ease.OutQuad)
    {
        return transform.DOScale(scale, duration).SetEase(ease);
    }

    // Pulse animation
    public static Tween Pulse(this Transform transform, float scale = 1.1f, float duration = 0.5f)
    {
        Vector3 originalScale = transform.localScale;
        return DOTween.Sequence()
            .Append(transform.DOScale(originalScale * scale, duration * 0.5f))
            .Append(transform.DOScale(originalScale, duration * 0.5f));
    }

    // Shake animation
    public static Tween Shake(this Transform transform, float duration = 0.5f, float strength = 1f)
    {
        return transform.DOShakePosition(duration, strength);
    }

    // Get all children
    public static Transform[] GetChildren(this Transform transform)
    {
        Transform[] children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }
        return children;
    }

    // Find child by name recursively
    public static Transform FindChildRecursive(this Transform transform, string name)
    {
        if (transform.name == name)
            return transform;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform result = transform.GetChild(i).FindChildRecursive(name);
            if (result != null)
                return result;
        }

        return null;
    }

    // Destroy all children
    public static void DestroyAllChildren(this Transform transform)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
                Object.Destroy(transform.GetChild(i).gameObject);
            else
                Object.DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    // Set parent with world position stay option
    public static void SetParent(this Transform transform, Transform parent, bool worldPositionStays = false)
    {
        transform.SetParent(parent, worldPositionStays);
    }
}