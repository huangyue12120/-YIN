using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityQuickSheet;
public class MachineQueryCellTypeBinder 
{
    static private ExcelQuery excelQuery;
    static private List<CellType> ct = new List<CellType>(); 
    static private List<bool> iarr = new List<bool>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static void readMachine(BaseMachine baseMachine)
    {
        if(ct!=null)
        {
            ct = new List<CellType>();
        }
        else
        {
            ct.Clear();
        }
        if(iarr!=null)
        {
            iarr = new List<bool>();
        }
        else
        {
            iarr.Clear();
        }
        for(var i =0;i<baseMachine.ColumnHeaderList.Count;i++)
        {
            ct.Add(baseMachine.ColumnHeaderList[i].type);
            iarr.Add(baseMachine.ColumnHeaderList[i].isArray);
        }
    }
    public static List<CellType> bind(ExcelQuery ine)
    {
        return ct;
        // List<CellType> cellTypes= new List<CellType>();
        // for( var i =0;i<machine.ColumnHeaderList.Count;i++)
        // {
        //     cellTypes.Add(machine.ColumnHeaderList[i].type);
        // }
        // return cellTypes;
    }
    public static List<bool> bind2(ExcelQuery ine)
    {
        return iarr;
    }
}
