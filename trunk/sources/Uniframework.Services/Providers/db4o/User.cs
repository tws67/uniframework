using System;
using System.Configuration.Provider;
using System.Web.Security;
using Db4objects.Db4o.Config.Attributes;

namespace Uniframework.Services.db4oProviders
{
    public class User : DataContainer
    {
        [Indexed]
        public readonly Guid PKID;

        [Indexed]
        public string ApplicationName;

        public string Comment;
        public DateTime CreationDate;
        [Indexed]
        public string Email;
        public int FailedPasswordAnswerAttemptCount;
        public DateTime FailedPasswordAnswerAttemptWindowStart;
        public int FailedPasswordAttemptCount;
        public DateTime FailedPasswordAttemptWindowStart;
        public bool IsApproved;
        public bool IsLockedOut;
        public bool IsOnLine;

        [Indexed]
        public DateTime LastActivityDate;
        public DateTime LastLockedOutDate;

        public DateTime LastLoginDate;
        public DateTime LastPasswordChangedDate;
        public string Password;
        public string PasswordAnswer;
        public string PasswordQuestion;
        [Indexed]
        public string Username;

        public User(Guid pkid,
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
            PKID = pkid;
            Username = username;
            Password = password;
            Email = email;
            PasswordQuestion = passwordQuestion;
            PasswordAnswer = passwordAnswer;
            IsApproved = isApproved;
            Comment = comment;
            CreationDate = creationDate;
            LastPasswordChangedDate = lastPasswordChangedDate;
            LastActivityDate = lastActivityDate;
            ApplicationName = applicationName;
            IsLockedOut = isLockedOut;
            LastLockedOutDate = lastLockedOutDate;
            FailedPasswordAttemptCount = failedPasswordAttemptCount;
            FailedPasswordAttemptWindowStart = failedPasswordAttemptWindowStart;
            FailedPasswordAnswerAttemptCount = failedPasswordAnswerAttemptCount;
            FailedPasswordAnswerAttemptWindowStart = failedPasswordAnswerAttemptWindowStart;
        }

        public override string ToString()
        {
            return string.Format("User:{0}:{1}",
                                 Username,
                                 ApplicationName);
        }

        public void UpdateFailureCount(string failureType, MembershipProvider provider)
        {
            DateTime windowStart;
            int failureCount;

            if (failureType == "password")
            {
                windowStart = FailedPasswordAttemptWindowStart;
                failureCount = FailedPasswordAttemptCount;
            }
            else if (failureType == "passwordAnswer")
            {
                windowStart = FailedPasswordAnswerAttemptWindowStart;
                failureCount = FailedPasswordAnswerAttemptCount;
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
                    FailedPasswordAttemptCount = 1;
                    FailedPasswordAttemptWindowStart = DateTime.Now;
                }
                else if (failureType == "passwordAnswer")
                {
                    FailedPasswordAnswerAttemptCount = 1;
                    FailedPasswordAnswerAttemptWindowStart = DateTime.Now;
                }
            }
            else
            {
                if (failureCount++ >= provider.MaxInvalidPasswordAttempts)
                {
                    // Password attempts have exceeded the failure threshold. Lock out
                    // the user.

                    IsLockedOut = true;
                    LastLockedOutDate = DateTime.Now;
                }
                else
                {
                    // Password attempts have not exceeded the failure threshold. Update
                    // the failure counts. Leave the window the same.

                    if (failureType == "password")
                    {
                        FailedPasswordAttemptCount = failureCount;
                    }
                    else if (failureType == "passwordAnswer")
                    {
                        FailedPasswordAnswerAttemptCount = failureCount;
                    }
                }
            }
        }
    }
}