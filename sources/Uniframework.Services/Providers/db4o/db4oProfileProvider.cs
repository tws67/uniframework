using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Web.Profile;
using Db4objects.Db4o;
using Uniframework.Db4o;
using System.Web.Hosting;

namespace Uniframework.Services.db4oProviders
{
    public class Profile : DataContainer, IComparable<Profile>
    {
        public readonly string ApplicationName;
        public readonly string Username;
        public bool IsAnonymous;
        public DateTime LastActivityDate;
        public DateTime LastUpdatedDate;
        public List<SettingsPropertyValue> SettingsPropertyValueList;

        public Profile(string username, string applicationName, bool isAnonymous, DateTime lastActivityDate,
                       DateTime lastUpdatedDate)
        {
            Username = username;
            ApplicationName = applicationName;
            IsAnonymous = isAnonymous;
            LastActivityDate = lastActivityDate;
            LastUpdatedDate = lastUpdatedDate;
            SettingsPropertyValueList = new List<SettingsPropertyValue>();
        }

        #region IComparable<Profile> Members

        public int CompareTo(Profile other)
        {
            return Username.CompareTo(other.Username);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Profile:{0}:{1}",
                                 Username,
                                 ApplicationName);
        }
    }

    public class db4oProfileProvider : ProfileProvider
    {
        public static readonly string PROVIDER_NAME = "db4oProfileProvider";

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
                config.Add("description", "db4o ASP.NET Profile provider");
            }

            base.Initialize(name, config);

            applicationName = Utils.DefaultIfBlank(config["applicationName"], HostingEnvironment.ApplicationVirtualPath);

            connectionString = ConnectionStringStore.GetConnectionString(config["connectionStringName"]);
            if (connectionString == null)
                throw new ProviderException("Connection string cannot be blank.");

            Db4oFactory.Configure().ObjectClass(new Profile("", "", false, DateTime.MinValue, DateTime.MinValue)).
                CascadeOnUpdate(true);
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context,
                                                                          SettingsPropertyCollection spc)
        {
            using (Db4oDatabase dbc = new Db4oDatabase(connectionString))
            {
                string username = (string)context["UserName"];
                bool isAuthenticated = (bool)context["IsAuthenticated"];

                var profiles = dbc.Query((Profile p) => p.Username == username && p.ApplicationName == applicationName);

                Profile profile = null;

                if (profiles.Count > 0)
                    profile = profiles[0];

                var spvc = new SettingsPropertyValueCollection();

                foreach (SettingsProperty sp in spc)
                {
                    if (!isAuthenticated && sp.Attributes.ContainsKey("AllowAnonymous") && !(bool)sp.Attributes["AllowAnonymous"])
                        continue;

                    SettingsPropertyValue found = null;

                    if (profile != null)
                        found = profile.SettingsPropertyValueList.Find(spv => sp.Name == spv.Name);

                    var created = new SettingsPropertyValue(sp);

                    if (found != null)
                    {
                        created.Property.PropertyType = found.Property.PropertyType;
                        created.PropertyValue = found.PropertyValue;
                        created.IsDirty = false;
                        created.Deserialized = true;
                    }

                    spvc.Add(created);
                }

                UpdateActivityDates(dbc, username, true);

                return spvc;
            }
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection spvc)
        {
            using (Db4oDatabase dbc = new Db4oDatabase(connectionString))
            {
                string username = (string)context["UserName"];
                bool isAuthenticated = (bool)context["IsAuthenticated"];

                var profiles = dbc.Query((Profile p) => p.Username == username && p.ApplicationName == applicationName);

                Profile profile;

                if (profiles.Count > 0)
                    profile = profiles[0];
                else
                {
                    profile = new Profile(username, applicationName, !isAuthenticated, DateTime.Now, DateTime.Now);
                    dbc.Store(profile);
                }

                foreach (SettingsPropertyValue spv in spvc)
                {
                    if (!isAuthenticated && spv.Property.Attributes.ContainsKey("AllowAnonymous") &&
                        !(bool)spv.Property.Attributes["AllowAnonymous"])
                        continue;

                    if (!spv.IsDirty && spv.UsingDefaultValue)
                        continue;

                    var found = profile.SettingsPropertyValueList.Find(existingSpv => existingSpv.Name == spv.Name);

                    if (found != null)
                        profile.SettingsPropertyValueList.Remove(found);

                    profile.SettingsPropertyValueList.Add(spv);
                }

                dbc.Store(profile);

                UpdateActivityDates(dbc, username, false);
            }
        }

        private void UpdateActivityDates(IObjectContainer db, string username, bool activityOnly)
        {
            var activityDate = DateTime.Now;

            var profiles = db.Query((Profile p) => p.Username == username && p.ApplicationName == applicationName);

            if (profiles.Count == 0)
                return;

            var profile = profiles[0];

            profile.LastActivityDate = activityDate;

            if (!activityOnly)
                profile.LastUpdatedDate = activityDate;

            db.Store(profile);
        }

        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            var usernames = new List<string>();

            foreach (ProfileInfo pi in profiles)
                usernames.Add(pi.UserName);

            return DeleteProfiles(usernames.ToArray());
        }

        public override int DeleteProfiles(string[] usernames)
        {
            int deleteCount = 0;

            using (Db4oDatabase dbc = new Db4oDatabase(connectionString))
            {
                foreach (string user in usernames)
                {
                    if (DeleteProfile(dbc, user))
                        deleteCount++;
                }
            }

            return deleteCount;
        }

        public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            int totalRecords;

            var profiles = GetProfiles(authenticationOption, null, userInactiveSinceDate, 0, int.MaxValue, out totalRecords);

            List<string> usernames = new List<string>();

            foreach (Profile found in profiles)
                usernames.Add(found.Username);

            return DeleteProfiles(usernames.ToArray());
        }

        private bool DeleteProfile(IObjectContainer db, string username)
        {
            if (username == null)
                throw new ArgumentNullException("username", "User name cannot be null.");
            if (username.Length > 255)
                throw new ArgumentException("User name exceeds 255 characters.");
            if (username.Contains(","))
                throw new ArgumentException("User name cannot contain a comma (,).");

            var profiles = db.Query((Profile p) => p.Username == username && p.ApplicationName == applicationName);

            if (profiles.Count == 0)
                return false;

            Profile profile = profiles[0];

            foreach (SettingsPropertyValue spv in profile.SettingsPropertyValueList)
                db.Delete(spv);

            db.Delete(profile);

            return true;
        }

        public override ProfileInfoCollection FindProfilesByUserName(
            ProfileAuthenticationOption authenticationOption,
            string usernameToMatch,
            int pageIndex,
            int pageSize,
            out int totalRecords)
        {
            CheckParameters(pageIndex, pageSize);

            return ProfilesToProfileInfoCollection(GetProfiles(authenticationOption, usernameToMatch, null, pageIndex, pageSize, out totalRecords));
        }

        public override ProfileInfoCollection FindInactiveProfilesByUserName(
            ProfileAuthenticationOption authenticationOption,
            string usernameToMatch,
            DateTime userInactiveSinceDate,
            int pageIndex,
            int pageSize,
            out int totalRecords)
        {
            CheckParameters(pageIndex, pageSize);

            return ProfilesToProfileInfoCollection(GetProfiles(authenticationOption, usernameToMatch, userInactiveSinceDate, pageIndex, pageSize, out totalRecords));
        }

        private ProfileInfoCollection ProfilesToProfileInfoCollection(IEnumerable<Profile> profiles)
        {
            var result = new ProfileInfoCollection();

            foreach (Profile profile in profiles)
            {
                result.Add(new ProfileInfo(
                               profile.Username,
                               profile.IsAnonymous,
                               profile.LastActivityDate,
                               profile.LastUpdatedDate,
                               GetProfileSize(profile.Username)));
            }

            return result;
        }

        private int GetProfileSize(string username)
        {
            using (Db4oDatabase dbc = new Db4oDatabase(connectionString))
            {
                var profiles = dbc.Query((Profile p) => p.Username == username && p.ApplicationName == applicationName);

                if (profiles.Count == 0)
                    return 0;

                var profile = profiles[0];
                int result = 0;

                foreach (SettingsPropertyValue spv in profile.SettingsPropertyValueList)
                {
                    // this will be way off for any types that don't translate well to a string
                    result += spv.PropertyValue.ToString().Length * 2;
                }

                return result;
            }
        }

        private List<Profile> GetProfiles(
            ProfileAuthenticationOption authenticationOption,
            string usernameToMatch,
            DateTime? userInactiveSinceDate,
            int pageIndex,
            int pageSize,
            out int totalRecords)
        {
            using (Db4oDatabase dbc = new Db4oDatabase(connectionString))
            {
                bool canBeAnonymous = (authenticationOption == ProfileAuthenticationOption.All) ||
                                      (authenticationOption == ProfileAuthenticationOption.Anonymous);
                bool hasToBeAnonymous = authenticationOption == ProfileAuthenticationOption.Anonymous;

                var profiles = new List<Profile>(dbc.Query(
                                                     (Profile p) => p.ApplicationName == applicationName
                                                                    &&
                                                                    (String.IsNullOrEmpty(usernameToMatch) ||
                                                                     (p.Username.Contains(usernameToMatch)))
                                                                    &&
                                                                    ((userInactiveSinceDate == null) ||
                                                                     (p.LastActivityDate <
                                                                      userInactiveSinceDate))
                                                                    &&
                                                                    (p.IsAnonymous == canBeAnonymous ||
                                                                     p.IsAnonymous == hasToBeAnonymous)));

                totalRecords = profiles.Count;

                profiles.Sort();

                int firstIndex = pageIndex * pageSize;

                if (firstIndex < profiles.Count)
                {
                    int upperBound = (firstIndex + pageSize > profiles.Count) ? profiles.Count : firstIndex + pageSize;

                    return profiles.GetRange(firstIndex, upperBound - firstIndex);
                }

                return new List<Profile>();
            }
        }

        public override ProfileInfoCollection GetAllProfiles(
            ProfileAuthenticationOption authenticationOption,
            int pageIndex,
            int pageSize,
            out int totalRecords)
        {
            CheckParameters(pageIndex, pageSize);

            return ProfilesToProfileInfoCollection(GetProfiles(authenticationOption, null, null, pageIndex, pageSize, out totalRecords));
        }

        public override ProfileInfoCollection GetAllInactiveProfiles(
            ProfileAuthenticationOption authenticationOption,
            DateTime userInactiveSinceDate,
            int pageIndex,
            int pageSize,
            out int totalRecords)
        {
            CheckParameters(pageIndex, pageSize);

            return ProfilesToProfileInfoCollection(GetProfiles(authenticationOption, null, userInactiveSinceDate, pageIndex, pageSize, out totalRecords));
        }

        public override int GetNumberOfInactiveProfiles(
            ProfileAuthenticationOption authenticationOption,
            DateTime userInactiveSinceDate)
        {
            int inactiveProfiles;

            GetProfiles(authenticationOption, null, userInactiveSinceDate, 0, 0, out inactiveProfiles);

            return inactiveProfiles;
        }

        private static void CheckParameters(int pageIndex, int pageSize)
        {
            if (pageIndex < 0)
                throw new ArgumentException("Page index must 0 or greater.");
            if (pageSize < 1)
                throw new ArgumentException("Page size must be greater than 0.");
        }
    }
}
