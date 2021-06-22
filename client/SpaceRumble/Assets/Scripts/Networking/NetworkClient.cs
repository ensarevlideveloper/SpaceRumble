using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SocketIO;
using SpaceRumble.Utility;
using SpaceRumble.Scriptable;
using SpaceRumble.Gameplay;
using SpaceRumble.Player;

namespace SpaceRumble.Networking
{

    public class NetworkClient : SocketIOComponent
    {
        public const float SERVER_UPDATE_TIME = 10;

        [Header("Network Client")]
        [SerializeField]
        public Transform networkContainer;
        [SerializeField]
        private GameObject playerPrefab;
        [SerializeField]
        private ServerObjects serverSpawnables;

        public GameObject clientSceneManager;
        public GameObject offlineSceneManager;

        private string username;

        public static string ClientID { get; private set; }
        private Dictionary<string, NetworkIdentity> serverObjects;
        // Use this for initialization
        public override void Start()
        {
            base.Start();
            Initialize();
            SetupEvents();
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();

        }
        private void Initialize()
        {
            serverObjects = new Dictionary<string, NetworkIdentity>();
            offlineSceneManager = GameObject.Find("OfflineSceneManager");
        }

        public void JoinGame() {
            DontDestroyOnLoad(this);
            DontDestroyOnLoad(networkContainer);
            username = GameObject.Find("NameInputFieldText").GetComponent<Text>().text;
            SceneManager.LoadScene("Client");
            clientSceneManager = GameObject.Find("ClientSceneManager");
            JoinedPlayer joinedPlayer = new JoinedPlayer();
            joinedPlayer.username = username;
            Emit("joinGame", new JSONObject(JsonUtility.ToJson(joinedPlayer)));
        }

