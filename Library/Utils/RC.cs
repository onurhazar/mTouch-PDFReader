﻿//
// mTouch-PDFReader library
// RC.cs
//
//  Author:
//       Alexander Matsibarov <amatsibarov@gmail.com>
//
//  Copyright (c) 2014 Alexander Matsibarov
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Reflection;

namespace mTouchPDFReader.Library.Utils
{
	public class RC
	{
		private static readonly RC _instance = new RC();
		public static RC Instance {
			get { return _instance; }
		}

		private readonly object _lockObject = new object();

		private readonly Dictionary<string, object> _objects = new Dictionary<string, object>();

		private readonly Dictionary<string, Type> _references = new Dictionary<string, Type>();

		public static void RegisterReference<TInterface, TClass>() where TClass : TInterface
		{
			var interfaceInfo = typeof(TInterface);
			var classInfo = typeof(TClass);
			if (interfaceInfo == classInfo) {
				throw new Exception("Can't register reference! The tring to register interface by interface");
			}

			if (!Instance._references.ContainsKey(interfaceInfo.FullName)) {
				_instance._references[interfaceInfo.FullName] = classInfo;
			} else {
				throw new RefCollDuplicateKeyException(interfaceInfo.FullName);
			}
		}

		public static T Get<T>() where T : class
		{							 
			return _instance.getObject<T>(typeof(T).FullName);
		}

		private T getObject<T>(string fullName) where T : class
		{
			T obj;
			if (_objects.ContainsKey(fullName)) {
				try	{
					obj = (T)_objects[fullName];
				} catch (Exception ex) {
					throw new Exception("Interface is out of the service.", ex);
				}
			} else {
				lock (_lockObject) {
					if (_objects.ContainsKey(fullName)) {
						try	{
							obj = (T)_objects[fullName];
						} catch (Exception ex) {
							throw new Exception("Interface is out of the service.", ex);
						}
					} else {
						if (_references.ContainsKey(fullName)) {
							try	{
								Type tInfo = _references[fullName];
								obj = (T)tInfo.GetConstructor(
									BindingFlags.Instance | BindingFlags.NonPublic,
									null,
									new Type[0],
									new ParameterModifier[0]).Invoke(null);
								_objects.Add(fullName, obj);
							} catch (Exception ex) {
								throw new Exception("GetObject<T>() : Object as interface [" + fullName + "] is not created. See inner exception.", ex);
							}
						} else {
							var sb = new System.Text.StringBuilder();
							sb.Append("RC.getObject<");
							sb.Append(typeof(T).FullName);
							sb.Append(">()");
							sb.AppendLine("");
							sb.Append("Error message: key [");
							sb.Append(fullName);
							sb.Append("] is not registered.");
							sb.AppendLine("");
							sb.AppendLine("Call stack (4 last):");
							var s = new System.Diagnostics.StackTrace(2, true);
							var stFrames = s.GetFrames();
							for (int i = 0; i < Math.Min(stFrames.Length, 4); i++) {
								sb.AppendLine("at " + stFrames[i].GetMethod()
									+ " in " + stFrames[i].GetFileName()
									+ " line: " + stFrames[i].GetFileLineNumber().ToString());
							}
							throw new Exception(sb.ToString());
						}
					}
				}
			}
			return obj;
		}
	}

	public class RefCollDuplicateKeyException : Exception
	{
		public RefCollDuplicateKeyException(string key) : base(key) {}
	}
}
