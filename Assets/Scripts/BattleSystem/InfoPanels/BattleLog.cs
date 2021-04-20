using UnityEngine;
using UnityEngine.UI;

public class BattleLog : MonoBehaviour
{
    [SerializeField] Text _log;

    public void SetLog(string logInformation)
    {
        if(!this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(true);
        }

        _log.text = logInformation;
    }

}
