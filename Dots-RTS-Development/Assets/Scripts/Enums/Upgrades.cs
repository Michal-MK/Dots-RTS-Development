public enum Upgrades {
	NONE = -1,
	/// <summary>
	/// [NO_STACKING] - Inflicts variable amount of damage over set time.		IMPLEMENTED
	/// </summary>
	ATK_DOT,
	/// <summary>
	/// [STACKING] - Adds a chance to double element damage.		IMPLEMENTED
	/// </summary>
	ATK_CRITICAL_CHANCE,
	/// <summary>
	/// [NO_STACKING] - 100% chance to double element damage.		IMPLEMENTED
	/// </summary>
	ATK_DOUBLE_DAMAGE,
	/// <summary>
	/// Undecided - Slows cell regenaration by a factor of 1.25.		IMPLEMENTED
	/// </summary>
	ATK_SLOW_REGENERATION,

	/// <summary>
	/// [STACKING] - Adds a chance to not take damage from incoming element.		IMPLEMENTED NOT FULLY TESTED
	/// </summary>
	DEF_ELEMENT_RESIST_CHANCE = 100,
	/// <summary>
	/// [NO_STACKING] - Adds a chance to reflect element back at atacker, element changes team to that of the attacked cell.
	/// </summary>
	DEF_REFLECTION,
	/// <summary>
	/// [NO_STACKING]. [TEMPORARY] - Removes this cell from possible targets of enemy AI, lasts for set amount of time.		IMPLEMENTED NOT FULLY TESTED
	/// </summary>
	DEF_CAMOUFLAGE,
	/// <summary>
	/// [NO_STACKING] - incoming elements of the same team have a chance to contain one extra element.		IMPLEMENTED NOT FULLY TESTED
	/// </summary>
	DEF_AID_BONUS_CHANCE,


	/// <summary>
	/// [STACKING] - Increases the speed of elements.
	/// </summary>
	UTIL_FASTER_ELEMENT_SPEED = 200,
	/// <summary>
	/// [STACKING] - Increases cell regeneration rate.		IMPLEMENETED NOT FULLY TESTED IMBALANCED
	/// </summary>
	UTIL_FASTER_REGENERATION,

}
