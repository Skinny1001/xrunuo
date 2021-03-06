﻿//
//  X-RunUO - Ultima Online Server Emulator
//  Copyright (C) 2015 Pedro Pardal
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Server
{
	public class LocalizedText
	{
		private int m_Number;
		private string m_Args;

		public int Number { get { return m_Number; } }
		public string Args { get { return m_Args; } }

		public LocalizedText( int number )
			: this( number, null )
		{
		}

		public LocalizedText( string text )
			: this( 1042971, text )
		{
		}

		public LocalizedText( int number, string format, params object[] args )
			: this( number, string.Format( format, args ) )
		{
		}

		public LocalizedText( string textFormat, params object[] args )
			: this( 1042971, string.Format( textFormat, args ) )
		{
		}

		public LocalizedText( int number, string args )
		{
			m_Number = number;
			m_Args = args;
		}

		public void AddTo( ObjectPropertyList list )
		{
			if ( m_Args == null )
				list.Add( m_Number );
			else
				list.Add( m_Number, m_Args );
		}

		private static Regex LocalizedTextExpression = new Regex( @"#(\d+)", RegexOptions.IgnoreCase );

		public string Delocalize()
		{
			string format = StringList.Localization[m_Number];

			if ( format == null )
				return "(empty)";
			else if ( string.IsNullOrEmpty( m_Args ) )
				return format;
			else
			{
				string args = LocalizedTextExpression.Replace( m_Args, m => StringList.Localization[Utility.ToInt32( m.Groups[1].Value )] ?? string.Empty );
				return StringList.CombineArguments( format, args );
			}
		}
	}
}
