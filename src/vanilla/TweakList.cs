using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRest.Ansible
{
    public class Tweaks
    {
        public static Tweak[] All =
        {
            // SQL
            new Tweak_RenameModule("azure_rm_sql_elasticpools", "azure_rm_sql_elasticpool"),
            new Tweak_RenameModule("azure_rm_sql_configurations", "azure_rm_sql_configuration"),
            new Tweak_RenameModule("azure_rm_sql_configurations_facts", "azure_rm_sql_configuration_facts"),
            new Tweak_RenameModule("azure_rm_sql_databases", "azure_rm_sql_database"),
            new Tweak_RenameModule("azure_rm_sql_databases_facts", "azure_rm_sql_database_facts"),
            new Tweak_RenameModule("azure_rm_sql_databases", "azure_rm_sql_database"),
            new Tweak_RenameModule("azure_rm_sql_databases_facts", "azure_rm_sql_database_facts"),
            new Tweak_RenameModule("azure_rm_sql_firewallrules", "azure_rm_sql_firewallrule"),
            new Tweak_RenameModule("azure_rm_sql_firewallrules_facts", "azure_rm_sql_firewallrule_facts"),
            new Tweak_RenameModule("azure_rm_sql_servers", "azure_rm_sql_server"),
            new Tweak_RenameModule("azure_rm_sql_servers_facts", "azure_rm_sql_server_facts"),
            new Tweak_RenameModule("azure_rm_sql_virtualnetworkrules", "azure_rm_sql_virtualnetworkrule"),
            new Tweak_RenameModule("azure_rm_sql_virtualnetworkrules_facts", "azure_rm_sql_virtualnetworkrule_facts"),
            //new Tweak_RenameOption("azure_rm_sql_servers", "identity.type", "mukamuka"),

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
