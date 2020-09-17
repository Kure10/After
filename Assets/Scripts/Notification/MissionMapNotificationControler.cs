using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionMapNotificationControler : MonoBehaviour
{
    public List<MapEventNotification> Notifications = new List<MapEventNotification>();

    [SerializeField] private GameObject eventNotification;

    public void AddNotification(RegionOperator reg)
    {

        foreach (var item in Notifications)
            if(item.RegOperator == reg)
                return;

        GameObject notification = Instantiate(this.eventNotification, this.transform.position, Quaternion.identity);
        var not = notification.GetComponent<MapEventNotification>();
        not.SetMessage("Mise nepřístupná. Ještě je třeba splnit " + reg.Region.MissCompReq + " misí");
        notification.transform.SetParent(this.gameObject.transform);
        Notifications.Add(not);
    }


}
