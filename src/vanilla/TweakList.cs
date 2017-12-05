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
            new Tweak_Module_Rename("azure_rm_sql_servers", "azure_rm_sqlserver"),
            new Tweak_Module_ObjectName("azure_rm_sql_servers", "SQL Server"),
            new Tweak_Option_Rename("azure_rm_sql_servers", "server_name", "name"),
            new Tweak_Option_Rename("azure_rm_sql_servers", "administrator_login", "admin_username"),
            new Tweak_Option_Rename("azure_rm_sql_servers", "administrator_login_password", "admin_password"),
            new Tweak_Option_DefaultValueSample("azure_rm_sql_servers", "resource_group_name", "resource_group"),
            new Tweak_Option_DefaultValueSample("azure_rm_sql_servers", "location", "westus"),
            new Tweak_Option_DefaultValueSample("azure_rm_sql_servers", "administrator_login", "mylogin"),
            new Tweak_Option_DefaultValueSample("azure_rm_sql_servers", "administrator_login_password", "Testpasswordxyz12!"),
            new Tweak_Option_DefaultValueSample("azure_rm_sql_servers", "tags", ""),
            new Tweak_Option_DefaultValueSample("azure_rm_sql_servers", "identity", ""),
            new Tweak_Option_DefaultValueSample("azure_rm_sql_servers", "identity.type", ""), // XXX - this is a bug
            new Tweak_Option_DefaultValueSample("azure_rm_sql_servers", "version", ""),
            new Tweak_Response_FieldSampleValue("azure_rm_sql_servers", "id", "/subscriptions/00000000-1111-2222-3333-444444444444/resourceGroups/sqlcrudtest-7398/providers/Microsoft.Sql/servers/sqlcrudtest-4645"),
            new Tweak_Response_FieldSampleValue("azure_rm_sql_servers", "version", "12.0"),
            new Tweak_Response_FieldSampleValue("azure_rm_sql_servers", "fully_qualified_domain_name", "sqlcrudtest-4645.database.windows.net"),
            new Tweak_Option_DefaultValueTest("azure_rm_sql_servers", "administrator_login", "mylogin"),
            new Tweak_Option_DefaultValueTest("azure_rm_sql_servers", "administrator_login_password", "Testpasswordxyz12!"),
            new Tweak_Option_DefaultValueTest("azure_rm_sql_servers", "location", "eastus"),
            new Tweak_Option_DefaultValueTest("azure_rm_sql_servers", "server_name", "sql-test-server-dauih"),

            // SQL Database
            new Tweak_Module_Rename("azure_rm_sql_databases", "azure_rm_sqldatabase"),
            new Tweak_Module_ObjectName("azure_rm_sql_databases", "SQL Database"),
            new Tweak_Option_Rename("azure_rm_sql_databases", "database_name", "name"),
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sql_databases", "azure_rm_sql_servers"),
            new Tweak_Option_DefaultValueTest("azure_rm_sql_databases", "server_name", "sql-test-server-dauih"),
            new Tweak_Option_DefaultValueTest("azure_rm_sql_databases", "database_name", "test-database"),
            new Tweak_Option_DefaultValueTest("azure_rm_sql_databases", "location", "eastus"),
            new Tweak_Response_AddField("azure_rm_sql_databases", "database_id"),
            new Tweak_Module_AssertStateVariable("azure_rm_sql_databases", "status"),
            new Tweak_Module_AssertStateExpectedValue("azure_rm_sql_databases", "Online"),

            // SQL Elastic Pool
            new Tweak_Module_Rename("azure_rm_sql_elasticpools", "azure_rm_sqlelasticpool"),
            new Tweak_Module_ObjectName("azure_rm_sql_elasticpools", "ElasticPool"),
            new Tweak_Option_Rename("azure_rm_sql_elasticpools", "elastic_pool_name", "name"),
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sql_elasticpools", "azure_rm_sql_servers"),
            new Tweak_Option_DefaultValueTest("azure_rm_sql_elasticpools", "server_name", "zims-server"),
            new Tweak_Option_DefaultValueTest("azure_rm_sql_elasticpools", "elastic_pool_name", "test-elastic-pool"),
            new Tweak_Option_DefaultValueTest("azure_rm_sql_elasticpools", "location", "westus"),

            // SQL ...
            new Tweak_Module_Rename("azure_rm_sql_configurations", "azure_rm_sqlconfiguration"),
            new Tweak_Module_Rename("azure_rm_sql_configurations_facts", "azure_rm_sqlconfiguration_facts"),
            new Tweak_Module_Rename("azure_rm_sql_databases_facts", "azure_rm_sqldatabase_facts"),
            new Tweak_Module_Rename("azure_rm_sql_databases", "azure_rm_sqldatabase"),
            new Tweak_Module_Rename("azure_rm_sql_databases_facts", "azure_rm_sqldatabase_facts"),
            new Tweak_Module_Rename("azure_rm_sql_firewallrules", "azure_rm_sqlfirewallrule"),
            new Tweak_Module_Rename("azure_rm_sql_firewallrules_facts", "azure_rm_sqlfirewallrule_facts"),
            new Tweak_Module_ObjectName("azure_rm_sql_servers_facts", "SQL Server"),
            new Tweak_Module_Rename("azure_rm_sql_servers_facts", "azure_rm_sqlserver_facts"),
            new Tweak_Module_Rename("azure_rm_sql_virtualnetworkrules", "azure_rm_sqlvirtualnetworkrule"),
            new Tweak_Module_Rename("azure_rm_sql_virtualnetworkrules_facts", "azure_rm_sqlvirtualnetworkrule_facts"),

            // MySQL Server
            new Tweak_Module_Rename("azure_rm_mysql_servers", "azure_rm_mysqlserver"),
            new Tweak_Module_ObjectName("azure_rm_mysql_servers", "MySQL Server"),
            new Tweak_Option_Rename("azure_rm_mysql_servers", "server_name", "name"),
            new Tweak_Option_Rename("azure_rm_mysql_servers", "properties.administrator_login", "admin_username"),
            new Tweak_Option_Rename("azure_rm_mysql_servers", "properties.administrator_login_password", "admin_password"),
            new Tweak_Option_Required("azure_rm_mysql_servers", "properties", false),
            new Tweak_Option_DefaultValueTest("azure_rm_mysql_servers", "server_name", "test-mysql-server"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysql_servers", "properties.version", "5.6"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysql_servers", "properties.create_mode", "Default"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysql_servers", "properties.administrator_login", "zimxyz"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysql_servers", "properties.administrator_login_password", "Testpasswordxyz12!"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysql_servers", "location", "westus"),
            new Tweak_Module_AssertStateVariable("azure_rm_mysql_servers", "user_visible_state"),
            new Tweak_Module_AssertStateExpectedValue("azure_rm_mysql_servers", "Ready"),

            // MySQL Database
            new Tweak_Module_Rename("azure_rm_mysql_databases", "azure_rm_mysqldatabase"),
            new Tweak_Module_ObjectName("azure_rm_mysql_databases", "MySQL Database"),
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysql_databases", "azure_rm_mysql_servers"),
            new Tweak_Option_Rename("azure_rm_mysql_databases", "database_name", "name"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysql_databases", "server_name", "test-mysql-server"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysql_databases", "database_name", "testdatabase"),
            new Tweak_Module_AssertStateVariable("azure_rm_mysql_databases", "name"),
            new Tweak_Module_AssertStateExpectedValue("azure_rm_mysql_databases", "testdatabase"),
            new Tweak_Module_NeedsDeleteBeforeUpdate("azure_rm_mysql_databases"),

            // MySQL Server Firewall Rule
            new Tweak_Module_Rename("azure_rm_mysql_firewallrules", "azure_rm_mysqlfirewallrule"),
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysql_firewallrules", "azure_rm_mysql_servers"),
            new Tweak_Option_Rename("azure_rm_mysql_firewallrules", "firewall_rule_name", "name"),
            new Tweak_Option_Required("azure_rm_mysql_firewallrules", "start_ip_address", false),
            new Tweak_Option_Required("azure_rm_mysql_firewallrules", "end_ip_address", false),
            new Tweak_Option_DefaultValueTest("azure_rm_mysql_firewallrules", "server_name", "test-mysql-server"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysql_firewallrules", "firewall_rule_name", "test-firewall-rule"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysql_firewallrules", "start_ip_address", "172.28.10.136"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysql_firewallrules", "end_ip_address", "172.28.10.138"),
            new Tweak_Module_FlattenParametersDictionary("azure_rm_mysql_firewallrules"),
            new Tweak_Module_AssertStateVariable("azure_rm_mysql_firewallrules", "name"),
            new Tweak_Module_AssertStateExpectedValue("azure_rm_mysql_firewallrules", "test-firewall-rule"),
            new Tweak_Module_AddUpdateRule("azure_rm_mysql_firewallrules", "start_ip_address", "start_ip_address"),
            new Tweak_Module_AddUpdateRule("azure_rm_mysql_firewallrules", "end_ip_address", "end_ip_address"),

            // MySQL Server Configuration
            new Tweak_Module_Rename("azure_rm_mysql_configurations", "azure_rm_mysqlconfiguration"),
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysql_configurations", "azure_rm_mysql_servers"),
            new Tweak_Option_Rename("azure_rm_mysql_configurations", "configuration_name", "name"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysql_configurations", "server_name", "test-mysql-server"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysql_configurations", "configuration_name", "event_scheduler"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysql_configurations", "value", "\"ON\""),
            new Tweak_Option_DefaultValueTest("azure_rm_mysql_configurations", "source", "user-override"),
            new Tweak_Module_FlattenParametersDictionary("azure_rm_mysql_configurations"),
            new Tweak_Module_AssertStateVariable("azure_rm_mysql_configurations", "value"),
            new Tweak_Module_AssertStateExpectedValue("azure_rm_mysql_configurations", "ON"),

            // MySQL Server Virtual Network Rule
            new Tweak_Module_Rename("azure_rm_mysql_virtualnetworkrules", "azure_rm_mysqlvirtualnetworkrule"),
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysql_virtualnetworkrules", "azure_rm_mysql_servers"),
            new Tweak_Option_Rename("azure_rm_mysql_virtualnetworkrules", "virtual_network_rule_name", "name"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysql_virtualnetworkrules", "server_name", "test-mysql-server"),
            new Tweak_Option_DefaultValueTest("azure_rm_mysql_virtualnetworkrules", "virtual_network_rule_name", "test-virtual-network-rule"),

            new Tweak_Module_Rename("azure_rm_mysql_servers_facts", "azure_rm_mysqlserver_facts"),
            new Tweak_Module_Rename("azure_rm_mysql_configurations_facts", "azure_rm_mysqlconfiguration_facts"),
            new Tweak_Module_Rename("azure_rm_mysql_databases_facts", "azure_rm_mysqldatabase_facts"),
            new Tweak_Module_Rename("azure_rm_mysql_firewallrules_facts", "azure_rm_mysqlfirewallrule_facts"),
            new Tweak_Module_Rename("azure_rm_mysql_virtualnetworkrules_facts", "azure_rm_mysqlvirtualnetworkrule_facts"),


            // PostgreSQL
            new Tweak_Module_Rename("azure_rm_postgresql_configurations", "azure_rm_postgresqlconfiguration"),
            new Tweak_Module_ObjectName("azure_rm_postgresql_servers", "PostgreSQL Server"),
            new Tweak_Module_Rename("azure_rm_postgresql_configurations_facts", "azure_rm_postgresqlconfiguration_facts"),
            new Tweak_Module_Rename("azure_rm_postgresql_databases", "azure_rm_postgresqldatabase"),
            new Tweak_Module_ObjectName("azure_rm_postgresql_databases", "PostgreSQL Database"),
            new Tweak_Module_Rename("azure_rm_postgresql_databases_facts", "azure_rm_postgresqldatabase_facts"),
            new Tweak_Module_Rename("azure_rm_postgresql_databases", "azure_rm_postgresqldatabase"),
            new Tweak_Module_Rename("azure_rm_postgresql_databases_facts", "azure_rm_postgresqldatabase_facts"),
            new Tweak_Module_Rename("azure_rm_postgresql_firewallrules", "azure_rm_postgresqlfirewallrule"),
            new Tweak_Module_Rename("azure_rm_postgresql_firewallrules_facts", "azure_rm_postgresqlfirewallrule_facts"),
            new Tweak_Module_Rename("azure_rm_postgresql_servers", "azure_rm_postgresqlserver"),
            new Tweak_Module_Rename("azure_rm_postgresql_servers_facts", "azure_rm_postgresqlserver_facts"),
            new Tweak_Module_Rename("azure_rm_postgresql_virtualnetworkrules", "azure_rm_postgresqlvirtualnetworkrule"),
            new Tweak_Module_Rename("azure_rm_postgresql_virtualnetworkrules_facts", "azure_rm_postgresqlvirtualnetworkrule_facts"),

        };
    }
}
