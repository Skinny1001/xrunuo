//
//  X-RunUO - Ultima Online Server Emulator
//  Copyright (C) 2015 Pedro Pardal
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
	#region Definitions
	[Flags]
	public enum MagicalAttribute
	{
		RegenHits = 0x00000001,
		RegenStam = 0x00000002,
		RegenMana = 0x00000004,
		DefendChance = 0x00000008,
		AttackChance = 0x00000010,
		BonusStr = 0x00000020,
		BonusDex = 0x00000040,
		BonusInt = 0x00000080,
		BonusHits = 0x00000100,
		BonusStam = 0x00000200,
		BonusMana = 0x00000400,
		WeaponDamage = 0x00000800,
		WeaponSpeed = 0x00001000,
		SpellDamage = 0x00002000,
		CastRecovery = 0x00004000,
		CastSpeed = 0x00008000,
		LowerManaCost = 0x00010000,
		LowerRegCost = 0x00020000,
		ReflectPhysical = 0x00040000,
		EnhancePotions = 0x00080000,
		Luck = 0x00100000,
		SpellChanneling = 0x00200000,
		NightSight = 0x00400000,
		LowerAmmoCost = 0x00800000,
		IncreasedKarmaLoss = 0x01000000,
		CastingFocus = 0x02000000
	}

	[Flags]
	public enum WeaponAttribute
	{
		LowerStatReq = 0x00000001,
		SelfRepair = 0x00000002,
		HitLeechHits = 0x00000004,
		HitLeechStam = 0x00000008,
		HitLeechMana = 0x00000010,
		HitLowerAttack = 0x00000020,
		HitLowerDefend = 0x00000040,
		HitMagicArrow = 0x00000080,
		HitHarm = 0x00000100,
		HitFireball = 0x00000200,
		HitLightning = 0x00000400,
		HitDispel = 0x00000800,
		HitColdArea = 0x00001000,
		HitFireArea = 0x00002000,
		HitPoisonArea = 0x00004000,
		HitEnergyArea = 0x00008000,
		HitPhysicalArea = 0x00010000,
		HitCurse = 0x00020000,
		HitFatigue = 0x00040000,
		HitManaDrain = 0x00080000,
		SplinteringWeapon = 0x00100000,
		BattleLust = 0x00200000,
		UseBestSkill = 0x00400000,
		MageWeapon = 0x00800000,
		DurabilityBonus = 0x01000000,
		FireDamagePercent = 0x02000000,
		ColdDamagePercent = 0x04000000,
		PoisonDamagePercent = 0x08000000,
		EnergyDamagePercent = 0x10000000,
		Velocity = 0x20000000,
		Balanced = 0x40000000,
		BloodDrinker = unchecked( (int) 0x80000000 )
	}

	[Flags]
	public enum ArmorAttribute
	{
		LowerStatReq = 0x00000001,
		SelfRepair = 0x00000002,
		MageArmor = 0x00000004,
		DurabilityBonus = 0x00000008,
		SoulCharge = 0x00000010,
		ReactiveParalyze = 0x00000020
	}

	[Flags]
	public enum ElementAttribute
	{
		Physical = 0x00000001,
		Fire = 0x00000002,
		Cold = 0x00000004,
		Poison = 0x00000008,
		Energy = 0x00000010
	}

	[Flags]
	public enum AbsorptionAttribute
	{
		KineticEater = 0x00000001,
		FireEater = 0x00000002,
		ColdEater = 0x00000004,
		PoisonEater = 0x00000008,
		EnergyEater = 0x00000010,
		DamageEater = 0x00000020,
		KineticResonance = 0x00000040,
		FireResonance = 0x00000080,
		ColdResonance = 0x00000100,
		PoisonResonance = 0x00000200,
		EnergyResonance = 0x00000400
	}
	#endregion

	#region Collections
	[PropertyObject]
	public abstract class BaseAttributes
	{
		private IEntity m_Owner;
		private uint m_Names;
		private short[] m_Values;

		private static short[] m_Empty = new short[0];

		public bool IsEmpty { get { return ( m_Names == 0 ); } }
		public IEntity Owner { get { return m_Owner; } }
		public int Count { get { return m_Values.Length; } }

		public uint Names { get { return m_Names; } set { m_Names = value; } }

		public BaseAttributes( IEntity owner )
		{
			m_Owner = owner;
			m_Values = m_Empty;
		}

		public BaseAttributes( IEntity owner, GenericReader reader )
		{
			m_Owner = owner;

			int version = reader.ReadByte();

			switch ( version )
			{
				case 1:
					{
						m_Names = reader.ReadUInt();
						m_Values = new short[reader.ReadEncodedInt()];

						for ( int i = 0; i < m_Values.Length; ++i )
							m_Values[i] = (short) reader.ReadEncodedInt();

						break;
					}
				case 0:
					{
						m_Names = reader.ReadUInt();
						m_Values = new short[reader.ReadInt()];

						for ( int i = 0; i < m_Values.Length; ++i )
							m_Values[i] = (short) reader.ReadInt();

						break;
					}
			}
		}

		public void Serialize( GenericWriter writer )
		{
			writer.Write( (byte) 1 ); // version;

			writer.Write( (uint) m_Names );
			writer.WriteEncodedInt( (int) m_Values.Length );

			for ( int i = 0; i < m_Values.Length; ++i )
				writer.WriteEncodedInt( (int) m_Values[i] );
		}

		public int GetValue( int bitmask )
		{
			uint mask = (uint) bitmask;

			if ( ( m_Names & mask ) == 0 )
				return 0;

			int index = GetIndex( mask );

			if ( index >= 0 && index < m_Values.Length )
				return m_Values[index];

			return 0;
		}

		public void SetValue( int bitmask, int value )
		{
			if ( ( bitmask == (int) WeaponAttribute.DurabilityBonus ) && ( this is WeaponAttributes ) )
			{
				if ( m_Owner is IWeapon )
					( (IWeapon) m_Owner ).UnscaleDurability();
			}
			else if ( ( bitmask == (int) ArmorAttribute.DurabilityBonus ) && ( this is ArmorAttributes ) )
			{
				if ( m_Owner is IArmor )
					( (IArmor) m_Owner ).UnscaleDurability();
			}

			uint mask = (uint) bitmask;

			if ( value != 0 )
			{
				if ( ( m_Names & mask ) != 0 )
				{
					int index = GetIndex( mask );

					if ( index >= 0 && index < m_Values.Length )
						m_Values[index] = (short) value;
				}
				else
				{
					int index = GetIndex( mask );

					if ( index >= 0 && index <= m_Values.Length )
					{
						short[] old = m_Values;
						m_Values = new short[old.Length + 1];

						for ( int i = 0; i < index; ++i )
							m_Values[i] = old[i];

						m_Values[index] = (short) value;

						for ( int i = index; i < old.Length; ++i )
							m_Values[i + 1] = old[i];

						m_Names |= mask;
					}
				}
			}
			else if ( ( m_Names & mask ) != 0 )
			{
				int index = GetIndex( mask );

				if ( index >= 0 && index < m_Values.Length )
				{
					m_Names &= ~mask;

					if ( m_Values.Length == 1 )
					{
						m_Values = m_Empty;
					}
					else
					{
						short[] old = m_Values;
						m_Values = new short[old.Length - 1];

						for ( int i = 0; i < index; ++i )
							m_Values[i] = old[i];

						for ( int i = index + 1; i < old.Length; ++i )
							m_Values[i - 1] = old[i];
					}
				}
			}

			if ( ( bitmask == (int) WeaponAttribute.DurabilityBonus ) && ( this is WeaponAttributes ) )
			{
				if ( m_Owner is IWeapon )
					( (IWeapon) m_Owner ).ScaleDurability();
			}
			else if ( ( bitmask == (int) ArmorAttribute.DurabilityBonus ) && ( this is ArmorAttributes ) )
			{
				if ( m_Owner is IArmor )
					( (IArmor) m_Owner ).ScaleDurability();
			}

			if ( m_Owner is Item )
			{
				Item owner = (Item) m_Owner;

				if ( owner.Parent is Mobile )
				{
					Mobile m = (Mobile) owner.Parent;

					m.CheckStatTimers();
					m.UpdateResistances();
					m.Delta( MobileDelta.Stat | MobileDelta.WeaponDamage | MobileDelta.Hits | MobileDelta.Stam | MobileDelta.Mana );

					if ( this is SkillBonuses )
					{
						SkillBonuses sb = (SkillBonuses) this;

						sb.Remove();
						sb.AddTo( m );
					}
				}

				owner.InvalidateProperties();
			}
		}

		public int GetIndex( uint mask )
		{
			int index = 0;
			uint ourNames = m_Names;
			uint currentBit = 1;

			while ( currentBit != mask )
			{
				if ( ( ourNames & currentBit ) != 0 )
					++index;

				if ( currentBit == 0x80000000 )
					return -1;

				currentBit <<= 1;
			}

			return index;
		}
	}

	public sealed class MagicalAttributes : BaseAttributes
	{
		public MagicalAttributes( IEntity owner )
			: base( owner )
		{
		}

		public MagicalAttributes( IEntity owner, GenericReader reader )
			: base( owner, reader )
		{
		}

		public static int GetValue( Mobile m, MagicalAttribute attribute )
		{
			int value = 0;

			foreach ( var item in m.GetEquippedItems() )
			{
				if ( item is IMagicalItem )
				{
					MagicalAttributes attrs = ( (IMagicalItem) item ).Attributes;

					if ( attrs != null )
						value += attrs[attribute];
				}

				if ( item is IMagicalBonus )
					value += ( (IMagicalBonus) item ).GetAttributeBonus( attribute );
			}

			return value;
		}

		public int this[MagicalAttribute attribute]
		{
			get { return GetValue( (int) attribute ); }
			set { SetValue( (int) attribute, value ); }
		}

		public override string ToString()
		{
			return "...";
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int RegenHits { get { return this[MagicalAttribute.RegenHits]; } set { this[MagicalAttribute.RegenHits] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int RegenStam { get { return this[MagicalAttribute.RegenStam]; } set { this[MagicalAttribute.RegenStam] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int RegenMana { get { return this[MagicalAttribute.RegenMana]; } set { this[MagicalAttribute.RegenMana] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int DefendChance { get { return this[MagicalAttribute.DefendChance]; } set { this[MagicalAttribute.DefendChance] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int AttackChance { get { return this[MagicalAttribute.AttackChance]; } set { this[MagicalAttribute.AttackChance] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int BonusStr { get { return this[MagicalAttribute.BonusStr]; } set { this[MagicalAttribute.BonusStr] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int BonusDex { get { return this[MagicalAttribute.BonusDex]; } set { this[MagicalAttribute.BonusDex] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int BonusInt { get { return this[MagicalAttribute.BonusInt]; } set { this[MagicalAttribute.BonusInt] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int BonusHits { get { return this[MagicalAttribute.BonusHits]; } set { this[MagicalAttribute.BonusHits] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int BonusStam { get { return this[MagicalAttribute.BonusStam]; } set { this[MagicalAttribute.BonusStam] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int BonusMana { get { return this[MagicalAttribute.BonusMana]; } set { this[MagicalAttribute.BonusMana] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int WeaponDamage { get { return this[MagicalAttribute.WeaponDamage]; } set { this[MagicalAttribute.WeaponDamage] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int WeaponSpeed { get { return this[MagicalAttribute.WeaponSpeed]; } set { this[MagicalAttribute.WeaponSpeed] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int SpellDamage { get { return this[MagicalAttribute.SpellDamage]; } set { this[MagicalAttribute.SpellDamage] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int CastRecovery { get { return this[MagicalAttribute.CastRecovery]; } set { this[MagicalAttribute.CastRecovery] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int CastSpeed { get { return this[MagicalAttribute.CastSpeed]; } set { this[MagicalAttribute.CastSpeed] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int LowerManaCost { get { return this[MagicalAttribute.LowerManaCost]; } set { this[MagicalAttribute.LowerManaCost] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int LowerRegCost { get { return this[MagicalAttribute.LowerRegCost]; } set { this[MagicalAttribute.LowerRegCost] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int ReflectPhysical { get { return this[MagicalAttribute.ReflectPhysical]; } set { this[MagicalAttribute.ReflectPhysical] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int EnhancePotions { get { return this[MagicalAttribute.EnhancePotions]; } set { this[MagicalAttribute.EnhancePotions] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Luck { get { return this[MagicalAttribute.Luck]; } set { this[MagicalAttribute.Luck] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int SpellChanneling { get { return this[MagicalAttribute.SpellChanneling]; } set { this[MagicalAttribute.SpellChanneling] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int NightSight { get { return this[MagicalAttribute.NightSight]; } set { this[MagicalAttribute.NightSight] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int LowerAmmoCost { get { return this[MagicalAttribute.LowerAmmoCost]; } set { this[MagicalAttribute.LowerAmmoCost] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int IncreasedKarmaLoss { get { return this[MagicalAttribute.IncreasedKarmaLoss]; } set { this[MagicalAttribute.IncreasedKarmaLoss] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int CastingFocus { get { return this[MagicalAttribute.CastingFocus]; } set { this[MagicalAttribute.CastingFocus] = value; } }
	}

	public sealed class WeaponAttributes : BaseAttributes
	{
		public WeaponAttributes( Item owner )
			: base( owner )
		{
		}

		public WeaponAttributes( Item owner, GenericReader reader )
			: base( owner, reader )
		{
		}

		public static int GetValue( Mobile m, WeaponAttribute attribute )
		{
			int value = 0;

			foreach ( var item in m.GetEquippedItems() )
			{
				if ( item is IWeapon )
				{
					WeaponAttributes attrs = ( (IWeapon) item ).WeaponAttributes;

					if ( attrs != null )
						value += attrs[attribute];
				}
			}

			return value;
		}

		public int this[WeaponAttribute attribute]
		{
			get
			{
				return GetValue( (int) attribute );
			}
			set
			{
				SetValue( (int) attribute, value );
			}
		}

		public override string ToString()
		{
			return "...";
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int LowerStatReq { get { return this[WeaponAttribute.LowerStatReq]; } set { this[WeaponAttribute.LowerStatReq] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int SelfRepair { get { return this[WeaponAttribute.SelfRepair]; } set { this[WeaponAttribute.SelfRepair] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitLeechHits { get { return this[WeaponAttribute.HitLeechHits]; } set { this[WeaponAttribute.HitLeechHits] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitLeechStam { get { return this[WeaponAttribute.HitLeechStam]; } set { this[WeaponAttribute.HitLeechStam] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitLeechMana { get { return this[WeaponAttribute.HitLeechMana]; } set { this[WeaponAttribute.HitLeechMana] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitLowerAttack { get { return this[WeaponAttribute.HitLowerAttack]; } set { this[WeaponAttribute.HitLowerAttack] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitLowerDefend { get { return this[WeaponAttribute.HitLowerDefend]; } set { this[WeaponAttribute.HitLowerDefend] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitMagicArrow { get { return this[WeaponAttribute.HitMagicArrow]; } set { this[WeaponAttribute.HitMagicArrow] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitHarm { get { return this[WeaponAttribute.HitHarm]; } set { this[WeaponAttribute.HitHarm] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitFireball { get { return this[WeaponAttribute.HitFireball]; } set { this[WeaponAttribute.HitFireball] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitLightning { get { return this[WeaponAttribute.HitLightning]; } set { this[WeaponAttribute.HitLightning] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitDispel { get { return this[WeaponAttribute.HitDispel]; } set { this[WeaponAttribute.HitDispel] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitColdArea { get { return this[WeaponAttribute.HitColdArea]; } set { this[WeaponAttribute.HitColdArea] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitFireArea { get { return this[WeaponAttribute.HitFireArea]; } set { this[WeaponAttribute.HitFireArea] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitPoisonArea { get { return this[WeaponAttribute.HitPoisonArea]; } set { this[WeaponAttribute.HitPoisonArea] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitEnergyArea { get { return this[WeaponAttribute.HitEnergyArea]; } set { this[WeaponAttribute.HitEnergyArea] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitPhysicalArea { get { return this[WeaponAttribute.HitPhysicalArea]; } set { this[WeaponAttribute.HitPhysicalArea] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitCurse { get { return this[WeaponAttribute.HitCurse]; } set { this[WeaponAttribute.HitCurse] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitFatigue { get { return this[WeaponAttribute.HitFatigue]; } set { this[WeaponAttribute.HitFatigue] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitManaDrain { get { return this[WeaponAttribute.HitManaDrain]; } set { this[WeaponAttribute.HitManaDrain] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int SplinteringWeapon { get { return this[WeaponAttribute.SplinteringWeapon]; } set { this[WeaponAttribute.SplinteringWeapon] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int BattleLust { get { return this[WeaponAttribute.BattleLust]; } set { this[WeaponAttribute.BattleLust] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int UseBestSkill { get { return this[WeaponAttribute.UseBestSkill]; } set { this[WeaponAttribute.UseBestSkill] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int MageWeapon { get { return this[WeaponAttribute.MageWeapon]; } set { this[WeaponAttribute.MageWeapon] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int DurabilityBonus { get { return this[WeaponAttribute.DurabilityBonus]; } set { this[WeaponAttribute.DurabilityBonus] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int DamageFirePercent
		{
			get
			{
				return this[WeaponAttribute.FireDamagePercent];
			}
			set
			{
				if ( value < 0 )
					value = 0;
				if ( value > 100 )
					value = 100;

				this[WeaponAttribute.FireDamagePercent] = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int DamageColdPercent
		{
			get
			{
				return this[WeaponAttribute.ColdDamagePercent];
			}
			set
			{
				if ( value < 0 )
					value = 0;
				if ( value > 100 )
					value = 100;

				this[WeaponAttribute.ColdDamagePercent] = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int DamagePoisonPercent
		{
			get
			{
				return this[WeaponAttribute.PoisonDamagePercent];
			}
			set
			{
				if ( value < 0 )
					value = 0;
				if ( value > 100 )
					value = 100;

				this[WeaponAttribute.PoisonDamagePercent] = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int DamageEnergyPercent
		{
			get
			{
				return this[WeaponAttribute.EnergyDamagePercent];
			}
			set
			{
				if ( value < 0 )
					value = 0;
				if ( value > 100 )
					value = 100;

				this[WeaponAttribute.EnergyDamagePercent] = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Velocity { get { return this[WeaponAttribute.Velocity]; } set { this[WeaponAttribute.Velocity] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Balanced { get { return this[WeaponAttribute.Balanced]; } set { this[WeaponAttribute.Balanced] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int BloodDrinker { get { return this[WeaponAttribute.BloodDrinker]; } set { this[WeaponAttribute.BloodDrinker] = value; } }
	}

	public sealed class ArmorAttributes : BaseAttributes
	{
		public ArmorAttributes( Item owner )
			: base( owner )
		{
		}

		public ArmorAttributes( Item owner, GenericReader reader )
			: base( owner, reader )
		{
		}

		public static int GetValue( Mobile m, ArmorAttribute attribute )
		{
			int value = 0;

			foreach ( var item in m.GetEquippedItems() )
			{
				if ( item is IArmor )
				{
					ArmorAttributes attrs = ( (IArmor) item ).ArmorAttributes;

					if ( attrs != null )
						value += attrs[attribute];
				}
				else if ( item is ICloth )
				{
					ArmorAttributes attrs = ( (ICloth) item ).ClothingAttributes;

					if ( attrs != null )
						value += attrs[attribute];
				}
			}

			return value;
		}

		public int this[ArmorAttribute attribute] { get { return GetValue( (int) attribute ); } set { SetValue( (int) attribute, value ); } }

		public override string ToString()
		{
			return "...";
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int LowerStatReq { get { return this[ArmorAttribute.LowerStatReq]; } set { this[ArmorAttribute.LowerStatReq] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int SelfRepair { get { return this[ArmorAttribute.SelfRepair]; } set { this[ArmorAttribute.SelfRepair] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int MageArmor { get { return this[ArmorAttribute.MageArmor]; } set { this[ArmorAttribute.MageArmor] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int DurabilityBonus { get { return this[ArmorAttribute.DurabilityBonus]; } set { this[ArmorAttribute.DurabilityBonus] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int SoulCharge { get { return this[ArmorAttribute.SoulCharge]; } set { this[ArmorAttribute.SoulCharge] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int ReactiveParalyze { get { return this[ArmorAttribute.ReactiveParalyze]; } set { this[ArmorAttribute.ReactiveParalyze] = value; } }
	}

	public sealed class ElementAttributes : BaseAttributes
	{
		public ElementAttributes( Item owner )
			: base( owner )
		{
		}

		public ElementAttributes( Item owner, GenericReader reader )
			: base( owner, reader )
		{
		}

		public int this[ElementAttribute attribute] { get { return GetValue( (int) attribute ); } set { SetValue( (int) attribute, value ); } }

		public override string ToString()
		{
			return "...";
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Physical { get { return this[ElementAttribute.Physical]; } set { this[ElementAttribute.Physical] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Fire { get { return this[ElementAttribute.Fire]; } set { this[ElementAttribute.Fire] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Cold { get { return this[ElementAttribute.Cold]; } set { this[ElementAttribute.Cold] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Poison { get { return this[ElementAttribute.Poison]; } set { this[ElementAttribute.Poison] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Energy { get { return this[ElementAttribute.Energy]; } set { this[ElementAttribute.Energy] = value; } }
	}

	public sealed class SkillBonuses : BaseAttributes
	{
		private List<SkillMod> m_Mods;

		public SkillBonuses( Item owner )
			: base( owner )
		{
		}

		public SkillBonuses( Item owner, GenericReader reader )
			: base( owner, reader )
		{
		}

		public void GetProperties( ObjectPropertyList list )
		{
			for ( int i = 0; i < 5; ++i )
			{
				SkillName skill;
				double bonus;

				if ( !GetValues( i, out skill, out bonus ) )
					continue;

				list.Add( 1060451 + i, "#{0}\t{1}", 1044060 + (int) skill, bonus );
			}
		}

		public void AddTo( Mobile m )
		{
			Remove();

			for ( int i = 0; i < 5; ++i )
			{
				SkillName skill;
				double bonus;

				if ( !GetValues( i, out skill, out bonus ) )
					continue;

				if ( m_Mods == null )
					m_Mods = new List<SkillMod>();

				SkillMod sk = new DefaultSkillMod( skill, true, bonus );
				sk.ObeyCap = true;
				m.AddSkillMod( sk );
				m_Mods.Add( sk );
			}
		}

		public void Remove()
		{
			if ( m_Mods == null )
				return;

			for ( int i = 0; i < m_Mods.Count; ++i )
				m_Mods[i].Remove();

			m_Mods = null;
		}

		public bool GetValues( int index, out SkillName skill, out double bonus )
		{
			int v = GetValue( 1 << index );
			int vSkill = 0;
			int vBonus = 0;

			for ( int i = 0; i < 8; ++i )
			{
				vSkill <<= 1;
				vSkill |= ( v & 1 );
				v >>= 1;

				vBonus <<= 1;
				vBonus |= ( v & 1 );
				v >>= 1;
			}

			skill = (SkillName) vSkill;
			bonus = (double) vBonus / 10;

			return ( bonus != 0 );
		}

		public void SetValues( int index, SkillName skill, double bonus )
		{
			int v = 0;
			int vSkill = (int) skill;
			int vBonus = (int) ( bonus * 10 );

			for ( int i = 0; i < 8; ++i )
			{
				v <<= 1;
				v |= ( vBonus & 1 );
				vBonus >>= 1;

				v <<= 1;
				v |= ( vSkill & 1 );
				vSkill >>= 1;
			}

			SetValue( 1 << index, (short) v );
		}

		public SkillName GetSkill( int index )
		{
			SkillName skill;
			double bonus;

			GetValues( index, out skill, out bonus );

			return skill;
		}

		public void SetSkill( int index, SkillName skill )
		{
			SetValues( index, skill, GetBonus( index ) );
		}

		public double GetBonus( int index )
		{
			SkillName skill;
			double bonus;

			GetValues( index, out skill, out bonus );

			return bonus;
		}

		public void SetBonus( int index, double bonus )
		{
			SetValues( index, GetSkill( index ), bonus );
		}

		public override string ToString()
		{
			return "...";
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double Skill_1_Value { get { return GetBonus( 0 ); } set { SetBonus( 0, value ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public SkillName Skill_1_Name { get { return GetSkill( 0 ); } set { SetSkill( 0, value ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public double Skill_2_Value { get { return GetBonus( 1 ); } set { SetBonus( 1, value ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public SkillName Skill_2_Name { get { return GetSkill( 1 ); } set { SetSkill( 1, value ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public double Skill_3_Value { get { return GetBonus( 2 ); } set { SetBonus( 2, value ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public SkillName Skill_3_Name { get { return GetSkill( 2 ); } set { SetSkill( 2, value ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public double Skill_4_Value { get { return GetBonus( 3 ); } set { SetBonus( 3, value ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public SkillName Skill_4_Name { get { return GetSkill( 3 ); } set { SetSkill( 3, value ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public double Skill_5_Value { get { return GetBonus( 4 ); } set { SetBonus( 4, value ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public SkillName Skill_5_Name { get { return GetSkill( 4 ); } set { SetSkill( 4, value ); } }
	}

	public sealed class AbsorptionAttributes : BaseAttributes
	{
		public AbsorptionAttributes( Item owner )
			: base( owner )
		{
		}

		public AbsorptionAttributes( Item owner, GenericReader reader )
			: base( owner, reader )
		{
		}

		public static int GetValue( Mobile m, AbsorptionAttribute attribute )
		{
			int value = 0;

			foreach ( var item in m.GetEquippedItems() )
			{
				if ( item is IAbsorption )
				{
					AbsorptionAttributes attrs = ( (IAbsorption) item ).AbsorptionAttributes;

					if ( attrs != null )
						value += attrs[attribute];
				}
			}

			return value;
		}

		public int this[AbsorptionAttribute attribute]
		{
			get { return GetValue( (int) attribute ); }
			set { SetValue( (int) attribute, value ); }
		}

		public override string ToString()
		{
			return "...";
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int KineticEater { get { return this[AbsorptionAttribute.KineticEater]; } set { this[AbsorptionAttribute.KineticEater] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int FireEater { get { return this[AbsorptionAttribute.FireEater]; } set { this[AbsorptionAttribute.FireEater] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int ColdEater { get { return this[AbsorptionAttribute.ColdEater]; } set { this[AbsorptionAttribute.ColdEater] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int PoisonEater { get { return this[AbsorptionAttribute.PoisonEater]; } set { this[AbsorptionAttribute.PoisonEater] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int EnergyEater { get { return this[AbsorptionAttribute.EnergyEater]; } set { this[AbsorptionAttribute.EnergyEater] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int DamageEater { get { return this[AbsorptionAttribute.DamageEater]; } set { this[AbsorptionAttribute.DamageEater] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int KineticResonance { get { return this[AbsorptionAttribute.KineticResonance]; } set { this[AbsorptionAttribute.KineticResonance] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int FireResonance { get { return this[AbsorptionAttribute.FireResonance]; } set { this[AbsorptionAttribute.FireResonance] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int ColdResonance { get { return this[AbsorptionAttribute.ColdResonance]; } set { this[AbsorptionAttribute.ColdResonance] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int PoisonResonance { get { return this[AbsorptionAttribute.PoisonResonance]; } set { this[AbsorptionAttribute.PoisonResonance] = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int EnergyResonance { get { return this[AbsorptionAttribute.EnergyResonance]; } set { this[AbsorptionAttribute.EnergyResonance] = value; } }
	}
	#endregion
}