using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace XkSystem.Code
{
    public class NpoiHelper
    {
        /// <summary>
        /// 将DataTable数据导入到excel中
        /// </summary>
        /// <param name="data">要导入的数据</param>
        /// <param name="isColumnWritten">DataTable的列名是否要导入</param>
        /// <param name="sheetName">要导入的excel的sheet的名称</param>
        /// <returns>导入数据行数(包含列名那一行)</returns>
        public static bool DataTableToExcel(string fileName, System.Data.DataTable data, string strHeaderText = "")
        {
            int i = 0;
            int j = 0;
            NPOI.SS.UserModel.ISheet sheet = null;
            var fs = new System.IO.FileStream(fileName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
            var workbook = new NPOI.XSSF.UserModel.XSSFWorkbook();
            //if (fileName.IndexOf(".xlsx") > 0) // 2007版本
            //    workbook = new XSSFWorkbook();
            //else if (fileName.IndexOf(".xls") > 0) // 2003版本
            //    workbook = new HSSFWorkbook();

            try
            {
                if (workbook != null)
                {
                    sheet = workbook.CreateSheet();
                }
                else
                {
                    return false;
                }

                var rowIndex = 0;
                if (string.IsNullOrEmpty(strHeaderText) == false)
                {
                    var headerRow = sheet.CreateRow(rowIndex);
                    headerRow.HeightInPoints = 25;
                    headerRow.CreateCell(0).SetCellValue(strHeaderText);

                    var headStyle = workbook.CreateCellStyle();
                    headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    var font = workbook.CreateFont();
                    font.FontHeightInPoints = 18;
                    //font.Boldweight = 700;
                    headStyle.SetFont(font);
                    headerRow.GetCell(0).CellStyle = headStyle;
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, data.Columns.Count - 1));

                    rowIndex = rowIndex + 1;
                }

                var cellStyle = workbook.CreateCellStyle();
                //设置单元格上下左右边框线  
                cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                //文字水平和垂直对齐方式  
                cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                cellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                //是否换行  
                //cellStyle.WrapText = true;  
                //缩小字体填充  
                cellStyle.ShrinkToFit = false;

                NPOI.SS.UserModel.IRow row = sheet.CreateRow(rowIndex);
                for (j = 0; j < data.Columns.Count; ++j)
                {
                    var cell = row.CreateCell(j);
                    cell.SetCellValue(data.Columns[j].ColumnName);
                    cell.CellStyle = cellStyle;
                }
                rowIndex = rowIndex + 1;

                for (i = 0; i < data.Rows.Count; ++i)
                {
                    row = sheet.CreateRow(rowIndex);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        var cell = row.CreateCell(j);
                        cell.SetCellValue(data.Rows[i][j].ToString());
                        cell.CellStyle = cellStyle;
                    }
                    rowIndex = rowIndex + 1;
                }

                for (j = 0; j < data.Columns.Count; ++j)
                {
                    sheet.AutoSizeColumn(j);
                }

                workbook.Write(fs);
                fs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool DataTableToExcel(string fileName, System.Data.DataTable data, bool isColumnWritten, List<NPOI.SS.Util.CellRangeAddress> regions, string strHeaderText = "", bool isWriteHeader = true)
        {
            int i = 0;
            int j = 0;
            NPOI.SS.UserModel.ISheet sheet = null;
            var fs = new System.IO.FileStream(fileName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
            var workbook = new NPOI.XSSF.UserModel.XSSFWorkbook();

            try
            {
                if (workbook != null)
                {
                    if (!String.IsNullOrWhiteSpace(strHeaderText))
                    {
                        sheet = workbook.CreateSheet(strHeaderText);
                    }
                    else
                    {
                        sheet = workbook.CreateSheet();
                    }
                }
                else
                {
                    return false;
                }

                var rowIndex = 0;
                if (!String.IsNullOrEmpty(strHeaderText) && isWriteHeader)
                {
                    var headerRow = sheet.CreateRow(rowIndex);
                    headerRow.HeightInPoints = 25;
                    headerRow.CreateCell(0).SetCellValue(strHeaderText);

                    var headStyle = workbook.CreateCellStyle();
                    headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    var font = workbook.CreateFont();
                    font.FontHeightInPoints = 18;
                    //font.Boldweight = 700;
                    headStyle.SetFont(font);
                    headerRow.GetCell(0).CellStyle = headStyle;
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, data.Columns.Count - 1));

                    rowIndex = rowIndex + 1;
                }

                var cellStyle = workbook.CreateCellStyle();
                //设置单元格上下左右边框线  
                cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                //文字水平和垂直对齐方式  
                cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                cellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                //是否换行  
                cellStyle.WrapText = true;
                //缩小字体填充  
                cellStyle.ShrinkToFit = false;

                NPOI.SS.UserModel.IRow row = sheet.CreateRow(rowIndex);

                if (isColumnWritten)
                {
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        var cell = row.CreateCell(j);
                        cell.SetCellValue(data.Columns[j].ColumnName);
                        cell.CellStyle = cellStyle;
                    }
                    rowIndex = rowIndex + 1;
                }

                for (i = 0; i < data.Rows.Count; ++i)
                {
                    row = sheet.CreateRow(rowIndex);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        var cell = row.CreateCell(j);
                        cell.SetCellValue(data.Rows[i][j].ToString());
                        cell.CellStyle = cellStyle;
                    }
                    rowIndex = rowIndex + 1;
                }

                for (j = 0; j < data.Columns.Count; ++j)
                {
                    sheet.AutoSizeColumn(j);
                }

                if (regions != null)
                {
                    foreach (NPOI.SS.Util.CellRangeAddress region in regions)
                    {
                        sheet.AddMergedRegion(region);
                    }
                }

                workbook.Write(fs);
                fs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 多标签页导出
        /// </summary>
        public class DataTableToExcelPram
        {
            public int sheetId { get; set; }
            public System.Data.DataTable data { get; set; }
            public bool isColumnWritten { get; set; }
            public List<NPOI.SS.Util.CellRangeAddress> regions { get; set; }
            public string strHeaderText { get; set; }
            public bool isWriteHeader { get; set; }
        }
        /// <summary>
        /// 多标签页导出
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="dataTableToExcelList"></param>
        /// <returns></returns>
        public static bool DataTableToExcel(string fileName, List<DataTableToExcelPram> dataTableToExcelList)
        {
            var workbook = new NPOI.XSSF.UserModel.XSSFWorkbook();
            var fs = new System.IO.FileStream(fileName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
            foreach (var sheetData in dataTableToExcelList)
            {
                int i = 0;
                int j = 0;
                NPOI.SS.UserModel.ISheet sheet = null;
                try
                {
                    #region 生成多头
                    if (workbook != null)
                    {
                        if (!String.IsNullOrWhiteSpace(sheetData.strHeaderText))
                        {
                            sheet = workbook.CreateSheet(sheetData.strHeaderText);
                        }
                        else
                        {
                            sheet = workbook.CreateSheet();
                        }
                    }
                    else
                    {
                        return false;
                    }

                    var rowIndex = 0;
                    if (!String.IsNullOrEmpty(sheetData.strHeaderText) && sheetData.isWriteHeader)
                    {
                        var headerRow = sheet.CreateRow(rowIndex);
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(sheetData.strHeaderText);

                        var headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                        var font = workbook.CreateFont();
                        font.FontHeightInPoints = 18;
                        headStyle.SetFont(font);
                        headerRow.GetCell(0).CellStyle = headStyle;
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, sheetData.data.Columns.Count - 1));

                        rowIndex = rowIndex + 1;
                    }

                    var cellStyle = workbook.CreateCellStyle();
                    //设置单元格上下左右边框线  
                    cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    //文字水平和垂直对齐方式  
                    cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    cellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                    //是否换行  
                    cellStyle.WrapText = true;
                    //缩小字体填充  
                    cellStyle.ShrinkToFit = false;

                    NPOI.SS.UserModel.IRow row = sheet.CreateRow(rowIndex);

                    if (sheetData.isColumnWritten)
                    {
                        for (j = 0; j < sheetData.data.Columns.Count; ++j)
                        {
                            var cell = row.CreateCell(j);
                            cell.SetCellValue(sheetData.data.Columns[j].ColumnName);
                            cell.CellStyle = cellStyle;
                        }
                        rowIndex = rowIndex + 1;
                    }

                    for (i = 0; i < sheetData.data.Rows.Count; ++i)
                    {
                        row = sheet.CreateRow(rowIndex);
                        for (j = 0; j < sheetData.data.Columns.Count; ++j)
                        {
                            var cell = row.CreateCell(j);
                            cell.SetCellValue(sheetData.data.Rows[i][j].ToString());
                            cell.CellStyle = cellStyle;
                        }
                        rowIndex = rowIndex + 1;
                    }

                    for (j = 0; j < sheetData.data.Columns.Count; ++j)
                    {
                        sheet.AutoSizeColumn(j);
                    }

                    if (sheetData.regions != null)
                    {
                        foreach (NPOI.SS.Util.CellRangeAddress region in sheetData.regions)
                        {
                            sheet.AddMergedRegion(region);
                        }
                    }
                    #endregion
                }
                catch
                {
                    continue;
                }
            }
            workbook.Write(fs);
            fs.Close();
            return true;
        }

        /*
        合并单元格并且设置边框不要用以下代码，很费时间，并且多次设置就出错了，根本不能用。
        ((HSSFSheet)sheet).SetEnclosedBorderOfRegion(region, NPOI.SS.UserModel.BorderStyle.THIN, NPOI.HSSF.Util.HSSFColor.BLACK.index);
        下面是我临时解决方案，希望有好的想法的可以互相交流
        protected void SetBorderOfRegion(ISheet sheet, int firstRowIndex, int lastRowIndex, int firstColIndex, int lastColIndex, bool hasBorder, ICellStyle cellStyleBoder)
        {

        CellRangeAddress region = new CellRangeAddress(firstRowIndex, lastRowIndex, firstColIndex, lastColIndex);
        if (hasBorder)
        {
            int temp = 1;
            for (int i = firstRowIndex; i <= lastRowIndex; i++)
            {
                IRow row = sheet.GetRow(i);
                for (int j = firstColIndex; j 1)
                {
                    ICell cell = row.CreateCell(j);
                    cell.CellStyle = cellStyleBoder;
                }
        else
        {
                    temp++;
                }
            }
        }
        }
        sheet.AddMergedRegion(region);
        }
        }
        */

        /// <summary>读取excel
        /// 默认第一行为表头
        /// </summary>
        /// <param name="strFileName">excel文档绝对路径</param>
        /// <param name="rowIndex">内容行偏移量，第一行为表头，内容行从第二行开始则为1</param>
        /// <returns></returns>
        public static System.Data.DataTable Import(string strFileName, int rowIndex)
        {
            var dt = new System.Data.DataTable();

            NPOI.SS.UserModel.IWorkbook hssfworkbook;
            using (var file = new System.IO.FileStream(strFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                hssfworkbook = NPOI.SS.UserModel.WorkbookFactory.Create(file);
            }
            var sheet = hssfworkbook.GetSheetAt(0);

            var headRow = sheet.GetRow(0);
            if (headRow != null)
            {
                int colCount = headRow.LastCellNum;
                for (int i = 0; i < colCount; i++)
                {
                    dt.Columns.Add("COL_" + i);
                }
            }

            for (int i = (sheet.FirstRowNum + rowIndex); i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                bool emptyRow = true;
                object[] itemArray = null;

                if (row != null)
                {
                    itemArray = new object[row.LastCellNum];

                    for (int j = row.FirstCellNum; j < row.LastCellNum; j++)
                    {

                        if (row.GetCell(j) != null)
                        {

                            switch (row.GetCell(j).CellType)
                            {
                                case NPOI.SS.UserModel.CellType.Numeric:
                                    if (NPOI.HSSF.UserModel.HSSFDateUtil.IsCellDateFormatted(row.GetCell(j)))//日期类型
                                    {
                                        itemArray[j] = row.GetCell(j).DateCellValue.ToString("yyyy-MM-dd");
                                    }
                                    else//其他数字类型
                                    {
                                        itemArray[j] = row.GetCell(j).NumericCellValue;
                                    }
                                    break;
                                case NPOI.SS.UserModel.CellType.Blank:
                                    itemArray[j] = string.Empty;
                                    break;
                                case NPOI.SS.UserModel.CellType.Formula:
                                    if (System.IO.Path.GetExtension(strFileName).ToLower().Trim() == ".xlsx")
                                    {
                                        var eva = new NPOI.XSSF.UserModel.XSSFFormulaEvaluator(hssfworkbook);
                                        if (eva.Evaluate(row.GetCell(j)).CellType == NPOI.SS.UserModel.CellType.Numeric)
                                        {
                                            itemArray[j] = eva.Evaluate(row.GetCell(j)).NumberValue;
                                        }
                                        else
                                        {
                                            itemArray[j] = eva.Evaluate(row.GetCell(j)).StringValue;
                                        }
                                    }
                                    else
                                    {
                                        var eva = new NPOI.HSSF.UserModel.HSSFFormulaEvaluator(hssfworkbook);
                                        if (eva.Evaluate(row.GetCell(j)).CellType == NPOI.SS.UserModel.CellType.Numeric)
                                        {
                                            itemArray[j] = eva.Evaluate(row.GetCell(j)).NumberValue;
                                        }
                                        else
                                        {
                                            itemArray[j] = eva.Evaluate(row.GetCell(j)).StringValue;
                                        }
                                    }
                                    break;
                                default:
                                    itemArray[j] = row.GetCell(j).StringCellValue;
                                    break;

                            }

                            if (itemArray[j] != null && !string.IsNullOrEmpty(itemArray[j].ToString().Trim()))
                            {
                                emptyRow = false;
                            }
                        }
                    }
                }

                //非空数据行数据添加到DataTable
                if (!emptyRow)
                {
                    dt.Rows.Add(itemArray);
                }
            }
            return dt;
        }

        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="fileName">excel路径</param>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>返回的DataTable</returns>
        public static System.Data.DataTable ExcelToDataTable(string fileName, string fileEx, string sheetName)
        {
            NPOI.SS.UserModel.ISheet sheet = null;
            var data = new System.Data.DataTable();
            int startRow = 0;
            NPOI.SS.UserModel.IWorkbook workbook = null;
            try
            {
                var fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                if (string.Compare(fileEx, ".xlsx", true) == decimal.Zero) // 2007版本
                    workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(fs);
                else if (string.Compare(fileEx, ".xls", true) == decimal.Zero) // 2003版本
                    workbook = new NPOI.HSSF.UserModel.HSSFWorkbook(fs);

                if (string.IsNullOrEmpty(sheetName))
                {
                    sheet = workbook.GetSheetAt(0);
                }
                else
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }

                if (sheet != null)
                {
                    var firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                    {
                        var cell = firstRow.GetCell(i);
                        if (cell != null)
                        {
                            var cellValue = cell.StringCellValue.Trim();
                            if (cellValue != null)
                            {
                                var column = new System.Data.DataColumn(cellValue);
                                data.Columns.Add(column);
                            }
                        }
                    }

                    startRow = sheet.FirstRowNum + 1;

                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        var row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        var dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                                dataRow[j] = row.GetCell(j).ToString().Trim();
                        }
                        data.Rows.Add(dataRow);
                    }
                }

                return data;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 将excel中的数据导入到DataTable中（多行表头）
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileEx">文件格式</param>
        /// <param name="sheetName">导入数据的SheetName</param>
        /// <param name="headerRowNums">表头行数组</param>
        /// <returns></returns>
        public static System.Data.DataTable ExcelToDataTable(string filePath, string fileEx, string sheetName, int[] headerRowNums)
        {
            NPOI.SS.UserModel.ISheet sheet = null;
            var data = new System.Data.DataTable();
            int startRow = 0;
            NPOI.SS.UserModel.IWorkbook workbook = null;
            try
            {
                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                if (string.Compare(fileEx, ".xlsx", true) == decimal.Zero) // 2007版本
                {
                    workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(fs);
                }
                else if (string.Compare(fileEx, ".xls", true) == decimal.Zero) // 2003版本
                {
                    workbook = new NPOI.HSSF.UserModel.HSSFWorkbook(fs);
                }

                if (string.IsNullOrEmpty(sheetName))
                {
                    sheet = workbook.GetSheetAt(0);
                }
                else
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }

                if (sheet != null)
                {
                    var firstRow = sheet.GetRow(headerRowNums[0]);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                    // data列 = headerRowNums中全部组合
                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                    {
                        StringBuilder columnName = new StringBuilder();
                        foreach (int rowNum in headerRowNums)
                        {
                            var row = sheet.GetRow(rowNum);
                            var cell = row.GetCell(i);
                            var cellValue = cell.StringCellValue.Trim();
                            if (cellValue != null)
                            {
                                columnName.Append(cellValue);
                            }
                        }

                        var column = new System.Data.DataColumn(columnName.ToString());
                        data.Columns.Add(column);
                    }

                    startRow = headerRowNums.Max() + 1; // 数据开始行为headerRowNums中最大值 + 1

                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        var row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        var dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                                dataRow[j] = row.GetCell(j).ToString().Trim();
                        }
                        data.Rows.Add(dataRow);
                    }
                }

                return data;
            }
            catch
            {
                return null;
            }
        }

        public static System.Data.DataTable ExcelToDataTable(string filePath, string fileEx, int sheetIndex = 0)
        {
            var dt = new System.Data.DataTable();

            NPOI.SS.UserModel.ISheet sheet = null;
            NPOI.SS.UserModel.IWorkbook workbook = null;
            try
            {
                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                if (string.Compare(fileEx, ".xlsx", true) == decimal.Zero)
                {
                    workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(fs);
                }
                else if (string.Compare(fileEx, ".xls", true) == decimal.Zero)
                {
                    workbook = new NPOI.HSSF.UserModel.HSSFWorkbook(fs);
                }

                sheet = workbook.GetSheetAt(sheetIndex);
                if (sheet != null)
                {
                    var firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum;

                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                    {
                        var cell = firstRow.GetCell(i);
                        if (cell != null)
                        {
                            var cellValue = cell.StringCellValue.Trim();
                            if (String.IsNullOrWhiteSpace(cellValue))
                            {
                                cellValue = i.ToString();
                            }

                            var column = new System.Data.DataColumn(cellValue);
                            dt.Columns.Add(column);
                        }
                    }

                    int startRow = sheet.FirstRowNum + 1;

                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        var row = sheet.GetRow(i);
                        if (row != null)
                        {
                            var dataRow = dt.NewRow();
                            for (int j = row.FirstCellNum; j < cellCount; ++j)
                            {
                                if (row.GetCell(j) != null)
                                {
                                    dataRow[j] = row.GetCell(j).ToString().Trim();
                                }
                            }
                            dt.Rows.Add(dataRow);
                        }
                    }
                }
                return dt;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="path">excel路径</param>
        /// <param name="sheetName">excel工作薄sheet的名称(默认第一个)</param>
        /// <returns>返回的DataTable</returns>
        public static System.Data.DataTable ExcelToDataTable(string path, string sheetName = null)
        {
            NPOI.SS.UserModel.ISheet sheet = null;
            var data = new System.Data.DataTable();
            int startRow = 0;
            NPOI.SS.UserModel.IWorkbook workbook = null;
            try
            {
                var fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                if (string.Compare(path.Split('.').Last(), "xlsx", true) == decimal.Zero) // 2007版本
                    workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(fs);
                else //if (string.Compare(fileEx, ".xls", true) == decimal.Zero) // 2003版本
                    workbook = new NPOI.HSSF.UserModel.HSSFWorkbook(fs);

                if (string.IsNullOrEmpty(sheetName))
                {
                    sheet = workbook.GetSheetAt(0);
                }
                else
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }

                if (sheet != null)
                {
                    var firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                    {
                        var cell = firstRow.GetCell(i);
                        if (cell != null)
                        {
                            var cellValue = cell.StringCellValue.Trim();
                            if (cellValue != null)
                            {
                                var column = new System.Data.DataColumn(cellValue);
                                data.Columns.Add(column);
                            }
                        }
                    }

                    startRow = sheet.FirstRowNum + 1;

                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        var row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        var dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                                dataRow[j] = row.GetCell(j).ToString().Trim();
                        }
                        data.Rows.Add(dataRow);
                    }
                }

                return data;
            }
            catch
            {
                return null;
            }
        }

        public void Word()
        {
            using (var stream = System.IO.File.OpenRead(@"d:\1.docx"))
            {
                var doc = new NPOI.XWPF.UserModel.XWPFDocument(stream);
                foreach (var para in doc.Paragraphs)
                {
                    //string text = para.ParagraphText; //获得文本
                    //string styleid = para.Style;

                    //for (int i = 0; i < runs.Count; i++)
                    //{
                    //    var run = runs[i];
                    //    text = run.ToString(); //获得run的文本
                    //}

                    para.ReplaceText("[关键字]", "[替换]");
                }

                var tables = doc.Tables;
                foreach (var table in tables)    //遍历表格
                {
                    foreach (var row in table.Rows)    //遍历行
                    {
                        foreach (var cell in row.GetTableCells())
                        {
                            foreach (var para in cell.Paragraphs)
                            {
                                //string text = para.ParagraphText;
                                //处理段落

                                para.ReplaceText("[关键字]", "[替换]");
                            }
                        }
                    }
                }


                //插入图片
                var gfs = new System.IO.FileStream("f:\\pic\\1.jpg", System.IO.FileMode.Open, System.IO.FileAccess.Read);
                var gp = doc.CreateParagraph();
                var gr = gp.CreateRun();
                gr.AddPicture(gfs, (int)NPOI.XWPF.UserModel.PictureType.JPEG, "1.jpg", 1000000, 1000000);
                gfs.Close();

                var sw = System.IO.File.OpenWrite(@"d:\new.docx");
                doc.Write(sw);
                sw.Close();

                //var newDoc = new NPOI.XWPF.UserModel.XWPFDocument();
                //var sw = System.IO.File.OpenWrite("newSave.docx");
                //newDoc.
                //newDoc.Write(sw);
                //sw.Close();
            }
        }

        public static void WordExport()
        {
            string filepath = HttpContext.Current.Server.MapPath("~/simpleTable.docx");
            var tt = new WordReplace { name = "cjc", age = 29 };
            using (var stream = System.IO.File.OpenRead(filepath))
            {
                var doc = new NPOI.XWPF.UserModel.XWPFDocument(stream);
                //遍历段落                  
                foreach (var para in doc.Paragraphs)
                {
                    WorkReplaceKey(para, tt);
                }                    //遍历表格      
                var tables = doc.Tables;
                foreach (var table in tables)
                {
                    foreach (var row in table.Rows)
                    {
                        foreach (var cell in row.GetTableCells())
                        {
                            foreach (var para in cell.Paragraphs)
                            {
                                WorkReplaceKey(para, tt);
                            }
                        }
                    }
                }

                var out1 = new System.IO.FileStream(HttpContext.Current.Server.MapPath("~/simpleTable" + DateTime.Now.Ticks + ".docx"), System.IO.FileMode.Create);
                doc.Write(out1);
                out1.Close();
            }
        }

        private static void WorkReplaceKey(NPOI.XWPF.UserModel.XWPFParagraph para, object model)
        {
            string text = para.ParagraphText;
            var runs = para.Runs;
            string styleid = para.Style;
            for (int i = 0; i < runs.Count; i++)
            {
                var run = runs[i];
                text = run.ToString();
                Type t = model.GetType();
                var pi = t.GetProperties();
                foreach (var p in pi)
                {
                    //$$与模板中$$对应，也可以改成其它符号，比如{$name},务必做到唯一
                    if (text.Contains("$" + p.Name + "$"))
                    {
                        text = text.Replace("$" + p.Name + "$", p.GetValue(model, null).ToString());
                    }
                }
                runs[i].SetText(text, 0);
            }
        }

        public class WordReplace
        {
            public string name { get; set; }
            public int age { get; set; }
        }
    }
}