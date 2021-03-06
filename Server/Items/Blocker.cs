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
using Server;
using Server.Network;

namespace Server.Items
{
	public class Blocker : Item
	{
		public override int LabelNumber { get { return 503057; } } // Impassable!

		[Constructable]
		public Blocker()
			: base( 0x21A4 )
		{
			Movable = false;
		}

		public Blocker( Serial serial )
			: base( serial )
		{
		}

		public override void SendInfoTo( GameClient state )
		{
			Mobile mob = state.Mobile;

			if ( mob != null && mob.AccessLevel >= AccessLevel.GameMaster )
			{
				state.Send( new GMItemPacket( this ) );
			}
			else
			{
				state.Send( GetWorldPacketFor( state ) );
			}

			if ( ObjectPropertyListPacket.Enabled )
			{
				state.Send( OPLPacket );
			}
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

	public class SpawnBlocker : Item
	{
		[Constructable]
		public SpawnBlocker()
			: base( 0x21A4 )
		{
			Name = "SpawnBlocker";
			Movable = false;
		}

		public SpawnBlocker( Serial serial )
			: base( serial )
		{
		}

		public override void SendInfoTo( GameClient state )
		{
			Mobile mob = state.Mobile;

			if ( mob != null && mob.AccessLevel >= AccessLevel.GameMaster )
				state.Send( new GMItemPacket( this ) );
			else
				state.Send( GetWorldPacketFor( state ) );

			if ( ObjectPropertyListPacket.Enabled )
				state.Send( OPLPacket );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			//int version =
			reader.ReadInt();
		}
	}

	public sealed class GMItemPacket : Packet
	{
		public GMItemPacket( Item item )
			: base( 0x1A )
		{
			this.EnsureCapacity( 20 );

			// 14 base length
			// +2 - Amount
			// +2 - Hue
			// +1 - Flags

			uint serial = (uint) item.Serial.Value;
			int itemID = 0x1183;
			int amount = item.Amount;
			Point3D loc = item.Location;
			int x = loc.X;
			int y = loc.Y;
			int hue = item.Hue;
			int flags = item.GetPacketFlags();
			int direction = (int) item.Direction;

			if ( amount != 0 )
				serial |= 0x80000000;
			else
				serial &= 0x7FFFFFFF;

			m_Stream.Write( (uint) serial );
			m_Stream.Write( (ushort) ( itemID & TileData.MaxItemValue ) );

			if ( amount != 0 )
				m_Stream.Write( (short) amount );

			x &= 0x7FFF;

			if ( direction != 0 )
				x |= 0x8000;

			m_Stream.Write( (short) x );

			y &= 0x3FFF;

			if ( hue != 0 )
				y |= 0x8000;

			if ( flags != 0 )
				y |= 0x4000;

			m_Stream.Write( (short) y );

			if ( direction != 0 )
				m_Stream.Write( (byte) direction );

			m_Stream.Write( (sbyte) loc.Z );

			if ( hue != 0 )
				m_Stream.Write( (ushort) hue );

			if ( flags != 0 )
				m_Stream.Write( (byte) flags );
		}
	}
}