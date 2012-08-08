#region

using System.Xml;

#endregion

namespace Drydock.Utilities {
    class ConfigRetriever {
        private readonly string _configFile;
        private XmlReader _reader;
        public ConfigRetriever(string configFile){
            _configFile = configFile;
        }

        public string GetValue(string key) {
            _reader = XmlReader.Create(_configFile);
            _reader.ReadToFollowing(key);
            _reader.MoveToFirstAttribute();
            return _reader.Value;
        }
    }
}
