﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using SharpDX;
using FDK;

namespace DTXMania
{
	/// <summary>
	/// CActPerfDrumsGauge と CAct演奏Gutiarゲージ のbaseクラス。ダメージ計算やDanger/Failed判断もこのクラスで行う。
	/// </summary>
	internal class CActPerfCommonGauge : CActivity
	{
		// プロパティ
		public CActLVLNFont actLVLNFont { get; protected set; }

		// コンストラクタ
		public CActPerfCommonGauge()
		{
            //actLVLNFont = new CActLVLNFont(); // OnActivate()に移動
			//actLVLNFont.OnActivate();
		}
        // CActivity 実装
        public override void OnActivate()
        {
            actLVLNFont = new CActLVLNFont();
            actLVLNFont.OnActivate();
            base.OnActivate();
        }

        public override void OnDeactivate()
        {
            actLVLNFont.OnDeactivate();
            actLVLNFont = null;
            base.OnDeactivate();
        }
		
		
		const double GAUGE_MAX = 1.0;
		const double GAUGE_INITIAL =  2.0 / 3;
		const double GAUGE_MIN = -0.1;
		const double GAUGE_ZERO = 0.0;
		const double GAUGE_DANGER = 0.3;
	
		public bool bRisky							// Riskyモードか否か
		{
			get;
			private set;
		}
		public int nRiskyTimes_Initial				// Risky初期値
		{
			get;
			private set;
		}
		public int nRiskyTimes						// 残Miss回数
		{
			get;
			private set;
		}
		public bool IsFailed( EInstrumentPart part )	// 閉店状態になったかどうか
		{
			if ( bRisky ) 
            {
				return ( nRiskyTimes <= 0 );
			}
			return this.db現在のゲージ値[ (int) part ] <= GAUGE_MIN;
		}
		public bool IsDanger( EInstrumentPart part )	// DANGERかどうか
		{
			if ( bRisky )
			{
				switch ( nRiskyTimes_Initial ) {
					case 1:
						return false;
					case 2:
					case 3:
						return ( nRiskyTimes <= 1 );
					default: 
						return ( nRiskyTimes <= 2 );
				}
			}
			return ( this.db現在のゲージ値[ (int) part ] <= 0.3 );
		}

		public double dbゲージ値	// Drums専用
		{
			get
			{
				return this.db現在のゲージ値.Drums;
			}
			set
			{
				this.db現在のゲージ値.Drums = value;
				if ( this.db現在のゲージ値.Drums > GAUGE_MAX )
				{
					this.db現在のゲージ値.Drums = GAUGE_MAX;
				}
			}
		}


		/// <summary>
		/// ゲージの初期化
		/// </summary>
		/// <param name="nRiskyTimes_Initial_">Riskyの初期値(0でRisky未使用)</param>
		public void Init(int nRiskyTimes_InitialVal )		// ゲージ初期化
		{
			nRiskyTimes_Initial = nRiskyTimes_InitialVal;
			nRiskyTimes = nRiskyTimes_InitialVal;
			bRisky = ( this.nRiskyTimes > 0 );

			for ( int i = 0; i < 3; i++ )
			{
				if ( !bRisky)
				{
                    this.db現在のゲージ値[i] = GAUGE_INITIAL;
				}
				else if ( nRiskyTimes_InitialVal == 1 )
				{
					this.db現在のゲージ値[ i ] = GAUGE_ZERO;
				}
				else
				{
					this.db現在のゲージ値[ i ] = GAUGE_MAX;
				}
			}
		}

		#region [ DAMAGE ]
#if true		// DAMAGELEVELTUNING
		#region [ DAMAGELEVELTUNING ]
		// ----------------------------------
		public float[ , ] fDamageGaugeDelta = {			// #23625 2011.1.10 ickw_284: tuned damage/recover factors
			// drums,   guitar,  bass
			{  0.004f, 0.006f,  0.006f  },
			{  0.002f,  0.003f,  0.003f  },
			{  0.000f,  0.000f,  0.000f  },
			{ -0.020f, -0.030f,	-0.030f  },
			{ -0.050f, -0.050f, -0.050f  }
		};
		public float[] fDamageLevelFactor = {
			0.25f, 0.5f, 0.75f //Original: 0.5f, 1.0f, 1.5f
		};
		// ----------------------------------
#endregion
#endif

