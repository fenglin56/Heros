using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;

//def our own type
using XmlSpreadSheet = System.Collections.Generic.Dictionary< string, object[]>;



public static class XmlSpreadSheetReader 
{
	private static XmlSpreadSheet output; // Conversion of spreadsheet into something quick to access. Initialised on startup
	private static bool readFailed;
	
	private static string[] _keys;
	
	public static string[] Keys
	{
		get
		{
			return _keys;
		}
		
		
	}
	
	
	public static XmlSpreadSheet Output
	{
		get
		{
			return output;	
		}
	}
	
	// 
	static bool ReadToNode( XmlReader reader, string node )
	{
		while( reader.Read() )
		{
			if( reader.Name == node )
			{
				return true;
			}
		}
		return false;
	}
	
	static bool ReadToNodeWithAttribute( XmlReader reader, string node, string attribute, string name )
	{
		while( reader.Read() )
		{
			if( reader.Name == node &&
			   reader.GetAttribute( "ss:"+attribute ) == name )
			{
				return true;
			}
		}
		return false;
	}
	
	static int GetAttributeAsInt( XmlReader reader, string attribute )
	{
		return System.Int32.Parse( reader.GetAttribute( "ss:"+attribute ) );
	}
	
	static string GetAttributeAsString( XmlReader reader, string attribute )
	{
		return reader.GetAttribute( "ss:"+attribute );
	}
	
	static object ReadCell(XmlReader reader)
	{
		if(reader.GetAttribute("ss:Index") != null)
		{
			Debug.LogError("XML Error: some cells are blank before this cell in the table");
		}
		
		bool success = reader.Read();
//		if( !success || reader.Name != "Cell" )
//		{
//			return null;
//		}
//		success = reader.Read();
		
		if(!success)
		{
			return null;	
		}
		 
		while( reader.Name != "Data" && reader.Name != "ss:Data" )
		{
			reader.Read();
		}
		
		// Find out the type (String or Number)
		string typeName = reader.GetAttribute( "ss:Type" );
		
		//success = reader.Read();
		
		string data = "";
		while(true)
		{
			success = reader.Read();
			if(success)
			{
				if(reader.Name == "Font")
				{
					continue;
				}
				else if (reader.Name == "")
				{
					data += reader.Value;
				}
				else if(reader.Name == "Data" || reader.Name == "ss:Data")
				{
					break;	
				}
			}
		}
	
		//reader.Read(); // /Data
		reader.Read(); // /Cell
		
		if( typeName == "Number" )
		{
			return System.Double.Parse( data );
		}
		else
		{
			return data;
		}
		
	}
	

	
	public static void ReadSheet(string content)
	{
		ReadSheet(content, null);
	}
	
	public static bool ReadSheet(string content, string sheetName)
	{
		var settings = new XmlReaderSettings();
		settings.IgnoreComments = true;
		settings.IgnoreProcessingInstructions = true;
		settings.IgnoreWhitespace = true;
		
		var reader = XmlReader.Create( new System.IO.StringReader( content ), settings );
		reader.MoveToContent();
		
		bool success;
	
		// Find the worksheet
		
		if(sheetName == null)
		{
			success = ReadToNode( reader, "Worksheet" );
		}
		else
		{
			success = ReadToNodeWithAttribute( reader, "Worksheet", "Name", sheetName );
		}
		
		if(!success)
		{
			Debug.LogError( "XML Error: No worksheet in the xml string " );
			return false;
		}
		
		ReadToNode(reader, "Table");
		
		int numColumns = GetAttributeAsInt( reader, "ExpandedColumnCount" );
		int numRows = GetAttributeAsInt( reader, "ExpandedRowCount" ) - 1;
		
		if(output == null)
		{
			output = new XmlSpreadSheet();
		}
		
		output.Clear();
		//read the first line as the key
		string[] keys = new string[numColumns];
		
		reader.Read();
		if(reader.Name == "Table")
		{
			Debug.LogError("XML Error: empty table!!!!");	
			return false;
		}
		else if(reader.Name == "Column")
		{
			ReadToNode(reader, "Row");
		}
		
		//read the first row as the keys of the dictionary
		int indexColumns = 0;
		while(true)
		{
			reader.Read();
			if(reader.Name == "Row")
			{
				break;	
			}
			
			if(reader.Name != "Cell")
			{
				Debug.LogError("XML Error: wrong format!!!!");
			}
			
			object data = ReadCell(reader);
			if(data == null)
			{
				return false;
			}
			else
			{
				
				string heading = data.ToString();
				//the first row's element will all be string
				keys[indexColumns] = heading;
				
				
				
				indexColumns++;
				
				output[ heading ] = new object[numRows];
			}
			
		}
		
		_keys = new string[indexColumns];
		
		//save our keys
		for(int i = 0; i < indexColumns; i++)
		{
			_keys[i] = keys[i];
		}
		
		
		int indexRows = 0;
		while(true)
		{
			reader.Read();
			
			if(reader.Name == "Table")
			{
				break;
			}
			
			int indexColumn2 = 0;
			while(true)
			{
				reader.Read();
				if(reader.Name == "Row")
				{
					break;	
				}
				else
				{
					object data = ReadCell(reader);
					if(data == null)
					{
						return false;
					}
					else
					{
						output[keys[indexColumn2]][indexRows] = data;
						indexColumn2++;
					}
				}
			}	
			indexRows++;
		}
		
		return true;
		
		
		
		
		
		
		
		
		
		
	}
	
	
}
