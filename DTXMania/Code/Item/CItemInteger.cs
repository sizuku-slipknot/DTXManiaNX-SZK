﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DTXMania
{
	/// <summary>
	/// 「Integer」を表すアイテム。
	/// </summary>
	internal class CItemInteger : CItemBase
	{
		// プロパティ

		public int nCurrentValue;
		public bool b値がフォーカスされている;


		// コンストラクタ

		public CItemInteger()
		{
			base.eType = CItemBase.EType.Integer;
			this.n最小値 = 0;
			this.n最大値 = 0;
			this.nCurrentValue = 0;
			this.b値がフォーカスされている = false;
		}
		public CItemInteger( string str項目名, int n最小値, int n最大値, int n初期値 )
			: this()
		{
			this.t初期化( str項目名, n最小値, n最大値, n初期値 );
		}
		public CItemInteger(string str項目名, int n最小値, int n最大値, int n初期値, string str説明文jp)
			: this() {
			this.t初期化(str項目名, n最小値, n最大値, n初期値, str説明文jp);
		}
		public CItemInteger(string str項目名, int n最小値, int n最大値, int n初期値, string str説明文jp, string str説明文en)
			: this() {
			this.t初期化(str項目名, n最小値, n最大値, n初期値, str説明文jp, str説明文en);
		}

	
		public CItemInteger( string str項目名, int n最小値, int n最大値, int n初期値, CItemBase.EPanelType eパネル種別 )
			: this()
		{
			this.t初期化( str項目名, n最小値, n最大値, n初期値, eパネル種別 );
		}
		public CItemInteger(string str項目名, int n最小値, int n最大値, int n初期値, CItemBase.EPanelType eパネル種別, string str説明文jp)
			: this() {
			this.t初期化(str項目名, n最小値, n最大値, n初期値, eパネル種別, str説明文jp);
		}
		public CItemInteger(string str項目名, int n最小値, int n最大値, int n初期値, CItemBase.EPanelType eパネル種別, string str説明文jp, string str説明文en)
			: this() {
			this.t初期化(str項目名, n最小値, n最大値, n初期値, eパネル種別, str説明文jp, str説明文en);
		}


		// CItemBase 実装

		public override void tEnter押下()
		{
			this.b値がフォーカスされている = !this.b値がフォーカスされている;
		}
		public override void tMoveItemValueToNext()
		{
			if( ++this.nCurrentValue > this.n最大値 )
			{
				this.nCurrentValue = this.n最大値;
			}
		}
		public override void tMoveItemValueToPrevious()
		{
			if( --this.nCurrentValue < this.n最小値 )
			{
				this.nCurrentValue = this.n最小値;
			}
		}
		public void t初期化( string str項目名, int n最小値, int n最大値, int n初期値 )
		{
			this.t初期化( str項目名, n最小値, n最大値, n初期値, CItemBase.EPanelType.Normal, "", "" );
		}
		public void t初期化(string str項目名, int n最小値, int n最大値, int n初期値, string str説明文jp) {
			this.t初期化(str項目名, n最小値, n最大値, n初期値, CItemBase.EPanelType.Normal, str説明文jp, str説明文jp);
		}
		public void t初期化(string str項目名, int n最小値, int n最大値, int n初期値, string str説明文jp, string str説明文en) {
			this.t初期化(str項目名, n最小値, n最大値, n初期値, CItemBase.EPanelType.Normal, str説明文jp, str説明文en);
		}

	
		public void t初期化( string str項目名, int n最小値, int n最大値, int n初期値, CItemBase.EPanelType eパネル種別 )
		{
			this.t初期化( str項目名, n最小値, n最大値, n初期値, eパネル種別, "", "" );
		}
		public void t初期化(string str項目名, int n最小値, int n最大値, int n初期値, CItemBase.EPanelType eパネル種別, string str説明文jp) {
			this.t初期化(str項目名, n最小値, n最大値, n初期値, eパネル種別, str説明文jp, str説明文jp);
		}
		public void t初期化(string str項目名, int n最小値, int n最大値, int n初期値, CItemBase.EPanelType eパネル種別, string str説明文jp, string str説明文en) {
			base.tInitialize(str項目名, eパネル種別, str説明文jp, str説明文en);
			this.n最小値 = n最小値;
			this.n最大値 = n最大値;
			this.nCurrentValue = n初期値;
			this.b値がフォーカスされている = false;
		}
		public override object obj現在値()
		{
			return this.nCurrentValue;
		}
		public override int GetIndex()
		{
			return this.nCurrentValue;
		}
		public override void SetIndex( int index )
		{
			this.nCurrentValue = index;
		}
		// Other

		#region [ private ]
		//-----------------
		private int n最小値;
		private int n最大値;
		//-----------------
		#endregion
	}
}
