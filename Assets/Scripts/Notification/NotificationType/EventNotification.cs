using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Notification
{
    public class EventNotification : MonoBehaviour, INotification
    {
        [SerializeField]
        private Text tittle;

        [SerializeField]
        private Text buttonOneText;

        [SerializeField]
        private Text buttonTwoText;

        [Header("Buttons")]

        [SerializeField]
        private Button buttonOne;

        private long id;


        #region Properity

        public string Tittle { get { return tittle.text; } set { tittle.text = value; } }
        public Text ButtonOneText { get { return buttonOneText; } set { buttonOneText = value; } }
        public Button ButtonOne { get { return buttonOne; } set { buttonOne = value; } }

        public string MainMessage { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public long ID { get { return id; } set { id = value; } }

        #endregion

    }
}

