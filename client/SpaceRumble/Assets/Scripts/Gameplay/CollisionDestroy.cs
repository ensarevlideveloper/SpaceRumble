using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceRumble.Networking;

namespace SpaceRumble.Gameplay
{
    public class CollisionDestroy : MonoBehaviour
    {
        [SerializeField]
        private NetworkIdentity networkIdentity;
        [SerializeField]
        private WhoActivatedMe whoActivatedMe;
        [SerializeField]
        private bool didDamage = false;


        public void OnCollisionEnter2D(Collision2D collision)
        {
            NetworkIdentity ni = collision.gameObject.GetComponent<NetworkIdentity>();
            if(ni == null || ni.GetID() != whoActivatedMe.GetActivator()) {
                networkIdentity.GetSocket().Emit("collisionDestroy", new JSONObject(JsonUtility.ToJson(new IDData()
                {
                    id = networkIdentity.GetID(),
                })));

            }
            //else if (ni.GetID() != whoActivatedMe.GetActivator() && didDamage == false) {
            //    networkIdentity.GetSocket().Emit("collisionDestroyWithDamage", new JSONObject(JsonUtility.ToJson(new IDDataWithDamage()
            //    {
            //        id = networkIdentity.GetID(),
            //        ownerId = whoActivatedMe.GetActivator(),
            //        otherId = ni.GetID()
            //    })));
            //}
            //didDamage = true;
        }
        }

    }
