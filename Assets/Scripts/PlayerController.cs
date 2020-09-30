using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Com.JaisonFontaine.SpacePilots {
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable {

        #region Private Fields

        private Vector3 playerPos;
        private float xPos;

        private Rigidbody rbBall;
        private Ball scriptBall;

        #endregion


        #region Public Fields

        public float paddleSpeed = 1f;
        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;
        public GameObject ball;
        public Vector3 target;

        public GameObject spawnBall;
        public GameObject cloneBall;

        public bool isReady = false;
        //public float timeReadyPlayer1 = 0.0001f;

        public bool ballInPlay = false;

        [Tooltip("The current Health of player")]
        public int livesPlayer = 3;

        #endregion


        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(isReady);
                stream.SendNext(ballInPlay);
            }
            else
            {
                // Network player, receive data
                this.isReady = (bool)stream.ReceiveNext();
                this.ballInPlay = (bool)stream.ReceiveNext();
            }
        }


        #endregion


        #region MonoBehaviour CallBacks

        void Awake() {
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.IsMine)
            {
                PlayerController.LocalPlayerInstance = this.gameObject;

                GetComponent<MeshRenderer>().material.color = Color.blue;

                if (PhotonNetwork.IsMasterClient) {
                    //Player Bas
                    Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else {
                    //Player Haut
                    Camera.main.transform.rotation = Quaternion.Euler(0, 0, 180);
                }
            }
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(this.gameObject);

            spawnBall = transform.Find("SpawnBall").gameObject;
        }

        void Start() {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2) {
                if (PhotonNetwork.IsMasterClient) {
                    GameManager.Instance.GetComponent<PhotonView>().RPC("RpcMajLife1", RpcTarget.All, PhotonNetwork.PhotonViews[PhotonNetwork.PhotonViews[1].OwnerActorNr].ViewID, false);
                    //Debug.Log("Player 1 : " + PhotonNetwork.PhotonViews[PhotonNetwork.PhotonViews[1].OwnerActorNr].ViewID);
                }
                else {
                    GameManager.Instance.GetComponent<PhotonView>().RPC("RpcMajLife2", RpcTarget.All, PhotonNetwork.PhotonViews[PhotonNetwork.PhotonViews[2].OwnerActorNr].ViewID, false);
                    //Debug.Log("Player 2 : " + PhotonNetwork.PhotonViews[PhotonNetwork.PhotonViews[2].OwnerActorNr].ViewID);
                }

                SpawnBall(2);

                GameManager.Instance.GetComponent<PhotonView>().RPC("RpcCountdown", RpcTarget.All);
            }
            else {
                GameManager.Instance.GetComponent<PhotonView>().RPC("RpcMajLife1", RpcTarget.All, PhotonNetwork.PhotonViews[PhotonNetwork.PhotonViews[1].OwnerActorNr].ViewID, false);
                SpawnBall(1);
                //Debug.Log("Player 1 : " + PhotonNetwork.PhotonViews[PhotonNetwork.PhotonViews[1].OwnerActorNr].ViewID);
            }
        }

        void FixedUpdate() {
            /*if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.PhotonViews[PhotonNetwork.PhotonViews[1].OwnerActorNr].GetComponent<PlayerController>().isReady && PhotonNetwork.PhotonViews[PhotonNetwork.PhotonViews[2].OwnerActorNr].GetComponent<PlayerController>().isReady && allReady == false) {
                allReady = true;
                //Debug.Log("Player 1 isReady : " + PhotonNetwork.PhotonViews[PhotonNetwork.PhotonViews[1].OwnerActorNr].GetComponent<PlayerController>().isReady);
                //Debug.Log("Player 2 isReady : " + PhotonNetwork.PhotonViews[PhotonNetwork.PhotonViews[2].OwnerActorNr].GetComponent<PlayerController>().isReady);
            }*/
        }

        void Update() {
            if (!photonView.IsMine && PhotonNetwork.IsConnected) {
                return;
            }

#if (UNITY_EDITOR || UNITY_STANDALONE)
            if (Input.mousePosition.x >= 0 && Input.mousePosition.x <= Screen.width) {
                if (PhotonNetwork.IsMasterClient) {
                    //Player Bas
                    //xPos = transform.position.x + (Input.GetAxis("Horizontal") * paddleSpeed);                    {
                    xPos = transform.position.x + (Input.GetAxis("Mouse X") * paddleSpeed);
                }
                else {
                    //Player Haut
                    //xPos = transform.position.x + (Input.GetAxis("Horizontal") * -paddleSpeed);
                    xPos = transform.position.x + (Input.GetAxis("Mouse X") * -paddleSpeed);
                }

                playerPos = new Vector3(Mathf.Clamp(xPos, -2.1f, 2.1f), transform.position.y, 0f);
                transform.position = playerPos;

                if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {
                    //if (Input.GetKeyDown(KeyCode.Space) && isReady == false) {
                    /*if (Input.GetButtonDown("Fire1") && isReady == false) {
                        SpawnBall(1);
                    }*/

                    //if (Input.GetKeyDown(KeyCode.Space) && isReady == true && ballInPlay == false) {
                    if (Input.GetButtonDown("Fire1") && isReady == true && ballInPlay == false) {
                        target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        ShootBall();
                    }
                }
                else if (PhotonNetwork.CurrentRoom.PlayerCount == 2) {
                    //if (Input.GetKeyDown(KeyCode.Space) && isReady == false) {
                    /*if (Input.GetButtonDown("Fire1") && isReady == false) {
                        SpawnBall(2);
                    }*/

                    //if (Input.GetKeyDown(KeyCode.Space) && allReady == true && ballInPlay == false) {
                    if (Input.GetButtonDown("Fire1") && isReady == true && GameManager.Instance.allReady == true && ballInPlay == false) {
                        target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        ShootBall();
                    }
                }

                /*//if (Input.GetKeyDown(KeyCode.Space) && isReady == false && PhotonNetwork.CurrentRoom.PlayerCount == 2) {
                //if (Input.GetKeyDown(KeyCode.Space) && isReady == false) {
                if (Input.GetButtonDown("Fire1") && isReady == false && PhotonNetwork.CurrentRoom.PlayerCount == 2) {
                //if (Input.GetButtonDown("Fire1") && isReady == false) {
                    SpawnBall();
                }

                //if (Input.GetKeyDown(KeyCode.Space) && allReady == true && ballInPlay == false) {
                //if (Input.GetKeyDown(KeyCode.Space) && isReady == true && ballInPlay == false) {
                if (Input.GetButtonDown("Fire1") && allReady == true && ballInPlay == false) {
                //if (Input.GetButtonDown("Fire1") && isReady == true && ballInPlay == false) {
                    target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    ShootBall();
                }*/
            }
#else
            /*Touch touch = Input.GetTouch(0);

            if (Input.touchCount == 1)
            {
                if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
                {
                    Vector3 touchedPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10));
                    xPos = transform.position.x + (touchedPos.x * paddleSpeed);
                    playerPos = new Vector3(Mathf.Clamp(xPos, -2.1f, 2.1f), transform.position.y, 0f);
                    transform.position = playerPos;
                }
            }

            if (Input.touchCount == 2 && ballInPlay == false)
            {
                CmdShootBall();
            }*/
#endif
        }

        #endregion

        public void SpawnBall(int playerCount) {
            /*if (playerCount == 1) {
                allReady = true;
            }*/

            isReady = true;

            ballInPlay = false;

            cloneBall =  PhotonNetwork.Instantiate(ball.name, spawnBall.transform.position, Quaternion.identity, 0) as GameObject;

            GetComponent<PhotonView>().RPC("RpcSetParent", RpcTarget.All, gameObject.GetPhotonView().ViewID, cloneBall.GetPhotonView().ViewID);

            scriptBall = cloneBall.GetComponent<Ball>();
            scriptBall.idPlayerBall = gameObject.GetPhotonView().ViewID;
        }

        void ShootBall() {
            ballInPlay = true;

            GetComponent<PhotonView>().RPC("RpcSetParent", RpcTarget.All, -1, cloneBall.GetPhotonView().ViewID);

            rbBall = cloneBall.GetComponent<Rigidbody>();
            rbBall.isKinematic = false;

            rbBall.velocity = new Vector3(target.x, target.y, 0) * 0.02f;

            /*if (PhotonNetwork.IsMasterClient) {
                //Player Bas
                //rbBall.AddForce(new Vector3(target.x, target.y, 0));
                rbBall.velocity = new Vector3(target.x, target.y, 0) * 0.02f;
            } else {
                //Player Haut
                //rbBall.AddForce(new Vector3(-target.x, -target.y, 0));
                rbBall.velocity = new Vector3(-target.x, -target.y, 0) * 0.02f;
            }*/
        }

        [PunRPC]
        void RpcSetParent(int idParent, int idChild) {
            if (idParent == -1) {
                PhotonView.Find(idChild).transform.parent = null;
            }
            else {
                PhotonView.Find(idChild).transform.parent = PhotonView.Find(idParent).transform;
            } 
        }
    }
}
