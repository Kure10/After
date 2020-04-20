using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EventPanel))]
public class EventPanelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EventPanel eventPanel = (EventPanel)target;

        if (eventPanel.FontSize < 0)
            eventPanel.FontSize = 0;
        if (eventPanel.FontSize > 65)
            eventPanel.FontSize = 65;

        eventPanel.TitleField.fontSize = eventPanel.NameFontSize;
        eventPanel.DescriptionTextField.fontSize = eventPanel.FontSize;

        eventPanel.DescriptionTextField.text = eventPanel.Description;
        eventPanel.TitleField.text = eventPanel.EventName;

    }



}
