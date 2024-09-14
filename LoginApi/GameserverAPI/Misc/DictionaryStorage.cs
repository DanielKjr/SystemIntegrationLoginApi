namespace GameserverAPI.Misc
{
	public interface IStorage
	{
		bool AddItem(string name, string serviceUrl);
		bool RemoveItem(string name, string serviceUrl);
		List<string> GetAllServiceNames();
		List<string> GetServiceUrlForService(string serviceName);

	}

	public class DictionaryStorage : IStorage
	{
		private readonly Dictionary<string, HashSet<string>> _dictionary;

		public DictionaryStorage()
		{
			_dictionary = new Dictionary<string, HashSet<string>>();
		}

		public bool AddItem(string name, string serviceUrl)
		{
			if (!_dictionary.ContainsKey(name))
			{
				_dictionary[name] = new HashSet<string>();
			}

			return _dictionary[name].Add(serviceUrl);
		}

		public bool RemoveItem(string name, string serviceUrl)
		{
			if (_dictionary.ContainsKey(name))
			{
				if (_dictionary[name].Remove(serviceUrl))
				{
					if (_dictionary[name].Count == 0)
					{
						_dictionary.Remove(name);
					}
					return true;
				}
			}
			return false;
		}
		public List<string> GetAllServiceNames()
		{
			return _dictionary.Keys.ToList();
		}

		public List<string> GetServiceUrlForService(string serviceName)
		{
			if (_dictionary.ContainsKey(serviceName))
			{
				return _dictionary[serviceName].ToList();
			}

			return new List<string>();
		}
	}
}
