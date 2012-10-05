using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SimpleCsv;
using NUnit.Framework;
using Shouldly;
using System.Data;

namespace Pedigrees.Test {

	[TestFixture]
	public class CsvReaderTest {

		[Test]
		public void TestForeignCharacterFile() {
			var csv = new CsvReader(@"..\..\TestData\Encoding_ForeignChars1.csv");
			var header = csv.ReadLine();
			Assert.AreEqual("Name", header[0]);
			var line1 = csv.ReadLine();
			Assert.AreEqual("ÄLMÅSENS WILMA", line1[0]);
		}

	}

}