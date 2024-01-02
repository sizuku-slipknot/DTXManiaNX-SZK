﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using DTXCreator.汎用;
using DTXCreator.Properties;

namespace DTXCreator.WAV_BMP_AVI
{
	public partial class CImagePropertiesDialog : Form  // C画像プロパティダイアログ
	{
		internal CBMP bmp;
		private string str初期フォルダ = "";
		private string str相対パスの基点フォルダ = "";
		private static int[] カスタムカラー;

		public CImagePropertiesDialog( string str相対パスの基点フォルダ, string str初期フォルダ )
		{
			this.str相対パスの基点フォルダ = str相対パスの基点フォルダ;
			this.str初期フォルダ = str初期フォルダ;
			this.InitializeComponent();
		}

		private void textBoxラベル_KeyDown( object sender, KeyEventArgs e )
		{
			if( e.KeyCode == Keys.Return )
			{
				this.buttonOK.PerformClick();
			}
			if( e.KeyCode == Keys.Escape )
			{
				this.buttonキャンセル.PerformClick();
			}
		}
		private void textBoxファイル_KeyDown( object sender, KeyEventArgs e )
		{
			if( e.KeyCode == Keys.Return )
			{
				this.buttonOK.PerformClick();
			}
			if( e.KeyCode == Keys.Escape )
			{
				this.buttonキャンセル.PerformClick();
			}
		}
		private void button参照_Click( object sender, EventArgs e )
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Title = Resources.str画像ファイル選択ダイアログのタイトル;
			dialog.Filter = Resources.str画像ファイル選択ダイアログのフィルタ;
			dialog.FilterIndex = 1;
			dialog.InitialDirectory = this.str初期フォルダ;
			if( dialog.ShowDialog() == DialogResult.OK )
			{
				string str = CFileSelector_PathConversion.str基点からの相対パスに変換して返す( dialog.FileName, this.str相対パスの基点フォルダ );
				str.Replace( '/', '\\' );
				this.textBoxファイル.Text = str;
			}
		}
		private void button参照_KeyDown( object sender, KeyEventArgs e )
		{
			if( e.KeyCode == Keys.Escape )
			{
				this.buttonキャンセル.PerformClick();
			}
		}
		private void button背景色_Click( object sender, EventArgs e )
		{
			ColorDialog dialog = new ColorDialog();
			dialog.AllowFullOpen = true;
			dialog.FullOpen = true;
			dialog.Color = this.textBoxBMP番号.BackColor;
			dialog.CustomColors = カスタムカラー;
			if( dialog.ShowDialog() == DialogResult.OK )
			{
				this.textBoxBMP番号.BackColor = dialog.Color;
				カスタムカラー = dialog.CustomColors;
			}
		}
		private void button背景色_KeyDown( object sender, KeyEventArgs e )
		{
			if( e.KeyCode == Keys.Escape )
			{
				this.buttonキャンセル.PerformClick();
			}
		}
		private void button文字色_Click( object sender, EventArgs e )
		{
			ColorDialog dialog = new ColorDialog();
			dialog.AllowFullOpen = true;
			dialog.FullOpen = true;
			dialog.Color = this.textBoxBMP番号.ForeColor;
			dialog.CustomColors = カスタムカラー;
			if( dialog.ShowDialog() == DialogResult.OK )
			{
				this.textBoxBMP番号.ForeColor = dialog.Color;
				カスタムカラー = dialog.CustomColors;
			}
		}
		private void button文字色_KeyDown( object sender, KeyEventArgs e )
		{
			if( e.KeyCode == Keys.Escape )
			{
				this.buttonキャンセル.PerformClick();
			}
		}
		private void button標準色に戻す_Click( object sender, EventArgs e )
		{
			this.textBoxBMP番号.ForeColor = SystemColors.WindowText;
			this.textBoxBMP番号.BackColor = SystemColors.Window;
		}
		private void button標準色に戻す_KeyDown( object sender, KeyEventArgs e )
		{
			if( e.KeyCode == Keys.Escape )
			{
				this.buttonキャンセル.PerformClick();
			}
		}
		private void checkBoxBMPTEX_KeyDown( object sender, KeyEventArgs e )
		{
			if( e.KeyCode == Keys.Return )
			{
				this.buttonOK.PerformClick();
			}
			if( e.KeyCode == Keys.Escape )
			{
				this.buttonキャンセル.PerformClick();
			}
		}
	}
}
