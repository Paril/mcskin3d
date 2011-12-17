using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Paril.Controls
{
	public class AlphanumComparatorFast : IComparer, IComparer<string>
	{
		public int Compare(object o1, object o2)
		{
			string s1, s2;

			if (o1 is string)
				s1 = (string)o1;
			else
				s1 = o1.ToString();

			if (o2 is string)
				s2 = (string)o2;
			else
				s2 = o2.ToString();

			return Compare(s1, s2);
		}

		public int Compare(string s1, string s2)
		{
			int len1 = s1.Length;
			int len2 = s2.Length;
			int marker1 = 0;
			int marker2 = 0;

			// Walk through two the strings with two markers.
			while (marker1 < len1 && marker2 < len2)
			{
				char ch1 = s1[marker1];
				char ch2 = s2[marker2];

				// Some buffers we can build up characters in for each chunk.
				char[] space1 = new char[len1];
				int loc1 = 0;
				char[] space2 = new char[len2];
				int loc2 = 0;

				// Walk through all following characters that are digits or
				// characters in BOTH strings starting at the appropriate marker.
				// Collect char arrays.
				do
				{
					space1[loc1++] = ch1;
					marker1++;

					if (marker1 < len1)
						ch1 = s1[marker1];
					else
						break;
				} while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

				do
				{
					space2[loc2++] = ch2;
					marker2++;

					if (marker2 < len2)
						ch2 = s2[marker2];
					else
						break;
				}
				while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

				// If we have collected numbers, compare them numerically.
				// Otherwise, if we have strings, compare them alphabetically.
				string str1 = new string(space1);
				string str2 = new string(space2);

				int result;

				if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
				{
					int thisNumericChunk = int.Parse(str1);
					int thatNumericChunk = int.Parse(str2);
					result = thisNumericChunk.CompareTo(thatNumericChunk);
				}
				else
					result = str1.CompareTo(str2);

				if (result != 0)
					return result;
			}

			return len1 - len2;
		}
	}

	class SortedListBox : ListBox
	{
		public IComparer Comparer;
		public SortedListBox() { }
		public SortedListBox(IComparer comparator)
		{
			Comparer = comparator;
		}

		public void SortList()
		{
			this.Sort();
		}

		protected override void Sort()
		{
			if (Comparer == null)
				return;

			if (Items.Count > 1)
			{
				bool swapped;
				do
				{
					int counter = Items.Count - 1;
					swapped = false;

					IList source = (IList)DataSource;

					while (counter > 0)
					{
						// Compare the items' length. 
						if (Comparer.Compare(Items[counter], Items[counter - 1]) < 0)
						{
							// Swap the items.
							object temp = source[counter];
							source[counter] = Items[counter - 1];
							source[counter - 1] = temp;
							swapped = true;
						}
						// Decrement the counter.
						counter -= 1;
					}
				}
				while ((swapped == true));
			}		
		}
	}
}
