﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using SharpDX;
using FDK;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Color = System.Drawing.Color;

namespace DTXMania
{
    internal class CActPerfSkillMeter : CActivity
    {
        // グラフ仕様
        // _ギターとベースで同時にグラフを出すことはない。
        //
        // _目標のメーター画像
        //   →ゴーストがあった
        // 　　_ゴーストに基づいたグラフ(リアルタイム比較)
        // 　→なかった
        // 　　_ScoreIniの自己ベストのグラフ
        //

        private STDGBVALUE<int> nGraphBG_XPos = new STDGBVALUE<int>(); //ドラムにも座標指定があるためDGBVALUEとして扱う。
        private int nGraphBG_YPos = 200;
        private int DispHeight = 400;
        private int DispWidth = 60;
        private CCounter counterYposInImg = null;
        private readonly int slices = 10;
        private int nGraphUsePart = 0;
        private int[] nGraphGauge_XPos = new int[2];
        private int nPart = 0;

        public bool bIsTrainingMode = false;

        // プロパティ

        public double dbグラフ値現在_渡
        {
            get
            {
                return this.dbグラフ値現在;
            }
            set
            {
                this.dbグラフ値現在 = value;
            }
        }
        public double dbGraphValue_Goal
        {
            get
            {
                return this.dbグラフ値目標;
            }
            set
            {
                this.dbグラフ値目標 = value;
            }
        }
        public int[] n現在のAutoを含まない判定数_渡
        {
            get
            {
                return this.n現在のAutoを含まない判定数;
            }
            set
            {
                this.n現在のAutoを含まない判定数 = value;
            }
        }

        // コンストラクタ

        public CActPerfSkillMeter()
        {
            base.bNotActivated = true;
        }


        // CActivity 実装

