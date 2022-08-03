using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MouseCursor.CursorRenderer;

namespace MouseCursor
{

    [CreateAssetMenu(menuName = "ScriptableObject/CursorSetting", fileName = "_NewCursorSettings")]
    public class CursorType : ScriptableObject
    {

        public List<Texture2D> Textures = null;
        public float FrameRate;
        public bool IsAnimated = false;
        public GameState Style = GameState.standart;
    }

}
