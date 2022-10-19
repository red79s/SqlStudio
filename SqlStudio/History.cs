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
				int itemsToWrite = this.maxHistItems;
				if(this.maxHistItems == 0)
					itemsToWrite = this.alHist.Count;
				
				if(File.Exists(sFile))
					File.Delete(sFile);

				StreamWriter sr = File.CreateText(sFile);
				for(int i=0; i<this.alHist.Count && itemsToWrite>0; i++,itemsToWrite--)
				{
					sr.WriteLine((string)this.alHist[i]);
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
			this.alHist.Clear();
			try 
			{
				using (StreamReader sr = new StreamReader(sFile)) 
				{
					String line;
					while ((line = sr.ReadLine()) != null) 
					{
						if(line.Length > 0)
							this.alHist.Add(line);
					}
				}
			}
			catch (Exception) 
			{
			}
			this.index = this.alHist.Count;
			this.bTempItem = false;
		}

		public void Add(string sEntry)
		{
			if(sEntry.Length>0)
			{
				if(this.bTempItem)
					this.alHist[this.alHist.Count - 1] = this.RemoveNewLines(sEntry);
				else
					this.alHist.Add(this.RemoveNewLines(sEntry));
			}
			this.RemoveDuplicates(); //keep new entry
			this.index = this.alHist.Count;
			this.bTempItem = false;
		}
		
		public void AddTemp(string sEntry)
		{
			this.alHist.Add(this.RemoveNewLines(sEntry));
			this.index = this.alHist.Count - 1;
			this.bTempItem = true;
		}

		public string getPrev()
		{
			this.index--;
			if(this.index<0)
				this.index = 0;
			
			return (string)this.alHist[this.index];
		}

		public string getNext()
		{
			string ret = "";
			this.index++;
			if(this.index>=this.alHist.Count)
				this.index = this.alHist.Count;
			else
				ret = (string)this.alHist[this.index];
			return ret;
		}

		public string[] getHist(string sSearch)
		{
			ArrayList alRet = new ArrayList();
			for(int i=0; i<this.alHist.Count; i++)
			{
				string item = (string)this.alHist[i];
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
			return this.bTempItem;
		}
		
		public int MaxHistItems
		{
			get
			{
				return this.maxHistItems;
			}
			set
			{
				this.maxHistItems = value;
			}
		}

		private void RemoveDuplicates()
		{
			string search = (string)this.alHist[this.alHist.Count - 1];
			for(int i=0; i<this.alHist.Count - 1; i++)
			{
				if(search == (string)this.alHist[i])
				{
					this.alHist.RemoveAt(i);
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
