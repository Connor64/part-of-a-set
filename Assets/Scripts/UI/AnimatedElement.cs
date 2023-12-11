using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimatedElement : MonoBehaviour {

    [SerializeField]
    private Transform startTransform, endTransform;

    [SerializeField]
    private float inDuration, outDuration;

    private bool atStart = true;
    private bool isAnimating = false;

    void Start() {
        transform.position = startTransform.position;
    }

    public void TogglePosition() {
        if (isAnimating) {
            print("animating >:(");
            return;
        }

        if (atStart) {
            StartCoroutine(MoveObject(endTransform.position, true, inDuration));
            atStart = false;
        } else {
            StartCoroutine(MoveObject(startTransform.position, false, outDuration));
            atStart = true;
        }
    }

    private IEnumerator MoveObject(Vector3 targetPos, bool enter, float duration) {
        isAnimating = true;

        Vector3 oldPos = transform.position;

        float elapsedTime = 0;
        while (elapsedTime < duration) {
            float ratio = elapsedTime / duration;
            transform.position = Vector3.Lerp(oldPos, targetPos, enter ? easeOutQuint(ratio) : easeInQuint(ratio));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        isAnimating = false;
    }

    private float easeOutQuint(float x) {
        return 1f - Mathf.Pow(1f - x, 5f);
    }

    private float easeInQuint(float x) {
        return x * x * x * x * x;
    }
}
