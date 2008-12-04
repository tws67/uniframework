using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Web.Hosting;
using System.Web.Security;
using Db4objects.Db4o;

using Uniframework.Db4o;

namespace Uniframework.Services.db4oProviders
{
    public class db4oRoleProvider : RoleProvider
    {
        public static readonly string PROVIDER_NAME = "db4oRoleProvider";

        private string applicationName;
        private string connectionString;
        public IConnectionStringStore ConnectionStringStore = new ConfigurationManagerConnectionStringStore();

        public override string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (String.IsNullOrEmpty(name))
                name = PROVIDER_NAME;

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "db4o ASP.NET Role provider");
            }

            base.Initialize(name, config);

            applicationName =
                Utils.DefaultIfBlank(config["applicationName"],
                                     HostingEnvironment.ApplicationVirtualPath);

            connectionString = ConnectionStringStore.GetConnectionString(config["connectionStringName"]);
            if (connectionString == null)
                throw new ProviderException("Connection string cannot be blank.");

            Db4oFactory.Configure().ObjectClass(new Role("", "")).CascadeOnUpdate(true);
            Db4oFactory.Configure().ObjectClass(new EnrolledUser("", "")).CascadeOnUpdate(true);
        }

        public override void AddUsersToRoles(string[] usernames, string[] rolenames)
        {
            using (Db4oDatabase dbc = new Db4oDatabase(connectionString))
            {
                foreach (string rolename in rolenames)
                {
                    if (!RoleExists(rolename, dbc))
                        throw new ProviderException("Role name not found.");
                }

                foreach (string username in usernames)
                {
                    if (username.Contains(","))
                        throw new ArgumentException("User names cannot contain commas.");
                }

                foreach (string rolename in rolenames)
                {
                    IList<Role> results = dbc.Query((Role r) => r.Rolename == rolename && r.ApplicationName == applicationName);

                    Role found = results[0];

                    foreach (string username in usernames)
                    {
                        if (!found.EnrolledUsers.Contains(username))
                            found.EnrolledUsers.Add(username);
                    }

                    dbc.Store(found);
                }

                foreach (string username in usernames)
                {
                    IList<EnrolledUser> results = dbc.Query((EnrolledUser u) => u.Username == username && u.ApplicationName == applicationName);

                    EnrolledUser found;

                    if (results.Count == 0)
                    {
                        // this is the only place in the API that EnrolledUser's
                        // are added to the database
                        found = new EnrolledUser(username, applicationName);
                    }
                    else
                        found = results[0];

                    foreach (string rolename in rolenames)
                    {
                        if (!found.Roles.Contains(rolename))
                            found.Roles.Add(rolename);
                    }

                    dbc.Store(found);
                }
            }
        }

        public override void CreateRole(string rolename)
        {
            if (rolename.Contains(","))
                throw new ArgumentException("Role names cannot contain commas.");

            if (RoleExists(rolename))
                throw new ProviderException("Role name already exists.");

            using (Db4oDatabase dbc = new Db4oDatabase(connectionString))
            {
                dbc.Store(new Role(rolename, applicationName));
            }
        }

        public override bool DeleteRole(string rolename, bool throwOnPopulatedRole)
        {
            if (!RoleExists(rolename))
                throw new ProviderException("Role does not exist.");

            if (throwOnPopulatedRole && GetUsersInRole(rolename).Length > 0)
                throw new ProviderException("Cannot delete a populated role.");

            using (Db4oDatabase dbc = new Db4oDatabase(connectionString))
            {
                IList<Role> results = dbc.Query((Role r) => r.Rolename == rolename && r.ApplicationName == applicationName);

                Role found = results[0];

                dbc.Delete(found);

                return true;
            }
        }

        public override string[] GetAllRoles()
        {
            using (Db4oDatabase dbc = new Db4oDatabase(connectionString))
            {
                IList<Role> results = dbc.Query((Role r) => r.ApplicationName == applicationName);

                List<string> roleList = new List<string>();
                foreach (Role role in results)
                    roleList.Add(role.Rolename);

                return roleList.ToArray();
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            using (Db4oDatabase dbc = new Db4oDatabase(connectionString))
            {
                IList<EnrolledUser> results = dbc.Query((EnrolledUser u) => u.Username == username && u.ApplicationName == applicationName);

                if (results.Count == 0)
                    return new string[0];

                EnrolledUser found = results[0];

                return found.Roles.ToArray();
            }
        }

        public override string[] GetUsersInRole(string rolename)
        {
            using (Db4oDatabase dbc = new Db4oDatabase(connectionString))
            {
                IList<Role> results = dbc.Query((Role r) => r.Rolename == rolename && r.ApplicationName == applicationName);

                if (results.Count == 0)
                    return new string[0];

                Role found = results[0];

                return found.EnrolledUsers.ToArray();
            }
        }

        public override bool IsUserInRole(string username, string rolename)
        {
            List<string> results = new List<string>(GetRolesForUser(username));
            return results.Contains(rolename);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] rolenames)
        {
            using (Db4oDatabase dbc = new Db4oDatabase(connectionString))
            {
                foreach (string rolename in rolenames)
                {
                    if (!RoleExists(rolename, dbc))
                        throw new ProviderException("Role name not found.");
                }

                foreach (string rolename in rolenames)
                {
                    IList<Role> results = dbc.Query((Role r) => r.Rolename == rolename && r.ApplicationName == applicationName);

                    Role found = results[0];

                    foreach (string username in usernames)
                    {
                        if (found.EnrolledUsers.Contains(username))
                            found.EnrolledUsers.Remove(username);
                    }

                    dbc.Store(found);
                }

                foreach (string username in usernames)
                {
                    IList<EnrolledUser> results = dbc.Query((EnrolledUser u) => u.Username == username && u.ApplicationName == applicationName);

                    if (results.Count == 0)
                        continue;

                    EnrolledUser found = results[0];

                    foreach (string rolename in rolenames)
                    {
                        if (found.Roles.Contains(rolename))
                            found.Roles.Remove(rolename);
                    }

                    dbc.Store(found);
                }
            }
        }

        private bool RoleExists(string rolename, IObjectContainer db)
        {
            IList<Role> results = db.Query((Role r) => r.Rolename == rolename && r.ApplicationName == applicationName);

            return results.Count > 0;
        }

        public override bool RoleExists(string rolename)
        {
            using (Db4oDatabase dbc = new Db4oDatabase(connectionString))
            {
                return RoleExists(rolename, dbc);
            }
        }

        public override string[] FindUsersInRole(string rolename, string usernameToMatch)
        {
            List<string> results = new List<string>();

            string[] enrolledUsers = GetUsersInRole(rolename);

            foreach (string username in enrolledUsers)
            {
                if (username.Contains(usernameToMatch))
                    results.Add(username);
            }

            return results.ToArray();
        }
    }
}
