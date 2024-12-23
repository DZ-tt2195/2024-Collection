using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Week4
{
    public class Player : MonoBehaviour
    {
        public static Player instance;
        public bool leftDoor { get; private set; }
        public bool rightDoor { get; private set; }
        public bool lightPath { get; private set; }
        public bool soundPath { get; private set; }
        public bool lightCenter { get; private set; }
        public bool soundCenter { get; private set; }
        public List<Enemy> listOfEnemies { get; private set; }

        private void Awake()
        {
            instance = this;
            listOfEnemies = new();
        }
    }
}