using System.Collections;
using UnityEngine;
using HoneyLib.Events;

namespace CustomTagSounds.Behaviours
{
    class AudioManager : MonoBehaviour
    {
        AudioSource audio;
        public static AudioManager instance;
        private Coroutine routine;
        private bool audioLimit;

        void Awake()
        {
            instance = this;
            audio = gameObject.AddComponent<AudioSource>();
            Events.TagHitLocal += LocalTagHitEvent;
        }

        void LocalTagHitEvent(object sender, TagHitLocalArgs args)
        {
            if(!audioLimit)
                routine = StartCoroutine(rateLimit());
        }

        void OnDisable()
        {
            if (routine != null)
                StopCoroutine(routine);
        }

        void OnDestroy()
        {
            if (routine != null)
                StopCoroutine(routine);
        }

        IEnumerator rateLimit()
        {
            audioLimit = true;
            audio.PlayOneShot(Plugin.clip);
            yield return new WaitForSeconds(0.3f);
            audioLimit = false;
            routine = null;
        }
    }
}
