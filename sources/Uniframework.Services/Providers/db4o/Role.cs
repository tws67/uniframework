using System.Collections.Generic;

namespace Uniframework.Services.db4oProviders
{
    public class Role : DataContainer
    {
        public Role(string rolename, string applicationName)
        {
            this.Rolename = rolename;
            this.ApplicationName = applicationName;
            this.EnrolledUsers = new List<string>();
        }

        public override string ToString()
        {
            return string.Format("Role:{0}:{1}",
                                 this.Rolename,
                                 this.ApplicationName);
        }

        public readonly string Rolename;
        public readonly string ApplicationName;
        public List<string> EnrolledUsers;
    }

    public class EnrolledUser : DataContainer
    {
        public EnrolledUser(string username, string applicationName)
        {
            this.Username = username;
            this.ApplicationName = applicationName;
            this.Roles = new List<string>();
        }

        public override string ToString()
        {
            return string.Format("EnrolledUser:{0}:{1}",
                                 this.Username,
                                 this.ApplicationName);
        }

        public readonly string Username;
        public readonly string ApplicationName;
        public List<string> Roles;
    }
}
