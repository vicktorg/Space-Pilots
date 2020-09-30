using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.JaisonFontaine.SpacePilots {
    public class Bricks : MonoBehaviourPunCallbacks {

        public GameObject brickParticle;

        //private GameManager scriptGM;

        void Start() {
            //scriptGM = FindObjectOfType<GameManager>();
        }

        void OnCollisionEnter(Collision other) {
            //Instantiate(brickParticle, transform.position, Quaternion.identity);
            GameManager.Instance.GetComponent<PhotonView>().RPC("RpcDestroyBrick", RpcTarget.All, gameObject.GetPhotonView().ViewID);
        }
    }
}
