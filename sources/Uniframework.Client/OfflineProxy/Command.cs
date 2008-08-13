using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Uniframework.Client.OfflineProxy
{
	public class Command
	{
		object[] args;
		object targetObject;
		string commandName;

		/// <summary>
		/// Command constructor which creates a new command for the corresponding object, method name and
		/// with the given arguments.
		/// It's used from the RequestManager class to implement a command queue.
		/// </summary>
		/// <param name="target">Target object for the command.</param>
		/// <param name="commandName">Method name to be invoked.</param>
		/// <param name="args">Array of parameters to be used during the invoke.</param>
		public Command(object target, string commandName, params object[] args)
		{
			this.targetObject = target;
			this.commandName = commandName;
			this.args = args;
		}

		/// <summary>
		/// This method invokes the method with the command name using the parameteres.
		/// </summary>
		/// <returns>The result value of the method.</returns>
		public object Execute()
		{
			MethodInfo commandMethod = targetObject.GetType().GetMethod(commandName);
			return commandMethod.Invoke(targetObject, args);
		}

		/// <summary>
		/// Getter for the command method name to invoke.
		/// </summary>
		public string CommandName
		{
			get { return commandName; }
		}
	}
}
