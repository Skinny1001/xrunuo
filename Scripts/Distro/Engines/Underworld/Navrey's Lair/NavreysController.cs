﻿using System;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public class NavreysController : Item
	{
		#region Generation
		public static void Initialize()
		{
			CommandSystem.Register( "GenNavrey", AccessLevel.Developer, new CommandEventHandler( GenNavrey_Command ) );
		}

		[Usage( "GenNavrey" )]
		private static void GenNavrey_Command( CommandEventArgs e )
		{
			if ( Check() )
			{
				e.Mobile.SendMessage( "Navrey spawner is already present." );
			}
			else
			{
				e.Mobile.SendMessage( "Creating Navrey Night-Eyes Lair..." );

				NavreysController controller = new NavreysController();

				e.Mobile.SendMessage( "Generation completed!" );
			}
		}

		private static bool Check()
		{
			foreach ( Item item in World.Instance.Items )
				if ( item is NavreysController && !item.Deleted )
					return true;

			return false;
		}
		#endregion

		private NavreysPillar[] m_Pillars;
		private NavreyNightEyes m_Navrey;

		public bool AllPillarsHot
		{
			get
			{
				for ( int i = 0; i < m_Pillars.Length; i++ )
				{
					NavreysPillar pillar = m_Pillars[i];

					if ( pillar == null || pillar.Deleted || pillar.State != NavreysPillarState.Hot )
						return false;
				}

				return true;
			}
		}

		public NavreysController()
			: base( 0x1F13 )
		{
			Name = "Navrey Spawner - Do not remove !!";
			Visible = false;
			Movable = false;

			MoveToWorld( new Point3D( 1054, 861, -31 ), Map.TerMur );

			m_Pillars = new NavreysPillar[3];

			m_Pillars[0] = new NavreysPillar( this );
			m_Pillars[0].MoveToWorld( new Point3D( 1071, 847, 0 ), Map.TerMur );
			m_Pillars[1] = new NavreysPillar( this );
			m_Pillars[1].MoveToWorld( new Point3D( 1039, 879, 0 ), Map.TerMur );
			m_Pillars[2] = new NavreysPillar( this );
			m_Pillars[2].MoveToWorld( new Point3D( 1039, 850, 0 ), Map.TerMur );

			Respawn();
		}

		public void OnNavreyKilled()
		{
			SetAllPillars( NavreysPillarState.Off );

			Timer.DelayCall( TimeSpan.FromMinutes( 10.0 ), new TimerCallback( Respawn ) );
		}

		public void Respawn()
		{
			NavreyNightEyes navrey = new NavreyNightEyes( this );
			navrey.RangeHome = 20;

			navrey.MoveToWorld( Map.GetSpawnPosition( Location, 10 ), Map );

			SetAllPillars( NavreysPillarState.On );

			m_Navrey = navrey;
		}

		public void ResetPillars()
		{
			if ( m_Navrey != null && !m_Navrey.Deleted && m_Navrey.Alive )
				SetAllPillars( NavreysPillarState.On );
		}

		public void CheckPillars()
		{
			if ( AllPillarsHot )
			{
				m_Navrey.UsedPillars = true;

				Timer t = new RockRainTimer( m_Navrey );
				t.Start();

				SetAllPillars( NavreysPillarState.Off );
				Timer.DelayCall( TimeSpan.FromMinutes( 5.0 ), new TimerCallback( ResetPillars ) );
			}
		}

		private void SetAllPillars( NavreysPillarState state )
		{
			for ( int i = 0; i < m_Pillars.Length; i++ )
			{
				NavreysPillar pillar = m_Pillars[i];
				pillar.State = state;
			}
		}

		public NavreysController( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (Mobile) m_Navrey );

			writer.Write( (Item) m_Pillars[0] );
			writer.Write( (Item) m_Pillars[1] );
			writer.Write( (Item) m_Pillars[2] );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			/*int version = */
			reader.ReadInt();

			m_Navrey = (NavreyNightEyes) reader.ReadMobile();

			m_Pillars = new NavreysPillar[3];

			m_Pillars[0] = (NavreysPillar) reader.ReadItem();
			m_Pillars[1] = (NavreysPillar) reader.ReadItem();
			m_Pillars[2] = (NavreysPillar) reader.ReadItem();

			if ( m_Navrey == null )
				Timer.DelayCall( TimeSpan.Zero, new TimerCallback( Respawn ) );
			else
				SetAllPillars( NavreysPillarState.On );
		}

		private class RockRainTimer : Timer
		{
			private NavreyNightEyes m_Navrey;
			private int m_Ticks;

			public RockRainTimer( NavreyNightEyes navrey )
				: base( TimeSpan.Zero, TimeSpan.FromSeconds( 0.25 ) )
			{

				m_Navrey = navrey;
				m_Ticks = 120;

				m_Navrey.CantWalk = true;
			}

			protected override void OnTick()
			{
				m_Ticks--;

				Point3D dest = m_Navrey.Location;
				Point3D orig = new Point3D( dest.X - Utility.RandomMinMax( 3, 4 ), dest.Y - Utility.RandomMinMax( 8, 9 ), dest.Z + Utility.RandomMinMax( 41, 43 ) );
				int itemId = Utility.RandomMinMax( 0x1362, 0x136D );
				int speed = Utility.RandomMinMax( 5, 10 );
				int hue = Utility.RandomBool() ? 0 : Utility.RandomMinMax( 0x456, 0x45F );

				Effects.SendPacket( orig, m_Navrey.Map, new HuedEffect( EffectType.Moving, Serial.Zero, Serial.Zero, itemId, orig, dest, speed, 0, false, false, hue, 4 ) );
				Effects.SendPacket( orig, m_Navrey.Map, new HuedEffect( EffectType.Moving, Serial.Zero, Serial.Zero, itemId, orig, dest, speed, 0, false, false, hue, 4 ) );

				Effects.PlaySound( m_Navrey.Location, m_Navrey.Map, 0x15E + Utility.Random( 3 ) );
				Effects.PlaySound( m_Navrey.Location, m_Navrey.Map, Utility.RandomList( 0x305, 0x306, 0x307, 0x309 ) );

				int amount = Utility.RandomMinMax( 100, 150 );
				m_Navrey.Damage( amount );
				m_Navrey.SendDamageToAll( amount );

				if ( m_Ticks == 0 || !m_Navrey.Alive )
				{
					m_Navrey.CantWalk = false;
					Stop();
				}
			}
		}
	}
}