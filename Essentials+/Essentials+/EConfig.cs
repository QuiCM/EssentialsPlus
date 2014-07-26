using System.IO;
using Newtonsoft.Json;

namespace Essentials
{
    public class EConfig
    {
        public void Write(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static EConfig Read(string path)
        {
            return !File.Exists(path)
                ? new EConfig()
                : JsonConvert.DeserializeObject<EConfig>(File.ReadAllText(path));
        }
    }
}
