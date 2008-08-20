using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Web.Security;
using Db4objects.Db4o;

namespace Uniframework.Services.db4oProviders
{
    public class db4oRoleProvider : RoleProvider
    {
        public IConnectionStringStore ConnectionStringStore = new ConfigurationManagerConnectionStringStore();

        public static readonly string PROVIDER_NAME = "db4oRoleProvider";

        private string eventSource = PROVIDER_NAME;
        private string connectionString;

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
                                            System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);

            connectionString = this.ConnectionStringStore.GetConnectionString(config["connectionStringName"]);
            if (connectionString == null)
                throw new ProviderException("Connection string cannot be blank.");

            Db4oFactory.Configure().ObjectClass(new Role("", "")).CascadeOnUpdate(true);
            Db4oFactory.Configure().ObjectClass(new EnrolledUser("", "")).CascadeOnUpdate(true);
        }

        private string applicationName;

        public override string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        public override void AddUsersToRoles(string[] usernames, string[] rolenames)
        {
            using (db4oDatabase dbc = new db4oDatabase(this.connectionString))
            {
                foreach (string rolename in rolenames)
                {
                    if (!this.RoleExists(rolename, dbc.dbService))
                        throw new ProviderException("Role name not found.");
                }

                foreach (string username in usernames)
                {
                    if (username.Contains(","))
                        throw new ArgumentException("User names cannot contain commas.");
                }

                foreach (string rolename in rolenames)
                {
                    IList<Role> results = dbc.dbService.Query<Role>(
                        delegate(Role r) { return r.Rolename == rolename && r.ApplicationName == this.applicationName; });

                    Role found = results[0];

                    foreach (string username in usernames)
                    {
                        if (!found.EnrolledUsers.Contains(username))
                            found.EnrolledUsers.Add(username);
                    }

                    dbc.dbService.Set(found);
                }

                foreach (string username in usernames)
                {
                    IList<EnrolledUser> results = dbc.dbService.Query<EnrolledUser>(
                        delegate(EnrolledUser u) { return u.Username == username && u.ApplicationName == this.applicationName; });

                    EnrolledUser found;

                    if (results.Count == 0)
                    {
                        // this is the only place in the API that EnrolledUser's
                        // are added to the database
                        found = new EnrolledUser(username, this.applicationName);
                    }
                    else
                        found = results[0];

                    foreach (string rolename in rolenames)
                    {
                        if (!found.Roles.Contains(rolename))
                            found.Roles.Add(rolename);
                    }

                    dbc.dbService.Set(found);
                }
            }
        }

        public override void CreateRole(string rolename)
        {
            if (rolename.Contains(","))
                throw new ArgumentException("Role names cannot contain commas.");

            if (this.RoleExists(rolename))
                throw new ProviderException("Role name already exists.");

            using (db4oDatabase dbc = new db4oDatabase(this.connectionString))
            {
                dbc.dbService.Set(new Role(rolename, this.applicationName));
            }
        }

        public override bool DeleteRole(string rolename, bool throwOnPopulatedRole)
        {
            if (!RoleExists(rolename))
                throw new ProviderException("Role does not exist.");

            if (throwOnPopulatedRole && GetUsersInRole(rolename).Length > 0)
                throw new ProviderException("Cannot delete a populated role.");

            using (db4oDatabase dbc = new db4oDatabase(this.connectionString))
            {
                IList<Role> results = dbc.dbService.Query<Role>(
                    delegate(Role r) { return r.Rolename == rolename && r.ApplicationName == this.applicationName; });

                Role found = results[0];

                dbc.dbService.Delete(found);

                return true;
            }
        }

        public override string[] GetAllRoles()
        {
            using (db4oDatabase dbc = new db4oDatabase(this.connectionString))
            {
                IList<Role> results = dbc.dbService.Query<Role>(
                    delegate(Role r) { return r.ApplicationName == this.applicationName; });

                List<string> roleList = new List<string>();
                foreach (Role role in results)
                    roleList.Add(role.Rolename);

                return roleList.ToArray();
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            using (db4oDatabase dbc = new db4oDatabase(this.connectionString))
            {
                IList<EnrolledUser> results = dbc.dbService.Query<EnrolledUser>(
                    delegate(EnrolledUser u) { return u.Username == username && u.ApplicationName == this.applicationName; });

                if (results.Count == 0)
                    return new string[0];

                EnrolledUser found = results[0];

                return found.Roles.ToArray();
            }
        }

        public override string[] GetUsersInRole(string rolename)
        {
            using (db4oDatabase dbc = new db4oDatabase(this.connectionString))
            {
                IList<Role> results = dbc.dbService.Query<Role>(
                    delegate(Role r) { return r.Rolename == rolename && r.ApplicationName == this.applicationName; });

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
            using (db4oDatabase dbc = new db4oDatabase(this.connectionString))
            {
                foreach (string rolename in rolenames)
                {
                    if (!this.RoleExists(rolename, dbc.dbService))
                        throw new ProviderException("Role name not found.");
                }

                foreach (string rolename in rolenames)
                {
                    IList<Role> results = dbc.dbService.Query<Role>(
                        delegate(Role r) { return r.Rolename == rolename && r.ApplicationName == this.applicationName; });

                    Role found = results[0];

                    foreach (string username in usernames)
                    {
                        if (found.EnrolledUsers.Contains(username))
                            found.EnrolledUsers.Remove(username);
                    }

                    dbc.dbService.Set(found);
                }

                foreach (string username in usernames)
                {
                    IList<EnrolledUser> results = dbc.dbService.Query<EnrolledUser>(
                        delegate(EnrolledUser u) { return u.Username == username && u.ApplicationName == this.applicationName; });

                    if (results.Count == 0)
                        continue;

                    EnrolledUser found = results[0];

                    foreach (string rolename in rolenames)
                    {
                        if (found.Roles.Contains(rolename))
                            found.Roles.Remove(rolename);
                    }

                    dbc.dbService.Set(found);
                }
            }
        }

        private bool RoleExists(string rolename, IObjectContainer db)
        {
            IList<Role> results = db.Query<Role>(
                delegate(Role r) { return r.Rolename == rolename && r.ApplicationName == this.applicationName; });

            return results.Count > 0;
        }

        public override bool RoleExists(string rolename)
        {
            using (db4oDatabase dbc = new db4oDatabase(this.connectionString))
            {
                return this.RoleExists(rolename, dbc.dbService);
            }
        }

        public override string[] FindUsersInRole(string rolename, string usernameToMatch)
        {
            List<string> results = new List<string>();

            string[] enrolledUsers = this.GetUsersInRole(rolename);

            foreach (string username in enrolledUsers)
            {
                if (username.Contains(usernameToMatch))
                    results.Add(username);
            }

            return results.ToArray();
        }
    }
}
