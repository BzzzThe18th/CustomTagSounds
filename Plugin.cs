using BepInEx;
using HarmonyLib;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using CustomTagSounds.Behaviours;

namespace CustomTagSounds
{
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.6.7")]
    [BepInDependency("com.buzzbzzzbzzbzzzthe18th.gorillatag.HoneyLib", "1.0.9")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static AudioClip clip;
        private Coroutine routine;

        void Awake()
        {
            new Harmony(PluginInfo.GUID).PatchAll(Assembly.GetExecutingAssembly());
            Directory.CreateDirectory(string.Format("{0}/CustomTagSound", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)));
            LoadAudioClip();
            routine = StartCoroutine(delayAwake());
        }

        void OnEnable()
        {
            if(AudioManager.instance) AudioManager.instance.enabled = true;
        }

        void OnDisable()
        {
            if (AudioManager.instance) AudioManager.instance.enabled = false;
            if (routine != null)
                StopCoroutine(routine);
        }

        public async void LoadAudioClip()
        {
            clip = await LoadClip(string.Format("{0}/CustomTagSound/hitSound.ogg", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)));
        }

        async Task<AudioClip> LoadClip(string p)
        {
            AudioClip returnClip = null;
            using (UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip(p, AudioType.OGGVORBIS))
            {
                webRequest.SendWebRequest();
                try
                {
                    while (!webRequest.isDone) await Task.Delay(5);

                    if (webRequest.result != UnityWebRequest.Result.ConnectionError && webRequest.result != UnityWebRequest.Result.ProtocolError)
                    {
                        returnClip = DownloadHandlerAudioClip.GetContent(webRequest);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            return returnClip;
        }

        IEnumerator delayAwake()
        {
            yield return new WaitForSeconds(1.5f);
            GorillaLocomotion.Player.Instance.gameObject.AddComponent<AudioManager>();
            routine = null;
        }
    }
}
