﻿using Newtonsoft.Json;

namespace ProfiseeDevUtils.Init
{
    public class EnvironmentVariables
    {
        private string TfsBaseDirPath = @"\DevOps";
        private Dictionary<string, string> envVars = new Dictionary<string, string>
        {
            // locals to populate full values
            { "MaestroVersion", "22.2.0" },
            { "TfsDrive", "C:"},
            { "MaestroWebAppName", "Profisee"},
            { "MaestroDb", "Profisee"},
            { "TfsGetSource", "$/Products/"},
            { "SqlDataPath", @"C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA"},
            { "WebSiteName", "Default Web Site"},
            { "SqlServer", "."},
            { "UseWindowsAuthentication", "true"},
            { "SqlUserName", ""},
            { "SqlUserPassword", ""},
            { "MaestroAppPoolName", "Profisee"},
            { "MaestroAppPoolUserName", @"corp\svc_web"},
            { "MaestroAppPoolUserPassword", "Profisee1"},
            { "MaestroServicePort", "8003"},
            { "MaestroServiceUserName", @"corp\svc_maestro"},
            { "MaestroServiceUserPassword", "Profisee1"},
            { "TfsUtil", @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\tf.exe"},
            { "MsBuildUtil", @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe"},
            { "MsTestUtil", @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\Extensions\TestPlatform\VSTest.Console.exe"},
            { "AttachmentRepositoryLocation", @"C:\FileRepository"},
            { "AttachmentRepositoryUserName", @"corp\svc_web"},
            { "AttachmentRepositoryUserPassword", "Profisee1"},
            { "AttachmentRepositoryLogonType", "Interactive"},
            { "UseHttps", "true"},
            { "UseSeparateDatabases", "true"},
            { "SetupTestCategory", "00-ApolloSetup-Small"},

            // automation settings
            { "SqlEventingDatabaseName", "" },
            { "SqlConnectorSqlServer", ""},
            { "ServerURL", "net.tcp://127.0.0.1/Profisee"},
            { "CrmSqlServer", ""},
            { "CrmEventingDatabaseName", ""},
            { "CrmDatabaseName", ""},
            { "PathToFederation", @"C:\Program Files\Profisee\Master Data Maestro Integrator\4.0.1\IntegratorCLU.exe"},
            { "RunAsUserName", ""},
            { "ServerRESTVersion", "v1"},
            { "ServerRESTUrl", "http://127.0.0.1/profisee/rest"},
        };

        /// <summary>
        /// Sets the environment variables
        /// </summary>
        public void Set()
        {
            string projectSourcePath = ProjectSourcePath.Value;
            var customVars = this.ParseCustomVars(@$"{projectSourcePath}\local\customVars.json");

            foreach(var customVar in customVars)
            {
                this.envVars[customVar.Key] = customVar.Value;
            }

            this.AddDerivedEnvVars();
            foreach(var customVar in customVars)
            {
                this.envVars[customVar.Key] = customVar.Value;
            }

            foreach(var envVar in this.envVars)
            {
                this.SetEnvironmentVariable(envVar.Key, envVar.Value, EnvironmentVariableTarget.User);
            }
        }

        public Dictionary<string, string> ParseCustomVars(string file)
        {
            Dictionary<string, string>? vars;
            using (StreamReader r = new StreamReader(file))
            {
                string json = r.ReadToEnd();
                vars = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }

            return vars ?? new Dictionary<string, string>();
        }

        public void AddDerivedEnvVars()
        {
            var TfsBaseDir = $"{this.envVars["TfsDrive"]}{this.TfsBaseDirPath}";
            this.envVars["gitRepos"] = @$"{TfsBaseDir}\Repos";
            this.envVars["TfsSrc"] = @$"{this.envVars["gitRepos"]}\platform";
            this.envVars["ScriptsFolder"] = @$"{this.envVars["TfsSrc"]}\Scripts";
            this.envVars["BatchFileLocation"] = @$"{this.envVars["ScriptsFolder"]}\script_files";
            this.envVars["LicenseFile"] = @$"{this.envVars["BatchFileLocation"]}\Prof_2020r1_1Inst_3Nodes_OnlyProfiseeConnector_Production_@.corp.profisee.com.plic";
            //this.envVars["ScriptsDrive"] = this.envVars["TfsDrive"];
            this.envVars["TfsProto"] = @$"{TfsBaseDir}\Prototypes";
            this.envVars["TfsSdk"] = @$"{this.envVars["gitRepos"]}\sdk";
            this.envVars["AutomationFolder"] = @$"{this.envVars["TfsSrc"]}\Testing\Automation";
            this.envVars["UtilPath"] = @$"{this.envVars["TfsSrc"]}\Common\Utilities\bin\Debug";
            this.envVars["MaestroSvc"] = $"Profisee {this.envVars["MaestroVersion"]} ({this.envVars["MaestroWebAppName"]})";
            this.envVars["MaestroSnap"] = $"{this.envVars["MaestroDb"]}_Snapshot";
            this.envVars["TfsGetPlatformSource"] = $"{this.envVars["TfsGetSource"]}Platform";
            this.envVars["ServicesPublishPath"] = @$"{this.envVars["TfsSrc"]}\Server\Services";
            this.envVars["PathToUtilitiesExe"] = @$"{this.envVars["UtilPath"]}\Profisee.MasterDataMaestro.Utilities.exe";
        }

        public virtual string? GetEnvVar(string variable)
        {
            return Environment.GetEnvironmentVariable(variable);
        }

        public virtual void SetEnvironmentVariable(string variable, string value, EnvironmentVariableTarget envVarTarget)
        {
            Environment.SetEnvironmentVariable(variable, value, envVarTarget);
        }
    }
}
