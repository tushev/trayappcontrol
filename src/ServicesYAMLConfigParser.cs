using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace TrayAppControl
{
    internal class ServicesYAMLConfigParser
    {
        static public void ParseYAMLFile(string yamlPath, ref ManagedService managedService)
        {

            var yml = File.ReadAllText(yamlPath);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)  // see height_in_inches in sample yml 
                .Build();
            
            managedService = deserializer.Deserialize<ManagedService>(yml);
            
        }
    }
}
