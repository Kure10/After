using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 1. vytvori se nova notifikace
 2. instantiuje se prefab notifikace
 3. vyplnise podle vytvořene notifikace. (musi se urcit typ notifikace)

 4. Notifikace se nekde musí uložit aby je slo mazat atd.

 */

public class MissionNotificationManager : MonoBehaviour
{

    [SerializeField]
    private GameObject NotificationPrefab;

    [SerializeField]
    private Transform NotificationHolder;

    [SerializeField]
    private EventController eventController;

    //private List<Notification> notificationList = new List<Notification>();

    private Dictionary<GameObject, Notification> notificationDictionary = new Dictionary<GameObject, Notification>(); 


    // ToDo  je to teprve prvni pokus pro notificace
    public void CreateNewNotification(Mission currentMission)
    {
        Notification not = new Notification(NotificationType.Event, currentMission.Id);
                                                                     

        GameObject goNotification = Instantiate(NotificationPrefab, NotificationHolder.position, Quaternion.identity);
        uWindowMissionNotification windowSettings = goNotification.GetComponent<uWindowMissionNotification>();

        if (windowSettings == null)
            return;

        windowSettings.Tittle.text = not.tittle;
        windowSettings.SubTittle.text = not.subTittle;

        /* buttons */
        windowSettings.ButtonOne.onClick.AddListener(() => OpenEventPanel(currentMission.Id)) ;

        // Set parrent and Reset Scale.
        goNotification.transform.SetParent(NotificationHolder);
        goNotification.transform.localScale = new Vector3(1, 1, 1);

        notificationDictionary.Add(goNotification, not);

    }

    public void DestroyNotification(Mission mission)
    {
        GameObject foundedNotification = null;

        foreach (KeyValuePair<GameObject, Notification> item in notificationDictionary)
        {
            if (mission.Id == item.Value.id)
            {
                foundedNotification = item.Key;
                Destroy(item.Key);
            }
        }

        if (foundedNotification != null && notificationDictionary.ContainsKey(foundedNotification))
            notificationDictionary.Remove(foundedNotification);

    }

    /*Events*/

    public void OpenEventPanel(long missionID)
    {
        eventController.Maximaze();
    }

}

public enum NotificationType { Specialist , Event , Building} // jeden pro každy druh jine notifikace..


public class Notification
{
    #region Fields

    public long id;

    private NotificationType type;

    public string tittle;

    public string subTittle;

    #endregion

    #region Construktor
    public Notification(NotificationType type, long _id)
    {
        id = _id;

        switch (type)
        {
            case NotificationType.Building:
                tittle = "Building Finished";
                break;
            case NotificationType.Event:
                tittle = "New Event Triggered";
                break;
            case NotificationType.Specialist:
                tittle = "New Spec Added";
                break;
            default:
                Debug.Log("Defaultni state pro notification ERROR");
                break;
        }
    }
    #endregion
}