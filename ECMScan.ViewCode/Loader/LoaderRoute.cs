/*
 * Date: 10.08.2012
 * Time: 21:24
 */
using System;
using System.Data.SqlClient;
namespace ISBLScan.ViewCode
{
	/// <summary>
	/// Типовой маршрут.
	/// </summary>
	public class Route : LoaderCommon
	{
		public Route(SqlConnection sqlConnect) : base(sqlConnect)
		{
		}

		public Node Load()
		{
			return null;
		}
	}
}
