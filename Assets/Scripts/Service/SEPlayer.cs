using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Service
{
	public class SEPlayer : MonoBehaviour
	{
		AudioSource audioSource;

		void Start()
		{
			audioSource = GetComponent<AudioSource>();
			StartCoroutine(PlayCoroutine());
		}

		IEnumerator PlayCoroutine()
		{
			while (audioSource.clip == null)
			{
				yield return null;
			}

			audioSource.Play();

			yield return null;

			while (audioSource.isPlaying)
			{
				yield return null;
			}

			Destroy(gameObject);
		}
	}
}
