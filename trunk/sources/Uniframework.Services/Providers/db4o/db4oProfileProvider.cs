using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Web.Profile;
using Db4objects.Db4o;

namespace Uniframework.Services.db4oProviders
{
    public class Profile : DataContainer, IComparable<Profile>
    {
        public Profile(string username, string applicationName, bool isAnonymous, DateTime lastActivityDate,
                       DateTime lastUpdatedDate)
        {
            this.Username = username;
            this.ApplicationName = applicationName;
            this.IsAnonymous = isAnonymous;
            this.LastActivityDate = lastActivityDate;
            this.LastUpdatedDate = lastUpdatedDate;
            this.SettingsPropertyValueList = new List<SettingsPropertyValue>();
        }

        public readonly string Username;
        public readonly string ApplicationName;
        public bool IsAnonymous;
        public DateTime LastActivityDate;
        public DateTime LastUpdatedDate;
        public List<SettingsPropertyValue> SettingsPropertyValueList;

        public override string ToString()
        {
            return string.Format("Profile:{0}:{1}",
                                 this.Username,
                                 this.ApplicationName);
        }

        public int CompareTo(Profile other)
        {
            return this.Username.CompareTo(other.Username);
        }
    }

    public class db4oProfileProvider : ProfileProvider
    {
        public IConnectionStringStore ConnectionStringStore = new ConfigurationManagerConnectionStringStore();

        public static readonly string PROVIDER_NAME = "db4oProfileProvider";

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
                config.Add("description", "db4o ASP.NET Profile provider");
            }

            base.Initialize(name, config);

            applicationName =
                Utils.DefaultIfBlank(config["applicationName"],
                                     System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);

            connectionString = this.ConnectionStringStore.GetConnectionString(config["connectionStringName"]);
            if (connectionString == null)
                throw new ProviderException("Connection string cannot be blank.");

