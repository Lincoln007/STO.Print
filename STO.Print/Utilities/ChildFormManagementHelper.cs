//-----------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2015 , STO TECH, Ltd. 
//-----------------------------------------------------------------

using DotNet.Utilities;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace STO.Print.Utilities
{
    /// <summary>
    /// ���ڹ�������
    /// </summary>
    public sealed class ChildFormManagementHelper
    {
        /// <summary> 
        /// ��ȡMDI�������Ƿ��д��ڱ���Ϊָ���ַ������Ӵ��ڣ�����Ѿ����ڰѴ��Ӵ�������ǰ̨�� 
        /// </summary> 
        /// <param name="formMain">M������</param> 
        /// <param name="caption">���ڱ���</param> 
        /// <returns></returns> 
        public static bool FormExist(Form formMain, string caption)
        {
            bool result = false;
            foreach (Form form in formMain.MdiChildren)
            {
                if (form.Text == caption)
                {
                    result = true;
                    form.Show();
                    form.Activate();
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Ψһ����ĳ�����͵Ĵ��壬�����������ʾ�����򴴽���
        /// </summary>
        /// <param name="formMain">���������</param>
        /// <param name="formType">����ʾ�Ĵ�������</param>
        /// <returns></returns>
        public static Form LoadMdiForm(Form formMain, Type formType)
        {
            bool found = false;
            Form tableForm = null;
            foreach (Form form in formMain.MdiChildren)
            {
                if (form.GetType() == formType)
                {
                    found = true;
                    tableForm = form;
                    break;
                }
            }
            if (!found)
            {
                tableForm = (Form)Activator.CreateInstance(formType);
                tableForm.MdiParent = formMain;
                tableForm.Show();
            }
            // tableForm.Dock = DockStyle.Fill;
            // tableForm.WindowState = FormWindowState.Maximized;
            tableForm.BringToFront();
            tableForm.Activate();
            return tableForm;
        }

        public static Form LoadMdiForm(Form formMain, string formName, string assemblyName = "STO.Print")
        {
            bool found = false;
            Form tableForm = null;
            foreach (Form form in formMain.MdiChildren)
            {
                if (form.Name == formName)
                {
                    found = true;
                    tableForm = form;
                    break;
                }
            }
            if (!found)
            {
                // �������˻��漼�������Ѿ����������Ͳ�����������
                Assembly assembly = CacheManagerHelper.Instance.Load(assemblyName);
                string frmName = assemblyName + "." + formName;
                Type type = assembly.GetType(frmName, true, false);
                tableForm = (Form)Activator.CreateInstance(type);
                // �ж��Ƿ񵯳����ڣ��������⴦��
                var field = type.GetField("ShowDialogOnly");
                if (field != null && (bool)field.GetValue(tableForm))
                {
                    tableForm.HelpButton = false;
                    tableForm.ShowInTaskbar = false;
                    tableForm.MinimizeBox = false;
                    tableForm.ShowDialog(formMain);
                    return tableForm;
                }
                // tableForm = (Form)Activator.CreateInstance(formType);
                tableForm.MdiParent = formMain;
                tableForm.Show();
            }
            // tableForm.Dock = DockStyle.Fill;
            // tableForm.WindowState = FormWindowState.Maximized;
            tableForm.BringToFront();
            tableForm.Activate();
            return tableForm;
        }

        /// <summary>
        /// ��һ��ҳ��
        /// </summary>
        /// <param name="formMain">������</param>
        /// <param name="url">�򿪵����ӵ�ַ</param>
        /// <param name="title">����</param>
        /// <param name="formName">�������������</param>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static Form Navigate(Form formMain, string url, string title = "��ҳ", string formName = "BrowserForm", string assemblyName = "STO.Print")
        {
            bool found = false;
            Form tableForm = null;
            foreach (Form form in formMain.MdiChildren)
            {
                if (form.Name == title)
                {
                    tableForm = form;
                    ((IBaseBrowser)tableForm).Navigate(url);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                // �������˻��漼�������Ѿ����������Ͳ�����������
                Assembly assembly = CacheManagerHelper.Instance.Load(assemblyName);
                string frmName = assemblyName + "." + formName;
                Type type = assembly.GetType(frmName, true, false);
                tableForm = (Form)Activator.CreateInstance(type);
                // �ж��Ƿ񵯳����ڣ��������⴦��
                var field = type.GetField("ShowDialogOnly");
                if (field != null && (bool)field.GetValue(tableForm))
                {
                    tableForm.HelpButton = false;
                    tableForm.ShowInTaskbar = false;
                    tableForm.MinimizeBox = false;
                    tableForm.ShowDialog(formMain);
                    return tableForm;
                }
                // tableForm = (Form)Activator.CreateInstance(formType);
                tableForm.MdiParent = formMain;
                tableForm.Name = formName;
                tableForm.Text = title;
                ((IBaseBrowser)tableForm).Navigate(url);
                tableForm.Show();
            }
            tableForm.Dock = DockStyle.Fill;
            tableForm.WindowState = FormWindowState.Maximized;
            tableForm.BringToFront();
            tableForm.Activate();
            return tableForm;
        }

    }
}