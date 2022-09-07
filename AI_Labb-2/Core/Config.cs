using Azure;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Labb_2.Core
{
    public class Config
    {
        public string imageEndpoint { get; private set; }
        public string imageKey { get; private set; }
        public AzureKeyCredential questionsCreds { get; private set; }

        public Config()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("settings.json");
            IConfigurationRoot conf = builder.Build();

            imageEndpoint = conf["imageEndpoint"];
            imageKey = conf["imageKey"];
        }
    }
}
