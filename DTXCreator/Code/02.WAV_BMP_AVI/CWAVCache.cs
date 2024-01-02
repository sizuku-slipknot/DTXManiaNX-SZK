﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DTXCreator.WAV_BMP_AVI
{
	internal class CWAVCache  // CWAVキャッシュ
	{
		public Dictionary<int, CWAV> dicWAVディクショナリ = new Dictionary<int, CWAV>();
		public int n現在のキャッシュアイテム数
		{
			get
			{
				if( this.dicWAVディクショナリ == null )
				{
					return 0;
				}
				return this.dicWAVディクショナリ.Count;
			}
		}

		public CWAV tWAVをキャッシュから検索して返す( int nWAV番号1to3843 )
		{
			CWAV cwav;
			if (nWAV番号1to3843 == 0)
			{
				nWAV番号1to3843++;
			}

			if ( ( nWAV番号1to3843 < 1 ) || ( nWAV番号1to3843 > 62 * 62 - 1 ) )
			{
				throw new Exception( "WAV番号が範囲を超えています。-> [" + nWAV番号1to3843 + "]" );
			}
			if( this.dicWAVディクショナリ.TryGetValue( nWAV番号1to3843, out cwav ) )
			{
				return cwav;
			}
			return null;
		}
		public CWAV tWAVをキャッシュから検索して返す_なければ新規生成する( int nWAV番号1to3843 )
		{
			if (nWAV番号1to3843 == 0)
			{
				nWAV番号1to3843++;
			}

			if ( ( nWAV番号1to3843 < 1 ) || ( nWAV番号1to3843 > 62 * 62 - 1 ) )
			{
				throw new Exception( "WAV番号が範囲を超えています。-> [" + nWAV番号1to3843 + "]" );
			}
			CWAV cwav = null;
			if( !this.dicWAVディクショナリ.TryGetValue( nWAV番号1to3843, out cwav ) )
			{
				cwav = new CWAV();
				cwav.strラベル名 = "";
				cwav.nWAV番号1to3843 = nWAV番号1to3843;
				cwav.strファイル名 = "";
				cwav.n音量0to127 = 100;
				cwav.n位置_0to127 = 0;
				cwav.bBGMとして使用 = false;
				this.tキャッシュに追加する( cwav );
			}
			return cwav;
		}
		public void tWAVをキャッシュから削除する( int nWAV番号1to3843 )
		{
			if( ( nWAV番号1to3843 < 1 ) || ( nWAV番号1to3843 > 62 * 62 - 1 ) )
			{
				throw new Exception( "WAV番号が範囲を超えています。-> [" + nWAV番号1to3843 + "]" );
			}
			CWAV cwav = null;
			if( this.dicWAVディクショナリ.TryGetValue( nWAV番号1to3843, out cwav ) )
			{
				this.dicWAVディクショナリ.Remove( nWAV番号1to3843 );
			}
		}
		public void tキャッシュに追加する( CWAV 追加するセル )
		{
			CWAV cwav;
			if( this.dicWAVディクショナリ.TryGetValue( 追加するセル.nWAV番号1to3843, out cwav ) )
			{
				this.dicWAVディクショナリ.Remove( 追加するセル.nWAV番号1to3843 );
			}
			this.dicWAVディクショナリ.Add( 追加するセル.nWAV番号1to3843, 追加するセル );
		}
		public void tキャッシュに追加する( ListViewItem 追加するLVI )
		{
			CWAV cwav = new CWAV();
			cwav.tコピーfrom( 追加するLVI );
			this.tキャッシュに追加する( cwav );
		}
		public void t空にする()
		{
			this.dicWAVディクショナリ.Clear();
		}
	}
}
