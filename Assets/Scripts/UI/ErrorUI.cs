using System.Collections;
using UnityEngine;

namespace Agricultura {
    public class ErrorUI : MonoBehaviour {
        [SerializeField] private AnimationCurve floatingCurve;
        private const float duration = 0.75f;

        private IEnumerator Start() {
            float time = 0;
            RectTransform rectTransform = GetComponent<RectTransform>();
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            Vector2 start = rectTransform.anchoredPosition;
            while (time < duration) {
                rectTransform.anchoredPosition = start + Vector2.up * floatingCurve.Evaluate(time / duration) * 50;
                canvasGroup.alpha = 1 - (time / duration);
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            Destroy(gameObject);
        }
    }
}