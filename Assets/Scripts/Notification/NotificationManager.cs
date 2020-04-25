using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 1. vytvori se nova notifikace
 2. instantiuje se prefab notifikace
 3. vyplnise podle vytvořene notifikace. (musi se urcit typ notifikace)

 4. Notifikace se nekde musí uložit aby je slo mazat atd.

 */

public class NotificationManager : MonoBehaviour
{

    [SerializeField]
    private GameObject NotificationPrefab;

    [SerializeField]
    private Transform NotificationHolder;

    private List<Notification> impendingNotification = new List<Notification>();


    public void CreateNewNotification (EventBlueprint currentEvent)
    {
        Notification not = new Notification(NotificationType.Event); // tohle je huste nedodelane..
        not.subTittle = currentEvent.name;

        GameObject notification = Instantiate(NotificationPrefab, NotificationHolder.position, Quaternion.identity);
        var windowSettings = notification.GetComponent<uWindowNotification>();

        windowSettings.Tittle.text = not.tittle;
        windowSettings.SubTittle.text = not.subTittle;

        // Set parrent and Reset Scale.
        notification.transform.SetParent(NotificationHolder);
        notification.transform.localScale = new Vector3(1, 1, 1);


    }


    private void Start()
    {

    }

}

public enum NotificationType { Specialist , Event , Building} // jeden pro každy druh jine notifikace..


public class Notification
{
    private bool isBlockingTime;

    private NotificationType type;

    public string tittle;

    public string subTittle;

    public Notification(NotificationType type)
    {
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
}