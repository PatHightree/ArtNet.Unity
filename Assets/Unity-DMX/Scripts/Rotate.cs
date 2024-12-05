using UnityEngine;

namespace Unity_DMX.Scripts
{
    public class Rotate : MonoBehaviour
    {
        public float RotationSpeed = 1f;

        private void Update()
        {
            transform.Rotate(Vector3.forward, RotationSpeed * Time.deltaTime);
        }
    }
}