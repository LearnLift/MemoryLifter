/***************************************************************************************************************************************
 * Copyright (C) 2001-2012 LearnLift USA																	*
 * Contact: Learnlift USA, 12 Greenway Plaza, Suite 1510, Houston, Texas 77046, support@memorylifter.com					*
 *																								*
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License	*
 * as published by the Free Software Foundation; either version 2.1 of the License, or (at your option) any later version.			*
 *																								*
 * This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty	*
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more details.	*
 *																								*
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not,					*
 * write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA					*
 ***************************************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using Microsoft.Isam.Esent.Collections.Generic;
using System.IO;
using MLifter.Generics.Properties;

namespace MLifter.Generics
{
	public class PersistentMemoryCache<T> : ObjectCache, IDisposable
	{
		private PersistentDictionary<string, T> values;
		private PersistentDictionary<string, DateTimeOffset> lifetime;

		/// <summary>
		/// Prevents a default instance of the <see cref="PersistentMemoryCache"/> class from being created.
		/// </summary>
		/// <remarks>CFI, 2012-03-10</remarks>
		public PersistentMemoryCache(string name)
		{
			string root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Settings.Default.CacheFolder, name);
			string cacheFolder = Path.Combine(root, "values");
			string lifetimeFolder = Path.Combine(root, "lifetime");
			if (!Directory.Exists(cacheFolder))
				Directory.CreateDirectory(cacheFolder);
			if (!Directory.Exists(lifetimeFolder))
				Directory.CreateDirectory(lifetimeFolder);
			values = new PersistentDictionary<string, T>(cacheFolder);
			lifetime = new PersistentDictionary<string, DateTimeOffset>(lifetimeFolder);
		}

		/// <summary>
		/// When overridden in a derived class, inserts a cache entry into the cache, specifying a key and a value for the cache entry, and information about how the entry will be evicted.
		/// </summary>
		/// <param name="key">A unique identifier for the cache entry.</param>
		/// <param name="value">The object to insert.</param>
		/// <param name="policy">An object that contains eviction details for the cache entry. This object provides more options for eviction than a simple absolute expiration.</param>
		/// <param name="regionName">Optional. A named region in the cache to which the cache entry can be added, if regions are implemented. The default value for the optional parameter is null.</param>
		/// <returns>
		/// If a cache entry with the same key exists, the specified cache entry's value; otherwise, null.
		/// </returns>
		/// <remarks>CFI, 2012-03-10</remarks>
		public override object AddOrGetExisting(string key, object value, CacheItemPolicy policy, string regionName = null) { throw new NotImplementedException(); }
		/// <summary>
		/// When overridden in a derived class, inserts the specified <see cref="T:System.Runtime.Caching.CacheItem"/> object into the cache, specifying information about how the entry will be evicted.
		/// </summary>
		/// <param name="value">The object to insert.</param>
		/// <param name="policy">An object that contains eviction details for the cache entry. This object provides more options for eviction than a simple absolute expiration.</param>
		/// <returns>
		/// If a cache entry with the same key exists, the specified cache entry; otherwise, null.
		/// </returns>
		/// <remarks>CFI, 2012-03-10</remarks>
		public override CacheItem AddOrGetExisting(CacheItem value, CacheItemPolicy policy) { throw new NotImplementedException(); }
		
		/// <summary>
		/// Inserts a cache entry into the cache, by using a key, an object for the cache entry, an absolute expiration value, and an optional region to add the cache into.
		/// </summary>
		/// <param name="key">A unique identifier for the cache entry.</param>
		/// <param name="value">The object to insert.</param>
		/// <param name="absoluteExpiration">The fixed date and time at which the cache entry will expire.</param>
		/// <param name="regionName">Optional. A named region in the cache to which the cache entry can be added, if regions are implemented. The default value for the optional parameter is null.</param>
		/// <returns>
		/// If a cache entry with the same key exists, the specified cache entry's value; otherwise, null.
		/// </returns>
		/// <remarks>CFI, 2012-03-10</remarks>
		public override object AddOrGetExisting(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
		{
			if (Contains(key))
				return values[key];
			Set(key, value, absoluteExpiration, regionName);
			return null;
		}

		/// <summary>
		/// Checks whether the cache entry already exists in the cache.
		/// </summary>
		/// <param name="key">A unique identifier for the cache entry.</param>
		/// <param name="regionName">Optional. A named region in the cache where the cache can be found, if regions are implemented. The default value for the optional parameter is null.</param>
		/// <returns>
		/// true if the cache contains a cache entry with the same key value as <paramref name="key"/>; otherwise, false.
		/// </returns>
		/// <remarks>CFI, 2012-03-10</remarks>
		public override bool Contains(string key, string regionName = null)
		{
			CleanExpired();
			return values.ContainsKey(key);
		}

		/// <summary>
		/// Cleans the expired items from the cache.
		/// </summary>
		/// <remarks>CFI, 2012-03-10</remarks>
		private void CleanExpired()
		{
			var expired = from l in lifetime.AsParallel()
						  where l.Value <= DateTimeOffset.Now
						  select l;
			expired.ForAll((e) => { values.Remove(e.Key); lifetime.Remove(e.Key); });
		}

		/// <summary>
		/// When overridden in a derived class, creates a <see cref="T:System.Runtime.Caching.CacheEntryChangeMonitor"/> object that can trigger events in response to changes to specified cache entries.
		/// </summary>
		/// <param name="keys">The unique identifiers for cache entries to monitor.</param>
		/// <param name="regionName">Optional. A named region in the cache where the cache keys in the <paramref name="keys"/> parameter exist, if regions are implemented. The default value for the optional parameter is null.</param>
		/// <returns>
		/// A change monitor that monitors cache entries in the cache.
		/// </returns>
		/// <remarks>CFI, 2012-03-10</remarks>
		public override CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys, string regionName = null) { throw new NotImplementedException(); }
		/// <summary>
		/// When overridden in a derived class, gets a description of the features that a cache implementation provides.
		/// </summary>
		/// <returns>A bitwise combination of flags that indicate the default capabilities of a cache implementation.</returns>
		/// <remarks>CFI, 2012-03-10</remarks>
		public override DefaultCacheCapabilities DefaultCacheCapabilities { get { throw new NotImplementedException(); } }

		/// <summary>
		/// Gets the specified cache entry from the cache as an object.
		/// </summary>
		/// <param name="key">A unique identifier for the cache entry to get.</param>
		/// <param name="regionName">Optional. A named region in the cache to which the cache entry was added, if regions are implemented. The default value for the optional parameter is null.</param>
		/// <returns>
		/// The cache entry that is identified by <paramref name="key"/>.
		/// </returns>
		/// <remarks>CFI, 2012-03-10</remarks>
		public override object Get(string key, string regionName = null)
		{
			if(Contains(key))
				return values[key];
			return null;
		}

		/// <summary>
		/// When overridden in a derived class, gets the specified cache entry from the cache as a <see cref="T:System.Runtime.Caching.CacheItem"/> instance.
		/// </summary>
		/// <param name="key">A unique identifier for the cache entry to get.</param>
		/// <param name="regionName">Optional. A named region in the cache to which the cache was added, if regions are implemented. Because regions are not implemented in .NET Framework 4, the default is null.</param>
		/// <returns>
		/// The cache entry that is identified by <paramref name="key"/>.
		/// </returns>
		/// <remarks>CFI, 2012-03-10</remarks>
		public override CacheItem GetCacheItem(string key, string regionName = null) { throw new NotImplementedException(); }

		/// <summary>
		/// Gets the total number of cache entries in the cache.
		/// </summary>
		/// <param name="regionName">Optional. A named region in the cache for which the cache entry count should be computed, if regions are implemented. The default value for the optional parameter is null.</param>
		/// <returns>
		/// The number of cache entries in the cache. If <paramref name="regionName"/> is not null, the count indicates the number of entries that are in the specified cache region.
		/// </returns>
		/// <remarks>CFI, 2012-03-10</remarks>
		public override long GetCount(string regionName = null)
		{
			CleanExpired();
			return values.Count;
		}

		/// <summary>
		/// Creates an enumerator that can be used to iterate through a collection of cache entries.
		/// </summary>
		/// <returns>
		/// The enumerator object that provides access to the cache entries in the cache.
		/// </returns>
		/// <remarks>CFI, 2012-03-10</remarks>
		protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			CleanExpired();
			return values.GetEnumerator() as IEnumerator<KeyValuePair<string, object>>;
		}

		/// <summary>
		/// Gets a set of cache entries that correspond to the specified keys.
		/// </summary>
		/// <param name="keys">A collection of unique identifiers for the cache entries to get.</param>
		/// <param name="regionName">Optional. A named region in the cache to which the cache entry or entries were added, if regions are implemented. The default value for the optional parameter is null.</param>
		/// <returns>
		/// A dictionary of key/value pairs that represent cache entries.
		/// </returns>
		/// <remarks>CFI, 2012-03-10</remarks>
		public override IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null)
		{
			CleanExpired();
			IDictionary<string, object> result = new Dictionary<string, object>();
			foreach (string key in keys)
			{
				if (values.ContainsKey(key))
					result.Add(key, values[key]);
			}

			return result;
		}

		/// <summary>
		/// Gets the name of a specific <see cref="T:System.Runtime.Caching.ObjectCache"/> instance.
		/// </summary>
		/// <returns>The name of a specific cache instance.</returns>
		/// <remarks>CFI, 2012-03-10</remarks>
		public override string Name { get { return Settings.Default.CacheFolder; } }

		/// <summary>
		/// Removes the cache entry from the cache.
		/// </summary>
		/// <param name="key">A unique identifier for the cache entry.</param>
		/// <param name="regionName">Optional. A named region in the cache to which the cache entry was added, if regions are implemented. The default value for the optional parameter is null.</param>
		/// <returns>
		/// An object that represents the value of the removed cache entry that was specified by the key, or null if the specified entry was not found.
		/// </returns>
		/// <remarks>CFI, 2012-03-10</remarks>
		public override object Remove(string key, string regionName = null)
		{
			if (!Contains(key))
				return null;

			object value = values[key];
			values.Remove(key);
			lifetime.Remove(key);
			return value;
		}

		/// <summary>
		/// When overridden in a derived class, inserts a cache entry into the cache.
		/// </summary>
		/// <param name="key">A unique identifier for the cache entry.</param>
		/// <param name="value">The object to insert.</param>
		/// <param name="policy">An object that contains eviction details for the cache entry. This object provides more options for eviction than a simple absolute expiration.</param>
		/// <param name="regionName">Optional. A named region in the cache to which the cache entry can be added, if regions are implemented. The default value for the optional parameter is null.</param>
		/// <remarks>CFI, 2012-03-10</remarks>
		public override void Set(string key, object value, CacheItemPolicy policy, string regionName = null) { throw new NotImplementedException(); }
		/// <summary>
		/// When overridden in a derived class, inserts the cache entry into the cache as a <see cref="T:System.Runtime.Caching.CacheItem"/> instance, specifying information about how the entry will be evicted.
		/// </summary>
		/// <param name="item">The cache item to add.</param>
		/// <param name="policy">An object that contains eviction details for the cache entry. This object provides more options for eviction than a simple absolute expiration.</param>
		/// <remarks>CFI, 2012-03-10</remarks>
		public override void Set(CacheItem item, CacheItemPolicy policy) { throw new NotImplementedException(); }

		/// <summary>
		/// Inserts a cache entry into the cache, specifying time-based expiration details.
		/// </summary>
		/// <param name="key">A unique identifier for the cache entry.</param>
		/// <param name="value">The object to insert.</param>
		/// <param name="absoluteExpiration">The fixed date and time at which the cache entry will expire.</param>
		/// <param name="regionName">Optional. A named region in the cache to which the cache entry can be added, if regions are implemented. The default value for the optional parameter is null.</param>
		/// <remarks>CFI, 2012-03-10</remarks>
		public override void Set(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
		{
			values[key] = (T)value;
			lifetime[key] = absoluteExpiration;
		}

		/// <summary>
		/// Gets or sets the cache item.
		/// </summary>
		/// <returns>A key that serves as an indexer into the cache instance.</returns>
		/// <remarks>CFI, 2012-03-10</remarks>
		public override object this[string key]
		{
			get
			{
				if (Contains(key))
					return values[key];
				return null;
			}
			set { values[key] = (T)value; }
		}

		private bool IsDisposed = false;
		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <remarks>CFI, 2012-03-10</remarks>
		public void Dispose()
		{
			if (IsDisposed)
				return;

			values.Flush();
			values.Dispose();

			lifetime.Flush();
			lifetime.Dispose();

			IsDisposed = true;
		}
	}
}
