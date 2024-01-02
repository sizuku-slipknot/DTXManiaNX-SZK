﻿using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DTXMania
{
	/// <summary>
	/// すべてのアイテムの基本クラス。
	/// </summary>
	internal class CItemBase
	{
		// プロパティ

		public EPanelType ePanelType;
		public enum EPanelType
		{
			Normal,
			Other
		}

		public EType eType;
		public enum EType
		{
			基本形,
			ONorOFFToggle,
			ONorOFForUndefined3State,
			Integer,
			List,
			切替リスト
		}

		public string strItemName;
		public string str説明文;


		// コンストラクタ

		public CItemBase()
		{
			this.strItemName = "";
			this.str説明文 = "";
		}
		public CItemBase( string str項目名 )
			: this()
		{
			this.tInitialize( str項目名 );
		}
		public CItemBase(string str項目名, string str説明文jp)
			: this() {
			this.tInitialize(str項目名, str説明文jp);
		}
		public CItemBase(string str項目名,  string str説明文jp, string str説明文en)
			: this() {
			this.tInitialize(str項目名, str説明文jp, str説明文en);
		}

		public CItemBase(string str項目名, EPanelType eパネル種別)
			: this()
		{
			this.tInitialize( str項目名, eパネル種別 );
		}
		public CItemBase(string str項目名, EPanelType eパネル種別, string str説明文jp)
			: this() {
			this.tInitialize(str項目名, eパネル種別, str説明文jp);
		}
		public CItemBase(string str項目名, EPanelType eパネル種別, string str説明文jp, string str説明文en)
			: this() {
			this.tInitialize(str項目名, eパネル種別, str説明文jp, str説明文en);
		}

		
		// メソッド；子クラスで実装する

		public virtual void tEnter押下()
		{
		}
		public virtual void tMoveItemValueToNext()
		{
		}
		public virtual void tMoveItemValueToPrevious()
		{
		}
		public virtual void tInitialize( string str項目名 )
		{
			this.tInitialize( str項目名, EPanelType.Normal );
		}
		public virtual void tInitialize(string str項目名, string str説明文jp) {
			this.tInitialize(str項目名, EPanelType.Normal, str説明文jp, str説明文jp);
		}
		public virtual void tInitialize(string str項目名, string str説明文jp, string str説明文en) {
			this.tInitialize(str項目名, EPanelType.Normal, str説明文jp, str説明文en);
		}

		public virtual void tInitialize( string str項目名, EPanelType eパネル種別 )
		{
			this.tInitialize(str項目名, eパネル種別, "", "");
		}
		public virtual void tInitialize(string str項目名, EPanelType eパネル種別, string str説明文jp) {
			this.tInitialize(str項目名, eパネル種別, str説明文jp, str説明文jp);
		}
		public virtual void tInitialize(string str項目名, EPanelType eパネル種別, string str説明文jp, string str説明文en) {
			this.strItemName = str項目名;
			this.ePanelType = eパネル種別;
			this.str説明文 = (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ja") ? str説明文jp : str説明文en;
		}
		public virtual object obj現在値()
		{
			return null;
		}
		public virtual int GetIndex()
		{
			return 0;
		}
		public virtual void SetIndex( int index )
		{
		}
	}
}
