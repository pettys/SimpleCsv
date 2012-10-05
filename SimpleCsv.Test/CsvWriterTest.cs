using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SimpleCsv;
using NUnit.Framework;
using Shouldly;
using System.Data;

namespace Pedigrees.Test
{

	[TestFixture]
	public class CsvWriterTest
	{

		[Test]
		public void SimpleTest()
		{
			var table = new DataTable();
			table.Columns.Add("plain old");
			table.Columns.Add("\"quoted\"");
			table.Columns.Add("with,comma");
			table.Rows.Add("val1", "val2", "val3");
			table.Rows.Add("a,b", "a,\"b\",c", "\"");
			table.Rows.Add(string.Empty, null, DBNull.Value);

			var output = new StringWriter();
			CsvWriter.Write(output, table);

			var lines = output.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			lines.Length.ShouldBe(4);
			lines[0].ShouldBe("plain old,\"\"\"quoted\"\"\",\"with,comma\"");
			lines[1].ShouldBe("val1,val2,val3");
			lines[2].ShouldBe("\"a,b\",\"a,\"\"b\"\",c\",\"\"\"\"");
			lines[3].ShouldBe(",,");
		}
	}
}
