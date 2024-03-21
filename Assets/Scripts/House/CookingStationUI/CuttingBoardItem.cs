using System;
using UnityEngine;

namespace Agricultura {
    public class CuttingBoardItem : MonoBehaviour {
        private Vector3 speed;

        public void SetSpeed(Vector3 newSpeed) {
            speed = newSpeed;
        }

        private void Update() {
            transform.position += speed * Time.deltaTime;
        }
    }
}