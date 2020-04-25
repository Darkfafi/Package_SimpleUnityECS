using RasofiaGames.SimpleUnityECS.Core;
using UnityEngine;

namespace RasofiaGames.SimpleUnityECS.Sample1
{
	public class MovementComp : EntityComponent
	{
		[SerializeField]
		private AnimationCurve _movementCurve = null;

		[SerializeField]
		private float _movementSpeed = 5f;

		[SerializeField]
		private float _rotationSpeed = 5f;

		public AnimationCurve MovementCurve
		{
			get
			{
				return _movementCurve;
			}
		}

		public float MovementSpeed
		{
			get
			{
				return _movementSpeed;
			}
		}

		public float RotationSpeed
		{
			get
			{
				return _rotationSpeed;
			}
		}
	}
}