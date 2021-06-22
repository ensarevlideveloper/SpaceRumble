using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceRumble.Networking;
using SpaceRumble.UI;
using SpaceRumble.Utility;
using UnityStandardAssets.CrossPlatformInput;


namespace SpaceRumble.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("Data")]
        
        public int Health = 100;
        [SerializeField]
        private int Score;
        [SerializeField]
        private float maxSpeed = 10f;
        [SerializeField]
        private float rotSpeed = 180f;
        [SerializeField]
        private GameObject MainCamera;
        [SerializeField]
        private GameObject ShootButton;
        [SerializeField]
        private GameObject playerCanvas;
        [SerializeField]
        private Transform bulletSpawnPoint;



        [Header("Class References")]
        [SerializeField]
        private NetworkIdentity networkIdentity;
        public AudioSource laserSFX;

        private BulletData bulletData;
        private string username;


        public Joystick joystick;

        // Use this for initialization
        private void Start()
        {
            Debug.Log("im spawned:"+username);
            playerCanvas = GameObject.Find("[Canvas]");
            joystick = FindObjectOfType<Joystick>();
            if (networkIdentity.IsLocalPlayer()) {
                ShootButton = GameObject.Find("ShootButton");
                this.SetHealth(100);
                MainCamera = GameObject.Find("Main Camera");
                MainCamera.GetComponent<Camera>().transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -10);
                bulletData = new BulletData();
                bulletData.position = new Position();
                bulletData.direction = new Position();

            }
            this.initPlayerScore();

            //SetScore(this.Score);
        }

        // Update is called once per frame
        private void Update()
        {
            if (networkIdentity.IsLocalPlayer()) {
                checkMovement();
                checkShooting();
            }
        }

        public void Respawned () {
            this.SetScore(0);
            this.SetHealth(100);
            this.initPlayerScore();

        }

        public void initPlayerScore() {
            Debug.Log("initplayerScore");
            playerCanvas.GetComponent<PlayerUI>().addPlayer(this);
            playerCanvas.GetComponent<PlayerUI>().updateScoretable();
        }
        public void abortPlayerScore() {
            Debug.Log("abortPlayerScore");
            playerCanvas.GetComponent<PlayerUI>().removePlayer(this);
            playerCanvas.GetComponent<PlayerUI>().updateScoretable();
        }

        private void checkMovement () {
            Quaternion rot = transform.rotation;

            float z = rot.eulerAngles.z;


            // z = z - (CrossPlatformInputManager.GetAxis("Horizontal") * rotSpeed * Time.deltaTime);
            // z = z - (Input.GetAxis("Horizontal") * rotSpeed * Time.deltaTime);

            // z = z - (joystick.Horizontal * rotSpeed * Time.deltaTime);
            // z = z - (joystick.Horizontal * rotSpeed * Time.deltaTime);

            //rot = Quaternion.Euler(0, 0, z);

            //transform.rotation = rot ;
            // float heading = -Mathf.Atan2(joystick.Horizontal, joystick.Vertical);

            //  transform.rotation = Quaternion.Euler(0f, 0f, heading * Mathf.Rad2Deg * rotSpeed);
            float angle = Mathf.Atan2(-joystick.Horizontal, joystick.Vertical) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

            // transform.Translate(new Vector3(0, CrossPlatformInputManager.GetAxis("Vertical") * maxSpeed * Time.deltaTime, 0));
            //transform.Translate(new Vector3(0, Input.GetAxis("Vertical") * maxSpeed * Time.deltaTime, 0));

            //transform.Translate(new Vector3(0, joystick.Vertical * maxSpeed * Time.deltaTime, 0));
            //transform.Translate(new Vector3(0, Mathf.Sqrt(Mathf.Pow(joystick.Vertical,2)+Mathf.Pow(joystick.Horizontal, 2)) * maxSpeed * Time.deltaTime, 0));
            GetComponent<Rigidbody2D>().velocity = (transform.up * Mathf.Sqrt(Mathf.Pow(joystick.Vertical, 2) + Mathf.Pow(joystick.Horizontal, 2)) * 300 * Time.deltaTime);
            MainCamera.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -10);

        }


        private void checkShooting () {
          
            if (CrossPlatformInputManager.GetButtonDown("Fire1")) {
                laserSFX.Play();
                //Define Bullet
                bulletData.activator = NetworkClient.ClientID;
                bulletData.position.x = bulletSpawnPoint.position.x.TwoDecimals();
                bulletData.position.y = bulletSpawnPoint.position.y.TwoDecimals();
                bulletData.direction.x = bulletSpawnPoint.up.x;
                bulletData.direction.y = bulletSpawnPoint.up.y;

                //Send Bullet
                networkIdentity.GetSocket().Emit("fireBullet", new JSONObject(JsonUtility.ToJson(bulletData)));

            }
        }

        public void SetHealth (int health) {
            if (networkIdentity.IsLocalPlayer())
            {
                Debug.Log("My Health2 =" + health);
                Health = health;
                playerCanvas.gameObject.GetComponent<PlayerUI>().setHealth(health);
            }
        }

        public void SetScore(int score)
        {
            this.Score = score;
            this.UpdateScore();
           // Debug.Log("Set Score" + score + "of "+networkIdentity.GetID());
            //Score got change so change the score on the Scoreboard.
           // playerCanvas.gameObject.GetComponent<PlayerUI>().setScore(score, networkIdentity.GetID());
        }

        public int GetScore () {
            return this.Score;
        }

        public void UpdateScore () {
            playerCanvas.GetComponent<PlayerUI>().updateScoretable();
        }

        public void SetUsername(string username)
        {
            this.username = username;
        }

        public string GetUsername()
        {
            return username;
        }

    }

    }

