using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StrengthBarHandler : MonoBehaviour
{
    public Slider strengthBar;
    public CanvasGroup canvasGroup;
    private float maxAlpha, _curAlpha;
    public WeaponManager weaponManager;
    public float maxStrength;
    private float _curStrength;
    public Tween fadeTween;
    

    private void Start()
    {
        _curStrength = 0;
        strengthBar.value = 0;
        ResetBar();
    }

    public void FadeIn(float duration)
    {
        Fade(1f, duration, () =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        });
    }
    public IEnumerator FadeOut(float duration, float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        Fade(0f, duration, () =>
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        });
        yield return null;
    }

    private void Fade(float end, float duration, TweenCallback onEnd)
    {
        if(fadeTween != null)
            fadeTween.Kill(false);
        fadeTween = canvasGroup.DOFade(end, duration);
        fadeTween.onComplete += onEnd;
    }


    public void UpdateBar(float amount)
    {
        Debug.Log("Updating Bar");
        if (_curStrength < maxStrength)
        {
            _curStrength += (amount / 10f);
            strengthBar.value = _curStrength;
        }
        else
        {
            Debug.Log("Max Strength!");
        }
    }

    public void ResetBar()
    {
        _curStrength = 0;
        strengthBar.value = 0;
        strengthBar.maxValue = maxStrength;
    }
}