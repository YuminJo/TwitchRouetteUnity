using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Firesplash.UnityAssets.TwitchIntegration
{
	/// <summary>
	/// This behavior holds an action queue and dispatches those actions on the unity player's main thread. It's singleton is accessed through TIDispatcher.Instance.
	/// This is an internal component, you should not need to "get in touch" with it.
	/// </summary>
	[DisallowMultipleComponent, AddComponentMenu("Firesplash Entertainment/Twitch Integration/Only for special cases/TI-Dispatcher (Singleton! Read the docs.)")]
	internal class TIDispatcher : MonoBehaviour
	{
		private static TIDispatcher _instance = null;
		private static readonly Queue<Action> dispatchQueue = new Queue<Action>();
		int maxActionsPerFrame = 20;

		internal void Start()
		{
			StartCoroutine(DispatcherLoop());
		}

		IEnumerator DispatcherLoop()
		{
			int counter = 0;
			while (true)
			{
				counter = 0;
				lock (dispatchQueue)
				{
					while (dispatchQueue.Count > 0)
					{
						//Debug.Log("[TIDispatcher] Dispatching an action");
						dispatchQueue.Dequeue().Invoke();
						if (counter++ >= maxActionsPerFrame)
						{
							counter = 0;
							yield return 0; //Max 20 actions per frame to prevent framerate drops - TODO calculate a more representative value based on current framerate and current value
						}
					}
				}
				yield return 0; //without that, we'd have a DeathLock loop ;)
			}
		}

		//Enqueues an Action to be run on the main thread
		internal void Enqueue(Action action)
		{
			lock (dispatchQueue)
			{
				//Debug.Log("[TIDispatcher] Enqueueing an action");
				dispatchQueue.Enqueue(action);
			}
		}

		internal static TIDispatcher Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<TIDispatcher>();
					if (_instance == null)
					{
						_instance = new GameObject("Firesplash.UnityAssets.TwitchIntegration.TIDispatcher").AddComponent<TIDispatcher>();
						DontDestroyOnLoad(_instance.gameObject);
					}
				}
				return _instance;
			}
		}

		void Awake()
		{
			if (_instance == null)
			{
				_instance = this;
				DontDestroyOnLoad(this.gameObject);
			}
		}

		void OnDestroy()
		{
			lock(dispatchQueue)
			{
				dispatchQueue.Clear();
			}
			_instance = null;
		}

		public static bool CheckAvailability()
		{
			if (TIDispatcher.Instance == null)
			{
				UnityEngine.Debug.LogError("[Twitch Integration] Unable to instantiate dispatcher. You can try to manually create a GameObject with the TI-Dispatcher Behaviour in your scene.");
				return false;
			}
			return true;
		}
	}
}