        public override void OnActivate()
        {
            this.dbグラフ値目標 = 0f;
            this.dbグラフ値現在 = 0f;

            this.n現在のAutoを含まない判定数 = new int[6];

            base.OnActivate();
        }
        public override void OnDeactivate()
        {
            base.OnDeactivate();
        }
        public override void OnManagedCreateResources()
        {
            if (!base.bNotActivated)
            {
                //this.pfNameFont = new CPrivateFastFont( new FontFamily( "Arial" ), 16, FontStyle.Bold );
                this.txグラフ = CDTXMania.tGenerateTexture(CSkin.Path(@"Graphics\7_Graph_Main.png"));
                this.txグラフ_ゲージ = CDTXMania.tGenerateTexture(CSkin.Path(@"Graphics\7_Graph_Gauge.png"));

                //if( this.pfNameFont != null )
                //{
                //    if( CDTXMania.ConfigIni.eTargetGhost.Drums == ETargetGhostData.PERFECT )
                //    {
                //        this.txPlayerName = this.t指定された文字テクスチャを生成する( "DJ AUTO" );
                //    }
                //    else if( CDTXMania.ConfigIni.eTargetGhost.Drums == ETargetGhostData.LAST_PLAY )
                //    {
                //        this.txPlayerName = this.t指定された文字テクスチャを生成する( "LAST PLAY" );
                //    }
                //}
                base.OnManagedCreateResources();
            }
        }
        public override void OnManagedReleaseResources()
        {
            if (!base.bNotActivated)
            {
                CDTXMania.tReleaseTexture(ref this.txグラフ);
                CDTXMania.tReleaseTexture(ref this.txグラフ_ゲージ);
                CDTXMania.tReleaseTexture(ref this.txグラフ値自己ベストライン);
                base.OnManagedReleaseResources();
            }
        }
        public override int OnUpdateAndDraw()
        {
            if (!base.bNotActivated)
            {
                if (base.bJustStartedUpdate)
                {
                    //座標などの定義は初回だけにする。
                    //2016.03.29 kairera0467 非セッション譜面で、譜面が無いパートでグラフを有効にしている場合、譜面があるパートに一時的にグラフを切り替える。
                    //                       時間がなくて雑なコードになったため、後日最適化を行う。
                    if (CDTXMania.ConfigIni.bDrumsEnabled)
                    {
                        this.nPart = 0;
                        this.nGraphUsePart = 0;
                    }
                    else if (CDTXMania.ConfigIni.bGuitarEnabled)
                    {
                        this.nGraphUsePart = (CDTXMania.ConfigIni.bGraph有効.Guitar == true) ? 1 : 2;
                        if (CDTXMania.DTX.bチップがある.Guitar)
                            this.nPart = CDTXMania.ConfigIni.bGraph有効.Guitar ? 0 : 1;
                        else if (!CDTXMania.DTX.bチップがある.Guitar && CDTXMania.ConfigIni.bGraph有効.Guitar)
                        {
                            this.nPart = 1;
                            this.nGraphUsePart = 2;
                        }

                        if (!CDTXMania.DTX.bチップがある.Bass && CDTXMania.ConfigIni.bGraph有効.Bass)
                            this.nPart = 0;
                    }

                    this.nGraphBG_XPos.Drums = (CDTXMania.ConfigIni.bSmallGraph ? 885 : 905);//850 : 870
                    this.nGraphBG_XPos.Guitar = 356;
                    this.nGraphBG_XPos.Bass = 647;
                    this.nGraphBG_YPos = this.nGraphUsePart == 0 ? 87 : 110;
                    //2016.06.24 kairera0467 StatusPanelとSkillMaterの場合はX座標を調整する。
                    if (CDTXMania.ConfigIni.nInfoType == 1)
                    {
                        this.nGraphBG_XPos.Guitar = 629 + 9;
                        this.nGraphBG_XPos.Bass = 403;
                    }

                    if (CDTXMania.ConfigIni.eTargetGhost[this.nGraphUsePart] != ETargetGhostData.NONE)
                    {
                        if (CDTXMania.listTargetGhostScoreData[this.nGraphUsePart] != null)
                        {
                            //this.dbグラフ値目標 = CDTXMania.listTargetGhostScoreData[ this.nGraphUsePart ].dbPerformanceSkill;
                            this.dbグラフ値目標_表示 = CDTXMania.listTargetGhostScoreData[this.nGraphUsePart].dbPerformanceSkill;
                        }
                    }

                    this.nGraphGauge_XPos = new int[] { 3, 205 };

                    base.bJustStartedUpdate = false;
                }

                int stYposInImg = 0;
                int nGraphSizeOffset = (CDTXMania.ConfigIni.bSmallGraph ? -19 : 0);

                if (this.txグラフ != null)
                {
                    //背景
                    this.txグラフ.vcScaleRatio = new Vector3(1f, 1f, 1f);
                    if (CDTXMania.ConfigIni.bSmallGraph)
                    {
                        this.txグラフ.tDraw2D(CDTXMania.app.Device, nGraphBG_XPos[this.nGraphUsePart], nGraphBG_YPos, new Rectangle(452, 0, 107, 590));
                    }
                    else
                    {
                        this.txグラフ.tDraw2D(CDTXMania.app.Device, nGraphBG_XPos[this.nGraphUsePart], nGraphBG_YPos, new Rectangle(1, 0, 252, 590));
                    }

                    //自己ベスト数値表示
                    if (CDTXMania.ConfigIni.bSmallGraph)
                    {
                        this.t達成率文字表示(nGraphBG_XPos[this.nGraphUsePart] - 3, nGraphBG_YPos + 531, string.Format("{0,6:##0.00}" + "%", this.dbGraphValue_PersonalBest));
                    }
                    else
                    {
                        this.t達成率文字表示(nGraphBG_XPos[this.nGraphUsePart] + 136, nGraphBG_YPos + 501, string.Format("{0,6:##0.00}" + "%", this.dbGraphValue_PersonalBest));
                    }
                }

                //ゲージ現在
                if (this.txグラフ_ゲージ != null)
                {
                    //ゲージ本体
                    int nGaugeSize = (int)(434.0f * (float)this.dbグラフ値現在 / 100.0f);
                    int nPosY = this.nGraphUsePart == 0 ? 527 - nGaugeSize : 587 - nGaugeSize;
                    if (!this.bIsTrainingMode)
                    {
                        this.txグラフ_ゲージ.nTransparency = 255;
                        this.txグラフ_ゲージ.tDraw2D(CDTXMania.app.Device, nGraphBG_XPos[this.nGraphUsePart] + 41 + nGraphSizeOffset, nPosY, new Rectangle(2, 2, 30, nGaugeSize));
                    }
                    //ゲージ比較
                    int nTargetGaugeSize = (int)(434.0f * ((float)this.dbグラフ値目標 / 100.0f));
                    int nTargetGaugePosY = this.nGraphUsePart == 0 ? 527 - nTargetGaugeSize : 587 - nTargetGaugeSize;
                    int nTargetGaugeRectX = this.dbグラフ値現在 > this.dbグラフ値目標 ? 38 : 74;
                    this.txグラフ_ゲージ.nTransparency = 255;
                    this.txグラフ_ゲージ.tDraw2D(CDTXMania.app.Device, nGraphBG_XPos[this.nGraphUsePart] + 71 + nGraphSizeOffset, nTargetGaugePosY, new Rectangle(nTargetGaugeRectX, 2, 30, nTargetGaugeSize));
                    if (this.txグラフ != null)
                    {
                        //ターゲット達成率数値

                        //ターゲット名
                        //現在
                        this.txグラフ.tDraw2D(CDTXMania.app.Device, nGraphBG_XPos[this.nGraphUsePart] + 41 + nGraphSizeOffset, nGraphBG_YPos + 357, new Rectangle(260, 2, 30, 120));
                        //比較対象
                        this.txグラフ.tDraw2D(CDTXMania.app.Device, nGraphBG_XPos[this.nGraphUsePart] + 71 + nGraphSizeOffset, nGraphBG_YPos + 357, new Rectangle(260 + (30 * ((int)CDTXMania.ConfigIni.eTargetGhost[this.nGraphUsePart])), 2, 30, 120));

                        //以下使用予定
                        if (!CDTXMania.ConfigIni.bSmallGraph)
                        {
                            //最終プレイ
                            this.txグラフ.tDraw2D(CDTXMania.app.Device, nGraphBG_XPos[this.nGraphUsePart] + 102, nGraphBG_YPos + 357, new Rectangle(260 + 60, 2, 30, 120));
                            //自己ベスト
                            this.txグラフ.tDraw2D(CDTXMania.app.Device, nGraphBG_XPos[this.nGraphUsePart] + 132, nGraphBG_YPos + 357, new Rectangle(260 + 90, 2, 30, 120));
                            //最高スコア
                            this.txグラフ.tDraw2D(CDTXMania.app.Device, nGraphBG_XPos[this.nGraphUsePart] + 160, nGraphBG_YPos + 357, new Rectangle(260 + 120, 2, 30, 120));
                        }
                    }
                    this.t比較文字表示(nGraphBG_XPos[this.nGraphUsePart] + 40 + nGraphSizeOffset, nPosY - 10, string.Format("{0,5:##0.00}", Math.Abs(this.dbグラフ値現在)));
                    this.t比較文字表示(nGraphBG_XPos[this.nGraphUsePart] + 70 + nGraphSizeOffset, nTargetGaugePosY - 10, string.Format("{0,5:##0.00}", Math.Abs(this.dbグラフ値目標)));
                }
            }
            return 0;
        }

