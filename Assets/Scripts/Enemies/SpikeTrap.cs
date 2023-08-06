using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] List<Transform> spikes;
    [SerializeField] Vector3 finalScale = new Vector3 (1f, 1.5f, 1f);
    [SerializeField] float duration = 1f;
    [SerializeField] float delayTime = 1f;
    [SerializeField] Ease easeMode;
    [SerializeField] bool playOnAwake = true;
    [SerializeField] bool moveInSequence;

    // Start is called before the first frame update
    void Start()
    {
        if(playOnAwake)
        {
            ActivateTrap();
        }
    }

    public void ActivateTrap()
    {
            var sequence = DOTween.Sequence();
            foreach (var t in spikes)
            {
                if (moveInSequence)
                    sequence.Append(t.DOScale(finalScale, duration).SetEase(easeMode));
                else
                    sequence.Join(t.DOScale(finalScale, duration).SetEase(easeMode));
            }
            sequence.SetDelay(delayTime);
            sequence.AppendInterval(delayTime);
            sequence.SetLoops(-1, LoopType.Yoyo);
            sequence.Play();

        
    }
}
