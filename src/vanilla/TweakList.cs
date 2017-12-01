using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRest.Ansible
{
    public class Tweaks
    {
        public static Tweak[] All =
        {
            // SQL Server
            new Tweak_RenameModule("azure_rm_sql_servers", "azure_rm_sql_server"),
            new Tweak_ModuleObjectName("azure_rm_sql_servers", "SQL Server"),
            new Tweak_RenameOption("azure_rm_sql_servers", "resource_group_name", "resource_group"),
            new Tweak_RenameOption("azure_rm_sql_servers", "server_name", "name"),
            new Tweak_RenameOption("azure_rm_sql_servers", "administrator_login", "admin_username"),
            new Tweak_RenameOption("azure_rm_sql_servers", "administrator_login_password", "admin_password"),
            new Tweak_ChangeOptionDefaultValueSample("azure_rm_sql_servers", "resource_group_name", "resource_group"),
            new Tweak_ChangeOptionDefaultValueSample("azure_rm_sql_servers", "location", "westus"),
            new Tweak_ChangeOptionDefaultValueSample("azure_rm_sql_servers", "administrator_login", "mylogin"),
            new Tweak_ChangeOptionDefaultValueSample("azure_rm_sql_servers", "administrator_login_password", "Testpasswordxyz12!"),
            new Tweak_ChangeOptionDefaultValueSample("azure_rm_sql_servers", "tags", ""),
            new Tweak_ChangeOptionDefaultValueSample("azure_rm_sql_servers", "identity", ""),
            new Tweak_ChangeOptionDefaultValueSample("azure_rm_sql_servers", "identity.type", ""), // XXX - this is a bug
            new Tweak_ChangeOptionDefaultValueSample("azure_rm_sql_servers", "version", ""),
            new Tweak_ChangeOptionRequired("azure_rm_sql_servers", "location", false),
            new Tweak_RemoveResultField("azure_rm_sql_servers", "tags"),
            new Tweak_RemoveResultField("azure_rm_sql_servers", "identity"),
            new Tweak_RemoveResultField("azure_rm_sql_servers", "name"),
            new Tweak_RemoveResultField("azure_rm_sql_servers", "type"),
            new Tweak_RemoveResultField("azure_rm_sql_servers", "location"),
            new Tweak_RemoveResultField("azure_rm_sql_servers", "kind"),
            new Tweak_RemoveResultField("azure_rm_sql_servers", "administrator_login"),
            new Tweak_RemoveResultField("azure_rm_sql_servers", "administrator_login_password"),
            new Tweak_ResultField_UpdateSampleValue("azure_rm_sql_servers", "id", "/subscriptions/00000000-1111-2222-3333-444444444444/resourceGroups/sqlcrudtest-7398/providers/Microsoft.Sql/servers/sqlcrudtest-4645"),
            new Tweak_ResultField_UpdateSampleValue("azure_rm_sql_servers", "version", "12.0"),
            new Tweak_ResultField_UpdateSampleValue("azure_rm_sql_servers", "fully_qualified_domain_name", "sqlcrudtest-4645.database.windows.net"),
            new Tweak_ChangeOptionDefaultValueTest("azure_rm_sql_servers", "administrator_login", "mylogin"),
            new Tweak_ChangeOptionDefaultValueTest("azure_rm_sql_servers", "administrator_login_password", "Testpasswordxyz12!"),
            new Tweak_ChangeOptionDefaultValueTest("azure_rm_sql_servers", "location", "westus"),
            new Tweak_ChangeOptionDefaultValueTest("azure_rm_sql_servers", "resource_group_name", "\"{{ resource_group }}\""),
            new Tweak_ChangeOptionDefaultValueTest("azure_rm_sql_servers", "server_name", "zims-server"),

            // SQL Database
            new Tweak_RenameModule("azure_rm_sql_databases", "azure_rm_sql_database"),
            new Tweak_ModuleObjectName("azure_rm_sql_databases", "Database"),
            new Tweak_RenameOption("azure_rm_sql_databases", "resource_group_name", "resource_group"),
            new Tweak_RenameOption("azure_rm_sql_databases", "database_name", "name"),
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sql_databases", "azure_rm_sql_servers"),
            new Tweak_ChangeOptionDefaultValueTest("azure_rm_sql_databases", "resource_group_name", "\"{{ resource_group }}\""),
            new Tweak_ChangeOptionDefaultValueTest("azure_rm_sql_databases", "server_name", "zims-server"),
            new Tweak_ChangeOptionDefaultValueTest("azure_rm_sql_databases", "database_name", "test-database"),
            new Tweak_ChangeOptionDefaultValueTest("azure_rm_sql_databases", "location", "westus"),
            new Tweak_ModuleAssertStateVariable("azure_rm_sql_databases", "status"),
            new Tweak_ModuleAssertStateExpectedValue("azure_rm_sql_databases", "Online"),

            // SQL Elastic Pool
            new Tweak_RenameModule("azure_rm_sql_elasticpools", "azure_rm_sql_elasticpool"),
            new Tweak_ModuleObjectName("azure_rm_sql_elasticpools", "ElasticPool"),
            new Tweak_RenameOption("azure_rm_sql_elasticpools", "resource_group_name", "resource_group"),
            new Tweak_RenameOption("azure_rm_sql_elasticpools", "elastic_pool_name", "name"),
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sql_elasticpools", "azure_rm_sql_servers"),
            new Tweak_ChangeOptionDefaultValueTest("azure_rm_sql_elasticpools", "resource_group_name", "\"{{ resource_group }}\""),
            new Tweak_ChangeOptionDefaultValueTest("azure_rm_sql_elasticpools", "server_name", "zims-server"),
            new Tweak_ChangeOptionDefaultValueTest("azure_rm_sql_elasticpools", "elastic_pool_name", "test-elastic-pool"),
            new Tweak_ChangeOptionDefaultValueTest("azure_rm_sql_elasticpools", "location", "westus"),

            // SQL - ....
            new Tweak_RenameModule("azure_rm_sql_configurations", "azure_rm_sql_configuration"),
            new Tweak_RenameModule("azure_rm_sql_configurations_facts", "azure_rm_sql_configuration_facts"),
            new Tweak_RenameModule("azure_rm_sql_databases_facts", "azure_rm_sql_database_facts"),
            new Tweak_RenameModule("azure_rm_sql_databases", "azure_rm_sql_database"),
            new Tweak_RenameModule("azure_rm_sql_databases_facts", "azure_rm_sql_database_facts"),
            new Tweak_RenameModule("azure_rm_sql_firewallrules", "azure_rm_sql_firewallrule"),
            new Tweak_RenameModule("azure_rm_sql_firewallrules_facts", "azure_rm_sql_firewallrule_facts"),
            new Tweak_ModuleObjectName("azure_rm_sql_servers_facts", "SQL Server"),
            new Tweak_RenameModule("azure_rm_sql_servers_facts", "azure_rm_sql_server_facts"),
            new Tweak_RenameModule("azure_rm_sql_virtualnetworkrules", "azure_rm_sql_virtualnetworkrule"),
            new Tweak_RenameModule("azure_rm_sql_virtualnetworkrules_facts", "azure_rm_sql_virtualnetworkrule_facts"),

            // MySQL
            new Tweak_RenameModule("azure_rm_mysql_configurations", "azure_rm_mysql_configuration"),
            new Tweak_RenameModule("azure_rm_mysql_configurations_facts", "azure_rm_mysql_configuration_facts"),
            new Tweak_RenameModule("azure_rm_mysql_databases", "azure_rm_mysql_database"),
            new Tweak_RenameModule("azure_rm_mysql_databases_facts", "azure_rm_mysql_database_facts"),
            new Tweak_RenameModule("azure_rm_mysql_databases", "azure_rm_mysql_database"),
            new Tweak_RenameModule("azure_rm_mysql_databases_facts", "azure_rm_mysql_database_facts"),
            new Tweak_RenameModule("azure_rm_mysql_firewallrules", "azure_rm_mysql_firewallrule"),
            new Tweak_RenameModule("azure_rm_mysql_firewallrules_facts", "azure_rm_mysql_firewallrule_facts"),
            new Tweak_RenameModule("azure_rm_mysql_servers", "azure_rm_mysql_server"),
            new Tweak_RenameModule("azure_rm_mysql_servers_facts", "azure_rm_mysql_server_facts"),
            new Tweak_RenameModule("azure_rm_mysql_virtualnetworkrules", "azure_rm_mysql_virtualnetworkrule"),
            new Tweak_RenameModule("azure_rm_mysql_virtualnetworkrules_facts", "azure_rm_mysql_virtualnetworkrule_facts"),
            new Tweak_ModuleAssertStateVariable("azure_rm_mysql_servers", "user_visible_state"),
            new Tweak_ModuleAssertStateExpectedValue("azure_rm_mysql_servers", "Ready"),
            new Tweak_RenameOption("azure_rm_mysql_servers", "resource_group_name", "resource_group"),
            new Tweak_RenameOption("azure_rm_mysql_servers", "server_name", "name"),
            new Tweak_RenameOption("azure_rm_mysql_servers", "properties.administrator_login", "admin_username"),
            new Tweak_RenameOption("azure_rm_mysql_servers", "properties.administrator_login_password", "admin_password"),
            new Tweak_ChangeOptionRequired("azure_rm_mysql_servers", "properties", false),
            new Tweak_ChangeOptionRequired("azure_rm_mysql_servers", "location", false),
            new Tweak_ChangeOptionDefaultValueTest("azure_rm_mysql_servers", "resource_group_name", "\"{{ resource_group }}\""),
            new Tweak_ChangeOptionDefaultValueTest("azure_rm_mysql_servers", "server_name", "test-mysql-server"),
            new Tweak_ChangeOptionDefaultValueTest("azure_rm_mysql_servers", "properties.version", "5.6"),
            new Tweak_ChangeOptionDefaultValueTest("azure_rm_mysql_servers", "properties.create_mode", "Default"),
            new Tweak_ChangeOptionDefaultValueTest("azure_rm_mysql_servers", "properties.administrator_login", "zimxyz"),
            new Tweak_ChangeOptionDefaultValueTest("azure_rm_mysql_servers", "properties.administrator_login_password", "Testpasswordxyz12!"),
            new Tweak_ChangeOptionDefaultValueTest("azure_rm_mysql_servers", "location", "westus"),

            new Tweak_ChangeOptionDefaultValueTest("azure_rm_mysql_databases", "resource_group_name", "\"{{ resource_group }}\""),
            new Tweak_ChangeOptionDefaultValueTest("azure_rm_mysql_databases", "server_name", "test-mysql-server"),

            // PostgreSQL
            new Tweak_RenameModule("azure_rm_postgresql_configurations", "azure_rm_postgresql_configuration"),
            new Tweak_RenameModule("azure_rm_postgresql_configurations_facts", "azure_rm_postgresql_configuration_facts"),
            new Tweak_RenameModule("azure_rm_postgresql_databases", "azure_rm_postgresql_database"),
            new Tweak_RenameModule("azure_rm_postgresql_databases_facts", "azure_rm_postgresql_database_facts"),
            new Tweak_RenameModule("azure_rm_postgresql_databases", "azure_rm_postgresql_database"),
            new Tweak_RenameModule("azure_rm_postgresql_databases_facts", "azure_rm_postgresql_database_facts"),
            new Tweak_RenameModule("azure_rm_postgresql_firewallrules", "azure_rm_postgresql_firewallrule"),
            new Tweak_RenameModule("azure_rm_postgresql_firewallrules_facts", "azure_rm_postgresql_firewallrule_facts"),
            new Tweak_RenameModule("azure_rm_postgresql_servers", "azure_rm_postgresql_server"),
            new Tweak_RenameModule("azure_rm_postgresql_servers_facts", "azure_rm_postgresql_server_facts"),
            new Tweak_RenameModule("azure_rm_postgresql_virtualnetworkrules", "azure_rm_postgresql_virtualnetworkrule"),
            new Tweak_RenameModule("azure_rm_postgresql_virtualnetworkrules_facts", "azure_rm_postgresql_virtualnetworkrule_facts"),

        };
    }
}
