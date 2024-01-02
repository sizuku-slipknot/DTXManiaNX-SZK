﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using DTXCreator.UndoRedo;
using DTXCreator.Score;
using FDK;

namespace DTXCreator.WAV_BMP_AVI
{
	internal class CWAVListManager  // CWAVリスト管理
	{
		public int n現在選択中のItem番号0to3842 = -1;

		internal delegate void DGサウンドを再生する( int nWAV番号1to3843 );

		public CWAVListManager( CMainForm pメインフォーム, ListView pListViewWAVリスト )
		{
			this._Form = pメインフォーム;
			this.listViewWAVリスト = pListViewWAVリスト;
			this.sound管理 = new CSoundManager( this._Form.Handle );
			this.soundPreview = null;

			#region [ #26122 2011.8.31 yyagi; ストリーム再生のために、t再生中の処理をする()を定期的に呼び出す処理を追加 ]
			timerDelegate = new TimerCallback( this.sound管理.t再生中の処理をする );
			timer = new System.Threading.Timer( timerDelegate, null, 0, 300 );
			#endregion
		}
		public ListViewItem tCWAVとListViewItemを生成して返す( int n行番号1to3843 )
		{
			return this.tWAVをキャッシュから検索して返す_なければ新規生成する( n行番号1to3843 ).t現在の内容から新しいListViewItemを作成して返す();
		}
		public void tDirectSoundの解放()
		{
			if( this.soundPreview != null )
			{
				this.soundPreview.Dispose();
			}
			if ( timer != null )
			{
				timer.Change( Timeout.Infinite, Timeout.Infinite );
				timer.Dispose();
				timer = null;
			}
			if ( timerDelegate != null )
			{
				timerDelegate = null;
			}
			if( this.sound管理 != null )
			{
				this.sound管理.Dispose();
			}
		}
		public void tItemを交換する( int nItem番号1, int nItem番号2 )
		{
			if( !CUndoRedoManager.bUndoRedoした直後 )
			{
				this._Form.mgrUndoRedo管理者.tノードを追加する( new CUndoRedoCell<int>( null, new DGUndoを実行する<int>( this.t行交換のUndo ), new DGRedoを実行する<int>( this.t行交換のRedo ), nItem番号1, nItem番号2 ) );
				this._Form.tUndoRedo用GUIの有効_無効を設定する();
			}
			CUndoRedoManager.bUndoRedoした直後 = false;
			this.tItemを交換する_ListViewItem( nItem番号1, nItem番号2 );
			this.tItemを交換する_WAVキャッシュ( nItem番号1, nItem番号2 );
			this.tItemを交換する_チップパレット( nItem番号1, nItem番号2 );
			this.tItemを交換する_譜面上のチップ( nItem番号1, nItem番号2 );
			this.tItemを交換する_レーン割付チップ( nItem番号1, nItem番号2 );
			this.tItemを交換する_カーソル移動( nItem番号1, nItem番号2 );
			this._Form.listViewWAVリスト.Refresh();
			this._Form.pictureBox譜面パネル.Refresh();
			this._Form.b未保存 = true;
		}
		public void tItemを選択する( int nItem番号0to3842 )
		{
			this.n現在選択中のItem番号0to3842 = nItem番号0to3842;
			this.listViewWAVリスト.Items[ nItem番号0to3842 ].Selected = true;
			this.listViewWAVリスト.Items[ nItem番号0to3842 ].Focused = true;
		}
		public void tWAVリストにフォーカスを当てる()
		{
			this.listViewWAVリスト.Focus();
		}
		public CWAV tWAVをキャッシュから検索して返す( int nWAV番号1to3843 )
		{
			return this.WAVキャッシュ.tWAVをキャッシュから検索して返す( nWAV番号1to3843 );
		}
		public CWAV tWAVをキャッシュから検索して返す_なければ新規生成する( int nWAV番号1to3843 )
		{
			return this.WAVキャッシュ.tWAVをキャッシュから検索して返す_なければ新規生成する( nWAV番号1to3843 );
		}
		public ListViewItem tWAV番号に対応するListViewItemを返す( int nWAV番号1to3843 )
		{
			if( ( nWAV番号1to3843 < 1 ) || ( nWAV番号1to3843 > 62 * 62 - 1 ) )
			{
				throw new Exception( "WAV番号が範囲外です。--->[" + nWAV番号1to3843 + "]" );
			}
			return this.listViewWAVリスト.Items[ nWAV番号1to3843 - 1 ];
		}
		public void tWAV編集のRedo( CWAV wc変更前, CWAV wc変更後 )
		{
			int num = wc変更後.nWAV番号1to3843;
			CWAV cwav = this.WAVキャッシュ.tWAVをキャッシュから検索して返す( num );
			cwav.tコピーfrom( wc変更後 );
			cwav.tコピーto( this.listViewWAVリスト.Items[ num - 1 ] );
			this._Form.tWAV_BMP_AVIリストのカーソルを全部同じ行に合わせる( cwav.nWAV番号1to3843 - 1 );
			this._Form.tタブを選択する( CMainForm.Eタブ種別.WAV );
			this.listViewWAVリスト.Refresh();
		}
		public void tWAV編集のUndo( CWAV wc変更前, CWAV wc変更後 )
		{
			int num = wc変更前.nWAV番号1to3843;
			CWAV cwav = this.WAVキャッシュ.tWAVをキャッシュから検索して返す( num );
			cwav.tコピーfrom( wc変更前 );
			cwav.tコピーto( this.listViewWAVリスト.Items[ num - 1 ] );
			this._Form.tWAV_BMP_AVIリストのカーソルを全部同じ行に合わせる( cwav.nWAV番号1to3843 - 1 );
			this._Form.tタブを選択する( CMainForm.Eタブ種別.WAV );
			this.listViewWAVリスト.Refresh();
		}
		public void tサウンドプロパティを開いて編集する( int nWAV番号1to3843, string str相対パスの基本フォルダ )
		{
			this._Form.dlgチップパレット.t一時的に隠蔽する();
			CWAV cwav = this.tWAVをキャッシュから検索して返す_なければ新規生成する( nWAV番号1to3843 );
			ListViewItem item = cwav.t現在の内容から新しいListViewItemを作成して返す();
			string directoryName = "";
			if( item.SubItems[ 2 ].Text.Length > 0 )
			{
				directoryName = Path.GetDirectoryName( this._Form.strファイルの存在するディレクトリを絶対パスで返す( item.SubItems[ 2 ].Text ) );
			}
			CSoundPropertiesDialog cサウンドプロパティダイアログ = new CSoundPropertiesDialog( str相対パスの基本フォルダ, directoryName, new DGサウンドを再生する( this.tプレビュー音を再生する ) );
			cサウンドプロパティダイアログ.wav = cwav;
			cサウンドプロパティダイアログ.textBoxラベル.Text = item.SubItems[ 0 ].Text;
			cサウンドプロパティダイアログ.textBoxWAV番号.Text = item.SubItems[ 1 ].Text;
			cサウンドプロパティダイアログ.textBoxファイル.Text = item.SubItems[ 2 ].Text;
			cサウンドプロパティダイアログ.textBox音量.Text = item.SubItems[ 3 ].Text;
			cサウンドプロパティダイアログ.textBox位置.Text = item.SubItems[ 4 ].Text;
			cサウンドプロパティダイアログ.hScrollBar音量.Value = cサウンドプロパティダイアログ.wav.n音量0to127;
			cサウンドプロパティダイアログ.hScrollBar位置.Value = cサウンドプロパティダイアログ.wav.n位置_0to127 + 127;
			cサウンドプロパティダイアログ.checkBoxBGM.CheckState = cサウンドプロパティダイアログ.wav.bBGMとして使用 ? CheckState.Checked : CheckState.Unchecked;
			cサウンドプロパティダイアログ.textBoxWAV番号.ForeColor = item.ForeColor;
			cサウンドプロパティダイアログ.textBoxWAV番号.BackColor = item.BackColor;
			if( cサウンドプロパティダイアログ.ShowDialog() == DialogResult.OK )
			{
				CWAV wav = cサウンドプロパティダイアログ.wav;
				CWAV cwav3 = new CWAV();
				cwav3.nWAV番号1to3843 = cサウンドプロパティダイアログ.wav.nWAV番号1to3843;
				cwav3.strラベル名 = cサウンドプロパティダイアログ.textBoxラベル.Text;
				cwav3.strファイル名 = cサウンドプロパティダイアログ.textBoxファイル.Text;
				cwav3.n音量0to127 = cサウンドプロパティダイアログ.hScrollBar音量.Value;
				cwav3.n位置_0to127 = cサウンドプロパティダイアログ.hScrollBar位置.Value - 127;
				cwav3.bBGMとして使用 = cサウンドプロパティダイアログ.checkBoxBGM.Checked;
				cwav3.col文字色 = cサウンドプロパティダイアログ.textBoxWAV番号.ForeColor;
				cwav3.col背景色 = cサウンドプロパティダイアログ.textBoxWAV番号.BackColor;
				if( !cwav3.b内容が同じwith( wav ) )
				{
					wav = new CWAV();
					wav.tコピーfrom( cサウンドプロパティダイアログ.wav );
					this._Form.mgrUndoRedo管理者.tノードを追加する( new CUndoRedoCell<CWAV>( null, new DGUndoを実行する<CWAV>( this.tWAV編集のUndo ), new DGRedoを実行する<CWAV>( this.tWAV編集のRedo ), wav, cwav3 ) );
					this._Form.tUndoRedo用GUIの有効_無効を設定する();
					cサウンドプロパティダイアログ.wav.tコピーfrom( cwav3 );
					if( this.tWAV番号に対応するListViewItemを返す( nWAV番号1to3843 ) != null )
					{
						ListViewItem item2 = cサウンドプロパティダイアログ.wav.t現在の内容から新しいListViewItemを作成して返す();
						item = this.tWAV番号に対応するListViewItemを返す( nWAV番号1to3843 );
						item.SubItems[ 0 ].Text = item2.SubItems[ 0 ].Text;
						item.SubItems[ 1 ].Text = item2.SubItems[ 1 ].Text;
						item.SubItems[ 2 ].Text = item2.SubItems[ 2 ].Text;
						item.SubItems[ 3 ].Text = item2.SubItems[ 3 ].Text;
						item.SubItems[ 4 ].Text = item2.SubItems[ 4 ].Text;
						item.SubItems[ 5 ].Text = item2.SubItems[ 5 ].Text;
						item.ForeColor = item2.ForeColor;
						item.BackColor = item2.BackColor;
					}
					this.listViewWAVリスト.Refresh();
					this._Form.b未保存 = true;
				}
			}
			this._Form.dlgチップパレット.t一時的な隠蔽を解除する();
		}
		public void tファイル名の相対パス化( string str基本フォルダ名 )
		{
			for( int i = 1; i <= 62 * 62 - 1; i++ )
			{
				CWAV cwav = this.WAVキャッシュ.tWAVをキャッシュから検索して返す( i );
				if( ( cwav != null ) && ( cwav.strファイル名.Length > 0 ) )
				{
					try
					{
						Uri uri = new Uri( str基本フォルダ名 );
						cwav.strファイル名 = Uri.UnescapeDataString( uri.MakeRelativeUri( new Uri( cwav.strファイル名 ) ).ToString() ).Replace( '/', '\\' );
					}
					catch( UriFormatException )
					{
					}
				}
			}
		}
		public void tプレビュー音を再生する( CWAV wc )
		{
			if( ( wc != null ) && ( wc.strファイル名.Length != 0 ) )
			{
				string str = this._Form.strファイルの存在するディレクトリを絶対パスで返す( wc.strファイル名 );
				try
				{
					this.tプレビュー音を停止する();
					this.soundPreview = this.sound管理.tGenerateSound( str );
					this.soundPreview.nVolume = wc.n音量0to127;
					this.soundPreview.nPosition = wc.n位置_0to127;
					this.soundPreview.tStartPlaying();
				}
				catch
				{
				}
			}
		}
		public void tプレビュー音を再生する( int nWAV番号1to3843 )
		{
			CWAV wc = this.WAVキャッシュ.tWAVをキャッシュから検索して返す( nWAV番号1to3843 );
			this.tプレビュー音を再生する( wc );
		}
		public void tプレビュー音を停止する()
		{
			if( this.soundPreview != null )
			{
				this.soundPreview.tStopPlayback();
			}
		}
		public void t行交換のRedo( int n変更前のItem番号0to3842, int n変更後のItem番号0to3842 )
		{
			CUndoRedoManager.bUndoRedoした直後 = true;
			this.tItemを交換する( n変更前のItem番号0to3842, n変更後のItem番号0to3842 );
		}
		public void t行交換のUndo( int n変更前のItem番号0to3842, int n変更後のItem番号0to3842 )
		{
			CUndoRedoManager.bUndoRedoした直後 = true;
			this.tItemを交換する( n変更前のItem番号0to3842, n変更後のItem番号0to3842 );
		}
		public void t新規生成のRedo( CWAV wc生成前はNull, CWAV wc生成されたWAVの複製 )
		{
			int num = wc生成されたWAVの複製.nWAV番号1to3843;
			CWAV cwav = this.WAVキャッシュ.tWAVをキャッシュから検索して返す_なければ新規生成する( num );
			cwav.tコピーfrom( wc生成されたWAVの複製 );
			cwav.tコピーto( this.listViewWAVリスト.Items[ num - 1 ] );
			this._Form.tタブを選択する( CMainForm.Eタブ種別.WAV );
			this.listViewWAVリスト.Refresh();
		}
		public void t新規生成のUndo( CWAV wc生成前はNull, CWAV wc生成されたWAVの複製 )
		{
			int num = wc生成されたWAVの複製.nWAV番号1to3843;
			new CWAV().tコピーto( this.listViewWAVリスト.Items[ num - 1 ] );
			this.WAVキャッシュ.tWAVをキャッシュから削除する( num );
			this._Form.tタブを選択する( CMainForm.Eタブ種別.WAV );
			this.listViewWAVリスト.Refresh();
		}

