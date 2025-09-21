using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SqlHelpers
{
    public class EnvironmentFileHelper
    {
        public static string GetEnvironmentName()
        {
            string envsetup = "local";
            if (File.Exists(Directory.GetCurrentDirectory() + $"/Envsetup.txt"))
            {
                envsetup = File.ReadAllText(Directory.GetCurrentDirectory() + $"/Envsetup.txt");
            }
            return envsetup;
        }
        public static string GetEnvironmentFileName()
        {
            string envJsonFile = "appsettings.json";
            string envsetup = GetEnvironmentName();

            if (envsetup.ToLower() == "development") envJsonFile = "appsettings.Development.json";
            if (envsetup.ToLower() == "uat") envJsonFile = "appsettings.Staging.json";
            if (envsetup.ToLower() == "production") envJsonFile = "appsettings.Production.json";
            return envJsonFile;
        }
    }
}
