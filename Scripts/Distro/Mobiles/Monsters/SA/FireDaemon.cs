using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a fire daemon corpse" )]
	public class FireDaemon : BaseCreature
	{
		[Constructable]
		public FireDaemon()
			: base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a fire daemon";
			Body = 9;
			BaseSoundID = 357;

			Hue = 1636;

			SetStr( 504, 539 );
			SetDex( 126, 145 );
			SetInt( 329, 364 );

			SetHits( 1026, 1174 );

			SetDamage( 7, 14 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Fire, 80 );

			SetResistance( ResistanceType.Physical, 45, 60 );
			SetResistance( ResistanceType.Fire, 100 );
			SetResistance( ResistanceType.Cold, -10, 0 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.Anatomy, 75.5, 84.9 );
			SetSkill( SkillName.MagicResist, 95.7, 109.8 );
			SetSkill( SkillName.Tactics, 81.0, 98.6 );
			SetSkill( SkillName.Wrestling, 40.2, 78.7 );
			SetSkill( SkillName.EvalInt, 91.1, 104.5 );
			SetSkill( SkillName.Magery, 91.3, 105.0 );
			SetSkill( SkillName.Meditation, 90.1, 103.7 );

			Fame = 15000;
			Karma = -15000;

			if ( 0.2 > Utility.RandomDouble() )
				PackItem( new DaemonClaw() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Rich );
		}

		public override bool HasBreath { get { return true; } }
		public override bool CanRummageCorpses { get { return true; } }
		public override Poison PoisonImmune { get { return Poison.Regular; } }
		public override int TreasureMapLevel { get { return 4; } }
		public override int Meat { get { return 1; } }

		public FireDaemon( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			/*int version = */
			reader.ReadInt();
		}
	}
}
