using Game2048.Core;
using Game2048.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game2048.Input
{
    public sealed class KeyboardInputController : MonoBehaviour
    {
        [SerializeField] private BoardController boardController;

        private void Update()
        {
            if (boardController == null)
            {
                Debug.LogError($"KeyboardInputController on '{name}' requires a Board Controller reference in the Inspector.", this);
                enabled = false;
                return;
            }

            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.leftArrowKey.wasPressedThisFrame || keyboard.aKey.wasPressedThisFrame) boardController.TryMove(MoveDirection.Left);
            else if (keyboard.rightArrowKey.wasPressedThisFrame || keyboard.dKey.wasPressedThisFrame) boardController.TryMove(MoveDirection.Right);
            else if (keyboard.upArrowKey.wasPressedThisFrame || keyboard.wKey.wasPressedThisFrame) boardController.TryMove(MoveDirection.Up);
            else if (keyboard.downArrowKey.wasPressedThisFrame || keyboard.sKey.wasPressedThisFrame) boardController.TryMove(MoveDirection.Down);
        }
    }
}
