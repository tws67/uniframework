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
 */

#region Namespace imports

using System.Collections.Generic;
using System.Net;
using System.Xml;
using Uniframework.SharePoint.Linq.EntityGenerator;

#endregion

namespace Uniframework.SharePoint.Linq.Spml
{
    static class Helpers
    {
        public static List<List> GetLists(Connection connection)
        {
            Lists lists = GetListsProxy(connection);
            XmlNode listsXml = lists.GetListCollection();

            List<List> result = new List<List>();

            foreach (XmlNode l in listsXml.ChildNodes)
                if (l.Attributes["Hidden"].Value.ToUpperInvariant() != "TRUE")
                    result.Add(List.FromCaml(l));

            return result;
        }

        public static List GetList(Connection connection, string name)
        {
            Lists lists = GetListsProxy(connection);
            XmlNode listXml = lists.GetList(name);

            return List.FromCaml(listXml);
        }

        private static Lists GetListsProxy(Connection connection)
        {
            Lists lists = new Lists();
            lists.Url = connection.Url.ToString().TrimEnd('/') + "/_vti_bin/lists.asmx"; ;

            if (!connection.CustomAuthentication)
                lists.Credentials = CredentialCache.DefaultNetworkCredentials;
            else
                lists.Credentials = new NetworkCredential(connection.User, connection.Password, connection.Domain);

            return lists;
        }
    }
}
