using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimatedElement : MonoBehaviour {

    public enum AnimationCurveType {
        EASE_IN_QUINT,
        EASE_OUT_QUINT
    }

    public enum AnimationElementPosition {
        START_POSITION,
        END_POSITION
    }

    [SerializeField]
    private AnimationCurveType inCurve, outCurve;

    [SerializeField]
    private Transform startPosition, endPosition;

    [SerializeField]
    private float inDuration, outDuration;

    public bool deactivateAtStartPosition = true;
    public bool deactivateAtEndPosition = false;

    private Transform currentPosition;
    public bool isAnimating { get; private set; }

    private delegate float CurveDelegate(float x);

    private Dictionary<AnimationCurveType, CurveDelegate> curves;
    private float defaultDeltaTime;

    void Awake() {
        transform.position = startPosition.position;
        currentPosition = startPosition;
        isAnimating = false;
        curves = new Dictionary<AnimationCurveType, CurveDelegate> {
            { AnimationCurveType.EASE_IN_QUINT, easeInQuint },
            { AnimationCurveType.EASE_OUT_QUINT, easeOutQuint }
        };
    }

    void Start() {
        defaultDeltaTime = Time.deltaTime;
        transform.position = startPosition.position;
        currentPosition = startPosition;
    }

    public void TogglePosition(bool animate) {
        if (!animate) {
            if (isAnimating) {
                StopAllCoroutines();
                isAnimating = false;

                // If currently animating, set position to same as current position
                currentPosition = (currentPosition == startPosition) ? startPosition : endPosition;
                transform.position = currentPosition.position;
            } else {

                // If not currently animating, set position to opposite of current position
                currentPosition = (currentPosition == startPosition) ? endPosition : endPosition;
                transform.position = currentPosition.position;
            }

            gameObject.SetActive((currentPosition == startPosition) ? !deactivateAtStartPosition : !deactivateAtEndPosition);

            return;
        }

        if (isAnimating) return;

        if (currentPosition == startPosition) {
            print("at start!!!");
            gameObject.SetActive(true);
            StartCoroutine(MoveObject(endPosition, true, inDuration));
        } else {
            print("at end!!!");
            gameObject.SetActive(true);
            StartCoroutine(MoveObject(startPosition, false, outDuration));
        }
    }

    public void SetPosition(bool animate, AnimationElementPosition pos) {
        if (!animate) {
            if (isAnimating) {
                StopAllCoroutines();
                isAnimating = false;
            }

            if (pos == AnimationElementPosition.START_POSITION) {
                currentPosition = startPosition;
                gameObject.SetActive(!deactivateAtStartPosition);
            } else {
                currentPosition = endPosition;
                gameObject.SetActive(!deactivateAtEndPosition);
            }

            transform.position = currentPosition.position;

            return;
        }

        if (isAnimating) return;

        if (pos == AnimationElementPosition.START_POSITION) {
            gameObject.SetActive(deactivateAtEndPosition);
            StartCoroutine(MoveObject(startPosition, false, outDuration));
        } else {
            gameObject.SetActive(deactivateAtStartPosition);
            StartCoroutine(MoveObject(endPosition, true, inDuration));
        }
    }

    private IEnumerator MoveObject(Transform target, bool enter, float duration) {
        isAnimating = true;

        Vector3 oldPos = transform.position;

        float elapsedTime = 0;
        while (elapsedTime < duration) {
            float ratio = elapsedTime / duration;
            transform.position = Vector3.Lerp(oldPos, target.position, curves[enter ? inCurve : outCurve](ratio));
            elapsedTime += Time.timeScale == 0 ? defaultDeltaTime : Time.deltaTime;
            yield return null;
        }

        transform.position = target.position;
        currentPosition = target;
        isAnimating = false;
        gameObject.SetActive(enter ? !deactivateAtEndPosition : !deactivateAtStartPosition);
    }

    private float easeOutQuint(float x) {
        return 1f - Mathf.Pow(1f - x, 5f);
    }

    private float easeInQuint(float x) {
        return x * x * x * x * x;
    }
}
