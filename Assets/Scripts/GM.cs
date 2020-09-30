using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class GM : NetworkBehaviour {

    [SyncVar] public int livesPlayer1 = 3;
    [SyncVar] public int livesPlayer2 = 3;
    [SyncVar] public int nbBricks = 20;
    public float resetDelay = 1f;

    public GameObject bricks;
    public GameObject spawnBricks;
    public GameObject[] listBricks;

    public GameObject deathParticles;

    //public PlayerController[] listPlayer;

    void Start() {
        
    }

    void FixedUpdate() {
        //listPlayer = FindObjectsOfType<PlayerController>();

        /*if (!GameObject.FindWithTag("Bricks") && listPlayer.Length == 2) {
            GameObject cloneBricks = Instantiate(bricks, spawnBricks.transform.position, Quaternion.identity) as GameObject;
            NetworkServer.Spawn(cloneBricks);

            Transform[] spawnPoints = spawnBricks.GetComponentsInChildren<Transform>();

            foreach (Transform spawnPoint in spawnPoints)
            {
                GameObject cloneBrick = Instantiate(listBricks[Random.Range(0, listBricks.Length)], spawnPoint.position, Quaternion.identity) as GameObject;
                NetworkServer.Spawn(cloneBrick);
                cloneBrick.transform.SetParent(cloneBricks.transform);
            }
        }*/
    }

    public void Setup() {
        /*clonePaddle = Instantiate(paddle, transform.position, Quaternion.identity) as GameObject;
        Instantiate(bricksPrefab, transform.position, Quaternion.identity);*/
    }

    void Reset() {
        /*Time.timeScale = 1f;
        SceneManager.LoadScene(0);*/
    }

    [Command]
    public void CmdLoseLife1(bool playerUp) {
        livesPlayer1--;
        RpcMajLife1(livesPlayer1);

        /*foreach (PlayerController player in listPlayer) {
            if (!player.isSpawnUp && !playerUp) {
                Destroy(player.cloneBall);
                player.CmdSpawnBall();
            }

            if (player.isSpawnUp && playerUp) {
                Destroy(player.cloneBall);
                player.CmdSpawnBall();
            }
        }*/

        //Instantiate(deathParticles, transform.position, Quaternion.identity);
        //Invoke("SetupPaddle", resetDelay);
       
        CmdCheckGameOver1();
    }

    [Command]
    public void CmdLoseLife2(bool playerUp) {
        livesPlayer2--;
        RpcMajLife2(livesPlayer2);

        /*foreach (PlayerController player in listPlayer) {
            if (player.isSpawnUp && playerUp) {
                Destroy(player.cloneBall);
                player.CmdSpawnBall();
            }

            if (!player.isSpawnUp && !playerUp) {
                Destroy(player.cloneBall);
                player.CmdSpawnBall();
            }
        }*/

        //Instantiate(deathParticles, transform.position, Quaternion.identity);
        //Invoke("SetupPaddle", resetDelay);

        CmdCheckGameOver2();
    }

    [Command]
    void CmdCheckGameOver1() {
        /*if (nbBricks < 1)
        {
            youWon.SetActive(true);
            Time.timeScale = .25f;
            Invoke("Reset", resetDelay);
        }*/

        if (livesPlayer1 < 1) {
            livesPlayer1 = 0;
            GameObject.Find("GameOver").SetActive(true);
            //gameOver.SetActive(true);
            Time.timeScale = .25f;
            //Invoke("Reset", resetDelay);
        }

        if (livesPlayer2 < 1) {
            livesPlayer2 = 0;
            GameObject.Find("YouWon").SetActive(true);
            //youWon.SetActive(true);
            Time.timeScale = .25f;
            //Invoke("Reset", resetDelay);
        }
    }

    [Command]
    void CmdCheckGameOver2() {
        /*if (nbBricks < 1)
        {
            youWon.SetActive(true);
            Time.timeScale = .25f;
            Invoke("Reset", resetDelay);
        }*/

        if (livesPlayer1 < 1)
        {
            livesPlayer1 = 0;
            GameObject.Find("YouWon").SetActive(true);
            //youWon.SetActive(true);
            Time.timeScale = .25f;
            //Invoke("Reset", resetDelay);
        }

        if (livesPlayer2 < 1) {
            livesPlayer2 = 0;
            GameObject.Find("GameOver").SetActive(true);
            //gameOver.SetActive(true);
            Time.timeScale = .25f;
            //Invoke("Reset", resetDelay);
        }
    }

    [ClientRpc]
    public void RpcMajLife1(int live1) {
        GameObject.Find("LivesPlayer1").GetComponent<Text>().text = "Lives Player 1 : " + live1;
    }

    [ClientRpc]
    public void RpcMajLife2(int live2) {
        GameObject.Find("LivesPlayer2").GetComponent<Text>().text = "Lives Player 2 : " + live2;
    }

    void SetupPaddle()
    {
        //clonePaddle = Instantiate(paddle, transform.position, Quaternion.identity) as GameObject;
    }

    [Command]
    public void CmdDestroyBrick() {
        nbBricks--;
    }
}
