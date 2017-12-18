using System;
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

            // SQL Server
            new Tweak_Module_ObjectName("azure_rm_sqlserver", "SQL Server"),
            new Tweak_Option_Rename("azure_rm_sqlserver", "server_name", "name"),
            new Tweak_Option_Rename("azure_rm_sqlserver", "administrator_login", "admin_username"),
            new Tweak_Option_Rename("azure_rm_sqlserver", "administrator_login_password", "admin_password"),
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
            new Tweak_Response_SetFieldNoLog("azure_rm_sqlserver", "administrator_login_password"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlserver", "administrator_login", "mylogin"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlserver", "administrator_login_password", "Testpasswordxyz12!"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlserver", "location", "eastus"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlserver", "server_name", "\"sqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),
            new Tweak_Option_Flatten("azure_rm_sqlserver", "identity", ""),
            new Tweak_Module_AssertStateVariable("azure_rm_sqlserver", "state"),
            new Tweak_Module_AssertStateExpectedValue("azure_rm_sqlserver", "Ready"),

            // SQL Server Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sqlserver_facts", "azure_rm_sqlserver", null, null),
            new Tweak_Module_ObjectName("azure_rm_sqlserver_facts", "SQL Server"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlserver_facts", "server_name", "\"sqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),

            // SQL Database
            new Tweak_Module_ObjectName("azure_rm_sqldatabase", "SQL Database"),
            new Tweak_Option_Rename("azure_rm_sqldatabase", "database_name", "name"),
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sqldatabase", "azure_rm_sqlserver", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_sqldatabase", "server_name", "\"sqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_sqldatabase", "database_name", "test-database"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqldatabase", "location", "eastus"),
            new Tweak_Response_AddField("azure_rm_sqldatabase", "database_id"),
            new Tweak_Module_AssertStateVariable("azure_rm_sqldatabase", "status"),
            new Tweak_Module_AssertStateExpectedValue("azure_rm_sqldatabase", "Online"),

            // SQL Database Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sqldatabase_facts", "azure_rm_sqldatabase", null, null),
            new Tweak_Module_ObjectName("azure_rm_sqldatabase_facts", "SQL Database"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqldatabase_facts", "server_name", "\"sqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_sqldatabase_facts", "database_name", "test-database"),

            // SQL Elastic Pool
            new Tweak_Module_ObjectName("azure_rm_sqlelasticpool", "ElasticPool"),
            new Tweak_Option_Rename("azure_rm_sqlelasticpool", "elastic_pool_name", "name"),
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sqlelasticpool", "azure_rm_sqlserver", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlelasticpool", "server_name", "zims-server"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlelasticpool", "elastic_pool_name", "test-elastic-pool"),
            new Tweak_Option_DefaultValueTest("azure_rm_sqlelasticpool", "location", "westus"),

            new Tweak_Module_ObjectName("azure_rm_sqlserver_facts", "SQL Server"),

            // MySQL Server
            new Tweak_Module_ObjectName("azure_rm_mysqlserver", "MySQL Server"),
            new Tweak_Option_Rename("azure_rm_mysqlserver", "server_name", "name"),
            new Tweak_Option_Rename("azure_rm_mysqlserver", "properties.administrator_login", "admin_username"),
            new Tweak_Option_Rename("azure_rm_mysqlserver", "properties.administrator_login_password", "admin_password"),
            new Tweak_Option_Required("azure_rm_mysqlserver", "properties", false),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlserver", "server_name", "\"mysqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlserver", "properties.version", "5.6"),
            new Tweak_Option_DefaultValue("azure_rm_mysqlserver", "properties.create_mode", "'Default'"),
            new Tweak_Option_Documentation("azure_rm_mysqlserver", "properties.create_mode", "Currently only 'Default' value supported"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlserver", "properties.administrator_login", "zimxyz"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlserver", "properties.administrator_login_password", "Testpasswordxyz12!"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlserver", "location", "westus"),
            new Tweak_Option_Flatten("azure_rm_mysqlserver", "properties", ""),
            new Tweak_Module_AssertStateVariable("azure_rm_mysqlserver", "user_visible_state"),
            new Tweak_Module_AssertStateExpectedValue("azure_rm_mysqlserver", "Ready"),

            // MySQL Server Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqlserver_facts", "azure_rm_mysqlserver", null, null),
            new Tweak_Module_ObjectName("azure_rm_mysqlserver_facts", "MySQL Server"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlserver_facts", "server_name", "\"mysqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),

            // MySQL Database
            new Tweak_Module_ObjectName("azure_rm_mysqldatabase", "MySQL Database"),
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqldatabase", "azure_rm_mysqlserver", null, null),
            new Tweak_Option_Rename("azure_rm_mysqldatabase", "database_name", "name"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqldatabase", "server_name", "\"mysqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqldatabase", "database_name", "testdatabase"),
            new Tweak_Response_AddField("azure_rm_mysqldatabase", "name"),
            new Tweak_Module_AssertStateVariable("azure_rm_mysqldatabase", "name"),
            new Tweak_Module_AssertStateExpectedValue("azure_rm_mysqldatabase", "testdatabase"),
            new Tweak_Module_NeedsDeleteBeforeUpdate("azure_rm_mysqldatabase"),

            // MySQL Database Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqldatabase_facts", "azure_rm_mysqldatabase", null, null),
            new Tweak_Module_ObjectName("azure_rm_mysqldatabase_facts", "MySQL Database"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqldatabase_facts", "server_name", "\"mysqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqldatabase_facts", "database_name", "testdatabase"),

            // MySQL Server Firewall Rule
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqlfirewallrule", "azure_rm_mysqlserver", null, null),
            new Tweak_Option_Rename("azure_rm_mysqlfirewallrule", "firewall_rule_name", "name"),
            new Tweak_Option_Required("azure_rm_mysqlfirewallrule", "start_ip_address", false),
            new Tweak_Option_Required("azure_rm_mysqlfirewallrule", "end_ip_address", false),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlfirewallrule", "server_name", "\"mysqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlfirewallrule", "firewall_rule_name", "test-firewall-rule"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlfirewallrule", "start_ip_address", "172.28.10.136"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlfirewallrule", "end_ip_address", "172.28.10.138"),
            new Tweak_Module_FlattenParametersDictionary("azure_rm_mysqlfirewallrule"),
            //new Tweak_Module_AssertStateVariable("azure_rm_mysqlfirewallrule", "firewall_rule_name"),
            //new Tweak_Module_AssertStateExpectedValue("azure_rm_mysqlfirewallrule", "test-firewall-rule"),
            new Tweak_Module_AddUpdateRule("azure_rm_mysqlfirewallrule", "start_ip_address", "start_ip_address"),
            new Tweak_Module_AddUpdateRule("azure_rm_mysqlfirewallrule", "end_ip_address", "end_ip_address"),

            // MySQL Firewall Rule Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqlfirewallrule_facts", "azure_rm_mysqlfirewallrule", null, null),
            new Tweak_Module_ObjectName("azure_rm_mysqlfirewallrule_facts", "MySQL Firewall Rule"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlfirewallrule_facts", "server_name", "\"mysqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlfirewallrule_facts", "firewallrule_name", "test-firewall-rule"),

            // MySQL Server Configuration
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqlconfiguration", "azure_rm_mysqlserver", null, null),
            new Tweak_Option_Rename("azure_rm_mysqlconfiguration", "configuration_name", "name"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlconfiguration", "server_name", "\"mysqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlconfiguration", "configuration_name", "event_scheduler"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlconfiguration", "value", "\"ON\""),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlconfiguration", "source", "user-override"),
            new Tweak_Module_FlattenParametersDictionary("azure_rm_mysqlconfiguration"),
            //new Tweak_Module_AssertStateVariable("azure_rm_mysqlconfiguration", "value"),
            //new Tweak_Module_AssertStateExpectedValue("azure_rm_mysqlconfiguration", "ON"),

            // MySQL Configuration Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqlconfiguration_facts", "azure_rm_mysqlconfiguration", null, null),
            new Tweak_Module_ObjectName("azure_rm_mysqlconfiguration_facts", "MySQL Configuration"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlconfiguration_facts", "server_name", "\"mysqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlconfiguration_facts", "configuration_name", "event_scheduler"),

            // MySQL Server Virtual Network Rule
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqlvirtualnetworkrule", "azure_rm_mysqlserver", null, null),
            new Tweak_Option_Rename("azure_rm_mysqlvirtualnetworkrule", "virtual_network_rule_name", "name"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlvirtualnetworkrule", "server_name", "\"mysqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlvirtualnetworkrule", "virtual_network_rule_name", "test-virtual-network-rule"),

            // MySQL Virtual Network Rule Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqlvirtualnetworkrule_facts", "azure_rm_mysqlvirtualnetworkrule", null, null),
            new Tweak_Module_ObjectName("azure_rm_mysqlvirtualnetworkrule_facts", "MySQL Virtual Network Rule"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlvirtualnetworkrule_facts", "server_name", "\"mysqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqlvirtualnetworkrule_facts", "virtualnetworkrule_name", "test-virtual-network-rule"),

            // MySQL Server LogFile Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqllogfile_facts", "azure_rm_mysqlserver", null, null),
            new Tweak_Option_DefaultValueTest("azure_rm_mysqllogfile_facts", "server_name", "\"mysqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),

            // PostgreSQL Server
            new Tweak_Module_ObjectName("azure_rm_postgresqlserver", "PostgreSQL Server"),
            new Tweak_Option_Rename("azure_rm_postgresqlserver", "server_name", "name"),
            new Tweak_Option_Rename("azure_rm_postgresqlserver", "properties.administrator_login", "admin_username"),
            new Tweak_Option_Rename("azure_rm_postgresqlserver", "properties.administrator_login_password", "admin_password"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlserver", "location", "westus"),
            new Tweak_Option_Required("azure_rm_postgresqlserver", "properties", false),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlserver", "server_name", "\"postgresqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlserver", "properties.administrator_login", "zimxyz"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlserver", "properties.administrator_login_password", "Testpasswordxyz12!"),
            new Tweak_Option_DefaultValue("azure_rm_postgresqlserver", "properties.create_mode", "'Default'"),
            new Tweak_Option_Documentation("azure_rm_postgresqlserver", "properties.create_mode", "Currently only 'Default' value supported"),
            new Tweak_Option_Flatten("azure_rm_postgresqlserver", "properties", ""),
            new Tweak_Module_AssertStateVariable("azure_rm_postgresqlserver", "user_visible_state"),
            new Tweak_Module_AssertStateExpectedValue("azure_rm_postgresqlserver", "Ready"),

            // PostgreSQL Server Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqlserver_facts", "azure_rm_postgresqlserver", null, null),
            new Tweak_Module_ObjectName("azure_rm_postgresqlserver_facts", "MySQL Server"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlserver_facts", "server_name", "\"postgresqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),

            // PostgreSQL Database
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqldatabase", "azure_rm_postgresqlserver", null, null),
            new Tweak_Option_Rename("azure_rm_postgresqldatabase", "database_name", "name"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqldatabase", "server_name", "\"postgresqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqldatabase", "database_name", "testdatabase"),
            new Tweak_Response_AddField("azure_rm_postgresqldatabase", "name"),
            new Tweak_Module_AssertStateVariable("azure_rm_postgresqldatabase", "name"),
            new Tweak_Module_AssertStateExpectedValue("azure_rm_postgresqldatabase", "testdatabase"),
            new Tweak_Module_NeedsDeleteBeforeUpdate("azure_rm_postgresqldatabase"),

            // PostgreSQL Database Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqldatabase_facts", "azure_rm_postgresqldatabase", null, null),
            new Tweak_Module_ObjectName("azure_rm_postgresqldatabase_facts", "PostgreSQL Database"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqldatabase_facts", "server_name", "\"postgresqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqldatabase_facts", "database_name", "testdatabase"),

            new Tweak_Module_ObjectName("azure_rm_postgresqldatabase", "PostgreSQL Database"),

            // PostgreSQL Server Firewall Rule
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqlfirewallrule", "azure_rm_postgresqlserver", null, null),
            new Tweak_Option_Rename("azure_rm_postgresqlfirewallrule", "firewall_rule_name", "name"),
            new Tweak_Option_Required("azure_rm_postgresqlfirewallrule", "start_ip_address", false),
            new Tweak_Option_Required("azure_rm_postgresqlfirewallrule", "end_ip_address", false),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlfirewallrule", "server_name", "\"postgresqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlfirewallrule", "firewall_rule_name", "test-firewall-rule"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlfirewallrule", "start_ip_address", "172.28.10.136"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlfirewallrule", "end_ip_address", "172.28.10.138"),
            new Tweak_Module_FlattenParametersDictionary("azure_rm_postgresqlfirewallrule"),
            //new Tweak_Module_AssertStateVariable("azure_rm_postgresqlfirewallrule", "firewall_rule_name"),
            //new Tweak_Module_AssertStateExpectedValue("azure_rm_postgresqlfirewallrule", "test-firewall-rule"),
            new Tweak_Module_AddUpdateRule("azure_rm_postgresqlfirewallrule", "start_ip_address", "start_ip_address"),
            new Tweak_Module_AddUpdateRule("azure_rm_postgresqlfirewallrule", "end_ip_address", "end_ip_address"),

            // PostgreSQL Firewall Rule Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqlfirewallrule_facts", "azure_rm_postgresqlfirewallrule", null, null),
            new Tweak_Module_ObjectName("azure_rm_postgresqlfirewallrule_facts", "PostgreSQL Firewall Rule"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlfirewallrule_facts", "server_name", "\"postgresqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlfirewallrule_facts", "firewallrule_name", "test-firewall-rule"),

            // PostgreSQL Server Configuration
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqlconfiguration", "azure_rm_postgresqlserver", null, null),
            new Tweak_Option_Rename("azure_rm_postgresqlconfiguration", "configuration_name", "name"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlconfiguration", "server_name", "\"postgresqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlconfiguration", "configuration_name", "deadlock_timeout"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlconfiguration", "value", "2000"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlconfiguration", "source", "user-override"),
            new Tweak_Module_FlattenParametersDictionary("azure_rm_postgresqlconfiguration"),
            //new Tweak_Module_AssertStateVariable("azure_rm_postgresqlconfiguration", "value"),
            //new Tweak_Module_AssertStateExpectedValue("azure_rm_postgresqlconfiguration", "ON"),

            // PostgreSQL Configuration Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqlconfiguration_facts", "azure_rm_postgresqlconfiguration", null, null),
            new Tweak_Module_ObjectName("azure_rm_postgresqlconfiguration_facts", "PostgreSQL Configuration"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlconfiguration_facts", "server_name", "\"postgresqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlconfiguration_facts", "configuration_name", "event_scheduler"),

            // PostgreSQL Server Virtual Network Rule
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqlvirtualnetworkrule", "azure_rm_postgresqlserver", null, null),
            new Tweak_Option_Rename("azure_rm_postgresqlvirtualnetworkrule", "virtual_network_rule_name", "name"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlvirtualnetworkrule", "server_name", "\"postgresqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlvirtualnetworkrule", "virtual_network_rule_name", "test-virtual-network-rule"),

            // PostgreSQL Virtual Network Rule Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqlvirtualnetworkrule_facts", "azure_rm_postgresqlvirtualnetworkrule", null, null),
            new Tweak_Module_ObjectName("azure_rm_postgresqlvirtualnetworkrule_facts", "PostgreSQL Virtual Network Rule"),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlvirtualnetworkrule_facts", "server_name", "\"postgresqlsrv{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_postgresqlvirtualnetworkrule_facts", "virtualnetworkrule_name", "test-virtual-network-rule"),

            // Authorization
            new Tweak_Option_DefaultValueTest("azure_rm_authorizationroledefinition", "role_definition_id", "rolexyz"),
            new Tweak_Option_DefaultValueTest("azure_rm_authorizationroledefinition", "scope", "\"/subscriptions/{{ azure_subscription_id }}\""),
            new Tweak_Option_Flatten("azure_rm_authorizationroledefinition", "properties", "properties_"),

            new Tweak_Option_DefaultValueTest("azure_rm_authorizationroleassignment", "scope", "\"/subscriptions/{{ azure_subscription_id }}\""),
            new Tweak_Option_DefaultValueTest("azure_rm_authorizationroleassignment", "role_assignment_name", "\"d3881f73-7777-8888-8283-e981cbba0404\""),
            new Tweak_Option_DefaultValueTest("azure_rm_authorizationroleassignment", "properties.role_definition_id", "\"/subscriptions/{{ azure_subscription_id }}/providers/Microsoft.Authorization/roleDefinitions/9980e02c-c2be-4d73-94e8-173b1dc7cf3c\""),
            new Tweak_Option_DefaultValueTest("azure_rm_authorizationroleassignment", "properties.principal_id", "\"98b422c6-7bea-4706-b6f3-920a782746d4\""),
            new Tweak_Module_FlattenParametersDictionary("azure_rm_authorizationroleassignment"),
            //new Tweak_Option_Flatten("azure_rm_authorizationroleassignment", "properties", "properties_"),

            // Application Gateway
            new Tweak_Module_Rename("azure_rm_applicationgatewayapplicationgateway", "azure_rm_applicationgateway"),
            new Tweak_Module_ObjectName("azure_rm_applicationgatewayapplicationgateway", "Application Gateway"),
            new Tweak_Option_Rename("azure_rm_applicationgatewayapplicationgateway", "application_gateway_name", "name"),
            new Tweak_Option_DefaultValueTest("azure_rm_applicationgatewayapplicationgateway", "application_gateway_name", "\"appgateway{{ random_postfix }}{{ resource_group | hash('md5') | truncate(7, True, '') }}\""),

            // RELEASE STATUS FOR VARIOUS MODULES
            new Tweak_Module_ReleaseStatus("azure_rm_sqlserver", "RP"),
            new Tweak_Module_ReleaseStatus("azure_rm_mysqlserver", "RP"),
            new Tweak_Module_ReleaseStatus("azure_rm_postgresqlserver", "RP"),
            new Tweak_Module_ReleaseStatus("azure_rm_sqldatabase", "RP"),
            new Tweak_Module_ReleaseStatus("azure_rm_mysqldatabase", "RP"),
            new Tweak_Module_ReleaseStatus("azure_rm_postgresqldatabase", "RP"),
            //new Tweak_Module_ReleaseStatus("azure_rm_sqlfirewallrule", "RP"),
            new Tweak_Module_ReleaseStatus("azure_rm_mysqlfirewallrule", "RP"),
            new Tweak_Module_ReleaseStatus("azure_rm_postgresqlfirewallrule", "RP"),
            //new Tweak_Module_ReleaseStatus("azure_rm_sqlvirtualnetworkrule", "RP"),
            //new Tweak_Module_ReleaseStatus("azure_rm_mysqlvirtualnetworkrule", "RP"),
            //new Tweak_Module_ReleaseStatus("azure_rm_postgresqlvirtualnetworkrule", "RP"),
            new Tweak_Module_ReleaseStatus("azure_rm_sqlconfiguration", "RP"),
            new Tweak_Module_ReleaseStatus("azure_rm_mysqlconfiguration", "RP"),
            new Tweak_Module_ReleaseStatus("azure_rm_postgresqlconfiguration", "RP"),

            new Tweak_Module_ReleaseStatus("azure_rm_sqlserver_facts", "RP"),
            new Tweak_Module_ReleaseStatus("azure_rm_mysqlserver_facts", "RP"),
            new Tweak_Module_ReleaseStatus("azure_rm_postgresqlserver_facts", "RP"),
            new Tweak_Module_ReleaseStatus("azure_rm_sqldatabase_facts", "RP"),
            new Tweak_Module_ReleaseStatus("azure_rm_mysqldatabase_facts", "RP"),
            new Tweak_Module_ReleaseStatus("azure_rm_postgresqldatabase_facts", "RP"),
            //new Tweak_Module_ReleaseStatus("azure_rm_sqlfirewallrule_facts", "RP"),
            new Tweak_Module_ReleaseStatus("azure_rm_mysqlfirewallrule_facts", "RP"),
            new Tweak_Module_ReleaseStatus("azure_rm_postgresqlfirewallrule_facts", "RP"),
            //new Tweak_Module_ReleaseStatus("azure_rm_sqlvirtualnetworkrule_facts", "RP"),
            //new Tweak_Module_ReleaseStatus("azure_rm_mysqlvirtualnetworkrule_facts", "RP"),
            //new Tweak_Module_ReleaseStatus("azure_rm_postgresqlvirtualnetworkrule_facts", "RP"),
            //new Tweak_Module_ReleaseStatus("azure_rm_sqlconfiguration_facts", "RP"),
            new Tweak_Module_ReleaseStatus("azure_rm_mysqlconfiguration_facts", "RP"),
            new Tweak_Module_ReleaseStatus("azure_rm_postgresqlconfiguration_facts", "RP"),
            //new Tweak_Module_ReleaseStatus("azure_rm_authorizationroleassignment", "RP"),
        };
    }
}
