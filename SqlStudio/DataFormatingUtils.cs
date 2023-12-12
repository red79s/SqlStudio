using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;

namespace SqlStudio
{
    class DataFormatingUtils
    {
        public static string GetDataTableAsString(DataTable dt, bool includeHeader, string tableName, int headerNum)
        {
            StringBuilder sbRet = new StringBuilder();

            int cols = dt.Columns.Count;
            if (cols < 1)
                return "[no columns in result set]";

			int[] maxColLen = new int[cols];

			// Get max length of each column
			for(int i=0; i<cols; i++)
			{
				if(dt.Columns[i].ColumnName.Length > maxColLen[i])
					maxColLen[i] = dt.Columns[i].ColumnName.Length;
			}
			foreach(DataRow dr in dt.Rows)
			{
                if (dr.RowState == DataRowState.Deleted)
                    continue;

				for(int i=0; i<cols; i++)
				{
					if(dr[i].ToString().Length > maxColLen[i])
						maxColLen[i] = dr[i].ToString().Length;
				}
			}

			// Print table data
            if (includeHeader)
			{
				sbRet.Append(Environment.NewLine + $"[{tableName} Rows: {dt.Rows.Count}, ResultNum: {headerNum}]");
				string r = Environment.NewLine;
				int totLen = 0;
				for(int i=0; i<cols; i++)
				{
					r += dt.Columns[i].ColumnName.PadRight(maxColLen[i], ' ');
					if(i < (cols - 1))
						r += '|';
					totLen += maxColLen[i];
				}
				sbRet.Append(r);
				sbRet.Append(Environment.NewLine + "-".PadRight(totLen + dt.Columns.Count - 1, '-'));
			}
			foreach(DataRow dr in dt.Rows)
			{
                if (dr.RowState == DataRowState.Deleted)
                    continue;

				string r = Environment.NewLine;
				for(int i=0; i<cols; i++)
				{
					string sCol = dr[i].ToString();
					if(Regex.IsMatch(sCol, @"^[0-9,.]*$"))
						r += sCol.PadLeft(maxColLen[i], ' ');
					else
						r += sCol.PadRight(maxColLen[i], ' ');
					if(i < (cols - 1))
						r += '|';
				}
				sbRet.Append(r);
			}

            return sbRet.ToString();
        }
    }
}
