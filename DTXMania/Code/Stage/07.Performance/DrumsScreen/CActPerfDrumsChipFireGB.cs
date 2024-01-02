﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace DTXMania
{
	internal class CActPerfDrumsChipFireGB : CActPerfChipFireGB
	{
		// メソッド

		public override void Start( int nLane )
		{
			if( ( nLane < 0 ) && ( nLane > 5 ) )
			{
				throw new IndexOutOfRangeException();
			}
			EInstrumentPart e楽器パート = ( nLane < 3 ) ? EInstrumentPart.GUITAR : EInstrumentPart.BASS;
			int index = nLane;
			if( CDTXMania.ConfigIni.bLeft[ (int) e楽器パート ] )
			{
				index = ( ( index / 3 ) * 3 ) + ( 2 - ( index % 3 ) );
			}
			int x = this.pt中央[ index ].X;
			int y = this.pt中央[ index ].Y;
			if( CDTXMania.ConfigIni.bReverse[ (int) e楽器パート ] )
			{
				y = ( nLane < 3 ) ? 0x176 : 0x176;
			}
			base.Start( nLane, x, y );
		}


		// Other

		#region [ private ]
		//-----------------
		private readonly Point[] pt中央 = new Point[] { new Point( 0x207, 0x5f ), new Point( 0x221, 0x5f ), new Point( 0x23b, 0x5f ), new Point( 410, 0x5f ), new Point( 0x1b4, 0x5f ), new Point( 0x1ce, 0x5f ) };
		//-----------------
		#endregion
	}
}
