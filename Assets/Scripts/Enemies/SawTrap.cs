using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SawTrap : MonoBehaviour
{
    [SerializeField] bool playOnAwake = true;
    [SerializeField] GameObject effectsTransform;
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] Transform waypoint;
    [SerializeField] Ease easeMethod = Ease.Linear;
    [SerializeField] Transform sawbladeModel;
    [SerializeField] Vector3 rotationVector = new Vector3(0f,0f,360f);
    [SerializeField] float rotationDuration = 1f;

    Tweener moveTween;
    Tweener rotationTween;

    // Start is called before the first frame update
    void Start()
    {
        if (playOnAwake)
            StartTweens();
    }

    public void StartTweens()
    {
        if(effectsTransform != null)
            effectsTransform.SetActive(true);
        // Rotate the blade.
        if (sawbladeModel != null)
            rotationTween = sawbladeModel.DOLocalRotate(rotationVector, rotationDuration, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        //.OnStepComplete(() => { rotationVector *= -1; rotationTween.ChangeEndValue(rotationVector); });
        // Move the blade.
        if(waypoint != null)
            moveTween = transform.DOMove(waypoint.position, moveSpeed).SetLoops(-1, LoopType.Yoyo).SetEase(easeMethod)
                .OnStepComplete(() => {
                    Vector3 tmpScale = sawbladeModel.transform.localScale;
                    tmpScale.x *= -1f;
                    sawbladeModel.localScale = tmpScale;

                    rotationVector *= -1; rotationTween.ChangeEndValue(rotationVector);
                    moveTween.SetEase(easeMethod);
                });
    }
}
