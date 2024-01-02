﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using SharpDX;
using FDK;

namespace DTXMania
{
	internal class CActSelectPerfHistoryPanel : CActivity
	{
		// メソッド

		public CActSelectPerfHistoryPanel()
		{
            base.listChildActivities.Add( this.actステータスパネル = new CActSelectStatusPanel() );
            base.bNotActivated = true;
		}
		public void t選択曲が変更された()
		{
			CScore cスコア = CDTXMania.stageSongSelection.rSelectedScore;
			if( ( cスコア != null ) && !CDTXMania.stageSongSelection.bScrolling )
			{
				try
				{
					Bitmap image = new Bitmap( 800, 0xc3 );
					Graphics graphics = Graphics.FromImage( image );
					graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
					for ( int i = 0; i < 5; i++ )
					{
						if( ( cスコア.SongInformation.PerformanceHistory[ i ] != null ) && ( cスコア.SongInformation.PerformanceHistory[ i ].Length > 0 ) )
						{
							graphics.DrawString( cスコア.SongInformation.PerformanceHistory[ i ], this.ft表示用フォント, Brushes.Yellow, (float) 0f, (float) ( i * 36f ) );
						}
					}
					graphics.Dispose();
					if( this.tx文字列パネル != null )
					{
						this.tx文字列パネル.Dispose();
					}
					this.tx文字列パネル = new CTexture( CDTXMania.app.Device, image, CDTXMania.TextureFormat );
					this.tx文字列パネル.vcScaleRatio = new Vector3( 0.5f, 0.5f, 1f );
					image.Dispose();
				}
				catch( CTextureCreateFailedException )
				{
					Trace.TraceError( "演奏履歴文字列テクスチャの作成に失敗しました。" );
					this.tx文字列パネル = null;
				}
			}
		}


		// CActivity 実装

		public override void OnActivate()
		{
			this.ft表示用フォント = new Font( "メイリオ", 26f, FontStyle.Bold, GraphicsUnit.Pixel );
			base.OnActivate();
		}
		public override void OnDeactivate()
		{
			if( this.ft表示用フォント != null )
			{
				this.ft表示用フォント.Dispose();
				this.ft表示用フォント = null;
			}
			this.ct登場アニメ用 = null;
			base.OnDeactivate();
		}
		public override void OnManagedCreateResources()
		{
			if( !base.bNotActivated )
			{
				this.txパネル本体 = CDTXMania.tGenerateTexture( CSkin.Path( @"Graphics\5_play history panel.png" ), true );
                this.t選択曲が変更された();
				base.OnManagedCreateResources();
			}
		}
		public override void OnManagedReleaseResources()
		{
			if( !base.bNotActivated )
			{
				CDTXMania.tReleaseTexture( ref this.txパネル本体 );
				CDTXMania.tReleaseTexture( ref this.tx文字列パネル );
                base.OnManagedReleaseResources();
			}
		}
		public override int OnUpdateAndDraw()
		{
			if( !base.bNotActivated )
			{
				if( base.bJustStartedUpdate )
				{
					this.ct登場アニメ用 = new CCounter( 0, 100, 5, CDTXMania.Timer );
					base.bJustStartedUpdate = false;
				}
				this.ct登場アニメ用.tUpdate();

                if ( this.actステータスパネル.txパネル本体 != null )
                    this.n本体X = 700;
                else
                    this.n本体X = 210;

				if( this.ct登場アニメ用.bReachedEndValue )
				{
					this.n本体Y = 0x23a;
				}
				else
				{
					double num = ( (double) this.ct登場アニメ用.nCurrentValue ) / 100.0;
					double num2 = Math.Cos( ( 1.5 + ( 0.5 * num ) ) * Math.PI );
					this.n本体Y = 0x23a + ( (int) ( this.txパネル本体.szImageSize.Height * ( 1.0 - ( num2 * num2 ) ) ) );
				}

				if( this.txパネル本体 != null )
				{
					this.txパネル本体.tDraw2D( CDTXMania.app.Device, this.n本体X, this.n本体Y );

                    if ( this.tx文字列パネル != null )
                        this.tx文字列パネル.tDraw2D( CDTXMania.app.Device, this.n本体X + 18, this.n本体Y + 0x20 );
				}
			}
			return 0;
		}
		

		// Other

		#region [ private ]
		//-----------------
		private CCounter ct登場アニメ用;
		private Font ft表示用フォント;
		private int n本体X;
		private int n本体Y;
		private CTexture txパネル本体;
		private CTexture tx文字列パネル;
        private CActSelectStatusPanel actステータスパネル;
        //-----------------
		#endregion
	}
}
