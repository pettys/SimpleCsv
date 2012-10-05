using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;

namespace SimpleCsv
{

	/// <summary>
	/// Reads a Comma Separated Values, or CSV, file.
	/// </summary>
	public class CsvReader : IDisposable
	{

		TextReader _reader;

		public static string[][] Read(string file)
		{
			CsvReader cr = null;
			try
			{
				cr = new CsvReader(file);
				return cr.ReadToEnd();
			}
			finally
			{
				if (cr != null)
				{
					cr.Close();
				}
			}
		}


		//static System.Text.Encoding MyEncoding = System.Text.Encoding.GetEncoding(1252);
#if SILVERLIGHT
		public CsvReader(Stream input) : this(new StreamReader(input)) { }
#else
		public CsvReader(Stream input) : this(new StreamReader(input, System.Text.Encoding.GetEncoding(1252))) { }
#endif
		public CsvReader(string file) : this(new FileStream(file, FileMode.Open)) { }
		public CsvReader(TextReader input) { _reader = input; }

		protected enum LineState { Ready = 0, InPlainData = 1, InQuotedData = 2, OutQuotedData = 4 }

		public string[] ReadLine()
		{
			string line = _reader.ReadLine();
			if (line == null)
			{
				return null;
			}
			List<string> columns = new List<string>();
			LineState state = LineState.Ready;
			int dataStartIndex = -1;
			int quoteStartIndex = -1;
			int quoteEndIndex = -1;
			const char EOL = (char)0;
			for (int i = 0; i < line.Length + 1; i++)
			{
				char ch = i == line.Length ? EOL : line[i];
				switch (state)
				{
					case LineState.Ready:
						switch (ch)
						{
							case EOL: goto case ',';
							case ',': columns.Add(string.Empty); break;
							case '"': quoteStartIndex = i;
								state = LineState.InQuotedData;
								break;
							default: dataStartIndex = i;
								state = LineState.InPlainData;
								break;
						}
						break;
					case LineState.InPlainData:
						switch (ch)
						{
							case EOL: goto case ',';
							case ',':
								columns.Add(line.Substring(dataStartIndex, i - dataStartIndex));
								state = LineState.Ready;
								break;
						}
						break;
					case LineState.InQuotedData:
						switch (ch)
						{
							case EOL: columns.Add(line.Substring(quoteStartIndex, i - quoteStartIndex));
								break;
							case '"': quoteEndIndex = i;
								state = LineState.OutQuotedData;
								break;
							default: break;
						}
						break;
					case LineState.OutQuotedData:
						switch (ch)
						{
							case EOL: goto case ',';
							case ',':
								string data = line.Substring(quoteStartIndex + 1, quoteEndIndex - quoteStartIndex - 1);
								data = data.Replace("\"\"", "\"");
								columns.Add(data);
								state = LineState.Ready;
								break;
							case '"': state = LineState.InQuotedData;
								break;
						}
						break;
				}
			}
			return columns.ToArray();
		}

		public string[][] ReadToEnd()
		{
			List<string[]> lines = new List<string[]>();
			string[] line;
			while ((line = ReadLine()) != null)
			{
				lines.Add(line);
			}
			return lines.ToArray();
		}

		public void Close()
		{
			if (_reader != null)
			{
				_reader.Close();
			}
		}

		public void Dispose()
		{
			if (_reader != null)
			{
				_reader.Dispose();
			}
		}

	}
}