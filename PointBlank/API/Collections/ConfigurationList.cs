﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using PointBlank.Framework.Permissions.Ring;

namespace PointBlank.API.Collections
{
    /// <summary>
    /// Custom collection for configurations
    /// </summary>
    [RingPermission(SecurityAction.Demand, ring = RingPermissionRing.None)]
    public class ConfigurationList : ICollection
    {
        #region Properties
        /// <summary>
        /// The list of the configurations
        /// </summary>
        protected Dictionary<string, object> Configurations { get; private set; }

        /// <summary>
        /// Gets/Sets the configuration object with the configuration name provided
        /// </summary>
        /// <param name="configuration_name">The configuration name</param>
        /// <returns>The configuration object</returns>
        public object this[string configuration_name]
        {
            get
            {
                return Configurations[configuration_name];
            }
            set
            {
                if (Configurations.ContainsKey(configuration_name))
                    Configurations[configuration_name] = value;
                else
                    Configurations.Add(configuration_name, value);
            }
        }

        /// <summary>
        /// Gets/Sets the configuration KeyValuePair using the index provided
        /// </summary>
        /// <param name="index">The index of the configuration</param>
        /// <returns>The KeyValuePair of the configuration</returns>
        public KeyValuePair<string, object> this[int index]
        {
            get
            {
                return Configurations.ElementAt(index);
            }
            set
            {
                Configurations[Configurations.ElementAt(index).Key] = value.Value;
            }
        }

        /// <summary>
        /// Returns the amount of items in the configuration list
        /// </summary>
        public int Count
        {
            get
            {
                return Configurations.Count;
            }
        }

        public object SyncRoot => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();
        #endregion

        /// <summary>
        /// Custom collection for configurations
        /// </summary>
        public ConfigurationList()
        {
            // Initialize the variables
            Configurations = new Dictionary<string, object>();
        }

        #region Collection Functions
        /// <summary>
        /// Adds a configuration entry using the configuration name and configuration object
        /// </summary>
        /// <param name="configuration_name">Configuration name</param>
        /// <param name="configuration">Configuration object</param>
        public void Add(string configuration_name, object configuration)
        {
            Configurations.Add(configuration_name, configuration);
        }

        /// <summary>
        /// Adds a configuration entry using the KeyValuePair
        /// </summary>
        /// <param name="kvp">KeyValuePair of the entry</param>
        public void Add(KeyValuePair<string, object> kvp)
        {
            Configurations.Add(kvp.Key, kvp.Value);
        }

        /// <summary>
        /// Removes a configuration entry using the configuration name
        /// </summary>
        /// <param name="configuration_name">Configuration name</param>
        public void Remove(string configuration_name)
        {
            Configurations.Remove(configuration_name);
        }

        /// <summary>
        /// Removes a configuration entry using the index
        /// </summary>
        /// <param name="index">Index of the entry</param>
        public void RemoveAt(int index)
        {
            Configurations.Remove(this[index].Key);
        }

        /// <summary>
        /// Adds a range of configurations using the KeyValuePair list
        /// </summary>
        /// <param name="list">KeyValuePair list</param>
        public void AddRange(IEnumerable<KeyValuePair<string, object>> list)
        {
            for (int i = 0; i < list.Count(); i++)
                this.Add(list.ElementAt(i).Key, list.ElementAt(i).Value);
        }

        /// <summary>
        /// Adds a range of configurations using the configuration list
        /// </summary>
        /// <param name="list">Configuration list</param>
        public void AddRange(ConfigurationList list)
        {
            for (int i = 0; i < list.Count; i++)
                this.Add(list[i]);
        }

        /// <summary>
        /// Gets the enumerator and returns it
        /// </summary>
        /// <returns>List enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Configurations.GetEnumerator();
        }

        /// <summary>
        /// Copies the KeyValuePairs into the array
        /// </summary>
        /// <param name="array">The target array</param>
        /// <param name="index">The start index</param>
        public void CopyTo(Array array, int index)
        {
            for (int i = 0; i < (this.Count - index); i++)
                array.SetValue(this[index + i], i);
        }

        /// <summary>
        /// Clears the list
        /// </summary>
        public void Clear()
        {
            Configurations.Clear();
        }
        #endregion
    }
}
