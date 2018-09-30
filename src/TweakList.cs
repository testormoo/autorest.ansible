﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRest.Ansible
{
    public class Tweaks
    {
        public static Tweak[] All =
        {
            new Tweak_Option_Rename("*", "resource_group_name", "resource_group"),
            new Tweak_Option_DefaultValueTest("*", "resource_group_name", "\"{{ resource_group }}\""),
            new Tweak_Option_Required("*", "location", false),
            new Tweak_Response_AddField("*", "id"),
            new Tweak_Response_AddField("*", "status"),
            new Tweak_Response_AddField("*", "fully_qualified_domain_name"),
            new Tweak_Response_AddField("*", "version"),
            new Tweak_Response_AddField("*", "state"),
            new Tweak_Response_AddField("*", "user_visible_state"),
            new Tweak_Option_Documentation("*", "location", "Resource location. If not set, location from the resource group will be used as default."),
            //new Tweak_Option_DefaultValueTest("*", "location", "eastus"),
            new Tweak_Option_DefaultValueSample("*", "location", "eastus"),

            // SQL Server
            new Tweak_Option_Rename("azure_rm_sqlserver", "identity.type", "identity"),
            new Tweak_Option_DocumentationAppend("azure_rm_sqlserver", "version", " For example '12.0'."),
            new Tweak_Option_DefaultValueSample("azure_rm_sqlserver", "resource_group_name", "resource_group"),
            new Tweak_Option_DefaultValueSample("azure_rm_sqlserver", "location", "westus"),
            new Tweak_Option_DefaultValueSample("azure_rm_sqlserver", "administrator_login", "mylogin"),
            new Tweak_Option_DefaultValueSample("azure_rm_sqlserver", "administrator_login_password", "Testpasswordxyz12!"),
            new Tweak_Option_DefaultValueSample("azure_rm_sqlserver", "tags", ""),
            new Tweak_Option_DefaultValueSample("azure_rm_sqlserver", "identity", ""),
            new Tweak_Option_DefaultValueSample("azure_rm_sqlserver", "identity.type", ""), // XXX - this is a bug
            new Tweak_Option_DefaultValueSample("azure_rm_sqlserver", "version", ""),
            new Tweak_Response_FieldSampleValue("azure_rm_sqlserver", "id", "/subscriptions/00000000-1111-2222-3333-444444444444/resourceGroups/sqlcrudtest-7398/providers/Microsoft.Sql/servers/sqlcrudtest-4645"),
            new Tweak_Response_FieldSampleValue("azure_rm_sqlserver", "version", "12.0"),
            new Tweak_Response_FieldSampleValue("azure_rm_sqlserver", "fully_qualified_domain_name", "sqlcrudtest-4645.database.windows.net"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlserver", "administrator_login", "mylogin"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlserver", "administrator_login_password", "Testpasswordxyz12!"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlserver", "location", "eastus"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlserver", "server_name", "sqlsrv{{ rpfx }}"),
            new Tweak_Option_Collapse("azure_rm_sqlserver", "identity", ""),
            new Tweak_Module_AssertStateVariable("azure_rm_sqlserver", "state"),
            new Tweak_Module_AssertStateExpectedValue("azure_rm_sqlserver", "Ready"),

            // SQL Server Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sqlserver_facts", "azure_rm_sqlserver", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlserver_facts", "server_name", "sqlsrv{{ rpfx }}"),

            // SQL Database
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sqldatabase", "azure_rm_sqlserver", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_sqldatabase", "server_name", "sqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqldatabase", "database_name", "database{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqldatabase", "location", "eastus"),
            new Tweak_Option_Exclude("azure_rm_sqldatabase", "requested_service_objective_id", true, true),
            new Tweak_Option_Exclude("azure_rm_sqldatabase", "requested_service_objective_name", true, true),
            new Tweak_Option_MakeBoolean("azure_rm_sqldatabase", "zone_redundant", "Enabled", "Disabled", false, "Is this database is zone redundant? It means the replicas of this database will be spread across multiple availability zones."),
            new Tweak_Option_MakeBoolean("azure_rm_sqldatabase", "read_scale", "Enabled", "Disabled", false, "If the database is a geo-secondary, readScale indicates whether read-only connections are allowed to this database or not. Not supported for DataWarehouse edition."),
            new Tweak_Response_AddField("azure_rm_sqldatabase", "database_id"),
            new Tweak_Module_AssertStateVariable("azure_rm_sqldatabase", "status"),
            new Tweak_Module_AssertStateExpectedValue("azure_rm_sqldatabase", "Online"),

            new Tweak_Module_AddUpdateRule("azure_rm_sqldatabase", "parameters['location']", "location"),
            new Tweak_Module_AddUpdateRule("azure_rm_sqldatabase", "parameters['read_scale']", "read_scale"),
            //new Tweak_Module_AddUpdateRule("azure_rm_sqldatabase", "parameters['requested_service_objective_id']", "requested_service_objective_id"),
            //new Tweak_Module_AddUpdateRule("azure_rm_sqldatabase", "parameters['requested_service_objective_name']", "requested_service_objective_name"),
            new Tweak_Module_AddUpdateRule("azure_rm_sqldatabase", "parameters['max_size_bytes']", "max_size_bytes"),
            new Tweak_Module_AddUpdateRule("azure_rm_sqldatabase", "parameters['edition']", "edition"),
            new Tweak_Module_NeedsForceUpdate("azure_rm_sqldatabase"),

            // SQL Database Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sqldatabase_facts", "azure_rm_sqldatabase", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_sqldatabase_facts", "server_name", "sqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqldatabase_facts", "database_name", "database{{ rpfx }}"),

            // SQL Elastic Pool
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sqlelasticpool", "azure_rm_sqlserver", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlelasticpool", "server_name", "sqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlelasticpool", "elastic_pool_name", "elasticpool{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlelasticpool", "location", "eastus"),

            // SQL Elastic Pool Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sqlelasticpool_facts", "azure_rm_sqlelasticpool", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlelasticpool_facts", "server_name", "sqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlelasticpool_facts", "elastic_pool_name", "elasticpool{{ rpfx }}"),
            new Tweak_Response_FieldReturned("azure_rm_sqlelasticpool_facts", "kind", ""),

            // SQL Server Firewall Rule
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sqlfirewallrule", "azure_rm_sqlserver", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlfirewallrule", "server_name", "sqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlfirewallrule", "firewall_rule_name", "firewallrule{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlfirewallrule", "start_ip_address", "172.28.10.136"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlfirewallrule", "end_ip_address", "172.28.10.138"),
            new Tweak_Module_FlattenParametersDictionary("azure_rm_sqlfirewallrule"),
            //new Tweak_Module_AssertStateVariable("azure_rm_sqlfirewallrule", "firewall_rule_name"),
            //new Tweak_Module_AssertStateExpectedValue("azure_rm_sqlfirewallrule", "firewallrule{{ rpfx }}"),
            new Tweak_Module_AddUpdateRule("azure_rm_sqlfirewallrule", "start_ip_address", "start_ip_address"),
            new Tweak_Module_AddUpdateRule("azure_rm_sqlfirewallrule", "end_ip_address", "end_ip_address"),

            // SQL Firewall Rule Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sqlfirewallrule_facts", "azure_rm_sqlfirewallrule", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlfirewallrule_facts", "server_name", "sqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlfirewallrule_facts", "firewall_rule_name", "firewallrule{{ rpfx }}"),

            // SQL Geo Backup Policy
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sqlgeobackuppolicy", "azure_rm_sqldatabase", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlgeobackuppolicy", "server_name", "sqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlgeobackuppolicy", "database_name", "database{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlgeobackuppolicy", "geo_backup_policy_name", "geo-policy-name"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlgeobackuppolicy", "state", "\"enabled\""),

            new Tweak_Module_TestPrerequisitesModule("azure_rm_sqlgeobackuppolicy_facts", "azure_rm_sqlgeobackuppolicy", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlgeobackuppolicy_facts", "server_name", "sqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlgeobackuppolicy_facts", "database_name", "database{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlgeobackuppolicy_facts", "geo_backup_policy_name", "geo-policy-name"),


            // MySQL Server
            new Tweak_Option_Exclude("azure_rm_mysqlserver", "properties.create_mode", true, false),
            new Tweak_Option_MakeBoolean("azure_rm_mysqlserver", "properties.ssl_enforcement", "Enabled", "Disabled", false, "Enable SSL enforcement."),
            new Tweak_Option_Required("azure_rm_mysqlserver", "properties", false),
            new Tweak_Option_Exclude("azure_rm_mysqlserver", "sku.family", true, true),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlserver", "server_name", "mysqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlserver", "properties.version", "5.6"),
            new Tweak_Option_DefaultValue("azure_rm_mysqlserver", "properties.create_mode", "'Default'"),
            new Tweak_Option_Documentation("azure_rm_mysqlserver", "properties.create_mode", "Currently only 'Default' value supported"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlserver", "properties.administrator_login", "zimxyz"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlserver", "properties.administrator_login_password", "Testpasswordxyz12!"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlserver", "location", "westus"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlserver", "sku.name", "MYSQLB50"),
            new Tweak_Option_DefaultValueSample("azure_rm_mysqlserver", "sku.name", "MYSQLB50"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlserver", "sku.tier", "basic"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlserver", "properties.ssl_enforcement", "True"),
            new Tweak_Option_DefaultValueSample("azure_rm_mysqlserver", "properties.ssl_enforcement", "True"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlserver", "properties.storage_mb", "51200"),
            new Tweak_Option_Collapse("azure_rm_mysqlserver", "properties", ""),
            new Tweak_Response_FieldSampleValue("azure_rm_mysqlserver", "fully_qualified_domain_name", "mysqlsrv1b6dd89593.mysql.database.azure.com"),
            new Tweak_Response_FieldSampleValue("azure_rm_mysqlserver", "id", "/subscriptions/12345678-1234-1234-1234-123412341234/testrg/providers/Microsoft.DBforMySQL/servers/mysqlsrv1b6dd89593"),
            new Tweak_Response_FieldSampleValue("azure_rm_mysqlserver", "user_visible_state", "Ready"),
            new Tweak_Response_FieldSampleValue("azure_rm_mysqlserver", "version", "5.6"),
            new Tweak_Module_AssertStateVariable("azure_rm_mysqlserver", "state"),
            new Tweak_Module_AssertStateExpectedValue("azure_rm_mysqlserver", "Ready"),

            // MySQL Server Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqlserver_facts", "azure_rm_mysqlserver", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlserver_facts", "server_name", "mysqlsrv{{ rpfx }}"),

            // MySQL Database
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqldatabase", "azure_rm_mysqlserver", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqldatabase", "server_name", "mysqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqldatabase", "database_name", "testdatabase"),
            new Tweak_Module_AddUpdateRule("azure_rm_mysqldatabase", "parameters['collation']", "collation"),
            new Tweak_Module_AddUpdateRule("azure_rm_mysqldatabase", "parameters['charset']", "charset"),
            new Tweak_Option_DocumentationAppend("azure_rm_mysqldatabase", "collation", " Check MySQL documentation for possible values."),
            new Tweak_Option_DocumentationAppend("azure_rm_mysqldatabase", "charset", " Check MySQL documentation for possible values."),
            new Tweak_Response_AddField("azure_rm_mysqldatabase", "name"),
            new Tweak_Module_AssertStateVariable("azure_rm_mysqldatabase", "name"),
            new Tweak_Module_AssertStateExpectedValue("azure_rm_mysqldatabase", "testdatabase"),
            new Tweak_Module_NeedsDeleteBeforeUpdate("azure_rm_mysqldatabase"),
            new Tweak_Module_NeedsForceUpdate("azure_rm_mysqldatabase"),

            // MySQL Database Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqldatabase_facts", "azure_rm_mysqldatabase", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqldatabase_facts", "server_name", "mysqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqldatabase_facts", "database_name", "testdatabase"),

            // MySQL Server Firewall Rule
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqlfirewallrule", "azure_rm_mysqlserver", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlfirewallrule", "server_name", "mysqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlfirewallrule", "firewall_rule_name", "firewallrule{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlfirewallrule", "start_ip_address", "172.28.10.136"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlfirewallrule", "end_ip_address", "172.28.10.138"),
            new Tweak_Module_FlattenParametersDictionary("azure_rm_mysqlfirewallrule"),
            //new Tweak_Module_AssertStateVariable("azure_rm_mysqlfirewallrule", "firewall_rule_name"),
            //new Tweak_Module_AssertStateExpectedValue("azure_rm_mysqlfirewallrule", "firewallrule{{ rpfx }}"),
            new Tweak_Module_AddUpdateRule("azure_rm_mysqlfirewallrule", "start_ip_address", "start_ip_address"),
            new Tweak_Module_AddUpdateRule("azure_rm_mysqlfirewallrule", "end_ip_address", "end_ip_address"),

            // MySQL Firewall Rule Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqlfirewallrule_facts", "azure_rm_mysqlfirewallrule", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlfirewallrule_facts", "server_name", "mysqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlfirewallrule_facts", "firewall_rule_name", "firewallrule{{ rpfx }}"),

            // MySQL Server Configuration
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqlconfiguration", "azure_rm_mysqlserver", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlconfiguration", "server_name", "mysqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlconfiguration", "configuration_name", "event_scheduler"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlconfiguration", "value", "\"ON\""),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlconfiguration", "source", "user-override"),
            new Tweak_Module_FlattenParametersDictionary("azure_rm_mysqlconfiguration"),
            //new Tweak_Module_AssertStateVariable("azure_rm_mysqlconfiguration", "value"),
            //new Tweak_Module_AssertStateExpectedValue("azure_rm_mysqlconfiguration", "ON"),

            // MySQL Configuration Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqlconfiguration_facts", "azure_rm_mysqlconfiguration", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlconfiguration_facts", "server_name", "mysqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlconfiguration_facts", "configuration_name", "event_scheduler"),

            // MySQL Server Virtual Network Rule
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqlvirtualnetworkrule", "azure_rm_mysqlserver", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlvirtualnetworkrule", "server_name", "mysqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlvirtualnetworkrule", "virtual_network_rule_name", "test-virtual-network-rule"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlvirtualnetworkrule", "virtual_network_subnet_id", "/subscriptions/{{ azure_subscription_id }}/resourceGroups/{{ resource_group }}/providers/Microsoft.Network/virtualNetworks/vnet{{ rpfx }}/subnets/subnet{{ rpfx }}"),
            new Tweak_Module_FlattenParametersDictionary("azure_rm_mysqlvirtualnetworkrule"),
            new Tweak_Module_TestPrerequisites("azure_rm_mysqlvirtualnetworkrule",
                                                new string[] {
                                                    "- name: Create a virtual network",
                                                    "  azure_rm_virtualnetwork:",
                                                    "    name: vnet{{ rpfx }}",
                                                    "    resource_group: \"{{ resource_group }}\"",
                                                    "    address_prefixes_cidr:",
                                                    "        - 10.1.0.0/16",
                                                    "        - 172.100.0.0/16",
                                                    "    dns_servers:",
                                                    "        - 127.0.0.1",
                                                    "        - 127.0.0.2",
                                                    "",
                                                    "- name: Create a subnet",
                                                    "  azure_rm_subnet:",
                                                    "    name: subnet{{ rpfx }}",
                                                    "    virtual_network_name: vnet{{ rpfx }}",
                                                    "    resource_group: \"{{ resource_group }}\"",
                                                    "    address_prefix_cidr: 10.1.0.0/24" },
                                                new string[] { }), // XXX - do not try to delete as causing internal server error
                                                    //"- name: Remove subnet",
                                                    //"  azure_rm_subnet:",
                                                    //"    name: subnet{{ rpfx }}",
                                                    //"    virtual_network_name: vnet{{ rpfx }}",
                                                    //"    resource_group: \"{{ resource_group }}\"",
                                                    //"    state: absent",
                                                    //"",
                                                    //"- name: Remove virtual network",
                                                    //"  azure_rm_virtualnetwork:",
                                                    //"    name: vnet{{ rpfx }}",
                                                    //"    resource_group: \"{{ resource_group }}\"",
                                                    //"    state: absent" }),

            // MySQL Virtual Network Rule Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqlvirtualnetworkrule_facts", "azure_rm_mysqlvirtualnetworkrule", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlvirtualnetworkrule_facts", "server_name", "mysqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlvirtualnetworkrule_facts", "virtualnetworkrule_name", "test-virtual-network-rule"),

            // MySQL Server LogFile Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqllogfile_facts", "azure_rm_mysqlserver", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqllogfile_facts", "server_name", "mysqlsrv{{ rpfx }}"),

            // PostgreSQL Server
            new Tweak_Option_Exclude("azure_rm_postgresqlserver", "properties.create_mode", true, false),
            new Tweak_Option_MakeBoolean("azure_rm_postgresqlserver", "properties.ssl_enforcement", "Enabled", "Disabled", false, "Enable SSL enforcement."),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlserver", "location", "westus"),
            new Tweak_Option_Required("azure_rm_postgresqlserver", "properties", false),
            new Tweak_Option_Exclude("azure_rm_postgresqlserver", "sku.family", true, true),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlserver", "server_name", "postgresqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlserver", "properties.administrator_login", "zimxyz"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlserver", "properties.administrator_login_password", "Testpasswordxyz12!"),
            new Tweak_Option_DefaultValue("azure_rm_postgresqlserver", "properties.create_mode", "'Default'"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlserver", "sku.name", "PGSQLS100"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlserver", "sku.tier", "basic"),
            new Tweak_Option_DefaultValueSample("azure_rm_postgresqlserver", "sku.name", "PGSQLS100"),
            new Tweak_Option_DefaultValueSample("azure_rm_postgresqlserver", "properties.ssl_enforcement", "True"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlserver", "properties.ssl_enforcement", "True"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlserver", "properties.storage_mb", "51200"),
            new Tweak_Option_Documentation("azure_rm_postgresqlserver", "properties.create_mode", "Currently only 'Default' value supported"),
            new Tweak_Option_Collapse("azure_rm_postgresqlserver", "properties", ""),
            new Tweak_Response_FieldSampleValue("azure_rm_postgresqlserver", "fully_qualified_domain_name", "postgresqlsrv1b6dd89593.postgresql.database.azure.com"),
            new Tweak_Response_FieldSampleValue("azure_rm_postgresqlserver", "id", "/subscriptions/12345678-1234-1234-1234-123412341234/resourceGroups/samplerg/providers/Microsoft.DBforPostgreSQL/servers/mysqlsrv1b6dd89593"),
            new Tweak_Response_FieldSampleValue("azure_rm_postgresqlserver", "user_visible_state", "Ready"),
            new Tweak_Response_FieldSampleValue("azure_rm_postgresqlserver", "version", "9.6"),
            new Tweak_Module_AssertStateVariable("azure_rm_postgresqlserver", "state"),
            new Tweak_Module_AssertStateExpectedValue("azure_rm_postgresqlserver", "Ready"),

            // PostgreSQL Server Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqlserver_facts", "azure_rm_postgresqlserver", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlserver_facts", "server_name", "postgresqlsrv{{ rpfx }}"),

            // PostgreSQL Database
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqldatabase", "azure_rm_postgresqlserver", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqldatabase", "server_name", "postgresqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqldatabase", "database_name", "testdatabase"),
            new Tweak_Module_AddUpdateRule("azure_rm_postgresqldatabase", "parameters['collation']", "collation"),
            new Tweak_Module_AddUpdateRule("azure_rm_postgresqldatabase", "parameters['charset']", "charset"),
            new Tweak_Option_DocumentationAppend("azure_rm_postgresqldatabase", "collation", " Check PostgreSQL documentation for possible values."),
            new Tweak_Option_DocumentationAppend("azure_rm_postgresqldatabase", "charset", " Check PostgreSQL documentation for possible values."),
            new Tweak_Response_AddField("azure_rm_postgresqldatabase", "name"),
            new Tweak_Module_AssertStateVariable("azure_rm_postgresqldatabase", "name"),
            new Tweak_Module_AssertStateExpectedValue("azure_rm_postgresqldatabase", "testdatabase"),
            new Tweak_Module_NeedsDeleteBeforeUpdate("azure_rm_postgresqldatabase"),
            new Tweak_Module_NeedsForceUpdate("azure_rm_postgresqldatabase"),

            // PostgreSQL Database Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqldatabase_facts", "azure_rm_postgresqldatabase", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqldatabase_facts", "server_name", "postgresqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqldatabase_facts", "database_name", "testdatabase"),

            // PostgreSQL Server Firewall Rule
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqlfirewallrule", "azure_rm_postgresqlserver", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlfirewallrule", "server_name", "postgresqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlfirewallrule", "firewall_rule_name", "firewallrule{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlfirewallrule", "start_ip_address", "172.28.10.136"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlfirewallrule", "end_ip_address", "172.28.10.138"),
            new Tweak_Module_FlattenParametersDictionary("azure_rm_postgresqlfirewallrule"),
            //new Tweak_Module_AssertStateVariable("azure_rm_postgresqlfirewallrule", "firewall_rule_name"),
            //new Tweak_Module_AssertStateExpectedValue("azure_rm_postgresqlfirewallrule", "firewallrule{{ rpfx }}"),
            new Tweak_Module_AddUpdateRule("azure_rm_postgresqlfirewallrule", "start_ip_address", "start_ip_address"),
            new Tweak_Module_AddUpdateRule("azure_rm_postgresqlfirewallrule", "end_ip_address", "end_ip_address"),

            // PostgreSQL Firewall Rule Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqlfirewallrule_facts", "azure_rm_postgresqlfirewallrule", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlfirewallrule_facts", "server_name", "postgresqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlfirewallrule_facts", "firewall_rule_name", "firewallrule{{ rpfx }}"),

            // PostgreSQL Server Configuration
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqlconfiguration", "azure_rm_postgresqlserver", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlconfiguration", "server_name", "postgresqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlconfiguration", "configuration_name", "deadlock_timeout"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlconfiguration", "value", "2000"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlconfiguration", "source", "user-override"),
            new Tweak_Module_FlattenParametersDictionary("azure_rm_postgresqlconfiguration"),
            //new Tweak_Module_AssertStateVariable("azure_rm_postgresqlconfiguration", "value"),
            //new Tweak_Module_AssertStateExpectedValue("azure_rm_postgresqlconfiguration", "ON"),

            // PostgreSQL Configuration Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqlconfiguration_facts", "azure_rm_postgresqlconfiguration", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlconfiguration_facts", "server_name", "postgresqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlconfiguration_facts", "configuration_name", "deadlock_timeout"),

            // PostgreSQL Server Virtual Network Rule
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqlvirtualnetworkrule", "azure_rm_postgresqlserver", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlvirtualnetworkrule", "server_name", "postgresqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlvirtualnetworkrule", "virtual_network_rule_name", "test-virtual-network-rule"),

            // PostgreSQL Virtual Network Rule Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqlvirtualnetworkrule_facts", "azure_rm_postgresqlvirtualnetworkrule", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlvirtualnetworkrule_facts", "server_name", "postgresqlsrv{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlvirtualnetworkrule_facts", "virtualnetworkrule_name", "test-virtual-network-rule"),

            // Authorization
            new Tweak_Option_DefaultValueTest("azure_rm_authorizationroledefinition", "role_definition_id", "rolexyz"),
            new Tweak_Option_DefaultValueTest("azure_rm_authorizationroledefinition", "scope", "\"/subscriptions/{{ azure_subscription_id }}\""),
            new Tweak_Option_Collapse("azure_rm_authorizationroledefinition", "properties", "properties_"),

            new Tweak_Option_DefaultValueTest("azure_rm_authorizationroleassignment", "scope", "\"/subscriptions/{{ azure_subscription_id }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_authorizationroleassignment", "role_assignment_name", "\"d3881f73-7777-8888-8283-e981cbba0404\""),
            new Tweak_Option_DefaultValueTest("azure_rm_authorizationroleassignment", "properties.role_definition_id", "\"/subscriptions/{{ azure_subscription_id }}/providers/Microsoft.Authorization/roleDefinitions/9980e02c-c2be-4d73-94e8-173b1dc7cf3c\""),
            new Tweak_Option_DefaultValueTest("azure_rm_authorizationroleassignment", "properties.principal_id", "\"98b422c6-7bea-4706-b6f3-920a782746d4\""),
            new Tweak_Module_FlattenParametersDictionary("azure_rm_authorizationroleassignment"),
            //new Tweak_Option_Collapse("azure_rm_authorizationroleassignment", "properties", "properties_"),

            // Application Gateway
            new Tweak_Module_CannotTestUpdate("azure_rm_applicationgateway"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "application_gateway_name", "\"appgateway{{ rpfx }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "sku.name", "Standard_Small"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "sku.tier", "Standard"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "sku.capacity", "2"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "gateway_ip_configurations.name", "app_gateway_ip_config"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "gateway_ip_configurations.subnet.id", "/subscriptions/{{ azure_subscription_id }}/resourceGroups/{{ resource_group }}/providers/Microsoft.Network/virtualNetworks/vnet{{ rpfx }}/subnets/subnet{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "frontend_ip_configurations.name", "sample_gateway_frontend_ip_config"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "frontend_ip_configurations.subnet.id", "/subscriptions/{{ azure_subscription_id }}/resourceGroups/{{ resource_group }}/providers/Microsoft.Network/virtualNetworks/vnet{{ rpfx }}/subnets/subnet{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "frontend_ports.name", "ag_frontend_port"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "frontend_ports.port", "90"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "backend_address_pools.name", "test_backend_address_pool"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "backend_address_pools.backend_addresses.ip_address", "10.0.0.4"),

            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "backend_http_settings_collection.name", "sample_appgateway_http_settings"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "backend_http_settings_collection.port", "80"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "backend_http_settings_collection.protocol", "Http"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "backend_http_settings_collection.cookie_based_affinity", "Enabled"),

            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "http_listeners.name", "sample_http_listener"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "http_listeners.frontend_ip_configuration.id", "/subscriptions/{{ azure_subscription_id }}/resourceGroups/{{ resource_group }}/providers/Microsoft.Network/applicationGateways/appgateway{{ rpfx }}/frontendIPConfigurations/sample_gateway_frontend_ip_config"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "http_listeners.frontend_port.id", "/subscriptions/{{ azure_subscription_id }}/resourceGroups/{{ resource_group }}/providers/Microsoft.Network/applicationGateways/appgateway{{ rpfx }}/frontendPorts/ag_frontend_port"),

            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "request_routing_rules.name", "rule1"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "request_routing_rules.rule_type", "Basic"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "request_routing_rules.http_listener.id", "/subscriptions/{{ azure_subscription_id }}/resourceGroups/{{ resource_group }}/providers/Microsoft.Network/applicationGateways/appgateway{{ rpfx }}/httpListeners/sample_http_listener"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "request_routing_rules.backend_address_pool.id", "/subscriptions/{{ azure_subscription_id }}/resourceGroups/{{ resource_group }}/providers/Microsoft.Network/applicationGateways/appgateway{{ rpfx }}/backendAddressPools/test_backend_address_pool"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway", "request_routing_rules.backend_http_settings.id", "/subscriptions/{{ azure_subscription_id }}/resourceGroups/{{ resource_group }}/providers/Microsoft.Network/applicationGateways/appgateway{{ rpfx }}/backendHttpSettingsCollection/sample_appgateway_http_settings"),

            new Tweak_Module_TestPrerequisites("azure_rm_applicationgateway",
                                                new string[] {
                                                    "- name: Create a virtual network",
                                                    "  azure_rm_virtualnetwork:",
                                                    "    name: vnet{{ rpfx }}",
                                                    "    resource_group: \"{{ resource_group }}\"",
                                                    "    address_prefixes_cidr:",
                                                    "        - 10.1.0.0/16",
                                                    "        - 172.100.0.0/16",
                                                    "    dns_servers:",
                                                    "        - 127.0.0.1",
                                                    "        - 127.0.0.2",
                                                    "",
                                                    "- name: Create a subnet",
                                                    "  azure_rm_subnet:",
                                                    "    name: subnet{{ rpfx }}",
                                                    "    virtual_network_name: vnet{{ rpfx }}",
                                                    "    resource_group: \"{{ resource_group }}\"",
                                                    "    address_prefix_cidr: 10.1.0.0/24" },
                                                new string[] { }), // XXX - do not try to delete as causing internal server error
                                                    //"- name: Remove subnet",
                                                    //"  azure_rm_subnet:",
                                                    //"    name: subnet{{ rpfx }}",
                                                    //"    virtual_network_name: vnet{{ rpfx }}",
                                                    //"    resource_group: \"{{ resource_group }}\"",
                                                    //"    state: absent",
                                                    //"",
                                                    //"- name: Remove virtual network",
                                                    //"  azure_rm_virtualnetwork:",
                                                    //"    name: vnet{{ rpfx }}",
                                                    //"    resource_group: \"{{ resource_group }}\"",
                                                    //"    state: absent" }),

            new Tweak_Option_DefaultValueTest("azure_rm_applicationgateway_facts", "application_gateway_name", "\"appgateway{{ rpfx }}\""),

            // Application Gateway Application Security Group
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgatewayapplicationsecuritygroup", "application_security_group_name", "appgwsg{{ rpfx }}"),

            // Application Gateway Application Security Group Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_applicationgatewayapplicationsecuritygroup_facts", "azure_rm_applicationgatewayapplicationsecuritygroup", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgatewayapplicationsecuritygroup_facts", "application_security_group_name", "appgwsg{{ rpfx }}"),

            // Application Gateway Route Table
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgatewayroutetable", "route_table_name", "routetablename{{ rpfx }}"),

            // Application Gateway Route Table Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_applicationgatewayroutetable_facts", "azure_rm_applicationgatewayroutetable", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgatewayroutetable_facts", "route_table_name", "routetablename{{ rpfx }}"),

            // Application Gateway Route
            new Tweak_Module_TestPrerequisitesModule("azure_rm_applicationgatewayroute", "azure_rm_applicationgatewayroutetable", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgatewayroute", "next_hop_type", "virtual_network_gateway"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgatewayroute", "route_name", "testroute{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgatewayroute", "route_table_name", "routetablename{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgatewayroute", "address_prefix", "208.128.0.0/11"),
            new Tweak_Module_CannotTestUpdate("azure_rm_applicationgatewayroute"),
            new Tweak_Option_Required("azure_rm_applicationgatewayroute", "next_hop_type", false),

            // Application Gateway Route Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_applicationgatewayroute_facts", "azure_rm_applicationgatewayroute", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgatewayroute_facts", "route_name", "testroute{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgatewayroute_facts", "route_table_name", "routetablename{{ rpfx }}"),

            // Application Inbound NAT Rule
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgatewayinboundnatrule", "load_balancer_name", "lb{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgatewayinboundnatrule", "inbound_nat_rule_name", "rule{{ rpfx }}"),

            new Tweak_Module_TestPrerequisites("azure_rm_applicationgatewayinboundnatrule",
                                                new string[] {
                                                    "- name: create public ip",
                                                    "  azure_rm_publicipaddress:",
                                                    "    name: ansiblepip{{ rpfx }}",
                                                    "    resource_group: '{{ resource_group }}'",
                                                    "",
                                                    "- name: create load balancer",
                                                    "  azure_rm_loadbalancer:",
                                                    "    resource_group: '{{ resource_group }}'",
                                                    "    name: lb{{ rpfx }}",
                                                    "    public_ip: ansiblepip{{ rpfx }}" },
                                                new string[] {
                                                    "- name: delete load balancer",
                                                    "  azure_rm_loadbalancer:",
                                                    "    resource_group: '{{ resource_group }}'",
                                                    "    name: lb{{ rpfx }}",
                                                    "    state: absent",
                                                    "",
                                                    "- name: cleanup public ip",
                                                    "  azure_rm_publicipaddress:",
                                                    "    name: ansiblepip{{ rpfx }}",
                                                    "    resource_group: '{{ resource_group }}'",
                                                    "    state: absent" }),

            // XXX - test pre and postrequisites
            new Tweak_Option_DefaultValueTest("azure_rm_containerinstancecontainergroup_facts", "resource_group", "\"{{ resource_group }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_containerinstancecontainergroup_facts", "container_group_name", "aci{{ rpfx }}"),
            new Tweak_Module_TestPrerequisites("azure_rm_containerinstancecontainergroup_facts",
                                                new string[] {
                                                    "- name: Create sample container instance",
                                                    "  azure_rm_containerinstance:",
                                                    "    resource_group: \"{{ resource_group }}\"",
                                                    "    name: aci{{ rpfx }}$postfix$",
                                                    "    os_type: linux",
                                                    "    ip_address: public",
                                                    "    location: eastus",
                                                    "    ports:",
                                                    "      - 80",
                                                    "    containers:",
                                                    "      - name: mycontainer1",
                                                    "        image: httpd",
                                                    "        memory: 1.5",
                                                    "        ports:",
                                                    "          - 80",
                                                    "          - 81",
                                                    "      - name: mycontainer2",
                                                    "        image: httpd",
                                                    "        memory: 1.5" },
                                                new string[] {
                                                    "- name: Delete sample container instance",
                                                    "  azure_rm_containerinstance:",
                                                    "    resource_group: \"{{ resource_group }}\"",
                                                    "    name: aci{{ rpfx }}$postfix$",
                                                    "    os_type: linux",
                                                    "    ip_address: public",
                                                    "    location: eastus",
                                                    "    ports:",
                                                    "      - 80",
                                                    "    containers:",
                                                    "      - name: mycontainer1",
                                                    "        image: httpd",
                                                    "        memory: 1.5",
                                                    "        ports:",
                                                    "          - 80",
                                                    "          - 81",
                                                    "      - name: mycontainer2",
                                                    "        image: httpd",
                                                    "        memory: 1.5",
                                                    "    state: absent" }),

            new Tweak_Option_DefaultValueTest("azure_rm_containerregistryregistry_facts", "resource_group", "\"{{ resource_group }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_containerregistryregistry_facts", "registry_name", "acr{{ rpfx }}"),
            new Tweak_Response_FieldReturned("azure_rm_containerregistryregistry_facts", "status", ""),
            new Tweak_Module_TestPrerequisites("azure_rm_containerregistryregistry_facts",
                                                new string[] {
                                                     "- name: Create an container registry",
                                                     "  azure_rm_containerregistry:",
                                                     "    name: acr{{ rpfx }}$postfix$",
                                                     "    resource_group: \"{{ resource_group }}\"",
                                                     "    location: eastus2",
                                                     "    state: present",
                                                     "    admin_user_enabled: true",
                                                     "    sku: Premium",
                                                     "    tags:",
                                                     "        Release: beta1",
                                                     "        Environment: Production" },
                                                new string[] {
                                                     "- name: Delete container registry",
                                                     "  azure_rm_containerregistry:",
                                                     "    name: acr{{ rpfx }}$postfix$",
                                                     "    resource_group: \"{{ resource_group }}\"",
                                                     "    state: absent" }),

            new Tweak_Option_DefaultValueTest("azure_rm_containerregistryreplication", "resource_group", "\"{{ resource_group }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_containerregistryreplication", "registry_name", "acr{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_containerregistryreplication", "replication_name", "replication{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_containerregistryreplication", "location", "westus"),
            new Tweak_Module_TestPrerequisites("azure_rm_containerregistryreplication",
                                                new string[] {
                                                     "- name: Create an container registry",
                                                     "  azure_rm_containerregistry:",
                                                     "    name: acr{{ rpfx }}",
                                                     "    resource_group: \"{{ resource_group }}\"",
                                                     "    location: eastus2",
                                                     "    state: present",
                                                     "    admin_user_enabled: true",
                                                     "    sku: Premium",
                                                     "    tags:",
                                                     "        Release: beta1",
                                                     "        Environment: Production" },
                                                new string[] {
                                                     "- name: Delete container registry",
                                                     "  azure_rm_containerregistry:",
                                                     "    name: acr{{ rpfx }}",
                                                     "    resource_group: \"{{ resource_group }}\"",
                                                     "    state: absent" }),
            new Tweak_Module_FlattenParametersDictionary("azure_rm_containerregistryreplication"),

            new Tweak_Option_DefaultValueTest("azure_rm_containerregistryreplication_facts", "resource_group", "\"{{ resource_group }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_containerregistryreplication_facts", "registry_name", "acr{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_containerregistryreplication_facts", "replication_name", "replication{{ rpfx }}"),
            new Tweak_Response_FieldReturned("azure_rm_containerregistryreplication_facts", "status.message", ""),
            new Tweak_Module_TestPrerequisitesModule("azure_rm_containerregistryreplication_facts", "azure_rm_containerregistryreplication", null, null),

            new Tweak_Option_DefaultValueTest("azure_rm_containerregistrywebhook", "resource_group", "\"{{ resource_group }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_containerregistrywebhook", "registry_name", "acr{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_containerregistrywebhook", "webhook_name", "webhook{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_containerregistrywebhook", "location", "eastus2"),
            new Tweak_Option_DefaultValueTest("azure_rm_containerregistrywebhook", "service_uri", "http://serviceuri.com"),
            new Tweak_Option_DefaultValueTest("azure_rm_containerregistrywebhook", "actions", "push"),
            new Tweak_Option_Required("azure_rm_containerregistrywebhook", "service_uri", false),
            new Tweak_Option_Required("azure_rm_containerregistrywebhook", "actions", false),
            new Tweak_Module_TestPrerequisites("azure_rm_containerregistrywebhook",
                                                new string[] {
                                                     "- name: Create an container registry",
                                                     "  azure_rm_containerregistry:",
                                                     "    name: acr{{ rpfx }}",
                                                     "    resource_group: \"{{ resource_group }}\"",
                                                     "    location: eastus2",
                                                     "    state: present",
                                                     "    admin_user_enabled: true",
                                                     "    sku: Premium",
                                                     "    tags:",
                                                     "        Release: beta1",
                                                     "        Environment: Production" },
                                                new string[] {
                                                     "- name: Delete container registry",
                                                     "  azure_rm_containerregistry:",
                                                     "    name: acr{{ rpfx }}",
                                                     "    resource_group: \"{{ resource_group }}\"",
                                                     "    state: absent" }),

            new Tweak_Option_DefaultValueTest("azure_rm_containerregistrywebhook_facts", "resource_group", "\"{{ resource_group }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_containerregistrywebhook_facts", "registry_name", "acr{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_containerregistrywebhook_facts", "webhook_name", "webhook{{ rpfx }}"),
            new Tweak_Response_FieldReturned("azure_rm_containerregistrywebhook_facts", "scope", ""),
            new Tweak_Module_TestPrerequisitesModule("azure_rm_containerregistrywebhook_facts", "azure_rm_containerregistrywebhook", null, null),

            // Key Vault
            new Tweak_Option_Collapse("azure_rm_vault", "properties.access_policies.permissions", ""),
            new Tweak_Option_Collapse("azure_rm_vault", "properties", ""),

            new Tweak_Option_Exclude("azure_rm_vault", "vault_uri", true, true),
            new Tweak_Option_SetType("azure_rm_vault", "enabled_for_deployment", "bool"),
            new Tweak_Option_SetType("azure_rm_vault", "enabled_for_disk_encryption", "bool"),
            new Tweak_Option_SetType("azure_rm_vault", "enabled_for_template_deployment", "bool"),
            new Tweak_Option_SetType("azure_rm_vault", "enable_soft_delete", "bool"),
            new Tweak_Option_MakeBoolean("azure_rm_vault", "create_mode", "Recover", "Default", false, "Create vault in recovery mode."),

            new Tweak_Option_DefaultValueTest("azure_rm_vault", "vault_name", "vault{{ rpfx }}"),
            new Tweak_Option_DefaultValueTest("azure_rm_vault", "enabled_for_deployment", "yes"),
            new Tweak_Option_DefaultValueTest("azure_rm_vault", "sku.family", "A"),
            new Tweak_Option_DefaultValueTest("azure_rm_vault", "sku.name", "standard"),
            new Tweak_Option_DefaultValueTest("azure_rm_vault", "access_policies.object_id", "99998888-8666-4144-9199-2d7cd0111111"),
            new Tweak_Option_DefaultValueTest("azure_rm_vault", "access_policies.keys", "get"),

            new Tweak_Option_DefaultValueSample("azure_rm_vault", "resource_group_name", "myresourcegroup"),
            new Tweak_Option_DefaultValueSample("azure_rm_vault", "vault_name", "samplekeyvault"),
            new Tweak_Option_DefaultValueSample("azure_rm_vault", "tenant_id", "72f98888-8666-4144-9199-2d7cd0111111"),
            new Tweak_Option_DefaultValueSample("azure_rm_vault", "enabled_for_deployment", "yes"),
            //new Tweak_Option_DefaultValueSample("azure_rm_vault", "sku.family", "A"),
            new Tweak_Option_DefaultValueSample("azure_rm_vault", "sku.name", "standard"),
            new Tweak_Option_DefaultValueSample("azure_rm_vault", "access_policies.tenant_id", "72f98888-8666-4144-9199-2d7cd0111111"),
            new Tweak_Option_DefaultValueSample("azure_rm_vault", "access_policies.object_id", "99998888-8666-4144-9199-2d7cd0111111"),
            new Tweak_Option_DefaultValueSample("azure_rm_vault", "access_policies.keys", "get")
        };
    }
}
