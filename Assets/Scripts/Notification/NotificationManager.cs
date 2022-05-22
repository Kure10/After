using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Notification
{
    public class NotificationManager : MonoBehaviour
    {


        [SerializeField] private EventController eventController;

        [Header("Prefabs")]
        [SerializeField] private GameObject eventNotificationPrefab;

        [SerializeField] private GameObject mapNotificationPrefab;

        [Header("Holders")]
        [SerializeField] private Transform eventNotificationHolder;

        [SerializeField] private Transform mapNotificationHolder;

        [Header("Others")]

        public List<MapNotification> mapNotifications = new List<MapNotification>();

        private Dictionary<GameObject, INotification> notificationDictionary = new Dictionary<GameObject, INotification>();

        private int lastID = 0;

        #region Singleton
        private static NotificationManager _instantion;
        public static NotificationManager Instantion
        {
            get
            {
                if (_instantion == null)
                {
                    GameObject go = new GameObject("NotificationManager");
                    go.AddComponent<NotificationManager>();
                }

                return _instantion;
            }
        }
        private void Awake()
        {
            _instantion = this;
        }

        #endregion

        #region Public Methods
        public void AddNotification<T>(T notInformation) where T : INotifiable
        {
            if (notInformation is Mission missionInfo)
            {
                CreateEventNotification(missionInfo);
            }
            else if (notInformation is RegionOperator regionInfo)
            {
                CreateMapNotification(regionInfo);
            }
            else
            {
                Debug.Log("unknow notification");
            }
        }

        public void DestroyNotification(long id)
        {
            GameObject foundedNotification = null;

            foreach (KeyValuePair<GameObject, INotification> item in notificationDictionary)
            {
                if (id == item.Value.ID)
                {
                    foundedNotification = item.Key;
                    Destroy(item.Key);
                }
            }

            if (foundedNotification != null && notificationDictionary.ContainsKey(foundedNotification))
                notificationDictionary.Remove(foundedNotification);

        }

        #endregion

        #region Private Methods
        private void CreateEventNotification(Mission mission)
        {
            GameObject instance = Instantiate(eventNotificationPrefab);
            EventNotification not = instance.GetComponent<EventNotification>();

            if (not == null)
            {
                Debug.LogError("Even Notification Error script is null");
                return;
            }


            not.ID = mission.Id;
            not.Tittle = "Event Triggered";
            not.ButtonOneText.text = "Open Event";

            /* buttons */
            not.ButtonOne.onClick.AddListener(() => OpenEventPanel());

            // Set parrent and Reset Scale.
            instance.transform.SetParent(eventNotificationHolder);
            instance.transform.localScale = new Vector3(1, 1, 1);

            notificationDictionary.Add(instance, not);

        }

        private void CreateMapNotification(RegionOperator regionOperator)
        {
            
            // max mapNotification check on 4
            if (mapNotifications.Count > 4)
            {
                MapNotification tmp = mapNotifications[0];
                mapNotifications.RemoveAt(0);
                tmp.DestroyNow();
            }

            // check for duplicity notification -> jestli tam je smaž ji a instantni novou. 
            // možna by bylo lepsi do budoucna proste aktualizovat puvodni..
            MapNotification notToDestroy = null;
            foreach (MapNotification item in mapNotifications)
            {
                if (item.ID == regionOperator.Region.Id)
                    notToDestroy = item;
            }

            if(notToDestroy != null)
                notToDestroy.DestroyNow();

            GameObject instance = Instantiate(mapNotificationPrefab, this.transform.position, Quaternion.identity);
            MapNotification mapNotification = instance.GetComponent<MapNotification>();

            mapNotification.ID = regionOperator.Region.Id;

            if (regionOperator.Region.IsInDarkness)
            {
                mapNotification.SetMessage("Region " + regionOperator.Region.RegionName + " neni odhalen.");
            }
            else if (regionOperator.Region.IsInShadow)
            {
                mapNotification.SetMessage("Mise nepřístupná. Ještě je třeba splnit " + regionOperator.Region.MissCompReq + " misí");
            }

            mapNotification.onDestroy += RemoveMapNotification;

            instance.transform.SetParent(mapNotificationHolder);
            mapNotifications.Add(mapNotification);

        }

        private void RemoveMapNotification(MapNotification current)
        {
            current.onDestroy -= RemoveMapNotification;
            mapNotifications.Remove(current);
        }

        private void OpenEventPanel()
        {
            eventController.Maximaze();
        }

        #endregion
    }
}





