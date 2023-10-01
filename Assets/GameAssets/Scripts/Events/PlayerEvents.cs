using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents
{
    public class GetPlayerWorldLocation
    {
        public Vector3 WorldPosition;
    }

	public class ClearLines
	{
		public ClearLines(int numCleared)
		{
			NumCleared = numCleared;
		}

		public readonly int NumCleared;
	}
}
