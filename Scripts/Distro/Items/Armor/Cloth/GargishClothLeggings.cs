using System;
using Server.Items;

namespace Server.Items
{
	public class GargishClothLeggings : BaseClothing
	{
		public override int LabelNumber { get { return 1095357; } } // gargish cloth leggings

		public override int BasePhysicalResistance { get { return 5; } }
		public override int BaseFireResistance { get { return 7; } }
		public override int BaseColdResistance { get { return 6; } }
		public override int BasePoisonResistance { get { return 6; } }
		public override int BaseEnergyResistance { get { return 6; } }

		public override int InitMinHits { get { return 30; } }
		public override int InitMaxHits { get { return 40; } }

		public override int StrengthReq { get { return 20; } }

		public override Race RequiredRace { get { return Race.Gargoyle; } }

		[Constructable]
		public GargishClothLeggings()
			: this( 0 )
		{
		}

		[Constructable]
		public GargishClothLeggings( int hue )
			: base( 0x4066, Layer.Pants )
		{
			Weight = 4.0;
			Hue = hue;
		}

		public GargishClothLeggings( Serial serial )
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