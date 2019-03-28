using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Conversions {

	public static class BasicConversions {

		public static Vector2 PolarToCartesian(float angle, float r) {
			return new Vector2(Mathf.Sin(angle) * (r), Mathf.Cos(angle) * (r));
		}

		public static List<List<Team>> CDToActualClans(Dictionary<Team, AIHolder> dict) {

			List<List<Team>> actualClans = new List<List<Team>>();

			Dictionary<Team, AIHolder>.KeyCollection keys = dict.Keys;
			foreach (Team j in keys) {
				AIHolder value;
				dict.TryGetValue(j, out value);
				List<Team> clanJ = new List<Team>(value.allies);
				clanJ.Add(j);
				clanJ.Sort();
				bool newClan = true;
				foreach (List<Team> clan in actualClans) {

					if (clanJ.Contains(clan[0])) {
						newClan = false;
						break;
					}
				}
				if (newClan && clanJ.Count > 1) {
					actualClans.Add(clanJ);
				}
			}
			return actualClans;
		}
	}
}