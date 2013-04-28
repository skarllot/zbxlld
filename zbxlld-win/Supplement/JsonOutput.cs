//
//  JsonOutput.cs
//
//  Author:
//       Fabricio Godoy <skarllot@gmail.com>
//
//  Copyright (c) 2012 Fabricio Godoy
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

using System;
using System.Text;
using System.Collections.Generic;

namespace zbxlld.Windows.Supplement
{
	public class JsonOutput : IList<Dictionary<string, string>>
	{
		private List<Dictionary<string,string>> list;

		public JsonOutput ()
		{
			list = new List<Dictionary<string, string>>();
		}

		public override string ToString ()
		{
			StringBuilder str = new StringBuilder ();
			str.AppendLine ("{\"data\": [");
			bool first_l1 = true;

			foreach (Dictionary<string, string> i in list) {
				if (!first_l1)
					str.AppendLine (",");
				first_l1 = false;

				str.AppendLine("\t{");
				bool first_l2 = true;
				foreach (KeyValuePair<string, string> j in i) {
					if (!first_l2)
						str.AppendLine (",");
					first_l2 = false;

					str.AppendFormat("\t\t\"{{#{0}}}\":\"{1}\"",
					                 j.Key,
					                 j.Value.Replace("\\", "\\\\").Replace("\"", "\\\""));
				}
				str.Append("\n\t}");
			}

			str.AppendLine("\n] }");
			return str.ToString();
		}
		
		#region IList implementation

		public int IndexOf (Dictionary<string, string> item)
		{
			throw new NotImplementedException ();
		}

		public void Insert (int index, Dictionary<string, string> item)
		{
			list.Insert(index, item);
		}

		public void RemoveAt (int index)
		{
			list.RemoveAt(index);
		}

		public Dictionary<string, string>this [int index] {
			get {
				return list[index];
			}
			set {
				list[index] = value;
			}
		}

		#endregion

		#region ICollection implementation

		public void Add (Dictionary<string, string> item)
		{
			list.Add(item);
		}

		public void Clear ()
		{
			list.Clear();
		}

		public bool Contains (Dictionary<string, string> item)
		{
			return list.Contains(item);
		}

		public void CopyTo (Dictionary<string, string>[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}

		public bool Remove (Dictionary<string, string> item)
		{
			return list.Remove(item);
		}

		public int Count {
			get {
				return list.Count;
			}
		}

		public bool IsReadOnly {
			get {
				return false;
			}
		}

		#endregion

		#region IEnumerable implementation

		public IEnumerator<Dictionary<string, string>> GetEnumerator ()
		{
			return list.GetEnumerator();
		}

		#endregion

		#region IEnumerable implementation

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return list.GetEnumerator();
		}

		#endregion
	}
}

