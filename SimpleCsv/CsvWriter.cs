using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace SimpleCsv {

	public class CsvWriter {

		private const string Delimiter = "\"";
		private const string EscapedDelimiter = "\"\"";
		private const string Separator = ",";
		private static readonly string[] EscapeTriggers = new[] { Delimiter, Separator, Environment.NewLine, "\r", "\n" };

		public static void Write(TextWriter writer, DataTable table) {
			WriteRow(writer, table.Columns.Cast<DataColumn>().Select(col => col.ColumnName));
			foreach (DataRow row in table.Rows) {
				WriteRow(writer, row.ItemArray.Select(i => (i ?? string.Empty).ToString()));
			}
		}

		/// <summary>
		/// Writes the row in CSV format, including the line termination character.
		/// </summary>
		public static void WriteRow(TextWriter writer, IEnumerable<string> row) {
			var first = true;
			foreach (var field in row) {
				if (first) {
					first = false;
				} else {
					writer.Write(Separator);
				}
				writer.Write(ToCsvField(field));
			}
			writer.Write(Environment.NewLine);
		}

		public static string ToCsvField(string text) {
			if (string.IsNullOrEmpty(text) || !EscapeTriggers.Any(text.Contains)) {
				return text;
			} else {
				return string.Concat(Delimiter, text.Replace(Delimiter, EscapedDelimiter), Delimiter);
			}
		}

		public static string FromCsvField(string text) {
			if (string.IsNullOrEmpty(text) || !text.StartsWith(Delimiter))
				return text;
			else
				return text
					.Substring(Delimiter.Length, text.Length - EscapedDelimiter.Length)
					.Replace(EscapedDelimiter, Delimiter);
		}

	}

}