        private void SetupEvents()
        {
            On("disconnected", (E) =>
            {
                string id = E.data["id"].ToString().RemoveQuotes();
                GameObject go = serverObjects[id].gameObject;
                Destroy(go);
                serverObjects.Remove(id);
            });

            On("open", (E) =>
            {
                Debug.Log("Connected to the server.");
            });
            On("register", (E) =>
            {
                ClientID = E.data["id"].ToString().RemoveQuotes();

                Debug.LogFormat("Our Client's ID ({0})", ClientID);
            });
            On("spawn", (E) =>
            {
                Debug.LogFormat("Spawn player");
                //Handling all spawning players
                //Passed data
                string id = E.data["id"].ToString().RemoveQuotes();
                string client_username = E.data["username"].ToString().RemoveQuotes();
                float x = E.data["position"]["x"].f;
                float y = E.data["position"]["y"].f;
                GameObject go = Instantiate(playerPrefab, networkContainer);
                go.name = string.Format("Player ({0})", id);
                NetworkIdentity ni = go.GetComponent<NetworkIdentity>();
                ni.SetControllerID(id);
                ni.SetSocketReference(this);
                go.GetComponent<PlayerManager>().SetUsername(client_username);
                go.transform.SetParent(networkContainer);
                ni.transform.position = new Vector3(x, y, 0);
                // go.GetComponent<PlayerManager>().initPlayerScore();
                serverObjects.Add(id, ni);

                //IDictionaryEnumerator serverObjectsEnumerator = serverObjects.GetEnumerator();

                //while (serverObjectsEnumerator.MoveNext())
                //{
                //    NetworkIdentity ni_scores = (NetworkIdentity) serverObjectsEnumerator.Value;
                //    ni_scores.GetComponent<PlayerManager>().SetScore(ni_scores.GetComponent<PlayerManager>().GetScore());
                //}

            });

            On("updatePositionPlayer", (E) => {
                string id = E.data["id"].ToString().RemoveQuotes();

                float x = E.data["position"]["x"].f;
                float y = E.data["position"]["y"].f;
                float z = E.data["rotation"].f;

                NetworkIdentity ni = serverObjects[id];
                ni.transform.position = new Vector3(x, y, 0);
                ni.transform.rotation = Quaternion.Euler(0, 0, z);

            });

            On("updatePosition", (E) => {
                string id = E.data["id"].ToString().RemoveQuotes();

                float x = E.data["position"]["x"].f;
                float y = E.data["position"]["y"].f;

                NetworkIdentity ni = serverObjects[id];
                ni.transform.position = new Vector3(x, y, 0);

            });

            On("serverSpawn", (E) =>
            {
            string name = E.data["name"].str;
            string id = E.data["id"].ToString().RemoveQuotes();
            float x = E.data["position"]["x"].f;
            float y = E.data["position"]["y"].f;

            Debug.LogFormat("Server wants us to spawn a '{0}'", name);

            if (!serverObjects.ContainsKey(id))
            {
                ServerObjectData sod = serverSpawnables.GetObjectByName(name);
                var spawnedObject = Instantiate(sod.Prefab, networkContainer);
                spawnedObject.transform.position = new Vector3(x, y, 0);

                var ni = spawnedObject.GetComponent<NetworkIdentity>();
                ni.SetControllerID(id);
                ni.SetSocketReference(this);
                if (name == "Bullet")
                {
                    float directionX = E.data["direction"]["x"].f;
                    float directionY = E.data["direction"]["y"].f;
                    string activator = E.data["activator"].ToString().RemoveQuotes();
                    float speed = E.data["speed"].f;

                    float rot = Mathf.Atan2(directionY, directionX) * Mathf.Rad2Deg;
                    Vector3 currentRotation = new Vector3(0, 0, rot - 90);
                    //float z = E.data["rotation"].f;
                    spawnedObject.transform.rotation = Quaternion.Euler(currentRotation);

                    WhoActivatedMe whoActivatedMe = spawnedObject.GetComponent<WhoActivatedMe>();
                    whoActivatedMe.SetActivator(activator);

                        Projectile projectile = spawnedObject.GetComponent<Projectile>();
                        projectile.Direction = new Vector2(directionX, directionY);
                        projectile.Speed = speed;


                    }
                    serverObjects.Add(id, ni);
                }
            });

            On("serverUnspawn", (E) =>
            {
                Debug.Log("Destroy Bullet or something else");
                string id = E.data["id"].ToString().RemoveQuotes();
                NetworkIdentity ni = serverObjects[id];
                serverObjects.Remove(id);
                DestroyImmediate(ni.gameObject);
            });
            On("playerTookDamage", (E) =>
            {
                Debug.Log("I took damage.");
                string id = E.data["id"].ToString().RemoveQuotes();
                int health = int.Parse(E.data["health"].ToString().RemoveQuotes());
                Debug.Log("My health:" + health);
                NetworkIdentity ni = serverObjects[id];
                ni.GetComponent<PlayerManager>().SetHealth(health);
            });

            On("updatePlayerScore", (E) => 
            {
                Debug.Log("UpdatePlayerScore");
                string id = E.data["id"].ToString().RemoveQuotes();
                int score = int.Parse(E.data["score"].ToString().RemoveQuotes());
                NetworkIdentity ni = serverObjects[id];
                ni.GetComponent<PlayerManager>().SetScore(score);
            });

            On("playerDied", (E) =>
            {
            string id = E.data["id"].ToString().RemoveQuotes();
                NetworkIdentity ni = serverObjects[id];
                ni.gameObject.GetComponent<PlayerManager>().abortPlayerScore();
                ni.gameObject.SetActive(false);
            });

            On("respawnPlayer", (E) =>
            {
                Debug.Log("playerRespawning");
                string username_name = E.data["username"].str;
                float x = E.data["position"]["x"].f;
                float y = E.data["position"]["y"].f;
                string id = E.data["id"].ToString().RemoveQuotes();
                NetworkIdentity ni = serverObjects[id];
                ni.transform.position = new Vector3(x, y, 0);
                ni.GetComponent<PlayerManager>().Respawned();
                ni.gameObject.SetActive(true);
            });

            On("playerRank", (E) =>
            {
                string id = E.data["id"].ToString().RemoveQuotes();
                int rank = int.Parse(E.data["rank"].ToString().RemoveQuotes());
                clientSceneManager = GameObject.Find("ClientSceneManager");
                NetworkIdentity ni = serverObjects[id];
                clientSceneManager.GetComponent<ClientSceneManager>().setRespawnableNetworkIdentity(ni);
                clientSceneManager.GetComponent<ClientSceneManager>().OpenRespawn();
                clientSceneManager.GetComponent<ClientSceneManager>().setRespawnPlaceText(rank);

            });
        }

    }

    [System.Serializable]
    public class JoinedPlayer
    {
        public string username;
    }

    [System.Serializable]
    public class Player {
        public string id;
        public Position position;
        public Rotation rotation;
    }


    [System.Serializable]
    public class Position
    {
        public float x;
        public float y;
    }

    [System.Serializable]
    public class Rotation
    {
        public float z;
    }

    [System.Serializable]
    public class BulletData
    {
        public string id;
        public string activator;
        public Position position;
        public Position direction;
    }

    [System.Serializable]
    public class IDData
    {
        public string id;
        public string activatorId;
    }

    [System.Serializable]
    public class IDDataWithDamage
    {
        public string id;
        public string ownerId;
        public string otherId;
    }

}
