using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace QuestingBots.BehaviorExtensions
{
    public abstract class MonoBehaviourDelayedUpdate : MonoBehaviour
    {
        public int UpdateInterval { get; set; } = 100;

        private float nextUpdateTime;
        private bool phaseInitialized;

        protected bool canUpdate()
        {
            if (!phaseInitialized)
            {
                phaseInitialized = true;
                // Stagger bots/monitors so they do not lockstep on the same frames
                nextUpdateTime = Time.realtimeSinceStartup + UnityEngine.Random.Range(0f, UpdateInterval / 1000f);
            }

            if (Time.realtimeSinceStartup < nextUpdateTime)
            {
                return false;
            }

            nextUpdateTime = Time.realtimeSinceStartup + UpdateInterval / 1000f;
            return true;
        }
    }
}
