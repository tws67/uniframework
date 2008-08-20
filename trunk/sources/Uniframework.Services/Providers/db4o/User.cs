using System;
using System.Configuration.Provider;
using System.Web.Security;

namespace Uniframework.Services.db4oProviders
{
    public class User : DataContainer
    {
        public User(Guid PKID,
                    string username,
                    string password,
                    string email,
                    string passwordQuestion,
                    string passwordAnswer,
                    bool isApproved,
                    string comment,
                    DateTime creationDate,
                    DateTime lastPasswordChangedDate,
                    DateTime lastActivityDate,
                    string applicationName,
                    bool isLockedOut,
                    DateTime lastLockedOutDate,
                    int failedPasswordAttemptCount,
                    DateTime failedPasswordAttemptWindowStart,
                    int failedPasswordAnswerAttemptCount,
                    DateTime failedPasswordAnswerAttemptWindowStart)
        {
            this.PKID = PKID;
            this.Username = username;
            this.Password = password;
            this.Email = email;
            this.PasswordQuestion = passwordQuestion;
            this.PasswordAnswer = passwordAnswer;
            this.IsApproved = isApproved;
            this.Comment = comment;
            this.CreationDate = creationDate;
            this.LastPasswordChangedDate = lastPasswordChangedDate;
            this.LastActivityDate = lastActivityDate;
            this.ApplicationName = applicationName;
            this.IsLockedOut = isLockedOut;
            this.LastLockedOutDate = lastLockedOutDate;
            this.FailedPasswordAttemptCount = failedPasswordAttemptCount;
            this.FailedPasswordAttemptWindowStart = failedPasswordAttemptWindowStart;
            this.FailedPasswordAnswerAttemptCount = failedPasswordAnswerAttemptCount;
            this.FailedPasswordAnswerAttemptWindowStart = failedPasswordAnswerAttemptWindowStart;
        }

        [Db4objects.Db4o.Config.Attributes.Indexed]
        public readonly Guid PKID;

        [Db4objects.Db4o.Config.Attributes.Indexed]
        public string Username;

        [Db4objects.Db4o.Config.Attributes.Indexed]
        public string Email;

        [Db4objects.Db4o.Config.Attributes.Indexed]
        public string ApplicationName;

        public string Comment;
        public string Password;
        public string PasswordQuestion;
        public string PasswordAnswer;
        public bool IsApproved;

        [Db4objects.Db4o.Config.Attributes.Indexed]
        public DateTime LastActivityDate;

        public DateTime LastLoginDate;
        public DateTime LastPasswordChangedDate;
        public DateTime CreationDate;
        public bool IsOnLine;
        public bool IsLockedOut;
        public DateTime LastLockedOutDate;
        public int FailedPasswordAttemptCount;
        public DateTime FailedPasswordAttemptWindowStart;
        public int FailedPasswordAnswerAttemptCount;
        public DateTime FailedPasswordAnswerAttemptWindowStart;

        public override string ToString()
        {
            return string.Format("User:{0}:{1}",
                                 this.Username,
                                 this.ApplicationName);
        }

        public void UpdateFailureCount(string failureType, MembershipProvider provider)
        {
            DateTime windowStart;
            int failureCount;

            if (failureType == "password")
            {
                windowStart = this.FailedPasswordAttemptWindowStart;
                failureCount = this.FailedPasswordAttemptCount;
            }
            else if (failureType == "passwordAnswer")
            {
                windowStart = this.FailedPasswordAnswerAttemptWindowStart;
                failureCount = this.FailedPasswordAnswerAttemptCount;
            }
            else
                throw new ProviderException("Invalid failure type");

            DateTime windowEnd = windowStart.AddMinutes(provider.PasswordAttemptWindow);

            if (failureCount == 0 || DateTime.Now > windowEnd)
            {
                // First password failure or outside of PasswordAttemptWindow. 
                // Start a new password failure count from 1 and a new window starting now.

                if (failureType == "password")
                {
                    this.FailedPasswordAttemptCount = 1;
                    this.FailedPasswordAttemptWindowStart = DateTime.Now;
                }
                else if (failureType == "passwordAnswer")
                {
                    this.FailedPasswordAnswerAttemptCount = 1;
                    this.FailedPasswordAnswerAttemptWindowStart = DateTime.Now;
                }
            }
            else
            {
                if (failureCount++ >= provider.MaxInvalidPasswordAttempts)
                {
                    // Password attempts have exceeded the failure threshold. Lock out
                    // the user.

                    this.IsLockedOut = true;
                    this.LastLockedOutDate = DateTime.Now;
                }
                else
                {
                    // Password attempts have not exceeded the failure threshold. Update
                    // the failure counts. Leave the window the same.

                    if (failureType == "password")
                    {
                        this.FailedPasswordAttemptCount = failureCount;
                    }
                    else if (failureType == "passwordAnswer")
                    {
                        this.FailedPasswordAnswerAttemptCount = failureCount;
                    }
                }
            }
        }
    }
}