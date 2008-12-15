/*
 * LINQ to SharePoint
 * http://www.codeplex.com/LINQtoSharePoint
 * 
 * Copyright Bart De Smet (C) 2007
 * info@bartdesmet.net - http://blogs.bartdesmet.net/bart
 * 
 * This project is subject to licensing restrictions. Visit http://www.codeplex.com/LINQtoSharePoint/Project/License.aspx for more information.
 */

/*
 * Version history:
 * 
 * 0.2.2 - Introduction of entity wizard
 * 0.2.3 - Rename to WizardContext
 */

#region Namespace imports

using Uniframework.SharePoint.Linq.EntityGenerator;

#endregion

namespace Uniframework.SharePoint.Linq.Spml
{
    public class WizardContext
    {
        public WizardContext(Context result)
        {
            FullContext = new Context();
            ResultContext = result;
        }

        public Context FullContext { get; set; }
        public Context ResultContext { get; set; }
    }
}