		#region [ private ]
		//-----------------
		private CMainForm _Form;
		private ListView listViewWAVリスト;
		private CSound soundPreview;
		private CSoundManager sound管理;
		private CWAVCache WAVキャッシュ = new CWAVCache();
		private TimerCallback timerDelegate;
		private System.Threading.Timer timer;

		private void tItemを交換する_ListViewItem( int nItem番号1, int nItem番号2 )
		{
			int num = nItem番号1 + 1;
			int num2 = nItem番号2 + 1;
			CWAV cwav = new CWAV();
			cwav.tコピーfrom( this.listViewWAVリスト.Items[ nItem番号1 ] );
			cwav.nWAV番号1to3843 = num2;
			CWAV cwav2 = new CWAV();
			cwav2.tコピーfrom( this.listViewWAVリスト.Items[ nItem番号2 ] );
			cwav2.nWAV番号1to3843 = num;
			cwav2.tコピーto( this.listViewWAVリスト.Items[ nItem番号1 ] );
			cwav.tコピーto( this.listViewWAVリスト.Items[ nItem番号2 ] );
		}
		private void tItemを交換する_WAVキャッシュ( int nItem番号1, int nItem番号2 )
		{
			int num = nItem番号1 + 1;
			int num2 = nItem番号2 + 1;
			CWAV wc = this.WAVキャッシュ.tWAVをキャッシュから検索して返す( num );
			CWAV cwav2 = this.WAVキャッシュ.tWAVをキャッシュから検索して返す( num2 );
			CWAV cwav3 = new CWAV();
			cwav3.tコピーfrom( wc );
			wc.tコピーfrom( cwav2 );
			wc.nWAV番号1to3843 = num;
			cwav2.tコピーfrom( cwav3 );
			cwav2.nWAV番号1to3843 = num2;
		}
		private void tItemを交換する_カーソル移動( int nItem番号1, int nItem番号2 )
		{
			this.tItemを選択する( nItem番号2 );
		}
		private void tItemを交換する_チップパレット( int nItem番号1, int nItem番号2 )
		{
			this._Form.dlgチップパレット.tパレットセルの番号を置換する( 0, nItem番号1 + 1, nItem番号2 + 1 );
		}
		private void tItemを交換する_レーン割付チップ( int nItem番号1, int nItem番号2 )
		{
			for( int i = 0; i < this._Form.mgr譜面管理者.listレーン.Count; i++ )
			{
				CLane cレーン = this._Form.mgr譜面管理者.listレーン[ i ];
				if( ( ( cレーン.eレーン種別 == CLane.E種別.WAV ) || ( cレーン.eレーン種別 == CLane.E種別.GtV ) ) || ( cレーン.eレーン種別 == CLane.E種別.BsV ) )
				{
					if( cレーン.nレーン割付チップ_表0or1to3843 == ( nItem番号1 + 1 ) )
					{
						cレーン.nレーン割付チップ_表0or1to3843 = nItem番号2 + 1;
					}
					else if( cレーン.nレーン割付チップ_表0or1to3843 == ( nItem番号2 + 1 ) )
					{
						cレーン.nレーン割付チップ_表0or1to3843 = nItem番号1 + 1;
					}
					if( cレーン.nレーン割付チップ_裏0or1to3843 == ( nItem番号1 + 1 ) )
					{
						cレーン.nレーン割付チップ_裏0or1to3843 = nItem番号2 + 1;
					}
					else if( cレーン.nレーン割付チップ_裏0or1to3843 == ( nItem番号2 + 1 ) )
					{
						cレーン.nレーン割付チップ_裏0or1to3843 = nItem番号1 + 1;
					}
				}
			}
		}
		private void tItemを交換する_譜面上のチップ( int nItem番号1, int nItem番号2 )
		{
			foreach( KeyValuePair<int, CMeasure> pair in this._Form.mgr譜面管理者.dic小節 )
			{
				CMeasure c小節 = pair.Value;
				for( int i = 0; i < c小節.listチップ.Count; i++ )
				{
					CChip cチップ = c小節.listチップ[ i ];
					switch( this._Form.mgr譜面管理者.listレーン[ cチップ.nレーン番号0to ].eレーン種別 )
					{
						case CLane.E種別.WAV:
						case CLane.E種別.GtV:
						case CLane.E種別.BsV:
							if( cチップ.n値_整数1to3843 == ( nItem番号1 + 1 ) )
							{
								cチップ.n値_整数1to3843 = nItem番号2 + 1;
							}
							else if( cチップ.n値_整数1to3843 == ( nItem番号2 + 1 ) )
							{
								cチップ.n値_整数1to3843 = nItem番号1 + 1;
							}
							break;
					}
				}
			}
		}
		//-----------------
		#endregion
	}
}
