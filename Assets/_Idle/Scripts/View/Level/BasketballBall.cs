using _Idle.Scripts.Balance;
using _Idle.Scripts.View.Unit;
using GeneralTools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Idle.Scripts.View.Level
{
	public class BasketballBall : BaseBehaviour
	{
		[SerializeField] private Rigidbody _rigidbody;
		[SerializeField] private float _jumpForce = 90;

		private Vector3 _lastVelocity;
		
		public void Start()
		{
			_rigidbody.AddForce(new Vector3(Random.Range(-0.3f, 0.3f), 1, Random.Range(-0.3f, 0.3f)) * _jumpForce, ForceMode.Impulse);
		}
		
		private void FixedUpdate()
		{
			_lastVelocity = _rigidbody.velocity;
		}

		private void OnCollisionEnter(Collision other)
		{
			_lastVelocity = Vector3.Reflect(_lastVelocity, other.contacts[0].normal);
			_rigidbody.velocity = _lastVelocity;

			if (((1 << other.gameObject.layer) & GameBalance.Instance.PlayerLayer) != 0)
			{
				if (other.gameObject.TryGetComponent<UnitView>(out var unit))
				{
					unit.Model.Kill();
				}
			}
		}
	}
}