        // Other

        #region [ private ]
        //----------------
        private double dbグラフ値目標;
        private double dbグラフ値目標_表示;
        private double dbグラフ値現在;
        private double dbグラフ値現在_表示;
        public double dbGraphValue_PersonalBest;
        private int[] n現在のAutoを含まない判定数;

        private CTexture txPlayerName;
        private CTexture txグラフ;
        private CTexture txグラフ_ゲージ;
        private CTexture txグラフ値自己ベストライン;

        private CPrivateFastFont pfNameFont;

        [StructLayout(LayoutKind.Sequential)]
        private struct ST文字位置
        {
            public char ch;
            public Point pt;
            public ST文字位置(char ch, Point pt)
            {
                this.ch = ch;
                this.pt = pt;
            }
        }

        private ST文字位置[] st比較数字位置 = new ST文字位置[]{
            new ST文字位置( '0', new Point( 0, 0 ) ),
            new ST文字位置( '1', new Point( 8, 0 ) ),
            new ST文字位置( '2', new Point( 16, 0 ) ),
            new ST文字位置( '3', new Point( 24, 0 ) ),
            new ST文字位置( '4', new Point( 32, 0 ) ),
            new ST文字位置( '5', new Point( 40, 0 ) ),
            new ST文字位置( '6', new Point( 48, 0 ) ),
            new ST文字位置( '7', new Point( 56, 0 ) ),
            new ST文字位置( '8', new Point( 64, 0 ) ),
            new ST文字位置( '9', new Point( 72, 0 ) ),
            new ST文字位置( '.', new Point( 80, 0 ) )
        };
        private ST文字位置[] st達成率数字位置 = new ST文字位置[]{
            new ST文字位置( '0', new Point( 0, 0 ) ),
            new ST文字位置( '1', new Point( 16, 0 ) ),
            new ST文字位置( '2', new Point( 32, 0 ) ),
            new ST文字位置( '3', new Point( 48, 0 ) ),
            new ST文字位置( '4', new Point( 64, 0 ) ),
            new ST文字位置( '5', new Point( 80, 0 ) ),
            new ST文字位置( '6', new Point( 96, 0 ) ),
            new ST文字位置( '7', new Point( 112, 0 ) ),
            new ST文字位置( '8', new Point( 128, 0 ) ),
            new ST文字位置( '9', new Point( 144, 0 ) ),
            new ST文字位置( '.', new Point( 160, 0 ) ),
            new ST文字位置( '%', new Point( 168, 0 ) ),
        };
        private void t比較文字表示(int x, int y, string str)
        {
            foreach (char ch in str)
            {
                for (int i = 0; i < this.st比較数字位置.Length; i++)
                {
                    if (this.st比較数字位置[i].ch == ch)
                    {
                        int RectX = 8;
                        if (ch == '.') RectX = 2;
                        Rectangle rectangle = new Rectangle(260 + this.st比較数字位置[i].pt.X, 162, RectX, 10);
                        if (this.txグラフ != null)
                        {
                            this.txグラフ.nTransparency = 255;
                            this.txグラフ.tDraw2D(CDTXMania.app.Device, x, y, rectangle);
                        }
                        break;
                    }
                }
                if (ch == '.') x += 2;
                else x += 7;
            }
        }
        private void t達成率文字表示(int x, int y, string str)
        {
            foreach (char ch in str)
            {
                for (int i = 0; i < this.st達成率数字位置.Length; i++)
                {
                    if (this.st達成率数字位置[i].ch == ch)
                    {
                        int RectX = 16;
                        if (ch == '.') RectX = 8;
                        Rectangle rectangle = new Rectangle(260 + this.st達成率数字位置[i].pt.X, 128, RectX, 28);
                        if (this.txグラフ != null)
                        {
                            this.txグラフ.nTransparency = 255;
                            this.txグラフ.tDraw2D(CDTXMania.app.Device, x, y, rectangle);
                        }
                        break;
                    }
                }
                if (ch == '.') x += 8;
                else x += 16;
            }
        }
        private CTexture t指定された文字テクスチャを生成する(string str文字)
        {
            Bitmap bmp;
            bmp = this.pfNameFont.DrawPrivateFont(str文字, Color.White, Color.Transparent);

            CTexture tx文字テクスチャ = CDTXMania.tGenerateTexture(bmp, false);

            if (tx文字テクスチャ != null)
                tx文字テクスチャ.vcScaleRatio = new Vector3(1.0f, 1.0f, 1f);

            bmp.Dispose();

            return tx文字テクスチャ;
        }
        private void t折れ線を描画する(int nBoardPosA, int nBoardPosB)
        {
            //やる気がまるでない線
            //2016.03.28 kairera0467 ギター画面では1Pと2Pで向きが変わるが、そこは残念ながら未対応。
            //参考 http://dobon.net/vb/dotnet/graphics/drawline.html
            if (this.txグラフ値自己ベストライン == null)
            {
                Bitmap canvas = new Bitmap(280, 720);

                Graphics g = Graphics.FromImage(canvas);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                int nMybestGaugeSize = (int)(560.0f * (float)this.dbGraphValue_PersonalBest / 100.0f);
                int nMybestGaugePosY = 600 - nMybestGaugeSize;

                int nTargetGaugeSize = (int)(560.0f * (float)this.dbグラフ値目標_表示 / 100.0f);
                int nTargetGaugePosY = 600 - nTargetGaugeSize;

                Point[] posMybest = {
                    new Point( 3, nMybestGaugePosY ),
                    new Point( 75, nMybestGaugePosY ),
                    new Point( 94, nBoardPosA + 31 ),
                    new Point( 102, nBoardPosA + 31 )
                };

                Point[] posTarget = {
                    new Point( 3, nTargetGaugePosY ),
                    new Point( 75, nTargetGaugePosY ),
                    new Point( 94, nBoardPosB + 59 ),
                    new Point( 102, nBoardPosB + 59 )
                };

                if (this.nGraphUsePart == 2)
                {
                    posMybest = new Point[]{
                        new Point( 271, nMybestGaugePosY ),
                        new Point( 206, nMybestGaugePosY ),
                        new Point( 187, nBoardPosA + 31 ),
                        new Point( 178, nBoardPosA + 31 )
                    };

                    posTarget = new Point[]{
                        new Point( 271, nTargetGaugePosY ),
                        new Point( 206, nTargetGaugePosY ),
                        new Point( 187, nBoardPosB + 59 ),
                        new Point( 178, nBoardPosB + 59 )
                    };
                }

                Pen penMybest = new Pen(Color.Pink, 2);
                g.DrawLines(penMybest, posMybest);

                if (CDTXMania.listTargetGhsotLag[this.nGraphUsePart] != null && CDTXMania.listTargetGhostScoreData[this.nGraphUsePart] != null)
                {
                    Pen penTarget = new Pen(Color.Orange, 2);
                    g.DrawLines(penTarget, posTarget);
                }

                g.Dispose();

                this.txグラフ値自己ベストライン = new CTexture(CDTXMania.app.Device, canvas, CDTXMania.TextureFormat, false);
            }
            if (this.txグラフ値自己ベストライン != null)
                this.txグラフ値自己ベストライン.tDraw2D(CDTXMania.app.Device, nGraphBG_XPos[this.nGraphUsePart], nGraphBG_YPos);
        }
        //-----------------
        #endregion
    }
}
