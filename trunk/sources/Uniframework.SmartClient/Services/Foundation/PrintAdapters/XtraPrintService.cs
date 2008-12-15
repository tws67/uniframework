using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;

using DevExpress.XtraPrinting;
using System.Windows.Forms;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// DevExpress相关控件打印服务
    /// </summary>
    public class XtraPrintService
    {
        private PrintingSystem printSystem;
        private string reportTitle = String.Empty;
        private string condition = String.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="XtraPrintService"/> class.
        /// </summary>
        /// <param name="printable">The printable.</param>
        public XtraPrintService(IPrintable printable)
        {
            printSystem = new PrintingSystem();
            printSystem.ShowMarginsWarning = false;

            PrintableComponentLink pcl = new PrintableComponentLink(printSystem);
            pcl.CreateMarginalHeaderArea += new CreateAreaEventHandler(pcl_CreateMarginalHeaderArea);
            pcl.CreateMarginalFooterArea += new CreateAreaEventHandler(pcl_CreateMarginalFooterArea);
            pcl.Component = printable;
            printSystem.Links.Add(pcl);

            PrinterSettingsUsing pst = new PrinterSettingsUsing();
            pst.UseMargins = false;
            pst.UsePaperKind = false;
            printSystem.PageSettings.PaperKind = PaperKind.A4;
            printSystem.PageSettings.PaperName = "A4";
            printSystem.PageSettings.LeftMargin = 5;
            printSystem.PageSettings.RightMargin = 5;
            printSystem.PageSettings.AssignDefaultPrinterSettings(pst);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XtraPrintService"/> class.
        /// </summary>
        /// <param name="printable">The printable.</param>
        /// <param name="reportTitle">The report title.</param>
        public XtraPrintService(IPrintable printable, string reportTitle)
            : this(printable)
        {
            this.reportTitle = reportTitle;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XtraPrintService"/> class.
        /// </summary>
        /// <param name="printable">The printable.</param>
        /// <param name="reportTitle">The report title.</param>
        /// <param name="condition">The condition.</param>
        public XtraPrintService(IPrintable printable, string reportTitle, string condition)
            : this(printable, reportTitle)
        {
            this.condition = condition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XtraPrintService"/> class.
        /// </summary>
        /// <param name="printable">The printable.</param>
        /// <param name="paperKind">Kind of the paper.</param>
        public XtraPrintService(IPrintable printable, PaperKind paperKind)
            : this(printable)
        {
            printSystem.PageSettings.PaperKind = paperKind;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XtraPrintService"/> class.
        /// </summary>
        /// <param name="printable">The printable.</param>
        /// <param name="reportTitle">The report title.</param>
        /// <param name="paperKind">Kind of the paper.</param>
        public XtraPrintService(IPrintable printable, string reportTitle, PaperKind paperKind)
            : this(printable, reportTitle)
        {
            printSystem.PageSettings.PaperKind = paperKind;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XtraPrintService"/> class.
        /// </summary>
        /// <param name="printable">The printable.</param>
        /// <param name="reportTitle">The report title.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="paperKind">Kind of the paper.</param>
        public XtraPrintService(IPrintable printable, string reportTitle, string condition, PaperKind paperKind)
            : this(printable, reportTitle, condition)
        {
            printSystem.PageSettings.PaperKind = paperKind;
        }

        /// <summary>
        /// 快速打印
        /// </summary>
        public void QuickPrint()
        {
            PreparePrint();
            printSystem.Print();
        }

        /// <summary>
        /// 提供打印对话框的打印
        /// </summary>
        public void Print()
        {
            PreparePrint();
            printSystem.PrintDlg();
        }

        /// <summary>
        /// 打印预览
        /// </summary>
        public void Preview()
        {
            PreparePrint();
            if (printSystem.Links.Count > 0)
                printSystem.Links[0].ShowPreviewDialog();
        }

        /// <summary>
        /// 打印预览
        /// </summary>
        /// <param name="owner">The owner.</param>
        public void Preview(IWin32Window owner)
        {
            PreparePrint();
            if (printSystem.Links.Count > 0)
                printSystem.Links[0].ShowPreviewDialog(owner);
        }

        /// <summary>
        /// 页面设置
        /// </summary>
        public void PageSetup()
        {
            printSystem.PageSetup();
        }

        #region Assistant functions

        /// <summary>
        /// 打印准备
        /// </summary>
        private void PreparePrint()
        {
            foreach (Link link in printSystem.Links) {
                link.CreateDocument();
            }
        }

        /// <summary>
        /// 添加页眉
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DevExpress.XtraPrinting.CreateAreaEventArgs"/> instance containing the event data.</param>
        private void pcl_CreateMarginalHeaderArea(object sender, CreateAreaEventArgs e)
        {
            if (!String.IsNullOrEmpty(reportTitle) && reportTitle.Length > 0) {
                e.Graph.Font = new Font("宋体", 15, FontStyle.Bold);
                e.Graph.BackColor = Color.Transparent;
                RectangleF r = new RectangleF(0, 20, 0, e.Graph.Font.Height + 20);
                PageInfoBrick brick = e.Graph.DrawPageInfo(PageInfo.NumberOfTotal, reportTitle, Color.Black, r, BorderSide.None);
                brick.Alignment = BrickAlignment.Center;
                brick.AutoWidth = true;
            }

            if (!String.IsNullOrEmpty(condition)) {
                e.Graph.Font = new Font("宋体", 10);
                e.Graph.BackColor = Color.Transparent;
                RectangleF r = new RectangleF(0, 50, 0, e.Graph.Font.Height + 20);
                PageInfoBrick brick = e.Graph.DrawPageInfo(PageInfo.NumberOfTotal, condition, Color.Black, r, BorderSide.None);
                brick.Alignment = BrickAlignment.Center;
                brick.AutoWidth = true;
            }
        }

        /// <summary>
        /// 添加页脚
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DevExpress.XtraPrinting.CreateAreaEventArgs"/> instance containing the event data.</param>
        private void pcl_CreateMarginalFooterArea(object sender, CreateAreaEventArgs e)
        {
            string format = "第{0}页 共{1}页";
            e.Graph.Font = new Font("宋体", 10);
            e.Graph.BackColor = Color.Transparent;

            RectangleF r = new RectangleF(0, 5, 0, e.Graph.Font.Height + 20);

            PageInfoBrick brick = e.Graph.DrawPageInfo(PageInfo.NumberOfTotal, format, Color.Black, r, BorderSide.None);
            brick.Alignment = BrickAlignment.Far;
            brick.AutoWidth = true;

            brick = e.Graph.DrawPageInfo(PageInfo.DateTime, "打印时间:" + DateTime.Today.ToLongDateString(), Color.Black, r, BorderSide.None);
            brick.Alignment = BrickAlignment.Near;
            brick.AutoWidth = true;
        }

        #endregion
    }
}
