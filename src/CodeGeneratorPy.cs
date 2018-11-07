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
using AutoRest.Ansible.Templates;
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

            if (codeModel.SelectFirstExample())
            {
                do
                {
                    ITemplate restTemplate = new AnsibleRestTemplate { Model = codeModel };
                    await WriteWithLf(restTemplate, Path.Combine("examples", codeModel.Namespace + "_" + codeModel.GetExampleName().Replace(' ', '_').Replace('/', '_').Replace('-', '_').Replace("$", "").Replace(".", "").Replace(",", "").ToLower() + ".yml"));
                } while (codeModel.SelectNextExample());
            }

            string[] empty = { "NOT SOURCE TEMPLATE" };
            map = codeModel.Map;
            report = empty;

            // apply all built in tweaks
            foreach (var tweak in Tweaks.All)
            {
                tweak.Apply(map);
                if (tweak.log != null)
                {
                    map.Info.Add("TWEAK: " + tweak.log);
                }
            }

            // TODO: load additional tweaks from file
            map.Info.Add("CURRENT DIRECTORY: " + System.IO.Directory.GetCurrentDirectory());

            map.Info.Add("--------------------- APPLYING TWEAKS FROM FILE.....");

            string currentLine = "";
            try
            {
                string[] lines = File.ReadLines("./tweaks/azure_rm_" + codeModel.Namespace + ".metadata.yml").ToArray();
                string module = null;
                foreach (var l in lines)
                {
                    currentLine = l;
                    if (l.StartsWith("- ") && l.EndsWith(":"))
                    {
                        module = l.Substring(2, l.Length - 3);
                    }
                    else if (l.StartsWith("    - "))
                    {
                        if (module != null)
                        {
                            int position = l.IndexOf(':');

                            if (position < 0) position = l.Length;

                            if (position > 0)
                            {
                                string tweakName = l.Substring(6, position - 6).Trim();
                                string tweakValue = (position < l.Length - 1) ?  l.Substring(position + 1).Trim() : "";
                                if (tweakValue.Length > 0 &&  tweakValue[0] == '"')
                                {
                                    if (tweakValue.Length > 2)
                                        tweakValue = tweakValue.Substring(1, tweakValue.Length - 2);
                                    else
                                        tweakValue = "";
                                }

                                try
                                {
                                    var tweak = Tweak.CreateTweak(module, tweakName, tweakValue);
                                    if (tweak != null)
                                    {
                                        if (!tweak.Apply(map))
                                        {
                                            map.Info.Add("TWEAK NOT APPLIED: " + l);
                                        }
                                    }
                                    else
                                    {
                                            map.Info.Add("TWEAK NOT CREATED: " + l);
                                    }
                                }
                                catch (Exception e)
                                {
                                    map.Info.Add("COULDN'T CREATE TWEAK");
                                    map.Info.Add(e.ToString());
                                    map.Info.Add(l);
                                }
                            }
                            else
                            {
                                map.Info.Add("NO : FOUND" + l);
                            }
                        }
                        else
                        {
                            map.Info.Add("INVALID METADATA -- NULL MODULE: " + l);
                        }
                    }
                    else
                    {
                        map.Info.Add("INVALID METADATA: " + l);
                    }
                }
            }
            catch (Exception e)
            {
                map.Info.Add("NO TWEAK FILE FOUND: " + "./tweaks/azure_rm_" + codeModel.Namespace + ".metadata.yml");
                map.Info.Add(e.ToString());
                map.Info.Add(currentLine);
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
            }

            if (codeModelPure != null)
            {
                var ansibleMergeInfo = new AnsibleMergeReportTemplate { Model = codeModelPure };
                await WriteWithLf(ansibleMergeInfo, Path.Combine("template", "azure_rm_" + codeModel.Namespace + ".log"));

                codeModelPure.Map.Info = null;

                var ansibleInfo = new AnsibleInfoTemplate { Model = codeModelPure };
                await WriteWithLf(ansibleInfo, Path.Combine("template", "azure_rm_" + codeModel.Namespace + ".template.json"));

                var metadataTemplate = new MetadataTemplateTemplate { Model = codeModelPure };
                await WriteWithLf(metadataTemplate, Path.Combine("template", "azure_rm_" + codeModel.Namespace + ".metadata.yml"));
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
