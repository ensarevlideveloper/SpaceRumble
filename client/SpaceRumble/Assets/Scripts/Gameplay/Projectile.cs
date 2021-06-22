using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceRumble.Networking;

namespace SpaceRumble.Gameplay
{
    public class Projectile : MonoBehaviour
    {
        private Vector2 direction;
        private float speed;

        public Vector2 Direction {
            set {
                direction = value;
            }

        }

        public float Speed {
            set {
                speed = value;
            }
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Vector2 pos = direction * speed * NetworkClient.SERVER_UPDATE_TIME * Time.deltaTime;
            transform.position += new Vector3(pos.x, pos.y, 0);
        }
    }
}
