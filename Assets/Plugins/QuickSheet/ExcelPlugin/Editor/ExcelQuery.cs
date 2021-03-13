///////////////////////////////////////////////////////////////////////////////
///
/// ExcelQuery.cs
///
/// (c)2014 Kim, Hyoun Woo
///
///////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System;
using System.Linq;
using System.ComponentModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace UnityQuickSheet
{
    /// <summary>
    /// Query each of cell data from the given excel sheet and deserialize it to the ScriptableObject's data array.
    /// </summary>
    public class ExcelQuery
    {
        private IWorkbook workbook = null;
        private ISheet sheet = null;
        private string filepath = string.Empty;
        public ExcelQuery(ExcelQuery e)
        {
            copyExcelQuery(e);
        }
        public void copyExcelQuery(ExcelQuery e)
        {
            filepath = e.filepath;
            initExcelQuery(filepath, e.sheet.SheetName);
        }
        private void initExcelQuery(string path, string sheetName)
        {
            try
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    string extension = GetSuffix(path);

                    if (extension == "xls")
                        workbook = new HSSFWorkbook(fileStream);
                    else if (extension == "xlsx")
                    {
#if UNITY_EDITOR_OSX
                        throw new Exception("xlsx is not supported on OSX.");
#else
                        workbook = new XSSFWorkbook(fileStream);
#endif
                    }
                    else
                    {
                        throw new Exception("Wrong file.");
                    }

                    //NOTE: An empty sheetName can be available. Nothing to do with an empty sheetname.
                    if (!string.IsNullOrEmpty(sheetName))
                        sheet = workbook.GetSheet(sheetName);

                    this.filepath = path;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        public ExcelQuery(string path, string sheetName = "")
        {
            initExcelQuery(path, sheetName);

        }

        /// <summary>
        /// Determine whether the excel file is successfully read in or not.
        /// </summary>
        public bool IsValid()
        {
            if (this.workbook != null && this.sheet != null)
                return true;

            return false;
        }

        /// <summary>
        /// Retrieves file extension only from the given file path.
        /// </summary>
        static string GetSuffix(string path)
        {
            string ext = Path.GetExtension(path);
            string[] arg = ext.Split(new char[] { '.' });
            return arg[1];
        }

        string GetHeaderColumnName(int start, int cellnum)
        {
            ICell headerCell = sheet.GetRow(start).GetCell(cellnum);
            if (headerCell != null)
                return headerCell.StringCellValue;
            return string.Empty;
        }

        /// <summary>
        /// Deserialize all the cell of the given sheet.
        ///
        /// NOTE:
        ///     The first row of a sheet is header column which is not the actual value
        ///     so it skips when it deserializes.
        /// </summary>
        public List<T> Deserialize<T>(int start = 2)
        {
            var t = typeof(T);
            PropertyInfo[] p = t.GetProperties();

            var result = new List<T>();

            int current = 0;
            foreach (IRow row in sheet)
            {
                if (current < start)
                {
                    current++; // skip header column.
                    continue;
                }

                var item = (T)Activator.CreateInstance(t);
                int cellIterValue = 0;
                for (var i = 0; i < p.Length; i++)
                {
                    ICell cell = row.GetCell(i);

                    var property = p[i];
                    if (property.CanWrite)
                    {
                        try
                        {//don't use var here, may cause List/Array cast to var exception
                            
                            object value = ConvertFrom(cell, property.PropertyType,ref cellIterValue);
                            Debug.Log("Convert Type:"+value.GetType().ToString());
                            property.SetValue(item, value, null);
                        }
                        catch (Exception e)
                        {
                            string pos = string.Format("Row[{0}], Cell[{1}]", (current).ToString(), GetHeaderColumnName(start - 1, i));
                            Debug.LogError(string.Format("Excel File {0} Deserialize Exception: {1} at {2}", this.filepath, e.Message, pos));
                        }
                    }
                }

                result.Add(item);

                current++;
            }

            return result;
        }

        /// <summary>
        /// Retrieves all sheet names.
        /// </summary>
        public string[] GetSheetNames()
        {
            List<string> sheetList = new List<string>();
            if (this.workbook != null)
            {
                int numSheets = this.workbook.NumberOfSheets;
                for (int i = 0; i < numSheets; i++)
                {
                    sheetList.Add(this.workbook.GetSheetName(i));
                }
            }
            else
                Debug.LogError("Workbook is null. Did you forget to import excel file first?");

            return (sheetList.Count > 0) ? sheetList.ToArray() : null;
        }

        /// <summary>
        /// Retrieves all first columns(aka. header column) which are needed to determine each type of a cell.
        /// </summary>
        public string[] GetTitle(int start, ref string error)
        {
            List<string> result = new List<string>();

            IRow title = sheet.GetRow(start);
            if (title != null)
            {
                for (int i = 0; i < title.LastCellNum; i++)
                {
                    ICell value = title.GetCell(i);
                    // (NPOI.SS.UserModel.CellType)(value.CellType).Equals((UnityQuickSheet.CellType)CellType.String) may be false 
                    UnityQuickSheet.CellType valueCellType = (UnityQuickSheet.CellType)value.CellType;
                    if (!(valueCellType.Equals(CellType.String)) || string.IsNullOrEmpty(value.StringCellValue))
                    {
                        // null or empty column is found. Note column index starts from 0.
                        Debug.LogWarningFormat("Null or empty column is found at {0}.The celltype of {0} is '{1}' type.\n", i, title.GetCell(i).CellType);
                    }
                    else
                    {
                        // column header is not an empty string, we check its validation later.
                        result.Add(value.StringCellValue);
                    }
                }

                return result.ToArray();
            }

            error = string.Format(@"Empty row at {0}", start);
            return null;
        }
        protected object ConvertFrom(ICell cell, Type t,ref int cellColumnIndex)
        {
            object value = null;
            List<UnityQuickSheet.CellType> cellTypes = new List<UnityQuickSheet.CellType>();
            cellTypes = MachineQueryCellTypeBinder.bind(this);
            List<bool> isArr = new List<bool>();
            isArr = MachineQueryCellTypeBinder.bind2(this);
            
            if (cellTypes == null || cellTypes.Count == 0)
            {
                Debug.Log("cellTypes can recognize but use normal Convert, may have bugs. try to run machine->update");
                value = normalConvertFrom(cell, t);
            }
            else
            {
                if(cell!=null)
                {
                    Debug.Log("ColumnIndex:"+cell.ColumnIndex);
                    var myCell = cellTypes[cell.ColumnIndex];
                    var myiArr = isArr[cell.ColumnIndex];
                    cellColumnIndex = cell.ColumnIndex;
                    value = UnityConvertFrom(cell, t, myCell, myiArr);
                }
                else
                {
                    cellColumnIndex = cellColumnIndex+1;
                    var myCell = cellTypes[cellColumnIndex];
                    var myiArr = isArr[cellColumnIndex];
                    Debug.Log("cell is null,cellType:"+myCell+","+myiArr+",ColumnIndex should be "+cellColumnIndex);
                    value = UnityConvertFrom(null,t,myCell,myiArr);
                }
            }
            return value;
        }
        protected bool TestBool(string cellstr,Type t)
        {
            if(cellstr=="0"||cellstr=="" || cellstr.ToLower()=="n"||cellstr.ToLower()=="no"||cellstr.ToLower()=="f"||cellstr.ToLower()=="false")
            {
                return false;
            }
            if(cellstr.ToLower()=="y" || cellstr.ToLower()=="yes"||cellstr.ToLower()=="t"||cellstr.ToLower()=="true")
            {
                return true;
            }
            return true;
        }
        protected bool TestBool(ICell cell,Type t)
        {
            bool value=false;
            if (cell.CellType == NPOI.SS.UserModel.CellType.Boolean)
            {
                value = cell.BooleanCellValue;
            }
            else if (cell.CellType == NPOI.SS.UserModel.CellType.Numeric)
            {
                if (cell.NumericCellValue != 0)
                {
                    value = true;
                }
                else
                {
                    value = false;
                }
            }
            else if (cell.CellType == NPOI.SS.UserModel.CellType.String)
            {
                if (cell.StringCellValue.ToLower() == "true" || cell.StringCellValue.ToLower() == "t" || cell.StringCellValue.ToLower() == "yes" || cell.StringCellValue.ToLower() == "y")
                {
                    value = true;
                }
                else if (cell.StringCellValue.ToLower() == "false" || cell.StringCellValue.ToLower() == "f" || cell.StringCellValue.ToLower() == "no" || cell.StringCellValue.ToLower() == "n")
                {
                    value = false;
                }
                else
                {
                    value = false;
                }
            }
            t = typeof(bool);
            return value;
        }
        protected object UnityConvertFrom(ICell cell, Type t, CellType mycell, bool myiArr)
        {
            object value = null;
            Debug.Log("mycell:" + mycell.ToString() + ",num:" + mycell+",isArr:"+myiArr);
            if (myiArr == false)
            {
                switch (mycell)
                {
                    case CellType.Undefined:
                        var nc = new NullableConverter(t);
                        Debug.Log("CT:nc");
                        return nc.ConvertFrom(value);
                    case CellType.String:
                        Debug.Log("CT:Str");
                        if(cell==null)
                        {
                            Debug.Log("CT:Str-cell is null");
                            value="";
                        }
                        else
                        {
                            if(cell.CellType == NPOI.SS.UserModel.CellType.Blank)
                            {
                                Debug.Log("NPOI.SS.UserModel.CellType.Blank:Str-cell is null");
                                value="";
                            }
                            else
                            {
                                if(cell.CellType == NPOI.SS.UserModel.CellType.String)
                                {
                                    if(cell.StringCellValue!=null)
                                    {
                                        Debug.Log("cell.StringCellV:"+cell.StringCellValue);
                                        value = cell.StringCellValue;
                                    }
                                    else
                                    {
                                        Debug.Log("NPOI.SS.UserModel.CellType.String:Str-cell is null");
                                        value = "";
                                    }
                                }
                            }
                        }
                        t = typeof(string);
                        break;
                    case CellType.Short:
                        Debug.Log("CT:short");
                        if(cell==null)
                        {
                            value=(Int16)(0);
                        }
                        else
                        {
                        value = Convert.ToInt16(cell.StringCellValue);
                        }
                        t = typeof(short);
                        break;
                    case CellType.Int:
                        Debug.Log("CT:Int");
                        if(cell==null)
                        {
                            value=(Int32)(0);
                        }
                        else
                        {
                            if (cell.CellType == NPOI.SS.UserModel.CellType.Numeric)
                            {
                                value = cell.NumericCellValue;
                            }
                            else if(cell.CellType==NPOI.SS.UserModel.CellType.String)
                            {
                                value = Convert.ToInt32(cell.StringCellValue);
                            }
                            else if(cell.CellType==NPOI.SS.UserModel.CellType.Blank)
                            {
                                value = (Int32)(0);
                            }
                            Debug.Log("intValue:"+value);
                        }
                        t = typeof(int);
                        break;
                    case CellType.Long:
                    Debug.Log("CT:Long");
                        if(cell==null)
                        {
                            value=(Int64)(0);
                        }
                        else
                        {
                        if (cell.CellType == NPOI.SS.UserModel.CellType.Numeric)
                            {
                                value = cell.NumericCellValue;
                            }
                            else if(cell.CellType==NPOI.SS.UserModel.CellType.String)
                            {
                                value = Convert.ToInt64(cell.StringCellValue);
                            }
                            else if(cell.CellType==NPOI.SS.UserModel.CellType.Blank)
                            {
                                value = (Int64)(0);
                            }
                        }
                        t = typeof(long);
                        break;
                    case CellType.Float:
                    Debug.Log("CT:Float");
                        if(cell==null)
                        {
                            value=(float)(0f);
                        }
                        else
                        {
                            if (cell.CellType == NPOI.SS.UserModel.CellType.Numeric)
                            {
                                value = cell.NumericCellValue;
                            }
                            else if(cell.CellType==NPOI.SS.UserModel.CellType.String)
                            {
                                value = Convert.ToSingle(cell.StringCellValue);
                            }
                            else if(cell.CellType==NPOI.SS.UserModel.CellType.Blank)
                            {
                                value = (float)(0f);
                            }
                        }
                        t = typeof(float);
                        break;
                    case CellType.Double:
                    Debug.Log("CT:Double");
                        if(cell==null)
                        {
                            value=(double)(0.0);
                        }
                        else
                        {
                        if (cell.CellType == NPOI.SS.UserModel.CellType.Numeric)
                        {
                            value = cell.NumericCellValue;
                        }
                        else if(cell.CellType==NPOI.SS.UserModel.CellType.String)
                        {
                            value = Convert.ToDouble(cell.StringCellValue);
                        }
                        else if(cell.CellType==NPOI.SS.UserModel.CellType.Blank)
                        {
                            value=(double)(0.0);
                        }
                        }
                        Debug.Log("doubleValue:"+value);
                        t = typeof(double);
                        break;
                    case CellType.Enum:
                    Debug.Log("CT:Enum");
                        if(cell==null)
                        {
                            value=(0);
                        }
                        else{
                        if (cell.CellType == NPOI.SS.UserModel.CellType.Numeric)
                        {
                            value = cell.NumericCellValue;
                        }
                        else if(cell.CellType==NPOI.SS.UserModel.CellType.String)
                        {
                            value = Convert.ToDouble(cell.StringCellValue);
                        }
                        else if(cell.CellType==NPOI.SS.UserModel.CellType.Blank)
                        {
                            value=0;
                        }
                        }
                        return Enum.Parse(t, value.ToString(), true);
                    case CellType.Bool:
                    Debug.Log("CT:Bool");
                        if(cell==null)
                        {
                            value=false;
                        }
                        else
                        {
                        value = TestBool(cell,t);
                        }
                        t = typeof(bool);
                        break;
                }
            }
            else
            {
                // string tempCell = cell.StringCellValue;
                switch(mycell)
                { 
                    case CellType.Bool:
                    Debug.Log("CT:Bool[]");
                    List<bool> tempListB = new List<bool>();
                    foreach(var i in ConvertExt.Split(cell.StringCellValue))
                    {
                        tempListB.Add(TestBool((string)i,t));
                    }
                    value = tempListB;
                    t = typeof(List<bool>);
                    break;
                    case CellType.Short:
                    Debug.Log("CT:Short[]");
                    List<short> tempListS = new List<short>();
                    if(cell.CellType == NPOI.SS.UserModel.CellType.Numeric)
                    {
                        tempListS.Add(Convert.ToInt16(cell.NumericCellValue));    
                    }
                    else
                    {
                        List<short> addRS= ConvertExt.ToInt16Array(cell.StringCellValue).ToList<short>();
                        tempListS.AddRange(addRS);
                    }
                    value = tempListS;
                    t = typeof(List<short>);
                    break;
                    case CellType.Long:
                    Debug.Log("CT:Long[]");
                    List<long> tempListL = new List<long>();
                    if(cell.CellType == NPOI.SS.UserModel.CellType.Numeric)
                    {
                        tempListL.Add(Convert.ToInt64(cell.NumericCellValue));    
                    }
                    else
                    {
                        List<long> addRL= ConvertExt.ToInt64Array(cell.StringCellValue).ToList<long>();
                        tempListL.AddRange(addRL);
                    }
                    value = tempListL;
                    t = typeof(List<long>);
                    break;
                    case CellType.Float:
                    Debug.Log("CT:Float[]");
                    List<float> tempListF = new List<float>();
                    if(cell.CellType == NPOI.SS.UserModel.CellType.Numeric)
                    {
                        tempListF.Add(Convert.ToSingle(cell.NumericCellValue));    
                    }
                    else
                    {
                    List<float> addRF= ConvertExt.ToSingleArray(cell.StringCellValue).ToList<float>();
                    tempListF.AddRange(addRF);
                    }
                    value = tempListF;
                    t = typeof(List<float>);
                    break;
                    case CellType.Int:
                    Debug.Log("CT:Int[]");
                    List<int> tempListI = new List<int>();
                    if(cell==null)
                    {
                        Debug.Log("intArr,cell is null,add tempListI new List<int>()");
                    }
                    else
                    {
                        Debug.Log("intArr,cell.CellType:"+cell.CellType);
                        if(cell.CellType == NPOI.SS.UserModel.CellType.Numeric )
                        {
                            Debug.Log("C1,cell.NumericCellValue:"+cell.NumericCellValue);
                            tempListI.Add(Convert.ToInt32(cell.NumericCellValue));
                            Debug.Log("Fin!tempListI[0]:"+tempListI[tempListI.Count - 1]);
                        }
                        else if(cell.CellType == NPOI.SS.UserModel.CellType.String)
                        {
                            Debug.Log("C2,cell.StringCellValue:"+cell.StringCellValue);
                            foreach(var i in ConvertExt.Split(cell.StringCellValue))
                            {
                                tempListI.Add(Int32.Parse((string)i));
                            }
                            Debug.Log("Fin!tempListI[0]:"+tempListI[tempListI.Count - 1]);
                        }
                        else if(cell.CellType == NPOI.SS.UserModel.CellType.Blank)
                        {
                            Debug.Log("C3,cell is blank:");
                            if(tempListI==null)
                            {
                                tempListI= new List<int>();
                            }
                            Debug.Log("Fin!tempListI is empty");
                        }
                    }
                    
                    // value = tempListI;
                    // Type tt=typeof(List<int>);
                    // t = tt;
                    return tempListI.ToArray();
                    // return Convert.ChangeType(value, value.GetType());
                    break;
                    case CellType.String:
                    Debug.Log("CT:String[]");
                    List<string> tempListSS = new List<string>();
                    List<string> addRSS= ConvertExt.ToStringArray(cell.StringCellValue).ToList<string>();
                    tempListSS.AddRange(addRSS);
                    value = tempListSS;
                    t = typeof(List<string>);
                    break;
                }
            }
            Debug.Log("valueType:"+value.GetType().ToString()+",tType:"+t.GetType().ToString());
            return Convert.ChangeType(value, t);
        }
        /// <summary>
        /// Convert type of cell value to its predefined type which is specified in the sheet's ScriptMachine setting file.
        /// </summary>
        protected object normalConvertFrom(ICell cell, Type t)
        {
            object value = null;
            if (t == typeof(float) || t == typeof(double) || t == typeof(short) || t == typeof(int) || t == typeof(long))
            {
                if (cell.CellType == NPOI.SS.UserModel.CellType.Numeric)
                {
                    value = cell.NumericCellValue;
                }
                else if (cell.CellType == NPOI.SS.UserModel.CellType.String)
                {
                    //Get correct numeric value even the cell is string type but defined with a numeric type in a data class.
                    if (t == typeof(float))
                        value = Convert.ToSingle(cell.StringCellValue);
                    if (t == typeof(double))
                        value = Convert.ToDouble(cell.StringCellValue);
                    if (t == typeof(short))
                        value = Convert.ToInt16(cell.StringCellValue);
                    if (t == typeof(int))
                        value = Convert.ToInt32(cell.StringCellValue);
                    if (t == typeof(long))
                        value = Convert.ToInt64(cell.StringCellValue);
                }
                else if (cell.CellType == NPOI.SS.UserModel.CellType.Formula)
                {
                    // Get value even if cell is a formula
                    if (t == typeof(float))
                        value = Convert.ToSingle(cell.NumericCellValue);
                    if (t == typeof(double))
                        value = Convert.ToDouble(cell.NumericCellValue);
                    if (t == typeof(short))
                        value = Convert.ToInt16(cell.NumericCellValue);
                    if (t == typeof(int))
                        value = Convert.ToInt32(cell.NumericCellValue);
                    if (t == typeof(long))
                        value = Convert.ToInt64(cell.NumericCellValue);
                }
            }
            else if (t == typeof(string) || t.IsArray)
            {
                // HACK: handles the case that a cell contains numeric value
                //       but a member field in a data class is defined as string type.
                //       e.g. string s = "123"
                if (cell.CellType == NPOI.SS.UserModel.CellType.Numeric)
                    value = cell.NumericCellValue;
                else
                    value = cell.StringCellValue;
            }
            else if (t == typeof(bool))
                value = cell.BooleanCellValue;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                var nc = new NullableConverter(t);
                return nc.ConvertFrom(value);
            }

            if (t.IsEnum)
            {
                // for enum type, first get value by string then convert it to enum.
                value = cell.StringCellValue;
                return Enum.Parse(t, value.ToString(), true);
            }
            else if (t.IsArray)
            {
                if (t.GetElementType() == typeof(float))
                    return ConvertExt.ToSingleArray((string)value);

                if (t.GetElementType() == typeof(double))
                    return ConvertExt.ToDoubleArray((string)value);

                if (t.GetElementType() == typeof(short))
                    return ConvertExt.ToInt16Array((string)value);

                if (t.GetElementType() == typeof(int))
                    return ConvertExt.ToInt32Array((string)value);

                if (t.GetElementType() == typeof(long))
                    return ConvertExt.ToInt64Array((string)value);

                if (t.GetElementType() == typeof(string))
                    return ConvertExt.ToStringArray((string)value);
            }

            // for all other types, convert its corresponding type.
            return Convert.ChangeType(value, t);
        }
    }
}
