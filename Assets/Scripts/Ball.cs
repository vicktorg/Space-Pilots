using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Com.JaisonFontaine.SpacePilots
{
    public class Ball : MonoBehaviourPunCallbacks, IPunObservable {

        #region Public Fields

        public float ballInitialVelocity = 400f;
        public int idPlayerBall;
        private Rigidbody rbBall;

        #endregion


        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(idPlayerBall);
            }
            else
            {
                // Network player, receive data
                this.idPlayerBall = (int)stream.ReceiveNext();
            }
        }

        #endregion


        #region MonoBehaviour CallBacks

        void Awake() {

        }

        void Start() {
            rbBall = GetComponent<Rigidbody>();
        }

        void Update() {
        
        }

        void OnCollisionEnter(Collision other) {
            if (other.transform.tag == "Player") {
                ContactPoint contact = other.contacts[0];

                Debug.Log("contact : " + contact.point.x);
                Debug.Log("position : " + other.transform.position.x);

                if (contact.point.x < other.transform.position.x) {
                    //Debug.Log("droite");

                    //rbBall.velocity = new Vector3(-ballInitialVelocity, ballInitialVelocity, 0) * 0.02f;

                    if (PhotonNetwork.IsMasterClient) {
                        //Player Bas
                        rbBall.velocity = new Vector3(-ballInitialVelocity, ballInitialVelocity, 0) * 0.02f;
                    } else {
                        //Player Haut
                        rbBall.velocity = new Vector3(ballInitialVelocity, -ballInitialVelocity, 0) * 0.02f;
                    }
                }
                else if (contact.point.x > other.transform.position.x) {
                    //Debug.Log("gauche");

                    //rbBall.velocity = new Vector3(ballInitialVelocity, ballInitialVelocity, 0) * 0.02f;

                    if (PhotonNetwork.IsMasterClient) {
                        //Player Bas
                        rbBall.velocity = new Vector3(ballInitialVelocity, ballInitialVelocity, 0) * 0.02f;
                    }
                    else {
                        //Player Haut
                        rbBall.velocity = new Vector3(-ballInitialVelocity, -ballInitialVelocity, 0) * 0.02f;
                    }
                }
                else {
                    //Debug.Log("milieu");

                    rbBall.velocity = new Vector3(0, 0, 0) * 0.2f;
                }
            }
        }

        #endregion
    }
}
