using Game2048.Core;
using Game2048.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game2048.Input
{
    public sealed class SwipeInputController : MonoBehaviour
    {
        [SerializeField] private BoardController boardController;
        [SerializeField, Range(0.01f, 0.5f)] private float minimumSwipeDistanceRatio = 0.08f;
        private Vector2 startPosition;
        private bool trackingPointer;

        private void Update()
        {
            Pointer pointer = Pointer.current;
            if (pointer == null || boardController == null) return;
            if (pointer.press.wasPressedThisFrame)
            {
                startPosition = pointer.position.ReadValue();
                trackingPointer = true;
            }
            else if (trackingPointer && pointer.press.wasReleasedThisFrame)
            {
                trackingPointer = false;
                HandleSwipe(pointer.position.ReadValue() - startPosition);
            }
        }

        private void HandleSwipe(Vector2 delta)
        {
            if (delta.magnitude < Mathf.Min(Screen.width, Screen.height) * minimumSwipeDistanceRatio) return;
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                boardController.TryMove(delta.x > 0f ? MoveDirection.Right : MoveDirection.Left);
            else
                boardController.TryMove(delta.y > 0f ? MoveDirection.Up : MoveDirection.Down);
        }
    }
}
