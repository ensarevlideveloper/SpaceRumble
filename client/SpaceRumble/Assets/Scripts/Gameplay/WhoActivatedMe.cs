using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceRumble.Utility.Attributes;


namespace SpaceRumble.Gameplay
{
    public class WhoActivatedMe : MonoBehaviour
    {

        [GreyOut]
        private string whoActivatedMe;

        public void SetActivator(string ID) {
            whoActivatedMe = ID;
        }

        public string GetActivator() {
            return whoActivatedMe;
        }
    }
}
