using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Conversions {

	public static class BasicConversions {

		public static Vector2 PolarToCartesian(float angle, float r) {
			return new Vector2(Mathf.Sin(angle) * (r), Mathf.Cos(angle) * (r));
		}

		public static List<List<Cell.enmTeam>> CDToActualClans(Dictionary<Cell.enmTeam, AIHolder> dict) {

			List<List<Cell.enmTeam>> actualClans = new List<List<Cell.enmTeam>>();

			Dictionary<Cell.enmTeam, AIHolder>.KeyCollection keys = dict.Keys;
			foreach (Cell.enmTeam j in keys) {
				AIHolder value;
				dict.TryGetValue(j, out value);
				List<Cell.enmTeam> clanJ = new List<Cell.enmTeam>(value.allies);
				clanJ.Add(j);
				clanJ.Sort();
				bool newClan = true;
				foreach (List<Cell.enmTeam> clan in actualClans) {

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