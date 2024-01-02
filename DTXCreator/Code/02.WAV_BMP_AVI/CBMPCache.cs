﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DTXCreator.WAV_BMP_AVI
{
	internal class CBMPCache  // CBMPキャッシュ
	{
		public Dictionary<int, CBMP> dicBMPディクショナリ = new Dictionary<int, CBMP>();
		public int n現在のキャッシュアイテム数
		{
			get
			{
				if( this.dicBMPディクショナリ == null )
				{
					return 0;
				}
				return this.dicBMPディクショナリ.Count;
			}
		}

		public CBMP tBMPをキャッシュから検索して返す( int nBMP番号1to3843 )
		{
			CBMP cbmp;
			if( ( nBMP番号1to3843 < 1 ) || ( nBMP番号1to3843 > 62 * 62 - 1 ) )
			{
				throw new Exception( "BMP番号が範囲を超えています。-> [" + nBMP番号1to3843 + "]" );
			}
			if( this.dicBMPディクショナリ.TryGetValue( nBMP番号1to3843, out cbmp ) )
			{
				return cbmp;
			}
			return null;
		}
		public CBMP tBMPをキャッシュから検索して返す_なければ新規生成する( int nBMP番号1to3843 )
		{
			if( ( nBMP番号1to3843 < 1 ) || ( nBMP番号1to3843 > 62 * 62 - 1 ) )
			{
				throw new Exception( "BMP番号が範囲を超えています。-> [" + nBMP番号1to3843 + "]" );
			}
			CBMP cbmp = null;
			if( !this.dicBMPディクショナリ.TryGetValue( nBMP番号1to3843, out cbmp ) )
			{
				cbmp = new CBMP();
				cbmp.nBMP番号1to3843 = nBMP番号1to3843;
				this.tキャッシュに追加する( cbmp );
			}
			return cbmp;
		}
		public void tBMPをキャッシュから削除する( int nBMP番号1to3843 )
		{
			if( ( nBMP番号1to3843 < 1 ) || ( nBMP番号1to3843 > 62 * 62 - 1 ) )
			{
				throw new Exception( "BMP番号が範囲を超えています。-> [" + nBMP番号1to3843 + "]" );
			}
			CBMP cbmp = null;
			if( this.dicBMPディクショナリ.TryGetValue( nBMP番号1to3843, out cbmp ) )
			{
				this.dicBMPディクショナリ.Remove( nBMP番号1to3843 );
			}
		}
		public void tキャッシュに追加する( CBMP bc追加するBMP )
		{
			CBMP cbmp;
			if( this.dicBMPディクショナリ.TryGetValue( bc追加するBMP.nBMP番号1to3843, out cbmp ) )
			{
				this.dicBMPディクショナリ.Remove( bc追加するBMP.nBMP番号1to3843 );
			}
			this.dicBMPディクショナリ.Add( bc追加するBMP.nBMP番号1to3843, bc追加するBMP );
		}
		public void tキャッシュに追加する( ListViewItem lvi )
		{
			CBMP cbmp = new CBMP();
			cbmp.tコピーfrom( lvi );
			this.tキャッシュに追加する( cbmp );
		}
		public void t空にする()
		{
			this.dicBMPディクショナリ.Clear();
		}
	}
}
