using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using DevExpress.XtraBars.Docking;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI.Utility;

using Uniframework.XtraForms.SmartPartInfos;

namespace Uniframework.XtraForms.Workspaces
{
    /// <summary>
    /// Implements a Workspace that shows smartparts in DockedWindows
    /// </summary>
    public class DockManagerWorkspace : Workspace<Control, DockManagerSmartPartInfo>
    {
        private readonly Dictionary<Control, DockPanel> dockPanels = new Dictionary<Control, DockPanel>();
        private readonly DockManager dockManager;
        private ImageList imageList;

    	/// <summary>
        /// Initializes the workspace with no DockManager windows.
        /// </summary>
        public DockManagerWorkspace() { 
            imageList = new ImageList();
            imageList.ImageSize = new System.Drawing.Size(16, 16);
        }

        /// <summary>
        /// Initializes the workspace with the DockManager which all new DockPanels are added to. 
        /// </summary>
        /// <param name="dockManager">The DockManager that new DockPanels are added to</param>
        public DockManagerWorkspace(DockManager dockManager)
            : this()
        {
            this.dockManager = dockManager;
            this.dockManager.Images = imageList;
        }

        /// <summary>
        /// Read-only view of DockPanelDictionary.
        /// </summary>
        [Browsable(false)]
        public ReadOnlyDictionary<Control, DockPanel> DockPanels
        {
            get { return new ReadOnlyDictionary<Control, DockPanel>(dockPanels); }
        }

        /// <summary>
        /// Gets or sets the image service.
        /// </summary>
        /// <value>The image service.</value>
        [ServiceDependency]
        public IImageService ImageService
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a DockPanel if it does not already exist and adds the given control.
        /// </summary>
        protected DockPanel GetOrCreateDockPanel(Control control, DockManagerSmartPartInfo smartPartInfo)
        {
            DockPanel dockPanel = null;
            if (dockPanels.ContainsKey(control))
            {
                dockPanel = dockPanels[control];
            }
			else
            {	
                dockPanel = CreateDockPanel(control, smartPartInfo, dockPanel);
            }
            return dockPanel;
        }

    	private DockPanel CreateDockPanel(Control control, DockManagerSmartPartInfo spi, DockPanel dockPanel)
    	{
            if (spi.Tabbed == true)
            {
                foreach (DockPanel dockRootPanel in dockManager.RootPanels)
                {
                    if (dockRootPanel.Dock != spi.Dock) continue;

                    dockPanel = dockManager.AddPanel(spi.Dock);
                    dockPanel.DockAsTab(dockRootPanel);
                    break;
                }

                if (dockPanel == null)
                    dockPanel = dockManager.AddPanel(spi.Dock);
            }
            else
            {
                if (String.IsNullOrEmpty(spi.ParentPanelName)) {
                    dockPanel = dockManager.AddPanel(spi.Dock);
                }
                else
                {
                    foreach (DockPanel dockRootPanel in dockManager.RootPanels) {
                        if (dockRootPanel.Name != spi.ParentPanelName)
                            continue;

                        dockPanel = dockManager.AddPanel(spi.Dock);
                        dockPanel.DockAsTab(dockRootPanel);
                        break;
                    }

                    if (dockPanel == null)
                        dockPanel = dockManager.AddPanel(spi.Dock); // If the panel is not found, just create one
                }
            }

    		control.Dock = DockStyle.Fill;
    		dockPanels.Add(control, dockPanel);
    		dockPanel.Controls.Add(control);
    		return dockPanel;
    	}

    	/// <summary>
		/// Sets  <see cref="DockManagerSmartPartInfo"/> specific properties for the given DockPanel 
        /// </summary>
        protected void SetDockPanelProperties(DockPanel dockPanel, DockManagerSmartPartInfo info)
        {
            if (String.IsNullOrEmpty(info.ParentPanelName))
                dockPanel.Dock = info.Dock;

            dockPanel.FloatLocation = info.FloatLocation;
            dockPanel.FloatSize = info.FloatSize;
            dockPanel.FloatVertical = info.FloatVertical;
            dockPanel.ID = info.ID;
            dockPanel.ImageIndex = info.ImageIndex;
            dockPanel.Index = info.Index;
            dockPanel.SavedDock = info.SavedDock;
            dockPanel.SavedIndex = info.SavedIndex;
            dockPanel.SavedParent = info.SavedParent;
            dockPanel.SavedTabbed = info.SavedTabbed;
            dockPanel.Tabbed = info.Tabbed;
            dockPanel.TabsPosition = info.TabsPosition;
            dockPanel.TabsScroll = info.TabsScroll;
            dockPanel.TabText = info.TabText;
            dockPanel.Text = info.Title;
            dockPanel.Name = info.Name;

            if (!String.IsNullOrEmpty(info.ImageFile)) {
                if (!imageList.Images.ContainsKey(info.ImageFile)) {
                    Icon icon = ImageService.GetIcon(info.ImageFile, new Size(16, 16));
                    if (icon != null)
                        imageList.Images.Add(info.ImageFile, icon);
                }

                dockPanel.ImageIndex = imageList.Images.IndexOfKey(info.ImageFile);
            }
        }

        private void ControlDisposed(object sender, EventArgs e)
        {
            Control control = sender as Control;
            if (control != null && SmartParts.Contains(sender))
            {
                CloseInternal(control);
                dockPanels[control].Close();
                dockPanels.Remove(control);
            }
        }

        private void ShowDockPanel(DockPanel dockPanel, DockManagerSmartPartInfo smartPartInfo)
        {
            SetDockPanelProperties(dockPanel, smartPartInfo);
        }

        /// <summary>
        /// Shows the DockPanel for the smart part and brings it to the front.
        /// </summary>
        protected override void OnActivate(Control smartPart)
        {
        	DockPanel dockPanel = dockPanels[smartPart];
        	dockPanel.BringToFront();
        	dockPanel.Show();
        }

        /// <summary>
        /// Sets the properties on the DockPanel based on the information.
        /// </summary>
        protected override void OnApplySmartPartInfo(Control smartPart, DockManagerSmartPartInfo smartPartInfo)
        {
            DockPanel dockPanel = dockPanels[smartPart];
            SetDockPanelProperties(dockPanel, smartPartInfo);
        }

        /// <summary>
        /// Shows a DockPanel for the smart part and sets its properties.
        /// </summary>
        protected override void OnShow(Control smartPart, DockManagerSmartPartInfo smartPartInfo)
        {
            DockPanel dockPanel = GetOrCreateDockPanel(smartPart, smartPartInfo);
            smartPart.Show();
            ShowDockPanel(dockPanel, smartPartInfo);
        }

        /// <summary>
        /// Hides the DockPanel where the smart part is being shown.
        /// </summary>
        protected override void OnHide(Control smartPart)
        {
        	dockPanels[smartPart].Hide();
        }

        /// <summary>
        /// Closes the DockPanel where the smart part is being shown.
        /// </summary>
        protected override void OnClose(Control smartPart)
        {
            DockPanel dockPanel = dockPanels[smartPart];
            smartPart.Disposed -= ControlDisposed;

			dockPanel.Controls.Remove(smartPart);	// Remove the smartPart from the DockPanel to avoid disposing it
			dockManager.RemovePanel(dockPanel);		// changed from dockPanel.Close() but not unit tested
            dockPanels.Remove(smartPart);
        }
    }
}