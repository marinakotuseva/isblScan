/*
 * Date: 17.10.2012
 * Time: 12:50
 */
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace isblTest
{
	/// <summary>
	/// Description of LoaderPerfomanceIndicator.
	/// </summary>
	public class PerfomanceIndicator : LoaderCommon
	{
		public PerfomanceIndicator(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}
		public isblTest.Node Load()
		{
			return null;
		}
	}
}
