using System.Collections.Generic;
using UnityEngine;

namespace MouseCursor
{
    public class CursorRenderer : MonoBehaviour
    {
        [SerializeField] List<CursorType> _cursorType;

        private GameState _currentGameState = GameState.standart;
        private CursorType _currentCursorType = null;
        // For Animation Cursor
        private int currentFrame;
        private float frameTimer;

        private void Awake()
        {
            _currentCursorType = _cursorType[0];
            Cursor.SetCursor(_currentCursorType.Textures[0], new Vector2(10, 10), CursorMode.Auto);
        }

        void Update()
        {
            GameState gameState = ChangeCurrentGameState();

            if(gameState != _currentGameState)
            {
                _currentGameState = gameState;
                _currentCursorType = GetCursorType();

                if(!_currentCursorType.IsAnimated && _currentCursorType.Textures.Count > 0)
                {
                    Cursor.SetCursor(_currentCursorType.Textures[0], new Vector2(10, 10), CursorMode.Auto);
                }
            }

            if (_currentCursorType.IsAnimated)
                AnimateCursor();
        }

        private void AnimateCursor()
        {
            frameTimer -= Time.deltaTime;
            if (frameTimer <= 0f)
            {
                frameTimer += _currentCursorType.FrameRate;
                currentFrame = (currentFrame + 1) % (_currentCursorType.Textures.Count);
                Debug.Log(currentFrame + "  currentFrame ..");
                Cursor.SetCursor(_currentCursorType.Textures[currentFrame], new Vector2(10, 10), CursorMode.Auto);
            }
        }

        private GameState ChangeCurrentGameState()
        {
            if (BattleController.IsBattleAlive)
            {
                return GameState.isInCombat;
            }
            else if (DragAndDropManager.IsDraging)
            {
                return GameState.isDraging;
            }
            else
            {
                return GameState.standart;
            }
        }

        private CursorType GetCursorType()
        {
            foreach (CursorType type in _cursorType)
            {
                if(_currentGameState == type.Style)
                    return type;
            }
            return _cursorType[0];
        }

        public enum GameState
        {
            standart,
            isDraging,
            isInCombat,
        }
    }
}