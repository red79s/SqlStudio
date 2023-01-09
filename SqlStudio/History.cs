using System;
using System.Collections;
using System.IO;

namespace SqlStudio
{
	/// <summary>
	/// Summary description for History.
	/// </summary>
	public class History
	{
		private ArrayList alHist;
		private int index = 0;
		private bool bTempItem = false;
		private int maxHistItems = 0;

		public History()
		{
			alHist = new ArrayList();
		}

		public void Save(string sFile)
		{
			try
			{
				int itemsToWrite = maxHistItems;
				if(maxHistItems == 0)
					itemsToWrite = alHist.Count;
				
				if(File.Exists(sFile))
					File.Delete(sFile);

				StreamWriter sr = File.CreateText(sFile);
				for(int i=0; i<alHist.Count && itemsToWrite>0; i++,itemsToWrite--)
				{
					sr.WriteLine((string)alHist[i]);
				}
				sr.Close();
			}
			catch(Exception e)
			{
				string s = e.Message;
			}
		}

		public void Load(string sFile)
		{
			alHist.Clear();
			try 
			{
				using (StreamReader sr = new StreamReader(sFile)) 
				{
					String line;
					while ((line = sr.ReadLine()) != null) 
					{
						if(line.Length > 0)
							alHist.Add(line);
					}
				}
			}
			catch (Exception) 
			{
			}
			index = alHist.Count;
			bTempItem = false;
		}

		public void Add(string sEntry)
		{
			if(sEntry.Length>0)
			{
				if(bTempItem)
					alHist[alHist.Count - 1] = RemoveNewLines(sEntry);
				else
					alHist.Add(RemoveNewLines(sEntry));
			}
			RemoveDuplicates(); //keep new entry
			index = alHist.Count;
			bTempItem = false;
		}
		
		public void AddTemp(string sEntry)
		{
			alHist.Add(RemoveNewLines(sEntry));
			index = alHist.Count - 1;
			bTempItem = true;
		}

		public string getPrev()
		{
			index--;
			if(index<0)
				index = 0;
			
			return (string)alHist[index];
		}

		public string getNext()
		{
			string ret = "";
			index++;
			if(index>=alHist.Count)
				index = alHist.Count;
			else
				ret = (string)alHist[index];
			return ret;
		}

		public string[] getHist(string sSearch)
		{
			ArrayList alRet = new ArrayList();
			for(int i=0; i<alHist.Count; i++)
			{
				string item = (string)alHist[i];
				if(sSearch == null)
					alRet.Add(item);
				else
				{
					if(item.ToLower().IndexOf(sSearch.ToLower(),0) >= 0)
						alRet.Add(item);
				}
			}
			string[] ret = new string[alRet.Count];
			for(int i=0; i<ret.Length; i++)
				ret[i] = (string)alRet[i];
			return ret;
		}

		public bool HaveTemp()
		{
			return bTempItem;
		}
		
		public int MaxHistItems
		{
			get
			{
				return maxHistItems;
			}
			set
			{
				maxHistItems = value;
			}
		}

		private void RemoveDuplicates()
		{
			string search = (string)alHist[alHist.Count - 1];
			for(int i=0; i<alHist.Count - 1; i++)
			{
				if(search == (string)alHist[i])
				{
					alHist.RemoveAt(i);
					return;
				}
			}
		}

		private string RemoveNewLines(string sIn)
		{
			return sIn.Replace("\r","").Replace("\n"," ");
		}
	}
}
