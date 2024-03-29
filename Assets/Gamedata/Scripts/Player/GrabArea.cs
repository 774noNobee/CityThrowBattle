using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        switch(other.tag)
        {
            case "Buildings":
                Grab.Instance.Targeting(other.gameObject);
                break;
            case "Enemy":
                if(other.GetComponent<EnemyStatus>().status.Type==Status.statusType.Defeat)
                {
                    Grab.Instance.Targeting(other.gameObject);
                }
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "Buildings":
                Grab.Instance.TargetOut(other.gameObject);
                break;
            case "Enemy":
                if (other.GetComponent<EnemyStatus>().status.Type == Status.statusType.Defeat)
                {
                    Grab.Instance.TargetOut(other.gameObject);
                }
                break;
        }
    }
}
