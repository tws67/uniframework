using DevExpress.XtraBars.Ribbon;
using Microsoft.Practices.CompositeUI.UIElements;
using Microsoft.Practices.CompositeUI.Utility;

namespace Uniframework.XtraForms.UIElements
{
	/// <summary>
	/// An adapter that wraps a <see cref="RibbonPageCollection"/> for use as an <see cref="IUIElementAdapter"/>.
	/// </summary>
	public class RibbonPageCollectionUIAdapter : UIElementAdapter<RibbonPage>
	{
		private readonly RibbonPageCollection ribbonPageCollection;

		/// <summary>
		/// Initializes a new instance of the <see cref="RibbonPageCollectionUIAdapter"/> class.
		/// </summary>
		/// <param name="ribbonPageCollection"></param>
		public RibbonPageCollectionUIAdapter(RibbonPageCollection ribbonPageCollection)
		{
			Guard.ArgumentNotNull(ribbonPageCollection, "RibbonPageCollection");
			this.ribbonPageCollection = ribbonPageCollection;
		}

		/// <summary>
		/// See <see cref="UIElementAdapter{TUIElement}.Add(TUIElement)"/> for more information.
		/// </summary>
		protected override RibbonPage Add(RibbonPage uiElement)
		{
			Guard.ArgumentNotNull(uiElement, "RibbonPage");

			ribbonPageCollection.Insert(GetInsertingIndex(uiElement), uiElement);
			return uiElement;
		}

		/// <summary>
		/// See <see cref="UIElementAdapter{TUIElement}.Remove(TUIElement)"/> for more information.
		/// </summary>
		protected override void Remove(RibbonPage uiElement)
		{
			Guard.ArgumentNotNull(uiElement, "RibbonPage");

			if (uiElement.Ribbon != null)
				uiElement.Ribbon.Pages.Remove(uiElement);		
				// not sure why this doesn't use ribbonPageCollection.Remove()
		}

		/// <summary>
		/// When overridden in a derived class, returns the correct index for the item being added. By default,
		/// it will return the length of the ribbonPageCollection.
		/// </summary>
		/// <param name="uiElement"></param>
		/// <returns></returns>
		protected virtual int GetInsertingIndex(object uiElement)
		{
			return ribbonPageCollection.Count;
		}
	}
}