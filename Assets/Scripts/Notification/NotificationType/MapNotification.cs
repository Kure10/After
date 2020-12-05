using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Notification
{
    public class MapNotification : MonoBehaviour, INotification
    {

        [SerializeField] Text messageText;

        private RegionOperator regOperator;
        public RegionOperator RegOperator { get { return this.regOperator; } set { this.regOperator = value; } }

        private long id;

        public UnityAction<MapNotification> onDestroy;

        public string MainMessage { get => messageText.text; set => throw new System.NotImplementedException(); }

        public long ID { get { return id; } set { id = value; } }

        public void SetMessage(string message)
        {
            messageText.text = message;
        }

        // Animation -> after few sec destroy
        public void DestroyOnTime()
        {
            onDestroy.Invoke(this);
            Destroy(this.gameObject);
            
        }

        public void DestroyNow()
        {
            onDestroy.Invoke(this);
            Destroy(this.gameObject);
        }
    } 
}
