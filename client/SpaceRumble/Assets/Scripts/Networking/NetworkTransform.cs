using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceRumble.Utility;
using SpaceRumble.Utility.Attributes;


namespace SpaceRumble.Networking
{
    [RequireComponent(typeof(NetworkIdentity))]

    public class NetworkTransform : MonoBehaviour
    {
        [SerializeField]
        [GreyOut]
        private Vector3 oldPosition;
        private Quaternion oldRotation;

        private NetworkIdentity networkIdentity;
        private Player player;

        private float stillCounter = 0;

        // Use this for initialization
        void Start()
        {
            networkIdentity = GetComponent<NetworkIdentity>();
            oldPosition = transform.position;
            oldRotation = transform.rotation;
            player = new Player();
            player.position = new Position();
            player.rotation = new Rotation();
            player.position.x = 0;
            player.position.y = 0;
            player.rotation.z = 0;

            if (!networkIdentity.IsLocalPlayer()) {
                enabled = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (networkIdentity.IsLocalPlayer()) {
                if(oldPosition != transform.position) {
                    oldPosition = transform.position;
                    stillCounter = 0;
                    SendData();
                }
                else if (oldRotation != transform.rotation) {
                    oldRotation = transform.rotation;
                    stillCounter = 0;
                    SendData();
                }
                else {
                    stillCounter += Time.deltaTime;

                    if (stillCounter >= 1) {
                        stillCounter = 0;
                        SendData();
                    }
                }
            }
        }

        private void SendData() {
            //Update player information
            player.position.x = transform.position.x.TwoDecimals();
            player.position.y = transform.position.y.TwoDecimals();
            player.rotation.z = transform.rotation.eulerAngles.z.TwoDecimals();
            networkIdentity.GetSocket().Emit("updatePositionPlayer", new JSONObject(JsonUtility.ToJson(player)));
        }



    }
}
