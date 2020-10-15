using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Main.Data;

namespace Main.Service
{
	public class SoundService : SingletonMonoBehaviour<SoundService>
	{
		AudioMixerGroup bgmAudioMixerGroup;
		AudioMixerGroup seAudioMixerGroup;

		AudioSource introAudioSource;
		AudioSource loopAudioSource;

		protected override async void Awake()
		{
			base.Awake();

			var audioMaster = await Addressables.LoadAssetAsync<AudioMaster>("AudioMaster");
			bgmAudioMixerGroup = audioMaster.bgmAudioMixerGroup;
			seAudioMixerGroup = audioMaster.seAudioMixerGroup;

			// BGM set up
			introAudioSource = gameObject.AddComponent<AudioSource>();
			loopAudioSource = gameObject.AddComponent<AudioSource>();
			
			introAudioSource.outputAudioMixerGroup = bgmAudioMixerGroup;
			introAudioSource.loop = false;
			introAudioSource.playOnAwake = false;
			introAudioSource.priority = 0;
			
			loopAudioSource.outputAudioMixerGroup = bgmAudioMixerGroup;
			loopAudioSource.loop = true;
			loopAudioSource.playOnAwake = false;
			loopAudioSource.priority = 0;
		}

		public void SetBGM(string introFilePath, string loopFilePath)
		{
			introAudioSource.clip = Resources.Load<AudioClip>(introFilePath);
			loopAudioSource.clip = Resources.Load<AudioClip>(loopFilePath);
		}

		public void SetBGM(string loopFilePath)
		{
			introAudioSource.clip = null;
			loopAudioSource.clip = Resources.Load<AudioClip>(loopFilePath);
		}

		public void PlayBGM(float fadeTime = 0f)
		{
			if (introAudioSource == null || loopAudioSource == null)
			{
				return;
			}

			if (introAudioSource.clip)
			{
				introAudioSource.Play();
				loopAudioSource.PlayScheduled(AudioSettings.dspTime + introAudioSource.clip.length);
				if (fadeTime > 0)
				{
					introAudioSource.volume = 0f;
					introAudioSource.DOFade(1f, fadeTime);
					loopAudioSource.volume = 0f;
					loopAudioSource.DOFade(1f, fadeTime);
				}
			}
			else
			{
				loopAudioSource.Play();
				if (fadeTime > 0)
				{
					loopAudioSource.volume = 0f;
					loopAudioSource.DOFade(1f, fadeTime);
				}
			}
		}

		public void StopBGM(float fadeTime = 0f)
		{
			if (introAudioSource == null || loopAudioSource == null)
			{
				return;
			}

			AudioSource audioSource;
			if (introAudioSource.isPlaying)
			{
				audioSource = introAudioSource;
			}
			else if (loopAudioSource.isPlaying)
			{
				audioSource = loopAudioSource;
			}
			else
			{
				return;
			}

			if (fadeTime > 0)
			{
				audioSource.DOFade(0f, fadeTime).OnComplete(() => 
				{
					audioSource.Stop();
					audioSource.volume = 1f;
				});
			}
			else
			{
				audioSource.Stop();
			}
		}

		public GameObject PlaySE(string seFilePath, float volume = 1f)
		{
			Debug.Log($"Play {seFilePath}");
			// GameObject set up
			var sePlayer = new GameObject("SEPlayer");
			
			var seAudioSource = sePlayer.AddComponent<AudioSource>();
			seAudioSource.clip = Resources.Load<AudioClip>(seFilePath); // Set SE
			seAudioSource.outputAudioMixerGroup = seAudioMixerGroup;
			seAudioSource.loop = false;
			seAudioSource.playOnAwake = true;
			seAudioSource.volume = volume;
			
			sePlayer.AddComponent<SEPlayer>();

			return sePlayer;
		}
	}
}
