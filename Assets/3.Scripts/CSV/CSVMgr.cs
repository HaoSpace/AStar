using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

public static class CSVMgr
{
	static private string DataPath = "CSVData";


	static public void ToCSV (Array vAry)
	{
		Console.Write(convToCsv(vAry));
		Console.Read();

		System.IO.File.WriteAllText(Application.dataPath + "/CSVData.csv", convToCsv(vAry));
		//System.IO.File.WriteAllText(@"C:\Users\Public\TestFolder\WriteText.txt", convToCsv(vAry));
	}
	
	static private string convToCsv (Array ary)
	{
		//取得陣列元素的型別
		Type elemType = ary.GetType().GetElementType();
		PropertyInfo[] props = elemType.GetProperties();
		StringBuilder sb = new StringBuilder();

		//第一列輸出屬性名稱
		sb.AppendLine(string.Join("\t", props.Select(o => o.Name).ToArray()));

		//藉由foreach巡迴每一元件，透過Refelction取出屬性值
		foreach (object elem in ary)
		{
			sb.AppendLine(string.Join("\t", props.Select(o => o.Name).ToArray()));
		}
		return sb.ToString();
	}
}
