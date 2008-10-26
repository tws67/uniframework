using Microsoft.Practices.CompositeUI.SmartParts;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Practices.CompositeUI.Utility;

namespace Microsoft.Practices.CompositeUI.Common
{
    public class FormSmartPartInfo : IconSmartPartInfo
    {
    	public Point Location { get; set; }
    	public int Height { get; set; }
    	public int Width { get; set; }
    	public bool ControlBox { get; set; }
    	public bool MinimizeBox { get; set; }
    	public bool MaximizeBox { get; set; }
    	public bool ShowIcon { get; set; }
    	public bool ShowInTaskBar { get; set; }
    	public bool ShowModal { get; set; }
    	public IButtonControl AcceptButton { get; set; }
    	public IButtonControl CancelButton { get; set; }
		public FormStartPosition StartPosition { get; set; }
    	public FormBorderStyle FormBorderStyle { get; set; }

    	public FormSmartPartInfo() 
            : base(null, null)
    	{
    		ControlBox = true;
    		MinimizeBox = true;
    		MaximizeBox = true;
    		ShowIcon = true;
    		ShowInTaskBar = true;
    	}

    	public FormSmartPartInfo(string title, string description) 
            : base(title, description)
    	{
    		ControlBox = true;
    		MinimizeBox = true;
    		MaximizeBox = true;
    		ShowIcon = true;
    		ShowInTaskBar = true;
    	}

    	/// <summary>
        /// Creates a new instance of FormSmartPartInfo with blank title/description and the settings for a modal dialog
        /// </summary>
        public static FormSmartPartInfo CreateModalDialog()
        {
            return CreateModalDialog(string.Empty, string.Empty);
        }

    	/// <summary>
    	/// Creates a new instance of FormSmartPartInfo with the settings for a modal dialog
    	/// </summary>
    	public static FormSmartPartInfo CreateModalDialog(string title, string description)
    	{
    		return new FormSmartPartInfo(title, description)
    		       	{
    		       		MinimizeBox = false,
    		       		MaximizeBox = false,
    		       		ShowIcon = false,
    		       		ShowInTaskBar = false,
    		       		ShowModal = true
    		       	};
    	}

        /// <summary>
        /// Creates a new instance of the FormSmartPartInfo and copies over the information 
        /// in the source smart part.
        /// </summary>
        public static FormSmartPartInfo ConvertTo(ISmartPartInfo source)
        {
            Guard.ArgumentNotNull(source, "source");

            var formInfo = new FormSmartPartInfo(source.Title, source.Description);
            var iconInfo = source as IconSmartPartInfo;
            if (iconInfo != null) formInfo.Icon = iconInfo.Icon;

            return formInfo;
        }
    }
}