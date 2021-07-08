public enum Upgrades {
	None = -1,
	/// <summary>
	/// [NO_STACKING] - Inflicts variable amount of damage over set time.		IMPLEMENTED
	/// </summary>
	AtkDot,
	/// <summary>
	/// [STACKING] - Adds a chance to double element damage.		IMPLEMENTED
	/// </summary>
	AtkCriticalChance,
	/// <summary>
	/// [NO_STACKING] - 100% chance to double element damage.		IMPLEMENTED
	/// </summary>
	AtkDoubleDamage,
	/// <summary>
	/// Undecided - Slows cell regenaration by a factor of 1.25.		IMPLEMENTED
	/// </summary>
	AtkSlowRegeneration,

	/// <summary>
	/// [STACKING] - Adds a chance to not take damage from incoming element.		IMPLEMENTED NOT FULLY TESTED
	/// </summary>
	DefElementResistChance = 100,
	/// <summary>
	/// [NO_STACKING] - Adds a chance to reflect element back at atacker, element changes team to that of the attacked cell.
	/// </summary>
	DefReflection,
	/// <summary>
	/// [NO_STACKING]. [TEMPORARY] - Removes this cell from possible targets of enemy AI, lasts for set amount of time.		IMPLEMENTED NOT FULLY TESTED
	/// </summary>
	DefCamouflage,
	/// <summary>
	/// [NO_STACKING] - incoming elements of the same team have a chance to contain one extra element.		IMPLEMENTED NOT FULLY TESTED
	/// </summary>
	DefAidBonusChance,


	/// <summary>
	/// [STACKING] - Increases the speed of elements.
	/// </summary>
	UtilFasterElementSpeed = 200,
	/// <summary>
	/// [STACKING] - Increases cell regeneration rate.		IMPLEMENETED NOT FULLY TESTED IMBALANCED
	/// </summary>
	UtilFasterRegeneration,
}