            Db4oFactory.Configure().ObjectClass(new Profile("", "", false, DateTime.MinValue, DateTime.MinValue)).
                CascadeOnUpdate(true);
        }

        private string applicationName;

        public override string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context,
                                                                          SettingsPropertyCollection spc)
        {
            using (db4oDatabase dbc = new db4oDatabase(this.connectionString))
            {
                string username = (string)context["UserName"];
                bool isAuthenticated = (bool)context["IsAuthenticated"];

                IList<Profile> profiles = dbc.dbService.Query<Profile>(
                    delegate(Profile p)
                    {
                        return p.Username == username
                               && p.ApplicationName == this.applicationName
                               && p.IsAnonymous == !isAuthenticated;
                    });

                if (profiles.Count == 0)
                    return new SettingsPropertyValueCollection();

                Profile profile = profiles[0];

                SettingsPropertyValueCollection spvc = new SettingsPropertyValueCollection();

                foreach (SettingsProperty sp in spc)
                {
                    SettingsPropertyValue found = profile.SettingsPropertyValueList.Find(delegate(SettingsPropertyValue spv)
                                                                                           { return sp.Name == spv.Name; });

                    if (found != null)
                        spvc.Add(found);
                }

                UpdateActivityDates(dbc.dbService, username, isAuthenticated, true);

                return spvc;
            }
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection spvc)
        {
            using (db4oDatabase dbc = new db4oDatabase(this.connectionString))
            {
                string username = (string)context["UserName"];
                bool isAuthenticated = (bool)context["IsAuthenticated"];

                IList<Profile> profiles = dbc.dbService.Query<Profile>(
                    delegate(Profile p)
                    {
                        return p.Username == username
                               && p.ApplicationName == this.applicationName
                               && p.IsAnonymous == !isAuthenticated;
                    });

                Profile profile = null;

                if (profiles.Count > 0)
                    profile = profiles[0];
                else
                {
                    profile = new Profile(username, this.applicationName, !isAuthenticated, DateTime.Now, DateTime.Now);
                    dbc.dbService.Set(profile);
                }

                foreach (SettingsPropertyValue spv in spvc)
                {
                    // set type if null, actual type really doesn't matter for this implementation
                    if (spv.Property.PropertyType == null)
                        spv.Property.PropertyType = typeof(object);

                    SettingsPropertyValue found = profile.SettingsPropertyValueList.Find(delegate(SettingsPropertyValue existingSpv)
                                                                                           { return existingSpv.Name == spv.Name; });

                    if (found != null)
                        profile.SettingsPropertyValueList.Remove(found);

                    profile.SettingsPropertyValueList.Add(spv);
                }

                dbc.dbService.Set(profile);

                UpdateActivityDates(dbc.dbService, username, isAuthenticated, false);
            }
        }

        private void UpdateActivityDates(IObjectContainer db, string username, bool isAuthenticated, bool activityOnly)
        {
            DateTime activityDate = DateTime.Now;

            IList<Profile> profiles = db.Query<Profile>(
                delegate(Profile p)
                {
                    return p.Username == username
                           && p.ApplicationName == this.applicationName
                           && p.IsAnonymous == !isAuthenticated;
                });

            if (profiles.Count == 0)
                return;

            Profile profile = profiles[0];

            profile.LastActivityDate = activityDate;

            if (!activityOnly)
                profile.LastUpdatedDate = activityDate;

            db.Set(profile);
        }

        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            List<string> usernames = new List<string>();

            foreach (ProfileInfo pi in profiles)
                usernames.Add(pi.UserName);

            return this.DeleteProfiles(usernames.ToArray());
        }

        public override int DeleteProfiles(string[] usernames)
        {
            int deleteCount = 0;

            using (db4oDatabase dbc = new db4oDatabase(this.connectionString))
            {
                foreach (string user in usernames)
                {
                    if (DeleteProfile(dbc.dbService, user))
                        deleteCount++;
                }
            }

            return deleteCount;
        }

        public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption,
                                                   DateTime userInactiveSinceDate)
        {
            int totalRecords;

            List<Profile> profiles =
                GetProfiles(authenticationOption, null, userInactiveSinceDate, 0, int.MaxValue, out totalRecords);

            List<string> usernames = new List<string>();

            foreach (Profile found in profiles)
                usernames.Add(found.Username);

            return this.DeleteProfiles(usernames.ToArray());
        }

        private bool DeleteProfile(IObjectContainer db, string username)
        {
            if (username == null)
                throw new ArgumentNullException("username", "User name cannot be null.");
            if (username.Length > 255)
                throw new ArgumentException("User name exceeds 255 characters.");
            if (username.Contains(","))
                throw new ArgumentException("User name cannot contain a comma (,).");

            IList<Profile> profiles = db.Query<Profile>(
                delegate(Profile p)
                {
                    return p.Username == username
                           && p.ApplicationName == this.applicationName;
                });

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

            return
                this.ProfilesToProfileInfoCollection(
                    this.GetProfiles(authenticationOption, usernameToMatch, null, pageIndex, pageSize, out totalRecords));
        }

        public override ProfileInfoCollection FindInactiveProfilesByUserName(
            ProfileAuthenticationOption authenticationOption,
            string usernameToMatch,
            DateTime userInactiveSinceDate,
            int pageIndex,
            int pageSize,
            out int totalRecords)
        {
            this.CheckParameters(pageIndex, pageSize);

            return
                this.ProfilesToProfileInfoCollection(
                    this.GetProfiles(authenticationOption, usernameToMatch, userInactiveSinceDate, pageIndex, pageSize,
                                     out totalRecords));
        }

        private ProfileInfoCollection ProfilesToProfileInfoCollection(IEnumerable<Profile> profiles)
        {
            ProfileInfoCollection result = new ProfileInfoCollection();

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
            using (db4oDatabase dbc = new db4oDatabase(this.connectionString))
            {
                IList<Profile> profiles = dbc.dbService.Query<Profile>(
                    delegate(Profile p)
                    {
                        return p.Username == username
                               && p.ApplicationName == this.applicationName;
                    });

                if (profiles.Count == 0)
                    return 0;

                Profile profile = profiles[0];
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
            using (db4oDatabase dbc = new db4oDatabase(this.connectionString))
            {
                bool canBeAnonymous = (authenticationOption == ProfileAuthenticationOption.All) ||
                                      (authenticationOption == ProfileAuthenticationOption.Anonymous);
                bool hasToBeAnonymous = authenticationOption == ProfileAuthenticationOption.Anonymous;

                List<Profile> profiles = new List<Profile>(dbc.dbService.Query<Profile>(
                                                               delegate(Profile p)
                                                               {
                                                                   return p.ApplicationName == this.applicationName
                                                                          &&
                                                                          (String.IsNullOrEmpty(usernameToMatch) ||
                                                                           (p.Username.Contains(usernameToMatch)))
                                                                          &&
                                                                          ((userInactiveSinceDate == null) ||
                                                                           (p.LastActivityDate <
                                                                            userInactiveSinceDate))
                                                                          &&
                                                                          (p.IsAnonymous == canBeAnonymous ||
                                                                           p.IsAnonymous == hasToBeAnonymous);
                                                               }));

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

            return
                this.ProfilesToProfileInfoCollection(
                    this.GetProfiles(authenticationOption, null, null, pageIndex, pageSize, out totalRecords));
        }

        public override ProfileInfoCollection GetAllInactiveProfiles(
            ProfileAuthenticationOption authenticationOption,
            DateTime userInactiveSinceDate,
            int pageIndex,
            int pageSize,
            out int totalRecords)
        {
            CheckParameters(pageIndex, pageSize);

            return
                this.ProfilesToProfileInfoCollection(
                    this.GetProfiles(authenticationOption, null, userInactiveSinceDate, pageIndex, pageSize,
                                     out totalRecords));
        }

        public override int GetNumberOfInactiveProfiles(
            ProfileAuthenticationOption authenticationOption,
            DateTime userInactiveSinceDate)
        {
            int inactiveProfiles = 0;

            this.GetProfiles(authenticationOption, null, userInactiveSinceDate, 0, 0, out inactiveProfiles);

            return inactiveProfiles;
        }

        private void CheckParameters(int pageIndex, int pageSize)
        {
            if (pageIndex < 0)
                throw new ArgumentException("Page index must 0 or greater.");
            if (pageSize < 1)
                throw new ArgumentException("Page size must be greater than 0.");
        }
    }
}
