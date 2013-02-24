#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace Drydock{
    internal static class Gbl{
        public static GraphicsDevice Device;
        public static ContentManager ContentManager;
        public static Dictionary<string, string> RawLookup;
        public static Matrix ProjectionMatrix;
        public static Point ScreenSize;

        /// <summary>
        /// whenever the md5 doesn't match up to the new one, the value of RawDir in the following structure is set to true
        /// </summary>
        public static Dictionary<RawDir, bool> HasRawHashChanged;

        /// <summary>
        /// If the md5 has changed, the new md5 won't be written to file until its RawDir in this dictionary is true.
        /// This prevents the md5 from updating when certain scripts havent been compiled/processed before the program terminates.
        /// </summary>
        public static Dictionary<RawDir, bool> AllowMD5Refresh;

        static Dictionary<RawDir, string> _fileHashes;

        //todo: write jsonconverter for this enum
        public enum RawDir {
            Config,
            Scripts,
            Templates
        }

        static Gbl() {
            CheckHashes();
            RawLookup = new Dictionary<string, string>();

            List<string> directories = new List<string>();
            List<string> directoriesToSearch = new List<string>();

            string currentDirectory = Directory.GetCurrentDirectory() + "\\Config\\";
            directoriesToSearch.Add(currentDirectory);
            
            while (directoriesToSearch.Count > 0) {
                string dir = directoriesToSearch[0];
                directoriesToSearch.RemoveAt(0);
                directoriesToSearch.AddRange(Directory.GetDirectories(dir).ToList());
                directories.Add(dir);
            }
            try {
                foreach (string directory in directories) {
                    string[] files = Directory.GetFiles(directory);
                    foreach (string file in files) {
                        var sr = new StreamReader(file);

                        var newConfigVals = JsonConvert.DeserializeObject<Dictionary<string, string>>(sr.ReadToEnd());
                        sr.Close();

                        string prefix = newConfigVals["InternalAbbreviation"] + "_";
                        newConfigVals.Remove("InternalAbbreviation");

                        foreach (var configVal in newConfigVals) {
                            try {
                                RawLookup.Add(prefix + configVal.Key, configVal.Value);
                            }
                            catch (Exception e) {
                                int f = 4;
                            }
                        }
                    }
                }
            }
            catch (Exception e){
               int g = 5;
            }
        }

        static void CheckHashes() {
            HasRawHashChanged = new Dictionary<RawDir, bool>();
            AllowMD5Refresh = new Dictionary<RawDir, bool>();
            _fileHashes = new Dictionary<RawDir, string>();
            var sr = new StreamReader((Directory.GetCurrentDirectory() + "\\Data\\Hashes.json"));
            var jobj = JObject.Parse(sr.ReadToEnd());
            sr.Close();
            MD5 md5Gen = MD5.Create();
            foreach (var hashEntry in jobj) {
                string fileDir = hashEntry.Key;
                var hash = hashEntry.Value.ToObject<string>();
                var files = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\" + fileDir + "\\");
                string newHash = "";
                foreach (var file in files) {
                    var fr = new FileStream(file, FileMode.Open, FileAccess.Read);
                    var fileHash = md5Gen.ComputeHash(fr);
                    sr.Close();

                    for (int i = 0; i < 16; i++) {
                        string temp = "";
                        foreach (var b in fileHash) {
                            temp += b.ToString();
                        }
                        newHash += temp;
                    }
                }

                RawDir dir = RawDir.Templates;
                switch (fileDir) {
                    case "Config":
                        dir = RawDir.Config;
                        break;
                    case "Templates":
                        dir = RawDir.Templates;
                        break;
                    case "Scripts":
                        dir = RawDir.Scripts;
                        break;
                }

                _fileHashes.Add(dir, newHash);
                if (!hash.SequenceEqual(newHash)) {
                    HasRawHashChanged.Add(dir, true);
                    AllowMD5Refresh.Add(dir, false);
                }
                else {
                    HasRawHashChanged.Add(dir, false);
                }
            }
        }

        public static void CommitHashChanges() {
            var sr = new StreamReader((Directory.GetCurrentDirectory() + "\\Data\\Hashes.json"));
            var jobj = JObject.Parse(sr.ReadToEnd());
            sr.Close();

            foreach (var hashEntry in AllowMD5Refresh) {
                var dir = hashEntry.Key;
                bool updateHash = hashEntry.Value;
                string s = dir.ToString();
                if (updateHash) {
                    jobj[s] = _fileHashes[dir];
                }
            }
            var sw = new StreamWriter((Directory.GetCurrentDirectory() + "\\Data\\Hashes.json"));
            string ss = JsonConvert.SerializeObject(jobj, Formatting.Indented);
            sw.Write(ss);
            sw.Close();

        }

        public static T LoadContent<T>(string str) {
            string objValue = "";
            try {
                objValue = RawLookup[str];
                return ContentManager.Load<T>(objValue);
            }
            catch (Exception e) {
                T obj;
                try {
                    obj = JsonConvert.DeserializeObject<T>(objValue);
                }
                catch (Exception ee) {
                    obj = VectorParser.Parse<T>(objValue);
                }
                return obj;
            }
        }

        public static string LoadScript(string str) {
            string address = RawLookup[str];
            var sr = new StreamReader(address);
            string scriptText = sr.ReadToEnd();
            sr.Close();
            return scriptText;
        }

        public static ShaderParam[] LoadShaderParams(string shaderName){
            var configs = new List<string>();
            var configValues = new List<string>();
            foreach (var valuePair in RawLookup){
                if (valuePair.Key.Contains(shaderName)){
                    configs.Add(valuePair.Key.Substring(shaderName.Count()+1));
                    configValues.Add(valuePair.Value);
                }
            }

            var retConfigs = new List<ShaderParam>();

            //figure out its datatype
            for (int i = 0; i < configs.Count; i++){
                string name = configs[i];
                string configVal = configValues[i];

                if (configVal.Contains(",")) {
                    //it's a vector
                    var commas = from value in configVal
                                 where value == ','
                                 select value;
                    int commaCount = commas.Count();
                    object parsedVector;
                    Type type;
                    switch (commaCount){
                        case 1:
                            type = typeof(Vector2);
                            parsedVector = VectorParser.Parse<Vector2>(configVal);
                            break;
                        case 2:
                            type = typeof(Vector3);
                            parsedVector = VectorParser.Parse<Vector3>(configVal);
                            break;
                        case 3:
                            type = typeof(Vector4);
                            parsedVector = VectorParser.Parse<Vector4>(configVal);
                            break;
                        default:
                            throw new Exception("vector4 is the largest dimension of vector supported");
                    }
                    retConfigs.Add(new ShaderParam(name, parsedVector, type));
                    continue;
                }
                //figure out if it's a string
                var alphanumerics = from value in configVal
                                    where char.IsLetter(value)
                                    select value;
                if (alphanumerics.Any()){
                    //it's a string
                    retConfigs.Add(new ShaderParam(name, configVal, typeof(string)));
                    continue;
                }

                if(configVal.Contains(".")){
                    //it's a float
                    retConfigs.Add(new ShaderParam(name, float.Parse(configVal), typeof(float)));
                    continue;
                }

                //assume its an integer
                retConfigs.Add(new ShaderParam(name, int.Parse(configVal), typeof(int)));
            }

            return retConfigs.ToArray();
        }

        public static ReadOnlyCollection<byte[]> LoadBinary(string str) {
            string address = "Compiled\\" + RawLookup[str];
            address += "c"; //the file extension for a compiled binary is .clc for .cl files

            var binaryFormatter = new BinaryFormatter();
            var fileStrm = new FileStream(address, FileMode.Open, FileAccess.Read, FileShare.None);
            var binary = (ReadOnlyCollection<byte[]>)binaryFormatter.Deserialize(fileStrm);
            fileStrm.Close();
            return binary;
        }

        public static void SaveBinary(ReadOnlyCollection<byte[]> binary, string str) {
            string address = "Compiled\\" + RawLookup[str];
            address += "c"; //the file extension for a compiled binary is .clc for .cl files

            var binaryFormatter = new BinaryFormatter();
            var fileStrm = new FileStream(address, FileMode.Create, FileAccess.Write, FileShare.None);
            binaryFormatter.Serialize(fileStrm, binary);
            fileStrm.Close();
        }


        public static string GetScriptDirectory(string str) {
            string curDir = Directory.GetCurrentDirectory();
            string address = RawLookup[str];
            string directory = "";
            for (int i = address.Length - 1; i >= 0; i--) {
                if (address[i] == '\\') {
                    directory = address.Substring(0, i);
                }
            }
            return curDir + "\\" + directory;
        }

        public struct ShaderParam{
            public string Name;
            public object Parameter;
            public Type Type;
            public ShaderParam(string name, object param, Type type){
                Name = name;
                Parameter = param;
                Type = type;
            }
        }
    }
}