using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using SpaceRumble.Utility.Attributes;

namespace SpaceRumble.Networking
{

    public class NetworkIdentity : MonoBehaviour
    {

        [Header("Helpful Values")]
        [SerializeField]
        [GreyOut]
        private string id;
        [SerializeField]
        [GreyOut]
        private bool isLocalPlayer;

        private SocketIOComponent socket;

        // Use this for initialization
        public void Awake()
        {
            isLocalPlayer = false;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetControllerID (string ID) {
            id = ID;
            isLocalPlayer = (NetworkClient.ClientID == ID) ? true : false;
        }



        public void SetSocketReference(SocketIOComponent Socket) {
            socket = Socket;
        }

        public string GetID() {
            return id;
        }



        public bool IsLocalPlayer() {
            return isLocalPlayer;
        }

        public SocketIOComponent GetSocket () {
            return socket;
        }
    }
}
