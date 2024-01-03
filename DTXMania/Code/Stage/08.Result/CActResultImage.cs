using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using SharpDX;
using FDK;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace DTXMania
{
    internal class CActResultImage : CActivity
    {

        // コンストラクタ

        public CActResultImage()
        {
            base.bNotActivated = true;
        }

        // メソッド

        public void tアニメを完了させる()
        {
            this.ct登場用.nCurrentValue = this.ct登場用.n終了値;
        }

        // CActivity 実装

        public override void OnActivate()
        {
            this.n本体X = 0x1d5;
            this.n本体Y = 0x11b;

            this.ftSongNameFont = new System.Drawing.Font("Impact", 24f, FontStyle.Regular, GraphicsUnit.Pixel);
            this.ftSongDifficultyFont = new System.Drawing.Font("Impact", 15f, FontStyle.Regular);
            this.ftSongNameFont = new System.Drawing.Font("ＤＦＧ平成ゴシック体W7", 21f, FontStyle.Regular, GraphicsUnit.Pixel);
            this.iDrumSpeed = Image.FromFile(CSkin.Path(@"Graphics\7_panel_icons.jpg"));
            this.txジャケットパネル = CDTXMania.tGenerateTexture(CSkin.Path(@"Graphics\7_JacketPanel.png"));
            base.OnActivate();

        }
        public override void OnDeactivate()
        {
            if (this.ct登場用 != null)
            {
                this.ct登場用 = null;
            }
            CDTXMania.tReleaseTexture(ref this.txジャケットパネル);
            base.OnDeactivate();
        }
        public override void OnManagedCreateResources()
        {
            if (!base.bNotActivated)
            {
                this.txDifficultyPanel = CDTXMania.tGenerateTexture(CSkin.Path(@"Graphics\Difficulty_Result.png"));
                this.txPartPanel = CDTXMania.tGenerateTexture(CSkin.Path(@"Graphics\6_Part_Small.png"));
                this.txリザルト画像がないときの画像 = CDTXMania.tGenerateTexture(CSkin.Path(@"Graphics\5_preimage default.png"));
                this.txジャケット枠 = CDTXMania.tGenerateTexture(CSkin.Path(@"Graphics\8_Jacket_waku.png"));
                this.tx曲枠 = CDTXMania.tGenerateTexture(CSkin.Path(@"Graphics\8_music_info.png"));
                if (CDTXMania.ConfigIni.bストイックモード)
                {
                    this.txリザルト画像 = this.txリザルト画像がないときの画像;
                }
                else if (((!this.tリザルト画像の指定があれば構築する()) && (!this.tプレビュー画像の指定があれば構築する())))
                {
                    this.txリザルト画像 = this.txリザルト画像がないときの画像;
                }

                #region[ Generation of song title, artist name and disclaimer textures ]
                if (string.IsNullOrEmpty(CDTXMania.DTX.TITLE) || (!CDTXMania.bCompactMode && CDTXMania.ConfigIni.b曲名表示をdefのものにする))
                    this.strSongName = CDTXMania.stageSongSelection.r現在選択中の曲.strタイトル;
                else
                    this.strSongName = CDTXMania.DTX.TITLE;

                CPrivateFastFont pfTitle = new CPrivateFastFont(new FontFamily(CDTXMania.ConfigIni.str選曲リストフォント), 20, FontStyle.Regular);
                Bitmap bmpSongName = pfTitle.DrawPrivateFont(this.strSongName, CPrivateFont.DrawMode.Edge, Color.Black, Color.Black, this.clGITADORAgradationTopColor, this.clGITADORAgradationBottomColor, true);
                this.txSongName = CDTXMania.tGenerateTexture(bmpSongName, false);
                bmpSongName.Dispose();
                pfTitle.Dispose();

                CPrivateFastFont pfArtist = new CPrivateFastFont(new FontFamily(CDTXMania.ConfigIni.str選曲リストフォント), 20, FontStyle.Regular);
                Bitmap bmpArtistName = pfArtist.DrawPrivateFont(CDTXMania.DTX.ARTIST, CPrivateFont.DrawMode.Edge, Color.Black, Color.Black, this.clGITADORAgradationTopColor, this.clGITADORAgradationBottomColor, true);
                this.txArtistName = CDTXMania.tGenerateTexture(bmpArtistName, false);
                bmpArtistName.Dispose();
                pfArtist.Dispose();

                if (CDTXMania.ConfigIni.nPlaySpeed != 20)
                {
                    double d = (double)(CDTXMania.ConfigIni.nPlaySpeed / 20.0);
                    String strModifiedPlaySpeed = "Play Speed: x" + d.ToString("0.000");
                    CPrivateFastFont pfModifiedPlaySpeed = new CPrivateFastFont(new FontFamily(CDTXMania.ConfigIni.str選曲リストフォント), 18, FontStyle.Regular);
                    Bitmap bmpModifiedPlaySpeed = pfModifiedPlaySpeed.DrawPrivateFont(strModifiedPlaySpeed, CPrivateFont.DrawMode.Edge, Color.White, Color.White, Color.Black, Color.Red, true);
                    this.txModifiedPlaySpeed = CDTXMania.tGenerateTexture(bmpModifiedPlaySpeed, false);
                    bmpModifiedPlaySpeed.Dispose();
                    pfModifiedPlaySpeed.Dispose();
                }

                if (CDTXMania.stageResult.bIsTrainingMode)
                {
                    String strResultsNotSavedTraining = "Training feature used";
                    CPrivateFastFont pfResultsNotSavedTraining = new CPrivateFastFont(new FontFamily(CDTXMania.ConfigIni.str選曲リストフォント), 18, FontStyle.Regular);
                    Bitmap bmpResultsNotSavedTraining = pfResultsNotSavedTraining.DrawPrivateFont(strResultsNotSavedTraining, CPrivateFont.DrawMode.Edge, Color.White, Color.White, Color.Black, Color.Red, true);
                    this.txTrainingMode = CDTXMania.tGenerateTexture(bmpResultsNotSavedTraining, false);
                    bmpResultsNotSavedTraining.Dispose();
                    pfResultsNotSavedTraining.Dispose();
                }

                String strResultsNotSaved = "Score will not be saved";
                CPrivateFastFont pfResultsNotSaved = new CPrivateFastFont(new FontFamily(CDTXMania.ConfigIni.str選曲リストフォント), 18, FontStyle.Regular);
                Bitmap bmpResultsNotSaved = pfResultsNotSaved.DrawPrivateFont(strResultsNotSaved, CPrivateFont.DrawMode.Edge, Color.White, Color.White, Color.Black, Color.Red, true);
                this.txResultsNotSaved = CDTXMania.tGenerateTexture(bmpResultsNotSaved, false);
                bmpResultsNotSaved.Dispose();
                pfResultsNotSaved.Dispose();
                #endregion

                Bitmap bitmap2 = new Bitmap(0x3a, 0x12);
                Graphics graphics = Graphics.FromImage(bitmap2);

                graphics.Dispose();
                this.txSongDifficulty = new CTexture(CDTXMania.app.Device, bitmap2, CDTXMania.TextureFormat, false);
                bitmap2.Dispose();
                Bitmap bitmap3 = new Bitmap(100, 100);
                graphics = Graphics.FromImage(bitmap3);
                float num;
                //If Skill Mode is CLASSIC, always display lvl as Classic Style
                if (CDTXMania.ConfigIni.nSkillMode == 0 || (CDTXMania.ConfigIni.bCLASSIC譜面判別を有効にする && 
                    (CDTXMania.DTX.bチップがある.LeftCymbal == false) && 
                    (CDTXMania.DTX.bチップがある.LP == false) && 
                    (CDTXMania.DTX.bチップがある.LBD == false) && 
                    (CDTXMania.DTX.bチップがある.FT == false) && 
                    (CDTXMania.DTX.bチップがある.Ride == false)))
                {
                    num = ((float)CDTXMania.stageSongSelection.rChosenScore.SongInformation.Level.Drums);
                }
                else
                {
                    if (CDTXMania.stageSongSelection.rChosenScore.SongInformation.Level.Drums > 100)
                    {
                        num = ((float)CDTXMania.stageSongSelection.rChosenScore.SongInformation.Level.Drums);
                    }
                    else
                    {
                        num = ((float)CDTXMania.stageSongSelection.rChosenScore.SongInformation.Level.Drums) / 10f;
                    }
                }
                //If Skill Mode is CLASSIC, always display lvl as Classic Style
                if (CDTXMania.ConfigIni.nSkillMode == 0 || (CDTXMania.ConfigIni.bCLASSIC譜面判別を有効にする && 
                    (CDTXMania.DTX.bチップがある.LeftCymbal == false) && 
                    (CDTXMania.DTX.bチップがある.LP == false) && 
                    (CDTXMania.DTX.bチップがある.LBD == false) && 
                    (CDTXMania.DTX.bチップがある.FT == false) && 
                    (CDTXMania.DTX.bチップがある.Ride == false) &&
                    (CDTXMania.DTX.b強制的にXG譜面にする == false)))
                {
                    graphics.DrawString(string.Format("{0:00}", num), this.ftSongDifficultyFont, new SolidBrush(Color.FromArgb(0xba, 0xba, 0xba)), (float)0f, (float)-4f);
                }
                else
                {
                    graphics.DrawString(string.Format("{0:0.00}", num), this.ftSongDifficultyFont, new SolidBrush(Color.FromArgb(0xba, 0xba, 0xba)), (float)0f, (float)-4f);
                }
                this.txSongLevel = new CTexture(CDTXMania.app.Device, bitmap3, CDTXMania.TextureFormat, false);
                graphics.Dispose();
                bitmap3.Dispose();
                Bitmap bitmap4 = new Bitmap(0x2a, 0x30);
                graphics = Graphics.FromImage(bitmap4);
                int speedTexturePosY = CDTXMania.ConfigIni.nScrollSpeed.Drums * 48 > 20 * 48 ? 20 * 48 : CDTXMania.ConfigIni.nScrollSpeed.Drums * 48;
                graphics.DrawImage(this.iDrumSpeed, new Rectangle(0, 0, 0x2a, 0x30), new Rectangle(0, speedTexturePosY, 0x2a, 0x30), GraphicsUnit.Pixel);
                this.txDrumSpeed = new CTexture(CDTXMania.app.Device, bitmap4, CDTXMania.TextureFormat, false);
                graphics.Dispose();
                //graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
                bitmap4.Dispose();
                base.OnManagedCreateResources();
            }
        }

        public override void OnManagedReleaseResources()
        {
            if (!base.bNotActivated)
            {
                CDTXMania.tReleaseTexture(ref this.txリザルト画像);
                CDTXMania.tReleaseTexture(ref this.txリザルト画像がないときの画像);
                CDTXMania.tReleaseTexture(ref this.txSongName);
                CDTXMania.tReleaseTexture(ref this.txArtistName);
                CDTXMania.tReleaseTexture(ref this.txModifiedPlaySpeed);
                CDTXMania.tReleaseTexture(ref this.txTrainingMode);
                CDTXMania.tReleaseTexture(ref this.txResultsNotSaved);
                CDTXMania.tReleaseTexture(ref this.r表示するリザルト画像);
                CDTXMania.tReleaseTexture(ref this.txSongLevel);
                CDTXMania.tReleaseTexture(ref this.txSongDifficulty);
                CDTXMania.tReleaseTexture(ref this.txDrumSpeed);
                CDTXMania.tReleaseTexture(ref this.txPartPanel);
                CDTXMania.tReleaseTexture(ref this.txDifficultyPanel);

                base.OnManagedReleaseResources();
            }
        }
        public override unsafe int OnUpdateAndDraw()
        {
            if (base.bNotActivated)
            {
                return 0;
            }
            if (base.bJustStartedUpdate)
            {
                this.ct登場用 = new CCounter(0, 100, 5, CDTXMania.Timer);
                base.bJustStartedUpdate = false;
            }
            this.ct登場用.tUpdate();
            int x = this.n本体X;
            int y = this.n本体Y;
            this.txジャケット枠.tDraw2D(CDTXMania.app.Device, 446, 127);
            this.tx曲枠.tDraw2D(CDTXMania.app.Device, 438, 530);
            //this.txジャケットパネル.tDraw2D(CDTXMania.app.Device, 517, 318);
            if (this.txリザルト画像 != null)
            {
                this.txリザルト画像.vcScaleRatio.X = 382.0f / this.txリザルト画像.szImageSize.Width;
                this.txリザルト画像.vcScaleRatio.Y = 382.0f / this.txリザルト画像.szImageSize.Height;
                //Matrix mat = Matrix.Identity;
                //mat *= Matrix.Scaling(245.0f / this.txリザルト画像.szImageSize.Width, 245.0f / this.txリザルト画像.szImageSize.Height, 1f);
                //mat *= Matrix.Translation(-28f, -94.5f, 0f);
                //mat *= Matrix.RotationZ(0.3f);

                this.txリザルト画像.tDraw2D(CDTXMania.app.Device, 450, 131);
            }

            if (this.txSongName.szImageSize.Width > 400)
                this.txSongName.vcScaleRatio.X = 400f / this.txSongName.szImageSize.Width;

            if (this.txArtistName.szImageSize.Width > 400)
                this.txArtistName.vcScaleRatio.X = 400f / this.txArtistName.szImageSize.Width;

            this.txSongName.tDraw2D(CDTXMania.app.Device, 448, 548);
            this.txArtistName.tDraw2D(CDTXMania.app.Device, 448, 588);

            if (this.txPartPanel != null && CDTXMania.ConfigIni.bDrumsEnabled)
            {
                if (CDTXMania.DTX.bチップがある.Drums)
                    this.txPartPanel.tDraw2D(CDTXMania.app.Device, 455, 624, new Rectangle(0, 0, 51, 25));
            }

            if (this.txPartPanel != null && CDTXMania.ConfigIni.bGuitarEnabled)
            {
                if (CDTXMania.DTX.bチップがある.Guitar)
                {
                    this.txPartPanel.tDraw2D(CDTXMania.app.Device, 455, 624, new Rectangle(0, 25, 51, 25));
                }

                if (this.txPartPanel != null && CDTXMania.ConfigIni.bGuitarEnabled)
                {
                    if (CDTXMania.DTX.bチップがある.Bass)
                    {
                            this.txPartPanel.tDraw2D(CDTXMania.app.Device, 664, 624, new Rectangle(0, 50, 51, 25));
                    }
                }

                else if (this.txPartPanel != null && CDTXMania.ConfigIni.bGuitarEnabled)
                {
                    if (CDTXMania.DTX.bチップがある.Guitar)
                    {
                        if (CDTXMania.ConfigIni.bIsSwappedGuitarBass)
                        {
                            this.txPartPanel.tDraw2D(CDTXMania.app.Device, 455, 624, new Rectangle(0, 25, 51, 25));
                        }

                        if (CDTXMania.DTX.bチップがある.Bass)
                        {
                            if (CDTXMania.ConfigIni.bIsSwappedGuitarBass)
                            {
                                this.txPartPanel.tDraw2D(CDTXMania.app.Device, 664, 624, new Rectangle(0, 25, 51, 25));
                            }
                        }
                    }
                }
            }

            int nDisclaimerY = 360;
            if (CDTXMania.ConfigIni.nPlaySpeed != 20)
            {
                this.txModifiedPlaySpeed.tDraw2D(CDTXMania.app.Device, 840, nDisclaimerY);
                nDisclaimerY += 25;
            }
            if (CDTXMania.stageResult.bIsTrainingMode)
            {
                this.txTrainingMode.tDraw2D(CDTXMania.app.Device, 840, nDisclaimerY);
                nDisclaimerY += 25;
            }
            if (CDTXMania.stageResult.bIsTrainingMode || ((CDTXMania.ConfigIni.nPlaySpeed != 20) && !CDTXMania.ConfigIni.bSaveScoreIfModifiedPlaySpeed))
            {
                this.txResultsNotSaved.tDraw2D(CDTXMania.app.Device, 840, nDisclaimerY);
            }

            if (!this.ct登場用.bReachedEndValue)
            {
                return 0;
            }
            return 1;
        }

        // Other

        #region [ private ]
        //-----------------
        private CCounter ct登場用;
        private System.Drawing.Font ftSongDifficultyFont;
        private System.Drawing.Font ftSongNameFont;
        private Image iDrumSpeed;
        private int n本体X;
        private int n本体Y;
        private string strSongName;
        private CTexture r表示するリザルト画像;
        private CTexture txDrumSpeed;
        private CTexture txSongDifficulty;
        private CTexture txSongLevel;
        private CTexture txリザルト画像;
        private CTexture txリザルト画像がないときの画像;
        private CTexture txジャケットパネル;
        private CTexture txジャケット枠;
        private CTexture tx曲枠;
        private CTexture txSongName;
        private CTexture txArtistName;
        private CTexture txModifiedPlaySpeed;
        private CTexture txTrainingMode;
        private CTexture txResultsNotSaved;
        private CTexture txPartPanel;
        private CTexture txDifficultyPanel;

        public struct STATUSPANEL
        {
            public string label;
            public int status;
        }
        public int nIndex;
        public STATUSPANEL[] stPanelMap;

        private STDGBVALUE<List<STGhostLag>> stGhostLag;

        private struct STGhostLag
        {
            public int index;
            public int nJudgeTime;
            public int nLagTime;
            public STGhostLag(int index, int nJudgeTime, int nLagTime)
            {
                this.index = index;
                this.nJudgeTime = nJudgeTime;
                this.nLagTime = nLagTime;
            }
        }

        //2014.04.05.kairera0467 GITADORAグラデーションの色。
        //本当は共通のクラスに設置してそれを参照する形にしたかったが、なかなかいいメソッドが無いため、とりあえず個別に設置。
        //private Color clGITADORAgradationTopColor = Color.FromArgb(0, 220, 200);
        //private Color clGITADORAgradationBottomColor = Color.FromArgb(255, 250, 40);
        private Color clGITADORAgradationTopColor = Color.FromArgb(255, 255, 255);
        private Color clGITADORAgradationBottomColor = Color.FromArgb(255, 255, 255);
        private bool tプレビュー画像の指定があれば構築する()
        {
            if (string.IsNullOrEmpty(CDTXMania.DTX.PREIMAGE))
            {
                return false;
            }
            CDTXMania.tReleaseTexture(ref this.txリザルト画像);
            this.r表示するリザルト画像 = null;
            string path = CDTXMania.DTX.strFolderName + CDTXMania.DTX.PREIMAGE;
            if (!File.Exists(path))
            {
                Trace.TraceWarning("ファイルが存在しません。({0})", new object[] { path });
                return false;
            }
            this.txリザルト画像 = CDTXMania.tGenerateTexture(path);
            this.r表示するリザルト画像 = this.txリザルト画像;
            return (this.r表示するリザルト画像 != null);
        }
        private bool tリザルト画像の指定があれば構築する()
        {
            int rank = CScoreIni.tCalculateOverallRankValue(CDTXMania.stageResult.stPerformanceEntry.Drums, CDTXMania.stageResult.stPerformanceEntry.Guitar, CDTXMania.stageResult.stPerformanceEntry.Bass);
            if (rank == 99)	// #23534 2010.10.28 yyagi: 演奏チップが0個のときは、rankEと見なす
            {
                rank = 6;
            }
            if (string.IsNullOrEmpty(CDTXMania.DTX.RESULTIMAGE[rank]))
            {
                return false;
            }
            CDTXMania.tReleaseTexture(ref this.txリザルト画像);
            this.r表示するリザルト画像 = null;
            string path = CDTXMania.DTX.strFolderName + CDTXMania.DTX.RESULTIMAGE[rank];
            if (!File.Exists(path))
            {
                Trace.TraceWarning("ファイルが存在しません。({0})", new object[] { path });
                return false;
            }
            this.txリザルト画像 = CDTXMania.tGenerateTexture(path);
            this.r表示するリザルト画像 = this.txリザルト画像;
            return (this.r表示するリザルト画像 != null);
        }
        private void tDrawDifficultyPanel(string strラベル名, int nX, int nY)
        {
            string strRawScriptFile;

            Rectangle rect = new Rectangle(0, 0, 161, 30);

            //ファイルの存在チェック
            if (File.Exists(CSkin.Path(@"Script\difficult.dtxs")))
            {
                //スクリプトを開く
                StreamReader reader = new StreamReader(CSkin.Path(@"Script\difficult.dtxs"), Encoding.GetEncoding("Shift_JIS"));
                strRawScriptFile = reader.ReadToEnd();

                strRawScriptFile = strRawScriptFile.Replace(Environment.NewLine, "\n");
                string[] delimiter = { "\n" };
                string[] strSingleLine = strRawScriptFile.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < strSingleLine.Length; i++)
                {
                    if (strSingleLine[i].StartsWith("//"))
                        continue; //コメント行の場合は無視

                    //まずSplit
                    string[] arScriptLine = strSingleLine[i].Split(',');

                    if ((arScriptLine.Length >= 4 && arScriptLine.Length <= 5) == false)
                        continue; //引数が4つか5つじゃなければ無視。

                    if (arScriptLine[0] != "6")
                        continue; //使用するシーンが違うなら無視。

                    if (arScriptLine.Length == 4)
                    {
                        if (String.Compare(arScriptLine[1], strラベル名, true) != 0)
                            continue; //ラベル名が違うなら無視。大文字小文字区別しない
                    }
                    else if (arScriptLine.Length == 5)
                    {
                        if (arScriptLine[4] == "1")
                        {
                            if (arScriptLine[1] != strラベル名)
                                continue; //ラベル名が違うなら無視。
                        }
                        else
                        {
                            if (String.Compare(arScriptLine[1], strラベル名, true) != 0)
                                continue; //ラベル名が違うなら無視。大文字小文字区別しない
                        }
                    }
                    rect.X = Convert.ToInt32(arScriptLine[2]);
                    rect.Y = Convert.ToInt32(arScriptLine[3]);

                    break;
                }
            }

            if (this.txDifficultyPanel != null)
                this.txDifficultyPanel.tDraw2D(CDTXMania.app.Device, nX, nY, rect);
        }
        //-----------------
        #endregion
    }
}
