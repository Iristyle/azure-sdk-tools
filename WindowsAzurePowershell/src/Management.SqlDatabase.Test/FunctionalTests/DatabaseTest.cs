﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------


namespace Microsoft.WindowsAzure.Management.SqlDatabase.Test.FunctionalTests
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Test.Utilities;
    using Microsoft.WindowsAzure.Management.Utilities.Common.XmlSchema;
    using Microsoft.WindowsAzure.Management.Utilities.Common;

    [TestClass]
    public class DatabaseTest
    {
        private string userName;
        private string password;
        private string manageUrl;
        private string subscriptionId;
        private string serializedCert;

        private const string CreateContextScript = @"Database\CreateContext.ps1";
        private const string CreateScript = @"Database\CreateAndGetDatabase.ps1";
        private const string UpdateScript = @"Database\UpdateDatabase.ps1";
        private const string DeleteScript = @"Database\DeleteDatabase.ps1";
        private const string FormatValidationScript = @"Database\FormatValidation.ps1";

        [TestInitialize]
        public void Setup()
        {
            XElement root = XElement.Load("SqlDatabaseSettings.xml");
            this.userName = root.Element("SqlAuthUserName").Value;
            this.password = root.Element("SqlAuthPassword").Value;
            this.manageUrl = root.Element("ManageUrl").Value;


            PublishData publishData = General.DeserializeXmlFile<PublishData>("Azure.publishsettings");
            PublishDataPublishProfile publishProfile = publishData.Items[0];
            this.serializedCert = publishProfile.ManagementCertificate;
            this.subscriptionId = publishProfile.Subscription[0].Id;
        }

        [TestMethod]
        [TestCategory("Functional")]
        public void CreateContext()
        {
            string arguments = string.Format(
                CultureInfo.InvariantCulture,
                "-ManageUrl \"{0}\" -UserName \"{1}\" -Password \"{2}\" -SubscriptionId \"{3}\" -SerializedCert \"{4}\" ",
                this.manageUrl,
                this.userName,
                this.password,
                this.subscriptionId,
                this.serializedCert);
            bool testResult = PSScriptExecutor.ExecuteScript(
                DatabaseTest.CreateContextScript,
                arguments);
            Assert.IsTrue(testResult);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public void CreateDatabase()
        {
            string arguments = string.Format(
                CultureInfo.InvariantCulture,
                "-Name \"{0}\" -ManageUrl \"{1}\" -UserName \"{2}\" -Password \"{3}\"",
                "testcreatedbfromcmdlet",
                this.manageUrl,
                this.userName,
                this.password);
            bool testResult = PSScriptExecutor.ExecuteScript(DatabaseTest.CreateScript, arguments);
            Assert.IsTrue(testResult);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public void UpdateDatabase()
        {
            string arguments = string.Format(
                CultureInfo.InvariantCulture,
                "-Name \"{0}\" -ManageUrl \"{1}\" -UserName \"{2}\" -Password \"{3}\"",
                "testupdatedbfromcmdlet",
                this.manageUrl,
                this.userName,
                this.password);
            bool testResult = PSScriptExecutor.ExecuteScript(DatabaseTest.UpdateScript, arguments);
            Assert.IsTrue(testResult);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public void DeleteDatabase()
        {
            string arguments = string.Format(
                CultureInfo.InvariantCulture,
                "-Name \"{0}\" -ManageUrl \"{1}\" -UserName \"{2}\" -Password \"{3}\"",
                "testDeletedbfromcmdlet",
                this.manageUrl,
                this.userName,
                this.password);
            bool testResult = PSScriptExecutor.ExecuteScript(DatabaseTest.DeleteScript, arguments);
            Assert.IsTrue(testResult);
        }

        [TestMethod]
        [TestCategory("Functional")]
        public void OutputObjectFormatValidation()
        {
            string outputFile = Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid() + ".txt");
            string arguments = string.Format(
                CultureInfo.InvariantCulture,
                "-Name \"{0}\" -ManageUrl \"{1}\" -UserName \"{2}\" -Password \"{3}\" -OutputFile \"{4}\"",
                "testFormatdbfromcmdlet",
                this.manageUrl,
                this.userName,
                this.password,
                outputFile);
            bool testResult = PSScriptExecutor.ExecuteScript(DatabaseTest.FormatValidationScript, arguments);
            Assert.IsTrue(testResult);

            OutputFormatValidator.ValidateOutputFormat(outputFile, @"Database\ExpectedFormat.txt");
        }
    }
}
