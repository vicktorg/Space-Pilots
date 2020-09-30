using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.JaisonFontaine.SpacePilots {
    public class DeadZoneUp : MonoBehaviourPunCallbacks {

        //private GameManager scriptGM;

        void Start() {
            //scriptGM = FindObjectOfType<GameManager>();
        }

        void OnTriggerEnter(Collider col) {
            GameManager.Instance.LoseLife2(col.gameObject, col.GetComponent<Ball>().idPlayerBall);
        }
    }
}
