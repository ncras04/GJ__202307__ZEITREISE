using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FloatingObject : MonoBehaviour
{
    [SerializeField] bool playOnAwake = true;
    [SerializeField] Vector3 positionOffest = new Vector3(0f, 0.2f, 0f);
    [SerializeField] Vector3 floatingSpeed = Vector3.one;
    [SerializeField] Ease easeMethod = Ease.Linear;
    [SerializeField] int loopAmount = -1;


    Tween currentTween;
    public Tween CurrentTween { get => currentTween; private set => currentTween = value; }

    // Start is called before the first frame update
    void Start()
    {
        if (playOnAwake)
            StartTweens();
    }

    public void StartTweens()
    {
        if (positionOffest.y != 0)
        {
            currentTween = transform.DOMoveY(transform.position.y + positionOffest.y, floatingSpeed.y).SetLoops(loopAmount, LoopType.Yoyo).SetEase(easeMethod)
                .OnStepComplete(() =>
                {
                    currentTween.SetEase(easeMethod);
                }).SetLink(transform.gameObject);
        }

        if (positionOffest.x != 0f)
        {
            transform.DOMoveX(transform.position.x + positionOffest.x, floatingSpeed.x).SetLoops(loopAmount, LoopType.Yoyo).SetEase(easeMethod).SetLink(transform.gameObject);
        }

        if (positionOffest.z != 0f)
        {
            transform.DOMoveZ(transform.position.z + positionOffest.z, floatingSpeed.z).SetLoops(loopAmount, LoopType.Yoyo).SetEase(easeMethod).SetLink(transform.gameObject);
        }
    }

}
