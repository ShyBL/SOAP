using System.Collections;
using UnityEngine;

namespace SOAP
{
    public class TransformAction : MonoBehaviour
    {
        [Header("Movement Settings")]
        public GameObject objectToMove;
        
        [Tooltip("The offset amount to move by (e.g., Y: 5 moves the object 5 units up).")]
        public Vector3 targetOffset;

        public float duration = 1f;
        
        private Vector3 startPosition;
        private Coroutine activeMove;

        private void Awake() 
        {
            if (objectToMove != null)
            {
                startPosition = objectToMove.transform.position;
            }
        }

        // Designers hook these into the CollisionHandler's onEnter and onExit events
        public void MoveToTarget()  => StartMovement(startPosition + targetOffset);
        public void ReturnToStart() => StartMovement(startPosition);

        private void StartMovement(Vector3 destination)
        {
            if (objectToMove == null) return;
            
            if (activeMove != null) StopCoroutine(activeMove);
            activeMove = StartCoroutine(MoveRoutine(destination));
        }

        private IEnumerator MoveRoutine(Vector3 destination)
        {
            float time = 0;
            
            Vector3 start = objectToMove.transform.position; 
            
            while (time < duration)
            {
                objectToMove.transform.position = Vector3.Lerp(start, destination, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            
            objectToMove.transform.position = destination;
        }
    }
}