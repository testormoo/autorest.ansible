﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRest.Ansible.Model
{
    /// <summary>
    /// MapAnsible contains all necessary information to generate ansible modules.
    /// Information comes from 2 sources:
    ///  - REST API Specification
    ///  - Applied external tweaks
    ///  This class is designed to be easily serialised to JSON file.
    ///  Tweaks can be then applied to that JSON file and it can be easily merged with next version of autogenerated MapAnsible
    /// </summary>

    public class ModuleOption
    {
        public ModuleOption(string name, string type, string required, string variableValue)
        {
            Name = name; NameAlt = name; Type = type; Required = required; VariableValue = variableValue;
            RequiredCount = 0;
            SubOptions = null;
            IsList = false;
            Disposition = "default";
            DefaultValue = null;
            DefaultValueSample = new Dictionary<string, string>();
            DefaultValueSample["default"] = NameAlt;
            DefaultValueSample["test:default"] = "";
            NoLog = false;
            ValueIfFalse = null;
            ValueIfTrue = null;
            AdditionalInfo = null;
            IncludeInDocumentation = true;
            IncludeInArgSpec = true;
            DocumentationMarkKeywords = true;
        }

        public string Name { get; set; }
        public string NameAlt { get; set; }

        // For first level options:
        //  "default" - module variable will be created
        //  "dictionary" - option is hidden (doesn't appear on list of parameters) and is used to store flattened level values
        //  [dictionary name] - option is flattened from second level and should be stored in [dictionary name]
        public string Disposition { get; set; }
        public string Type { get; set; }
        public string ValueIfFalse { get; set; }
        public string ValueIfTrue { get; set; }
        public bool IsList { get; set; }
        public string Required { get; set; }
        public string VariableValue { get; set; }
        public string Documentation { get; set; }
        public bool DocumentationMarkKeywords { get; set; }
        public string DefaultValue { get; set; }
        public Dictionary<string, string> DefaultValueSample { get; set; }
        public ModuleOption[] SubOptions { get; set; }
        public int RequiredCount { get; set; }
        public bool NoLog { get; set; }
        public KeyValuePair<string,string>[] EnumValues { get; set; }
        public bool IncludeInDocumentation { get; set; }
        public bool IncludeInArgSpec { get; set; }
        public string AdditionalInfo { get; set; }
        public string UpdateRule { get; set; }
    }

    public class ModuleResponseField
    {
        public ModuleResponseField(string name, string type, string description, string sampleValue)
        {
            Name = name; NameAlt = "x"; Type = type;
            SubFields = null; // if dictionary or list of dictionaries
            Description = description;
            SampleValue = sampleValue;
            Collapsed = false;
        }

        public string Name { get; set; }
        public string NameAlt { get; set; }

        public string Description { get; set; }
        public string Type { get; set; }

        public string Returned { get; set; }
        public string SampleValue { get; set; }
        public ModuleResponseField[] SubFields { get; set; }

        public string Info { get; set; }
        public bool Collapsed { get; set; }
    }

    public class ModuleMethod
    {
        public string Name { get; set; }

        public string[] Options { get; set; }
        public string[] RequiredOptions { get; set; }
    }

    public class UpdateComparisonRule
    {
        public UpdateComparisonRule(string[] optionPath, string[] returnPath)
        {
            Option = optionPath;
            ReturnField = returnPath;
        }
        public string[] Option { get; set; }
        public string[] ReturnField { get; set; }
    }

    public class MapAnsibleModule
    {
        public MapAnsibleModule()
        {
            NeedsDeleteBeforeUpdate = false;
            NeedsForceUpdate = false;
            CannotTestUpdate = true;
            AdditionalSampleLines = new List<string>().ToArray();
            VersionAdded = "2.8";
            YearAdded = "2018";
            Author = "Zim Kalinowski";
            AuthorEmail = "zikalino@microsoft.com";
            AuthorIRC = "@zikalino";
            MgmtClient = null;
        }

        public string ModuleName { get; set; }
        public string ModuleNameAlt { get; set; }
        public ModuleOption[] Options { get; set; }
        public ModuleMethod[] Methods { get; set; }
        public ModuleResponseField[] ResponseFields { get; set; }
        public string TestPrerequisitesModule { get; set; }
        public string[] TestPrerequisites { get; set; }
        public string[] TestPostrequisites { get; set; }
        public string AssertStateVariable { get; set; }
        public string AssertStateExpectedValue { get; set; }
        public string ModuleOperationNameUpper { get; set; }
        public string ModuleOperationName { get; set; }
        public string ObjectName { get; set; }
        public string ObjectNamePlural { get; set; }
        public string ResourceNameFieldInRequest { get; set; }
        public string ResourceNameFieldInResponse { get; set; }
        public string MgmtClient { get; set; }

        public string VersionAdded { get; set; }
        public string YearAdded { get; set; }
        public string Author { get; set; }
        public string AuthorEmail { get; set; }
        public string AuthorIRC { get; set; }

        public bool NeedsDeleteBeforeUpdate { get; set; }
        public bool NeedsForceUpdate { get; set; }

        public bool CannotTestUpdate { get; set; }

        public UpdateComparisonRule[] UpdateComparisonRules { get; set; }
        
        public string TestReplaceStringFrom { get; set; }
        public string TestReplaceStringTo { get; set; }

        public string[] AdditionalSampleLines { get; set; }
    }

    public class MapAnsible
    {
        public MapAnsible()
        {
            Info = new List<string>();
        }

        public MapAnsibleModule[] Modules { get; set; }

        public string Namespace { get; set; }
        public string NamespaceUpper { get; set; }
        public string Name { get; set; }
        public string ApiVersion { get; set; }
        //public string[] Operations { get; set; }

        public List<string> Info { get; set; }
    }
}
