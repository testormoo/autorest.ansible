// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoRest.Core;
using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using AutoRest.Ansible;
using AutoRest.Ansible.Model;
using AutoRest.Ansible.vanilla.Templates;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AutoRest.Ansible
{
    public class CodeGeneratorPy : CodeGenerator
    {
        private const string ClientRuntimePackage = "msrest version 0.4.0";

        public CodeGeneratorPy()
        {
        }

        public override string UsageInstructions => $"The {ClientRuntimePackage} pip package is required to execute the generated code.";

        public override string ImplementationFileExtension => ".py";

        /// <summary>
        /// Generate Python client code for given ServiceClient.
        /// </summary>
        /// <param name="serviceClient"></param>
        /// <returns></returns>
        public override async Task Generate(CodeModel cm)
        {
            MapAnsible map = null;
            string[] report = null;

            var codeModel = cm as CodeModelAnsible;
            if (codeModel == null)
            {
                throw new Exception("Code model is not a Python Code Model");
            }

            string oldMapPath = Path.Combine("template", "azure_rm_" + codeModel.Namespace + ".template.json");

            if (File.Exists(oldMapPath))
            {
                using (var streamReader = new StreamReader(oldMapPath, Encoding.UTF8))
                {
                    string text = streamReader.ReadToEnd();

                    var oldMap = JsonConvert.DeserializeObject<MapAnsible>(text);

                    var merger = new MapAnsibleMerger(oldMap, codeModel.Map);
                    map = merger.MergedMap;
                    report = merger.Report;
                }
            }
            else
            {
                string[] empty = { "NOT SOURCE TEMPLATE" };
                map = codeModel.Map;
                report = empty;
            }

            // apply tweaks
            foreach (var tweak in Tweaks.All)
            {
                tweak.Apply(map);
            }

            CodeModelAnsibleMap codeModelPure = null;

            for (int idx = 0; idx < codeModel.Map.Modules.Length; idx++)
            {
                codeModelPure = new CodeModelAnsibleMap(map, report, idx);

                bool isFacts = codeModelPure.ModuleName.EndsWith("_facts");
                ITemplate ansibleTemplate = new AnsibleTemplate { Model = codeModelPure };
                ITemplate ansibleTemplateFacts = new AnsibleFactsTemplate { Model = codeModelPure };
                ITemplate aliasesTemplate = new AliasesTemplate { Model = codeModelPure };
                ITemplate metaMainYmlTemplate = new MetaMainYmlTemplate { Model = codeModelPure };
                ITemplate tasksMainYmlTemplate = new TasksMainYmlTemplate { Model = codeModelPure };
                ITemplate tasksMainYmlFactsTemplate = new TasksMainYmlFactsTemplate { Model = codeModelPure };

                await Write((isFacts ? ansibleTemplateFacts : ansibleTemplate), Path.Combine("all", "modules", codeModelPure.ModuleNameAlt + ".py"));
                await WriteWithLf(aliasesTemplate, Path.Combine("all", "tests", codeModelPure.ModuleNameAlt, "aliases"));
                await WriteWithLf(metaMainYmlTemplate, Path.Combine("all", "tests", codeModelPure.ModuleNameAlt, "meta", "main.yml"));
                await WriteWithLf((isFacts ? tasksMainYmlFactsTemplate : tasksMainYmlTemplate), Path.Combine("all", "tests", codeModelPure.ModuleNameAlt, "tasks", "main.yml"));

                string status = codeModelPure.GetModuleReleaseStatus();

                if (status.Contains('R'))
                {
                    await Write((isFacts ? ansibleTemplateFacts : ansibleTemplate), Path.Combine("role", "modules", codeModelPure.ModuleNameAlt + ".py"));
                    await WriteWithLf(aliasesTemplate, Path.Combine("role", "tests", codeModelPure.ModuleNameAlt, "aliases"));
                    await WriteWithLf(metaMainYmlTemplate, Path.Combine("role", "tests", codeModelPure.ModuleNameAlt, "meta", "main.yml"));
                    await WriteWithLf((isFacts ? tasksMainYmlFactsTemplate : tasksMainYmlTemplate), Path.Combine("role", "tests", codeModelPure.ModuleNameAlt, "tasks", "main.yml"));
                }

                if (status.Contains('P'))
                {
                    await Write((isFacts ? ansibleTemplateFacts : ansibleTemplate), Path.Combine("prs", "modules", codeModelPure.ModuleNameAlt + ".py"));
                    await WriteWithLf(aliasesTemplate, Path.Combine("prs", "tests", codeModelPure.ModuleNameAlt, "aliases"));
                    await WriteWithLf(metaMainYmlTemplate, Path.Combine("prs", "tests", codeModelPure.ModuleNameAlt, "meta", "main.yml"));
                    await WriteWithLf((isFacts ? tasksMainYmlFactsTemplate : tasksMainYmlTemplate), Path.Combine("prs", "tests", codeModelPure.ModuleNameAlt, "tasks", "main.yml"));
                }
            }

            if (codeModelPure != null)
            {
                var ansibleInfo = new AnsibleInfoTemplate { Model = codeModelPure };
                await WriteWithLf(ansibleInfo, Path.Combine("template", "azure_rm_" + codeModel.Namespace + ".template.json"));

                var ansibleMergeInfo = new AnsibleMergeReportTemplate { Model = codeModelPure };
                await WriteWithLf(ansibleMergeInfo, Path.Combine("template", "azure_rm_" + codeModel.Namespace + ".merge.txt"));
            }
        }

        public static string BuildSummaryAndDescriptionString(string summary, string description)
        {
            StringBuilder builder = new StringBuilder();
            if (!string.IsNullOrEmpty(summary))
            {
                if (!summary.EndsWith(".", StringComparison.OrdinalIgnoreCase))
                {
                    summary += ".";
                }
                builder.AppendLine(summary);
            }

            if (!string.IsNullOrEmpty(summary) && !string.IsNullOrEmpty(description))
            {
                builder.AppendLine(TemplateConstants.EmptyLine);
            }

            if (!string.IsNullOrEmpty(description))
            {
                if (!description.EndsWith(".", StringComparison.OrdinalIgnoreCase))
                {
                    description += ".";
                }
                builder.Append(description);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Writes a template into the specified relative path.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public virtual async Task WriteWithLf(ITemplate template, string fileName)
        {
            template.Settings = Settings.Instance;
            var stringBuilder = new StringBuilder();
            using (template.TextWriter = new StringWriter(stringBuilder))
            {
                await template.ExecuteAsync().ConfigureAwait(false);
            }
            await WriteWithLf(stringBuilder.ToString(), fileName, true);
        }

        /// <summary>
        /// Writes a template string into the specified relative path.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="fileName"></param>
        /// <param name="skipEmptyLines"></param>
        /// <returns></returns>
        public async Task WriteWithLf(string content, string fileName, bool skipEmptyLines = false)
        {
            // Make sure the directory exist
            Settings.Instance.FileSystemOutput.CreateDirectory(Path.GetDirectoryName(fileName));

            var lineEnding = "\n";

            using (StringReader streamReader = new StringReader(content))
            using (TextWriter textWriter = Settings.Instance.FileSystemOutput.GetTextWriter(fileName))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    // remove any errant line endings, and trim whitespace from the end too.
                    line = line.Replace("\r", "").Replace("\n", "").TrimEnd(' ', '\r', '\n', '\t');

                    if (line.Contains(TemplateConstants.EmptyLine))
                    {
                        await textWriter.WriteAsync(lineEnding);
                    }
                    else if (!skipEmptyLines || !string.IsNullOrWhiteSpace(line))
                    {
                        await textWriter.WriteAsync(line);
                        await textWriter.WriteAsync(lineEnding);
                    }
                }
            }
        }


    }
}
