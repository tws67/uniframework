using System;
using System.Collections.Generic;
using System.Text;
using Uniframework.Services;

namespace Uniframework.SmartClient
{
    public class ComplexCondition : ICondition
    {
        private string name;
        private ConditionFailedAction action;
        private IConfiguration configuration;

        public ComplexCondition(string name, IConfiguration configuration)
        {
            this.name = name;
            this.configuration = configuration;
        }

        #region ICondition Members

        public string Name
        {
            get { return name; }
        }

        public ConditionFailedAction Action
        {
            get
            {
                return action;
            }
            set
            {
                action = value;
            }
        }

        public bool IsValid(object caller, Microsoft.Practices.CompositeUI.WorkItem context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
