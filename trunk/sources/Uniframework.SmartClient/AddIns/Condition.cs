using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Uniframework.Services;

namespace Uniframework.SmartClient
{
    public class Condition : ICondition
    {
        private string name;
        private ConditionFailedAction action;
        private IConfiguration configuration;

        public Condition(string name, IConfiguration configuration)
        {
            this.name = name;
            this.configuration = configuration;
            if (configuration.Attributes["action"] != null)
                action = (ConditionFailedAction)Enum.Parse(typeof(ConditionFailedAction), configuration.Attributes["action"]);
        }

        public string Name
        {
            get { return name; }
        }

        public ConditionFailedAction Action
        {
            get { return action; }
            set { action = value; }
        }

        public string this[string key]
        {
            get { return configuration.Attributes[key]; }
        }

        public bool IsValid(object caller, WorkItem context)
        {
            // to do
            return false;
        }

        public static ICondition Read(IConfiguration configuration)
        {
            return new Condition(configuration.Name, configuration);
        }

        public static ConditionFailedAction GetFailedAction(IEnumerable<ICondition> conditionList, object caller, WorkItem context)
        {
            ConditionFailedAction action = ConditionFailedAction.Nothing;
            foreach (ICondition condition in conditionList)
            {
                if (!condition.IsValid(caller, context))
                {
                    if (condition.Action == ConditionFailedAction.Disable)
                    {
                        action = ConditionFailedAction.Disable;
                    }
                    else
                    {
                        return ConditionFailedAction.Exclude;
                    }
                }
            }
            return action;
        }
    }
}