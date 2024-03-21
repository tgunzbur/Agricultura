using UnityEngine;

namespace Agricultura {
    public class ErrorUIManager : MonoSingleton<ErrorUIManager> {
        [SerializeField] private GameObject fullErrorPrefab;
        [SerializeField] private GameObject forbiddenErrorPrefab;
        [SerializeField] private GameObject noMoneyErrorPrefab;

        private void Awake() {
            SetInstance(this);
        }

        public void ShowForbiddenError(Transform target) {
            Instantiate(forbiddenErrorPrefab, target);
        }

        public void ShowFullError(Transform target) {
            Instantiate(fullErrorPrefab, target);
        }

        public void ShowNoMoneyError(Transform target) {
            Instantiate(noMoneyErrorPrefab, target);
        }
    }
}