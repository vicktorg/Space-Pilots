using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace Com.JaisonFontaine.SpacePilots
{
    public class GameManager : MonoBehaviourPunCallbacks, IPunObservable {

        #region Public Fields

        public static GameManager Instance;

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;
        public GameObject SpawnBas;
        public GameObject SpawnHaut;

        public GameObject bricks;
        public GameObject spawnBricks;
        public GameObject[] listBricks;
        public int nbBricks = 35;
        public float resetDelay = 1f;
        public bool allReady = false;

        #endregion


        #region Private Methods

        void Start() {
            Instance = this;

            if (playerPrefab == null) {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else {
                if (PlayerController.LocalPlayerInstance == null) {
                    if (PhotonNetwork.IsMasterClient) {
                        Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                        // we're in a room. spawn a character for the local player 1. it gets synced by using PhotonNetwork.Instantiate
                        PhotonNetwork.Instantiate(playerPrefab.name, SpawnBas.transform.position, SpawnBas.transform.rotation, 0);
                    }
                    else {
                        Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                        // we're in a room. spawn a character for the local player 2. it gets synced by using PhotonNetwork.Instantiate
                        PhotonNetwork.Instantiate(playerPrefab.name, SpawnHaut.transform.position, SpawnHaut.transform.rotation, 0);
                    }
                }
                else {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }  
            }
            

            if (PhotonNetwork.IsMasterClient) {
                //if (PhotonNetwork.CurrentRoom.PlayerCount == 2) {
                    GameObject cloneBricks = PhotonNetwork.Instantiate(bricks.name, spawnBricks.transform.position, Quaternion.identity, 0) as GameObject;

                    Transform[] spawnPoints = spawnBricks.GetComponentsInChildren<Transform>();

                    foreach (Transform spawnPoint in spawnPoints)
                    {
                        GameObject cloneBrick = PhotonNetwork.Instantiate(listBricks[Random.Range(0, listBricks.Length)].name, spawnPoint.transform.position, Quaternion.identity, 0) as GameObject;
                        GetComponent<PhotonView>().RPC("RpcSetParent", RpcTarget.All, cloneBricks.GetPhotonView().ViewID, cloneBrick.GetPhotonView().ViewID);
                    }
                //}
            }
        }

        void LoadGame() {
            if (!PhotonNetwork.IsMasterClient) {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            //PhotonNetwork.LoadLevel("Main");
            PhotonNetwork.LoadLevel("Main2");
        }


        #endregion


        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        void Reset()
        {
            //Time.timeScale = 1f;
            LeaveRoom();
        }


        #endregion


        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                //stream.SendNext(nbBricks);
            }
            else
            {
                // Network player, receive data
                //this.nbBricks = (int)stream.ReceiveNext();
            }
        }


        #endregion


        #region Photon Callbacks

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                LoadGame();
            }
        }


        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                //LoadGame();
                LeaveRoom();
            }
        }

        public void LoseLife1(GameObject ball, int idPlayerBall)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2) {
                GetComponent<PhotonView>().RPC("RpcMajLife1", RpcTarget.All, PhotonNetwork.PhotonViews[PhotonNetwork.PhotonViews[1].OwnerActorNr].ViewID, true);
                GetComponent<PhotonView>().RPC("RpcCheckGameOver1", RpcTarget.All, PhotonNetwork.PhotonViews[PhotonNetwork.PhotonViews[1].OwnerActorNr].GetComponent<PlayerController>().livesPlayer);
            }     

            GetComponent<PhotonView>().RPC("RpcDestroyBall", RpcTarget.All, ball.GetPhotonView().ViewID);

            PhotonView.Find(idPlayerBall).GetComponent<PlayerController>().SpawnBall(2);

            //Instantiate(deathParticles, transform.position, Quaternion.identity);
            //Invoke("SetupPaddle", resetDelay);

            
        }

        public void LoseLife2(GameObject ball, int idPlayerBall)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2) {
                GetComponent<PhotonView>().RPC("RpcMajLife2", RpcTarget.All, PhotonNetwork.PhotonViews[PhotonNetwork.PhotonViews[2].OwnerActorNr].ViewID, true);
                GetComponent<PhotonView>().RPC("RpcCheckGameOver2", RpcTarget.All, PhotonNetwork.PhotonViews[PhotonNetwork.PhotonViews[2].OwnerActorNr].GetComponent<PlayerController>().livesPlayer);
            }

            GetComponent<PhotonView>().RPC("RpcDestroyBall", RpcTarget.All, ball.GetPhotonView().ViewID);

            PhotonView.Find(idPlayerBall).GetComponent<PlayerController>().SpawnBall(2);

            //Instantiate(deathParticles, transform.position, Quaternion.identity);
            //Invoke("SetupPaddle", resetDelay);
        }

        [PunRPC]
        void RpcCheckGameOver1(int livesPlayer1)
        {
            if (livesPlayer1 < 1)
            {
                if (PhotonNetwork.IsMasterClient) {
                    //Player Bas
                    GameObject.Find("MsgEnd").GetComponent<Text>().text = "GameOver";
                    //GameObject.Find("Bricks").SetActive(false);
                }
                else {
                    //Player Haut
                    GameObject.Find("MsgEnd").GetComponent<Text>().text = "YouWon";
                }

                PlayerController[] players = GameObject.FindObjectsOfType<PlayerController>();

                foreach(PlayerController player in players) {
                    player.enabled = false;
                }

                //Time.timeScale = 0.25f;
                Invoke("Reset", resetDelay);
            }
        }

        [PunRPC]
        void RpcCheckGameOver2(int livesPlayer2)
        {
            if (livesPlayer2 < 1)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    //Player Bas
                    GameObject.Find("MsgEnd").GetComponent<Text>().text = "GameOver";
                    //GameObject.Find("Bricks").SetActive(false);
                }
                else
                {
                    //Player Haut
                    GameObject.Find("MsgEnd").GetComponent<Text>().text = "YouWon";
                }

                PlayerController[] players = GameObject.FindObjectsOfType<PlayerController>();

                foreach (PlayerController player in players) {
                    player.enabled = false;
                }

                //Time.timeScale = 0.25f;
                Invoke("Reset", resetDelay);
            }
        }

        [PunRPC]
        public void RpcMajLife1(int idPlayer1, bool isLoseLife1)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2) {
                if (isLoseLife1 && PhotonView.Find(idPlayer1).GetComponent<PlayerController>().livesPlayer > 0) {
                    PhotonView.Find(idPlayer1).GetComponent<PlayerController>().livesPlayer--;
                }

                if (PhotonNetwork.IsMasterClient) {
                    //Player Bas
                    GameObject.Find("LivesPlayer1").GetComponent<Text>().text = PhotonView.Find(idPlayer1).Owner.NickName + " : " + PhotonView.Find(idPlayer1).GetComponent<PlayerController>().livesPlayer;
                }
                else {
                    //Player Haut
                    GameObject.Find("LivesPlayer2").GetComponent<Text>().text = PhotonView.Find(idPlayer1).Owner.NickName + " : " + PhotonView.Find(idPlayer1).GetComponent<PlayerController>().livesPlayer;
                }
            }
            else {
                //Player Bas
                GameObject.Find("LivesPlayer1").GetComponent<Text>().text = PhotonView.Find(idPlayer1).Owner.NickName;
            }
        }

        [PunRPC]
        public void RpcMajLife2(int idPlayer2, bool isLoseLife2)
        {
            if (isLoseLife2 && PhotonView.Find(idPlayer2).GetComponent<PlayerController>().livesPlayer > 0)
            {
                PhotonView.Find(idPlayer2).GetComponent<PlayerController>().livesPlayer--;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                //Player Bas
                GameObject.Find("LivesPlayer2").GetComponent<Text>().text = PhotonView.Find(idPlayer2).Owner.NickName + " : " + PhotonView.Find(idPlayer2).GetComponent<PlayerController>().livesPlayer;
            }
            else
            {
                //Player Haut
                GameObject.Find("LivesPlayer1").GetComponent<Text>().text = PhotonView.Find(idPlayer2).Owner.NickName + " : " + PhotonView.Find(idPlayer2).GetComponent<PlayerController>().livesPlayer;
            }
        }

        IEnumerator CountdownReady()
        {
            Debug.Log("3");
            GameObject.Find("Countdown").GetComponent<Text>().text = "3";
            yield return new WaitForSeconds(1f);
            Debug.Log("2");
            GameObject.Find("Countdown").GetComponent<Text>().text = "2";
            yield return new WaitForSeconds(2f);
            Debug.Log("1");
            GameObject.Find("Countdown").GetComponent<Text>().text = "1";
            yield return new WaitForSeconds(3f);
            Debug.Log("Go!!!");
            GameObject.Find("Countdown").GetComponent<Text>().text = "Go!!!";
            allReady = true;
            yield return new WaitForSeconds(3.25f);
            GameObject.Find("Countdown").SetActive(false);
        }

        [PunRPC]
        public void RpcCountdown()
        {
            StartCoroutine(CountdownReady());
        }

        [PunRPC]
        void RpcDestroyBall(int idBall)
        {
            Destroy(PhotonView.Find(idBall).gameObject);
        }

        [PunRPC]
        void RpcSetParent(int idParent, int idChild)
        {
            PhotonView.Find(idChild).transform.parent = PhotonView.Find(idParent).transform;
        }

        [PunRPC]
        public void RpcDestroyBrick(int idBrick)
        {
            nbBricks--;
            Destroy(PhotonView.Find(idBrick).gameObject);
        }

        #endregion
    }
}
