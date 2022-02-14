using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using _Idle.Scripts.Model;
using _Idle.Scripts.Saves;
using _Idle.Scripts.UI.Windows;
using DG.Tweening;
using GeneralTools;
using GeneralTools.Model;
using GeneralTools.UI;
using UnityEngine;
using UnityEngine.AI;

namespace _Idle.Scripts
{
	public class MainGame : BaseBehaviour
	{
		public static Action<bool> OnConnected;
		
		private static MainGame _instance;
		
		[SerializeField] private Transform _playerContainer;
		[SerializeField] private Transform _enemiesContainer;
		[SerializeField] private Transform _itemsContainer;
		[SerializeField] private Transform _environmentContainer;
		[SerializeField] private Transform _levelContainer;
		[SerializeField] private Transform _effectContainer;
		[SerializeField] private Transform _worldSpaceCanvas;
		[SerializeField] private Feedbacks _feedbacks;
		
		private LoaderModel _loader;

		public static Transform Root { get; private set; }
		
		public static Transform PlayerContainer { get; private set; }
		public static Transform EnemiesContainer { get; private set; }
		
		public static Transform ItemsContainer { get; private set; }
		
		public static Transform EnvironmentContainer { get; private set; }
		public static Transform LevelContainer { get; private set; }
		public static Transform EffectContainer { get; private set; }
		
		public static Transform WorldSpaceCanvas { get; private set; }

		public static Feedbacks Feedbacks { get; set; }

		public static DateTime ServerTime { get; private set; }
		
		public static bool Active { get; private set; } = true;
		public static bool Pause { get; private set; }
		
		private const int _checkInternetDelay = 10;
		private static float _checkInternetTimer;
		private static bool _isCheckInternet;
		
		private void Start()
		{
#if UNITY_EDITOR
			QualitySettings.vSyncCount = 0;
#else
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 60;
#endif
			
			_instance = this;

			Root = transform;
			PlayerContainer = _playerContainer;
			EnemiesContainer = _enemiesContainer;
			ItemsContainer = _itemsContainer;
			EnvironmentContainer = _environmentContainer;
			LevelContainer = _levelContainer;
			EffectContainer = _effectContainer;
			WorldSpaceCanvas = _worldSpaceCanvas;
			
			Feedbacks = _feedbacks;

			_loader = Models.Add<LoaderModel>();
			_loader.Start();

			Application.focusChanged += OnApplicationFocusChanged;
		}

		private void Update()
		{
			var deltaTime = Time.deltaTime;

			if (_loader.State != LoaderState.Finished)
			{
				_loader.Update(deltaTime);
				return;
			}
			
			if (Pause) return;
			
			UpdateInternetTimer(deltaTime);

			DOTween.ManualUpdate(Time.deltaTime, Time.unscaledDeltaTime);
			
			GameCamera.Instance.UpdateMe(deltaTime);
			Models.Update(deltaTime);
			GameUI.UpdateMe(deltaTime);
			GameSave.Update(deltaTime);
			Cheats.Update();
		}

		private void FixedUpdate()
		{
			if (_loader.State != LoaderState.Finished)
				return;
			
			Models.FixedUpdate(Time.fixedDeltaTime);
		}

		private void LateUpdate()
		{
			if (_loader.State == LoaderState.Finished)
			{
				GameUI.LateUpdateMe();
			}
		}

		private void OnApplicationFocusChanged(bool active)
		{
#if UNITY_EDITOR
			return;
#endif
		}
		
		
		private void UpdateInternetTimer(float deltaTime)
		{
			_checkInternetTimer -= deltaTime;
			if (_checkInternetTimer > 0 || _isCheckInternet) return;
			_isCheckInternet = true;
			OnConnected += OnConnect;
			CheckConnection();
		}

		private void OnConnect(bool isConnected)
		{
			OnConnected -= OnConnect;
			
			if (isConnected) return;
			
			var window = GameUI.Get<NoConnectionWindow>();
			window.OnConnected += OnReconnected;
			window.Open();
		}
		
		private static void OnReconnected()
		{
			Pause = false;
			PauseGame();
		}

		private static void PauseGame()
		{
			Time.timeScale = Pause ? 0 : 1;
		}
		
		public static void CheckConnection()
		{
			_instance.StartCoroutine(checkInternetConnection(isConnected =>
			{
				if (isConnected)
				{
					SetInternetTime();
					_checkInternetTimer = _checkInternetDelay;
				}
				else
				{
					Pause = true;
					PauseGame();
				}

				_isCheckInternet = false;
				OnConnected?.Invoke(isConnected);
			}));
			
			IEnumerator checkInternetConnection(Action<bool> action)
			{
				var www = new WWW("http://windows.com");
				yield return www;
				action(www.error == null);
			}
		}

		public static void SetInternetTime()
		{
			try
			{
				const string NtpServer = "time.windows.com";
				const int DaysTo1900 = 1900 * 365 + 95; // 95 = offset for leap-years etc.
				const long TicksPerSecond = 10000000L;
				const long TicksPerDay = 24 * 60 * 60 * TicksPerSecond;
				const long TicksTo1900 = DaysTo1900 * TicksPerDay;

				var ntpData = new byte[48];
				ntpData[0] = 0x1B; // LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)

				var addresses = Dns.GetHostEntry(NtpServer).AddressList;
				var ipEndPoint = new IPEndPoint(addresses[0], 123);
				long pingDuration; // temp access (JIT-Compiler need some time at first call)
				using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
				{
					socket.Connect(ipEndPoint);
					socket.ReceiveTimeout = 5000;
					socket.Send(ntpData);
					pingDuration = Stopwatch.GetTimestamp(); // after Send-Method to reduce WinSocket API-Call time

					socket.Receive(ntpData);
					pingDuration = Stopwatch.GetTimestamp() - pingDuration;
				}

				var pingTicks = pingDuration * TicksPerSecond / Stopwatch.Frequency;
				var intPart = (long)ntpData[40] << 24 | (long)ntpData[41] << 16 | (long)ntpData[42] << 8 | ntpData[43];
				var fractPart = (long)ntpData[44] << 24 | (long)ntpData[45] << 16 | (long)ntpData[46] << 8 | ntpData[47];
				var netTicks = intPart * TicksPerSecond + (fractPart * TicksPerSecond >> 32);

				ServerTime = new DateTime(TicksTo1900 + netTicks + pingTicks / 2);
				if (ServerTime == DateTime.MinValue) ServerTime = DateTime.Now;
			}
			catch (Exception e)
			{
				ServerTime = DateTime.Now;
			}
		}
	}
}