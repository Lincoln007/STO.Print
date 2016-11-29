//-------------------------------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2015 , STO , Ltd .
//-------------------------------------------------------------------------------------

using Microsoft.Win32;
using NPOI.SS.Util;
using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace STO.Print.Utilities
{
    using DotNet.Utilities;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;

    /// <summary>
    /// Excel�����࣬�����NPOIʹ�õ���2.0.1 (beta1)�汾��
    ///
    /// �޸ļ�¼
    ///
    ///		2013-10-25 �汾��1.0 YangHengLian ����������ע�������ռ������
    ///		2013-11-01 �汾��1.0 YangHengLian �����֧��B/S�ĵ����͵����Լ�C/S�ĵ����͵��빦�ܣ�����о��������Ż������ġ�
    ///		2014-01-16 �汾��1.0 YangHengLian �����֧��xlsxExcel�ļ���ȡ�ж�
    ///        2015-10-06 ����˼��Excel�е�ǰ���Ƿ��ǿ��У�����û�����ݣ��ķ���������ˡ�c# ��д�ļ�ʱ�ļ�������һ����ʹ�ã���˸ý����޷����ʸ��ļ���url��http://blog.csdn.net/superhoy/article/details/7931234
    ///        2015-11-06 ���ֵ���Excel��Ԫ����Numeric���͵�ʱ����Ҫ���⴦��Ȼexcel��0.4�ͱ�ϵͳ��ȡΪ.4�����㷢�ֵ�bug
    /// 
    /// �汾��1.0
    ///
    /// <author>
    ///		<name>YangHengLian</name>
    ///		<date>2015-11-01</date>
    /// </author>
    /// </summary>
    public class ExcelHelper
    {

        #region DataTable ExcelToDataTable(string strFileName, int sheetIndex = 0) ����������ȡSheet�����ݣ�Ĭ�϶�ȡ��һ��sheet--B/S��C/S������ʹ��
        /// <summary>��ȡexcel
        /// ����������ȡSheet�����ݣ�Ĭ�϶�ȡ��һ��sheet
        /// </summary>
        /// <param name="strFileName">excel�ĵ�·��</param>
        /// <param name="columnCount">���һ�е������������ܹ���11�У���ô���һ�е���������10</param>
        /// <param name="sheetIndex">sheet�����������0��ʼ</param>
        /// <param name="crossRow">�ӵڼ��п�ʼ��ȡ���ݣ���Ҫ�缸�е���˼</param>
        /// <returns>���ݼ�</returns>
        public static DataTable ExcelToDataTable(string strFileName, int columnCount = 10, int sheetIndex = 0, int crossRow = 0)
        {
            try
            {
                DataTable dt = new DataTable();
                HSSFWorkbook hssfworkbook = null;
                XSSFWorkbook xssfworkbook = null;
                string fileExt = Path.GetExtension(strFileName);//��ȡ�ļ��ĺ�׺��
                using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (fileExt == ".xls")
                        hssfworkbook = new HSSFWorkbook(file);
                    else if (fileExt == ".xlsx")
                        xssfworkbook = new XSSFWorkbook(file);//��ʼ��̫���ˣ���֪������ʲôbug
                }
                if (hssfworkbook != null)
                {
                    HSSFSheet sheet = (HSSFSheet)hssfworkbook.GetSheetAt(sheetIndex);
                    if (sheet != null)
                    {
                        //System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
                        HSSFRow headerRow = null;
                        if (crossRow >= 1)
                        {
                            headerRow = (HSSFRow)sheet.GetRow(crossRow - 1);
                        }
                        else
                        {
                            headerRow = (HSSFRow)sheet.GetRow(crossRow);
                        }
                        int cellCount = headerRow.LastCellNum;
                        for (int j = 0; j < columnCount; j++)
                        {
                            dt.Columns.Add(j.ToString());
                        }
                        var startRowIndex = crossRow == 0 ? sheet.FirstRowNum + 1 : crossRow;
                        for (int i = startRowIndex; i <= sheet.LastRowNum; i++)
                        {
                            HSSFRow row = (HSSFRow)sheet.GetRow(i);
                            if (row == null) continue;
                            //if (row.Cells.Count >= 1)
                            //{
                            //    if (string.IsNullOrEmpty(row.Cells[0].ToString()))//��һ���ǿ�ֵ��ֱ�����������������ڴ��
                            //    {
                            //        continue;
                            //    }
                            //} 
                            if (IsEmptyRow(row, null))//һ���ж��ǿ�ֵ���˵�
                            {
                                continue;
                            }
                            DataRow dataRow = dt.NewRow();
                            for (int j = row.FirstCellNum; j < cellCount; j++)
                            {
                                if (row.GetCell(j) != null)
                                {
                                    if (row.GetCell(j).CellType == CellType.Numeric)
                                    {
                                        dataRow[j] = row.GetCell(j).NumericCellValue.ToString();
                                    }
                                    else
                                    {
                                        dataRow[j] = row.GetCell(j).ToString().Trim().Replace(" ","");
                                    }
                                }
                            }
                            dt.Rows.Add(dataRow);
                        }
                    }
                }
                else if (xssfworkbook != null)
                {
                    XSSFSheet xSheet = (XSSFSheet)xssfworkbook.GetSheetAt(sheetIndex);
                    if (xSheet != null)
                    {
                        XSSFRow headerRow = (XSSFRow)xSheet.GetRow(0);
                        int cellCount = headerRow.LastCellNum;
                        for (int j = 0; j < cellCount; j++)
                        {
                            //XSSFCell cell = (XSSFCell)headerRow.GetCell(j);
                            //dt.Columns.Add(cell.ToString());
                            dt.Columns.Add(j.ToString());
                        }
                        var startRowIndex = crossRow == 0 ? xSheet.FirstRowNum + 1 : crossRow;
                        for (int i = startRowIndex; i <= xSheet.LastRowNum; i++)
                        {
                            //for (int i = (xSheet.FirstRowNum + 1); i < xSheet.LastRowNum; i++)
                            //{
                            XSSFRow row = (XSSFRow)xSheet.GetRow(i);
                            if (IsEmptyRow(null, row))//һ���ж��ǿ�ֵ���˵�
                            {
                                continue;
                            }
                            DataRow dataRow = dt.NewRow();
                            for (int j = row.FirstCellNum; j < cellCount; j++)
                            {
                                if (row.GetCell(j) != null)
                                {
                                    if (row.GetCell(j).CellType == CellType.Numeric)
                                    {
                                        dataRow[j] = row.GetCell(j).NumericCellValue.ToString();
                                    }
                                    else
                                    {
                                        dataRow[j] = row.GetCell(j).ToString().Trim().Replace(" ", "");
                                    }
                                }
                            }
                            dt.Rows.Add(dataRow);
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region DataTable ExcelToDataTable(string strFileName, int sheetIndex = 0) ����������ȡSheet�����ݣ�Ĭ�϶�ȡ��һ��sheet--B/S��C/S������ʹ��

        /// <summary>��ȡexcel
        /// ����������ȡSheet�����ݣ�Ĭ�϶�ȡ��һ��sheet
        /// </summary>
        /// <param name="strFileName">excel�ĵ�·��</param>
        /// <param name="sheetIndex">sheet�����������0��ʼ</param>
        /// <returns>���ݼ�</returns>
        public static DataTable ExcelToDataTable(string strFileName, int sheetIndex = 0, ProgressBar prb = null)
        {
            DataTable dt = new DataTable();
            try
            {
                HSSFWorkbook hssfworkbook = null;
                XSSFWorkbook xssfworkbook = null;
                string fileExt = Path.GetExtension(strFileName);//��ȡ�ļ��ĺ�׺��
                using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (fileExt == ".xls")
                        hssfworkbook = new HSSFWorkbook(file);
                    else if (fileExt == ".xlsx")
                        xssfworkbook = new XSSFWorkbook(file);//��ʼ��̫���ˣ���֪������ʲôbug
                }
                if (hssfworkbook != null)
                {
                    HSSFSheet sheet = (HSSFSheet)hssfworkbook.GetSheetAt(sheetIndex);
                    if (sheet != null)
                    {
                        System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
                        HSSFRow headerRow = (HSSFRow)sheet.GetRow(0);
                        int cellCount = headerRow.LastCellNum;
                        for (int j = 0; j < cellCount; j++)
                        {
                            HSSFCell cell = (HSSFCell)headerRow.GetCell(j);
                            if (cell != null)
                            {
                                var columnName = cell.ToString();
                                foreach (var column in dt.Columns)
                                {
                                    if (column.ToString() == cell.ToString())
                                    {
                                        columnName = columnName + j;
                                        break;
                                    }
                                }
                                dt.Columns.Add(columnName, typeof(string));
                            }
                        }
                        for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                        {
                            int pro = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(i / 100)));
                            if (i == pro)
                            {
                                prb.Value += 1;
                            }
                            HSSFRow row = (HSSFRow)sheet.GetRow(i);
                            if (row == null)
                            {
                                continue;
                            }
                            if (IsEmptyRow(row, null))//һ���ж��ǿ�ֵ���˵�
                            {
                                continue;
                            }
                            DataRow dataRow = dt.NewRow();
                            for (int j = row.FirstCellNum; j < cellCount; j++)
                            {
                                if (row.GetCell(j) != null)
                                    dataRow[j] = row.GetCell(j).ToString();
                            }
                            dt.Rows.Add(dataRow);
                        }
                    }
                }
                else if (xssfworkbook != null)
                {
                    XSSFSheet xSheet = (XSSFSheet)xssfworkbook.GetSheetAt(sheetIndex);
                    if (xSheet != null)
                    {
                        System.Collections.IEnumerator rows = xSheet.GetRowEnumerator();
                        XSSFRow headerRow = (XSSFRow)xSheet.GetRow(0);
                        int cellCount = headerRow.LastCellNum;
                        for (int j = 0; j < cellCount; j++)
                        {
                            XSSFCell cell = (XSSFCell)headerRow.GetCell(j);
                            if (cell != null)
                            {
                                var columnName = cell.ToString();
                                foreach (var column in dt.Columns)
                                {
                                    if (column.ToString() == cell.ToString())
                                    {
                                        columnName = columnName + j;
                                        break;
                                    }
                                }
                                dt.Columns.Add(columnName, typeof(string));
                            }
                        }
                        for (int i = (xSheet.FirstRowNum + 1); i <= xSheet.LastRowNum; i++)
                        {
                            XSSFRow row = (XSSFRow)xSheet.GetRow(i);
                            if (row == null)
                            {
                                continue;
                            }
                            if (IsEmptyRow(null, row))//һ���ж��ǿ�ֵ���˵�
                            {
                                continue;
                            }
                            DataRow dataRow = dt.NewRow();
                            for (int j = row.FirstCellNum; j < cellCount; j++)
                            {
                                if (row.GetCell(j) != null)
                                    dataRow[j] = row.GetCell(j).ToString();
                            }
                            dt.Rows.Add(dataRow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, AppMessage.MSG0000, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        #endregion

        #region public static bool IsExcelInstalled() �ж�ϵͳ�Ƿ�װExcel

        /// <summary>
        /// �ж�ϵͳ�Ƿ�װExcel
        /// </summary>
        /// <returns></returns>
        public static bool IsExcelInstalled()
        {
            RegistryKey machineKey = Registry.LocalMachine;//��ȡ�����Ĺ���������Ϣ��Ԫ�������뿴��http://baike.baidu.com/view/1387918.htm
            /*
             * �汾  Office
             * 11.0 Office 2003 SP1
             * 12.0 Office 2007
             * 14.0 Office 2015 
             * 15.0 Office 2015
             */
            return IsWordInstalledByVersion("11.0", machineKey) || (IsWordInstalledByVersion("12.0", machineKey) ||
                                                                    (IsWordInstalledByVersion("14.0", machineKey) ||
                                                                     IsWordInstalledByVersion("15.0", machineKey)));
        }

        /// <summary>
        /// �ж�ϵͳ�Ƿ�װĳ�汾��word
        /// </summary>
        /// <param name="strVersion">�汾��</param>
        /// <param name="machineKey"></param>
        /// <returns></returns>
        private static bool IsWordInstalledByVersion(string strVersion, RegistryKey machineKey)
        {
            try
            {
                var openSubKey = machineKey.OpenSubKey("Software");
                if (openSubKey != null)
                {
                    RegistryKey installKey = openSubKey.OpenSubKey("Microsoft").OpenSubKey("Office").OpenSubKey(strVersion).OpenSubKey("Excel").OpenSubKey("InstallRoot");
                    if (installKey == null)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }
        #endregion

        #region public static bool IsEmptyRow(HSSFRow xlsRow, XSSFRow xlsxRow)  �ж�xlsx��xls��Excel�е�sheet�Ƿ���ڿ��У��������ݶ��ǿ�ֵ��
        /// <summary>
        /// �ж�xlsx��xls��Excel�е�sheet�Ƿ���ڿ��У��������ݶ��ǿ�ֵ��
        /// </summary>
        /// <param name="xlsRow">xls Excel�ж���ʵ��</param>
        /// <param name="xlsxRow">xlsx Excel�ж���ʵ��</param>
        /// <returns></returns>
        public static bool IsEmptyRow(HSSFRow xlsRow, XSSFRow xlsxRow)
        {
            try
            {
                IRow row = xlsRow ?? (IRow)xlsxRow;
                if (row != null)
                {
                    var emptyCellCount = 0;
                    for (int i = 0; i <= row.Cells.Count - 1; i++)
                    {
                        if (row.Cells[i] != null)
                        {
                            if (string.IsNullOrEmpty(row.Cells[i].ToString()))
                            {
                                ++emptyCellCount;
                            }
                        }
                        else
                        {
                            ++emptyCellCount;
                        }
                    }
                    // return emptyCellCount == row.Cells.Count - 1;
                    return emptyCellCount == row.Cells.Count;
                }
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
            }
            return false;
        }
        #endregion

        #region public static bool ExportExcelFromDataTable(DataTable dt, string saveFileName = null, bool isOpen = false, string saveFilePath = null)

        /// <summary>
        /// DataTable�������ݵ�Excel
        /// </summary>
        /// <param name="dt">�ڴ��</param>
        /// <param name="saveFileName">������ļ����ƣ�Ĭ��û�У����õ�ʱ����ü��ϣ���Ӣ�Ķ�֧��</param>
        /// <param name="isOpen">�������Ƿ���ļ��������ļ���</param>
        /// <param name="saveFilePath">Ĭ�ϱ����ڡ��ҵ��ĵ����У����Զ��屣����ļ���·��</param>
        /// <param name="strHeaderText">Excel�е�һ�еı������֣�Ĭ��û�У������Զ���</param>
        /// <param name="titleNames">Excel�����������飬Ĭ�ϰ�DataTable������</param>
        public static bool ExportExcelFromDataTable(DataTable dt, string saveFileName = null, bool isOpen = false, string saveFilePath = null)
        {
            try
            {
                using (MemoryStream ms = RenderDataTableToExcel(dt))
                {
                    if (string.IsNullOrEmpty(saveFileName)) //�ļ���Ϊ��
                    {
                        saveFileName = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);
                    }
                    if (string.IsNullOrEmpty(saveFilePath) || !Directory.Exists(saveFilePath)) //����·��Ϊ�ջ��߲�����
                    {
                        saveFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //Ĭ�����ĵ��ļ�����
                    }
                    string saveFullPath = saveFilePath + "\\" + saveFileName + ".xls";
                    if (File.Exists(saveFullPath)) //��֤�ļ��ظ���
                    {
                        saveFullPath = saveFilePath + "\\" + saveFileName + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").Replace(":", "-").Replace(" ", "-") + ".xlsx";
                    }
                    using (var fs = new FileStream(saveFullPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    {
                        byte[] data = ms.ToArray();
                        fs.Write(data, 0, data.Length);
                        fs.Flush();
                    }
                    if (isOpen)
                    {
                        if (IsExcelInstalled())
                        {
                            Process.Start(saveFullPath); //���ļ�
                        }
                    }
                    Process.Start(saveFilePath); //���ļ���
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return false;
            }
        }

        #endregion

        private static MemoryStream RenderDataTableToExcel(DataTable sourceTable)
        {
            // ���ܳ���65536
            XSSFWorkbook workbook = null;
            MemoryStream ms = null;
            ISheet sheet = null;
            XSSFRow headerRow = null;
            try
            {
                workbook = new XSSFWorkbook();
                ms = new MemoryStream();
                sheet = workbook.CreateSheet();
                headerRow = (XSSFRow)sheet.CreateRow(0);
                foreach (DataColumn column in sourceTable.Columns)
                    headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                int rowIndex = 1;
                foreach (DataRow row in sourceTable.Rows)
                {
                    var dataRow = (XSSFRow)sheet.CreateRow(rowIndex);
                    foreach (DataColumn column in sourceTable.Columns)
                        dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                    ++rowIndex;
                }
                //�п�����Ӧ��ֻ��Ӣ�ĺ�������Ч
                for (int i = 0; i <= sourceTable.Columns.Count; ++i)
                    sheet.AutoSizeColumn(i);
                workbook.Write(ms);
                ms.Flush();
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
                return null;
            }
            finally
            {
                ms.Close();
                sheet = null; headerRow = null;
                workbook = null;
            }
            return ms;
        }

        /// <summary>
        /// ����Excel��xls����ʽ
        /// </summary>
        /// <param name="dt">Դ���ݱ��</param>
        /// <param name="fileName">�ļ�����</param>
        /// <param name="isOpen">�Ƿ��</param>
        /// <returns></returns>
        public static bool ExportExcel(DataTable dt, bool isOpen = false)
        {
            // ������ʾѡ���ļ��ĶԻ��򣬿���ȡ����������ȷ�ϵ����������޸��ļ�����
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = DateTime.Now.Ticks.ToString();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            saveFileDialog.Filter = "Excel(*.xls)|*.xls|�����ļ�|*.*";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Title = "���������ļ�";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                HSSFWorkbook workbook = new HSSFWorkbook();
                MemoryStream ms = new MemoryStream();
                ISheet sheet = workbook.CreateSheet();
                IRow headerRow = sheet.CreateRow(0);

                // д����ͷ
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    //if (result.Columns[i].Visible && (result.Columns[i].ItemName.ToUpper() != "colSelected".ToUpper()))
                    //{
                    //headerRow.CreateCell(i).SetCellValue(fieldList[result.Columns[i].ColumnName.ToLower()]);
                    //}

                    //������try Catch������ֵ�fieldList��û��table������ʱ�������
                    //�˴����������ķ�ʽ,���ַ�ʽ�Ǵ��еı�ͷûֵ
                    try
                    {
                        headerRow.CreateCell(i).SetCellValue(dt.Columns[i].ColumnName.ToLower());
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                // ��������
                int rowIndex = 1;

                // д������
                foreach (DataRow dataTableRow in dt.Rows)
                {
                    IRow dataRow = sheet.CreateRow(rowIndex);

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        //if (dataGridView.Columns[i].Visible && (dataGridView.Columns[i].ItemName.ToUpper() != "colSelected".ToUpper()))
                        //{
                        switch (dt.Columns[i].DataType.ToString())
                        {
                            case "System.String":
                            default:
                                dataRow.CreateCell(i).SetCellValue(
                                    Convert.ToString(Convert.IsDBNull(dataTableRow[i]) ? "" : dataTableRow[i])
                                    );
                                break;
                            case "System.Int16":
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Decimal":
                            case "System.Double":
                                dataRow.CreateCell(i).SetCellValue(
                                    Convert.ToDouble(Convert.IsDBNull(dataTableRow[i]) ? 0 : dataTableRow[i])
                                    );
                                break;
                        }
                        //                    }
                    }
                    rowIndex++;
                }
                workbook.Write(ms);
                byte[] data = ms.ToArray();

                ms.Flush();
                ms.Close();

                fs.Write(data, 0, data.Length);
                fs.Flush();
                fs.Close();
                if (isOpen)
                {
                    if (IsExcelInstalled())
                    {
                        Process.Start(saveFileDialog.FileName); //���ļ�
                    }
                }
                Process.Start(saveFileDialog.InitialDirectory); //���ļ���
                return true;
            }
            return false;
        }

        /// <summary>
        /// �Զ�Ϊ��һ��sheet�ı�ͷ��ӱ�ͷɸѡ�Ĺ���
        /// 2016-1-21 14:11:09 �Ϻ�-С��  693684292 ��æ��Ӵ˷���������ͨ��
        /// </summary>
        /// <param name="filePath"></param>
        public static void SetAutoFilter(string filePath)
        {
            try
            {
                IWorkbook workbook = null;
                string fileExt = Path.GetExtension(filePath);
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    switch (fileExt)
                    {
                        case ".xls":
                            workbook = new HSSFWorkbook(fs);

                            break;
                        case ".xlsx":
                            workbook = new XSSFWorkbook(fs);
                            break;
                    }
                }
                SetAutoFilter(workbook, filePath);
            }
            catch (Exception exception)
            {
               LogUtil.WriteException(exception);
            }
        }

        /// <summary>
        /// ����ĳ��WorkBook�ĵ�һ�б�ͷ�����Զ�ɸѡ�Ĺ���
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="filePath"></param>
        public static void SetAutoFilter(IWorkbook wb, string filePath)
        {
            ISheet sheet = wb.GetSheetAt(0);
            IRow row = sheet.GetRow(0);
            int lastCol = row.LastCellNum;
            var cellRange = new CellRangeAddress(0, 0, 0, lastCol - 1);
            sheet.SetAutoFilter(cellRange);
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                wb.Write(fs);
            }
        }
    }
}
