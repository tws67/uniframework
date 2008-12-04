using System.Collections.Generic;

namespace Uniframework.Services.db4oProviders
{
    public class Role : DataContainer
    {
        public readonly string ApplicationName;
        public readonly string Rolename;
        public List<string> EnrolledUsers;

        public Role(string rolename, string applicationName)
        {
            Rolename = rolename;
            ApplicationName = applicationName;
            EnrolledUsers = new List<string>();
        }

        public override string ToString()
        {
            return string.Format("Role:{0}:{1}",
                                 Rolename,
                                 ApplicationName);
        }
    }

    public class EnrolledUser : DataContainer
    {
        public readonly string ApplicationName;
        public readonly string Username;
        public List<string> Roles;

        public EnrolledUser(string username, string applicationName)
        {
            Username = username;
            ApplicationName = applicationName;
            Roles = new List<string>();
        }

        public override string ToString()
        {
            return string.Format("EnrolledUser:{0}:{1}",
                                 Username,
                                 ApplicationName);
        }
    }
}
