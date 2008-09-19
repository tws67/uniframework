//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Cryptography Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Practices.EnterpriseLibrary.Security.Cryptography
{
	internal class ProtectedKeyCache
	{
		Dictionary<string, ProtectedKey> cache = new Dictionary<string, ProtectedKey>();

		public void Clear() { lock (cache) { cache.Clear(); } }

		public ProtectedKey this[string keyFileName]
		{
			get
			{
				if (String.IsNullOrEmpty(keyFileName)) throw new ArgumentException("keyFileName");

				lock (cache)
				{
					return cache.ContainsKey(keyFileName) ? cache[keyFileName] : null;
				}
			}
			set
			{
				if (value == null) throw new ArgumentNullException("ProtectedKey");
				if (String.IsNullOrEmpty(keyFileName)) throw new ArgumentException("keyFileName");

				lock (cache)
				{
					cache[keyFileName] = value;
				}
			}
		}
	}
}
