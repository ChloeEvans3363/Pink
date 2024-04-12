using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAudioListener : MonoBehaviour
{
    private static MoveAudioListener instance;

    public static MoveAudioListener Instance { get { return instance; } }

    private float highestValue;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void MoveListener(Transform other)
    {
        Debug.Log("call");
        highestValue = 0;
        //if (other.tag != "Explosion") { return; }
        Vector3 currentPlayerPosition = GameManger.Instance.players[0].transform.position;

        for (int i = 0; i < GameManger.Instance.players.Count; i++)
        {
            float distance = Vector3.Distance(other.position,GameManger.Instance.players[i].transform.position);
            if (highestValue < distance) {
                highestValue = distance;
                currentPlayerPosition = GameManger.Instance.players[i].transform.position; 
            }
        }

        Debug.Log(currentPlayerPosition);
        this.transform.position = currentPlayerPosition;
        Debug.Log(this.gameObject.transform.position);
    }

}
