using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Agricultura {
    public class DroppedItem : MonoBehaviour {
        [SerializeField] private AnimationCurve dropCurve;

        public Action OnCompleted;

        private IEnumerator Start() {
            RectTransform rectTransform = GetComponent<RectTransform>();
            float randomHeightFactor = Random.Range(50, 150);
            float randomWidthFactor = Random.Range(25, 75);
            int sign = Random.value < 0.5f ? -1 : 1;
            Vector2 start = rectTransform.anchoredPosition + new Vector2(Random.Range(-10, 10), Random.Range(-25, 25));
            for (float time = 0; time < 1; time += Time.deltaTime) {
                rectTransform.anchoredPosition = start + new Vector2(time * randomWidthFactor * sign, dropCurve.Evaluate(time) * randomHeightFactor);
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(0.5f);
            OnCompleted?.Invoke();
            Destroy(gameObject);
        }
    }
}