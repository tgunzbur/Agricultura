using UnityEngine;

namespace Agricultura {
    public class MixerBlade : MonoBehaviour {
        [SerializeField] private MixerUI mixerUI;

        private void OnTriggerEnter2D(Collider2D other) {
            SendUp(other);
        }

        private void OnTriggerStay2D(Collider2D other) {
            SendUp(other);
        }

        private void SendUp(Collider2D item) {
            if (item.TryGetComponent(out Rigidbody2D rb)) {
                mixerUI.OnItemHitBlade(rb);
            }
        }
    }
}