		public void Damage( EInstrumentPart screenmode, EInstrumentPart part, EJudgement e今回の判定 )
		{
			double fDamage;

#if true	// DAMAGELEVELTUNING
            if (CDTXMania.ConfigIni.nSkillMode == 1)// 0: Classic 1: XG
            {
                fDamageGaugeDelta[0, 0] =  0.005f;
                fDamageGaugeDelta[1, 0] =  0.001f;
                fDamageGaugeDelta[3, 0] = -0.017f;
                fDamageGaugeDelta[4, 0] = -0.041f;
            }
			switch ( e今回の判定 )
			{
                case EJudgement.Perfect:
                    {
                        fDamage = bRisky ? 0 : fDamageGaugeDelta[(int)e今回の判定, (int)part];
                        break;
                    }
				case EJudgement.Great:

                    if (CDTXMania.ConfigIni.bHAZARD)
                    {
                        if (bRisky)
                        {
                            fDamage = (nRiskyTimes == 1) ? 0 : -GAUGE_MAX / (nRiskyTimes_Initial - 1);	// Risky=1のときは1Miss即閉店なのでダメージ計算しない
                            if (nRiskyTimes >= 0) nRiskyTimes--;		// 念のため-1未満には減らないようにしておく
                        }
                        else
                        {
                            fDamage = fDamageGaugeDelta[(int)4, (int)part];
                        }
                    }
                    else
                    {
                        fDamage  = bRisky ? 0 : fDamageGaugeDelta[ (int) e今回の判定, (int) part ];
                    }
                    break;
				case EJudgement.Good:

                    if (CDTXMania.ConfigIni.bHAZARD)
                    {
                        if (bRisky)
                        {
                            fDamage = (nRiskyTimes == 1) ? 0 : -GAUGE_MAX / (nRiskyTimes_Initial - 1);	// Risky=1のときは1Miss即閉店なのでダメージ計算しない
                            if (nRiskyTimes >= 0) nRiskyTimes--;		// 念のため-1未満には減らないようにしておく
                        }
                        else
                        {
                            fDamage = fDamageGaugeDelta[(int)4, (int)part];
                        }
                    }
                    else
                    {
    					fDamage  = bRisky ? 0 : fDamageGaugeDelta[ (int) e今回の判定, (int) part ];
                    }
					break;
				case EJudgement.Poor:
				case EJudgement.Miss:
					if ( bRisky )
					{
						fDamage = (nRiskyTimes == 1)? 0 : -GAUGE_MAX / ( nRiskyTimes_Initial - 1);	// Risky=1のときは1Miss即閉店なのでダメージ計算しない
						if (nRiskyTimes >= 0) nRiskyTimes--;		// 念のため-1未満には減らないようにしておく
					}
					else
					{
						fDamage = fDamageGaugeDelta[ (int) e今回の判定, (int) part ];
					}
					if ( e今回の判定 == EJudgement.Miss && !bRisky )
					{
						fDamage *= fDamageLevelFactor[ (int) CDTXMania.ConfigIni.eDamageLevel ];
					}
					break;

				default:
					fDamage = 0.0f;
					break;
			}
#else													// before applying #23625 modifications
			switch (e今回の判定)
			{
				case E判定.Perfect:
					fDamage = ( part == E楽器パート.DRUMS ) ? 0.01 : 0.015;
					break;

				case E判定.Great:
					fDamage = ( part == E楽器パート.DRUMS ) ? 0.006 : 0.009;
					break;

				case E判定.Good:
					fDamage = ( part == E楽器パート.DRUMS ) ? 0.002 : 0.003;
					break;

				case E判定.Poor:
					fDamage = ( part == E楽器パート.DRUMS ) ? 0.0 : 0.0;
					break;

				case E判定.Miss:
					fDamage = ( part == E楽器パート.DRUMS ) ? -0.035 : -0.035;
					switch( CDTXMania.ConfigIni.eダメージレベル )
					{
						case Eダメージレベル.少ない:
							fDamage *= 0.6;
							break;

						case Eダメージレベル.普通:
							fDamage *= 1.0;
							break;

						case Eダメージレベル.大きい:
							fDamage *= 1.6;
							break;
					}
					break;

				default:
					fDamage = 0.0;
					break;
			}
#endif
			if ( screenmode == EInstrumentPart.DRUMS )		// ドラム演奏画面なら、ギター/ベースのダメージも全部ドラムのゲージに集約する
			{
				part = EInstrumentPart.DRUMS;
				this.db現在のゲージ値[ (int) part ] += fDamage;
			}
			else
			{
				if ( this.bRisky )						// ギター画面且つRISKYなら、ギターとベースのゲージをセットで減少
				{
					this.db現在のゲージ値[ (int) EInstrumentPart.GUITAR ] += fDamage;
					this.db現在のゲージ値[ (int) EInstrumentPart.BASS   ] += fDamage;
				}
				else
				{
					this.db現在のゲージ値[ (int) part ] += fDamage;
				}
			}

			if ( this.db現在のゲージ値[ (int) part ] > GAUGE_MAX )		// RISKY時は決してゲージが増加しないので、ギタレボモード時のギター/ベース両チェック(上限チェック)はしなくて良い
				this.db現在のゲージ値[ (int) part ] = GAUGE_MAX;
		}
		//-----------------
		#endregion

		public STDGBVALUE<double> db現在のゲージ値;
        protected STDGBVALUE<int> n本体X;
		protected CCounter ct本体移動;
		protected CCounter ct本体振動;
        //protected CTexture txマスクF;
        //protected CTexture txマスクD;
        protected CTexture txゲージ;
        protected CTexture txフルゲージ;
        protected STDGBVALUE<CTexture> txフレーム;
        protected CTexture txハイスピ;
    }
}
