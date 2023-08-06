using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] Vector3 rotationVector = new Vector3(0f, 0f, 360f);
    [SerializeField] float rotationDuration = 1f;
    Tween currentTween;

    public Tween CurrentTween { get => currentTween; private set => currentTween = value; }

    private void Start()
    {
        currentTween = transform.DOLocalRotate(rotationVector, rotationDuration, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear).SetLink(gameObject);
    }
}
