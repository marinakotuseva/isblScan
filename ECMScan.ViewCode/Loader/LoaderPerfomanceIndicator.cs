/*
 * Date: 17.10.2012
 * Time: 12:50
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ISBLScan.ViewCode
{
	/// <summary>
	/// Description of LoaderPerfomanceIndicator.
	/// </summary>
	public class PerfomanceIndicator : LoaderCommon
	{
		public PerfomanceIndicator(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}
		public Node Load()
		{
			return null;
		}
	}
}
