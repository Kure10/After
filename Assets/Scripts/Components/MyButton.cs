using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Audio;
using System;

public class MyButton : Button
{

    [SerializeField] Audio.AudioType audioType = Audio.AudioType.BTN_01;

    private SFXEvent audioEvent = default;
    private SelectionState _lastState = SelectionState.Normal;

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        if (IsInteractable() && _lastState != state)
        {
            audioEvent = SFXEvent.NoEvent;

            Debug.Log("State-----:  " + state);

            switch (state)
            {
                case SelectionState.Selected:
                case SelectionState.Highlighted:
                    audioEvent = Audio.SFXEvent.ButtonHover;
                    break;
                //case SelectionState.Normal:
                //    break;
                case SelectionState.Pressed:
                    audioEvent = Audio.SFXEvent.ButtonClick;
                    break;
                //case SelectionState.Disabled:
                //    break;
            }

            if (AudioManager.instance != null && audioEvent != SFXEvent.NoEvent)
                AudioManager.instance.PlayAudio(audioType, false, SFX_Event: audioEvent);
        }

        base.DoStateTransition(state, instant);

        _lastState = state;
    }
}
