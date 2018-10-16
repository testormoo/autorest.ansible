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
            new Tweak_Response_AddField("*_facts", "tags"),
            new Tweak_Response_RemoveField("*_facts", "type"),
            new Tweak_Option_Documentation("*", "location", "Resource location. If not set, location from the resource group will be used as default."),
            //new Tweak_Option_DefaultValueTest("*", "location", "eastus"),
            new Tweak_Option_DefaultValueSample("*", "location", "eastus"),

            // SQL Server
            new Tweak_Module_AssertStateVariable("azure_rm_sqlserver", "state"),
            new Tweak_Module_AssertStateExpectedValue("azure_rm_sqlserver", "Ready"),

            // SQL Server Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sqlserver_facts", "azure_rm_sqlserver", null, null),

            // SQL Database
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sqldatabase", "azure_rm_sqlserver", null, null),
            new Tweak_Response_AddField("azure_rm_sqldatabase", "database_id"),
            new Tweak_Module_AssertStateVariable("azure_rm_sqldatabase", "status"),
            new Tweak_Module_AssertStateExpectedValue("azure_rm_sqldatabase", "Online"),

            new Tweak_Module_NeedsForceUpdate("azure_rm_sqldatabase"),

            // SQL Database Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sqldatabase_facts", "azure_rm_sqldatabase", null, null),

            // SQL Elastic Pool
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sqlelasticpool", "azure_rm_sqlserver", null, null),

            // SQL Elastic Pool Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sqlelasticpool_facts", "azure_rm_sqlelasticpool", null, null),

            // SQL Server Firewall Rule
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sqlfirewallrule", "azure_rm_sqlserver", null, null),
            //new Tweak_Module_FlattenParametersDictionary("azure_rm_sqlfirewallrule"),

            // SQL Firewall Rule Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sqlfirewallrule_facts", "azure_rm_sqlfirewallrule", null, null),

            // SQL Geo Backup Policy
            new Tweak_Module_TestPrerequisitesModule("azure_rm_sqlgeobackuppolicy", "azure_rm_sqldatabase", null, null),

            new Tweak_Module_TestPrerequisitesModule("azure_rm_sqlgeobackuppolicy_facts", "azure_rm_sqlgeobackuppolicy", null, null),


            // MySQL Server
            new Tweak_Module_AssertStateVariable("azure_rm_mysqlserver", "state"),
            new Tweak_Module_AssertStateExpectedValue("azure_rm_mysqlserver", "Ready"),

            // MySQL Server Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqlserver_facts", "azure_rm_mysqlserver", null, null),

            // MySQL Database
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqldatabase", "azure_rm_mysqlserver", null, null),
            new Tweak_Response_AddField("azure_rm_mysqldatabase", "name"),
            new Tweak_Module_AssertStateVariable("azure_rm_mysqldatabase", "name"),
            new Tweak_Module_AssertStateExpectedValue("azure_rm_mysqldatabase", "testdatabase"),
            new Tweak_Module_NeedsDeleteBeforeUpdate("azure_rm_mysqldatabase"),
            new Tweak_Module_NeedsForceUpdate("azure_rm_mysqldatabase"),

            // MySQL Database Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqldatabase_facts", "azure_rm_mysqldatabase", null, null),

            // MySQL Server Firewall Rule
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqlfirewallrule", "azure_rm_mysqlserver", null, null),
            //new Tweak_Module_FlattenParametersDictionary("azure_rm_mysqlfirewallrule"),
            //new Tweak_Module_AssertStateVariable("azure_rm_mysqlfirewallrule", "firewall_rule_name"),
            //new Tweak_Module_AssertStateExpectedValue("azure_rm_mysqlfirewallrule", "firewallrule{{ rpfx }}"),

            // MySQL Firewall Rule Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqlfirewallrule_facts", "azure_rm_mysqlfirewallrule", null, null),

            // MySQL Server Configuration
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqlconfiguration", "azure_rm_mysqlserver", null, null),
            //new Tweak_Module_FlattenParametersDictionary("azure_rm_mysqlconfiguration"),
            //new Tweak_Module_AssertStateVariable("azure_rm_mysqlconfiguration", "value"),
            //new Tweak_Module_AssertStateExpectedValue("azure_rm_mysqlconfiguration", "ON"),

            // MySQL Configuration Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqlconfiguration_facts", "azure_rm_mysqlconfiguration", null, null),

            // MySQL Server Virtual Network Rule
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqlvirtualnetworkrule", "azure_rm_mysqlserver", null, null),
            //new Tweak_Module_FlattenParametersDictionary("azure_rm_mysqlvirtualnetworkrule"),
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

            // MySQL Server LogFile Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_mysqllogfile_facts", "azure_rm_mysqlserver", null, null),

            // PostgreSQL Server
            new Tweak_Module_AssertStateVariable("azure_rm_postgresqlserver", "state"),
            new Tweak_Module_AssertStateExpectedValue("azure_rm_postgresqlserver", "Ready"),

            // PostgreSQL Server Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqlserver_facts", "azure_rm_postgresqlserver", null, null),

            // PostgreSQL Database
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqldatabase", "azure_rm_postgresqlserver", null, null),
            new Tweak_Response_AddField("azure_rm_postgresqldatabase", "name"),
            new Tweak_Module_AssertStateVariable("azure_rm_postgresqldatabase", "name"),
            new Tweak_Module_AssertStateExpectedValue("azure_rm_postgresqldatabase", "testdatabase"),
            new Tweak_Module_NeedsDeleteBeforeUpdate("azure_rm_postgresqldatabase"),
            new Tweak_Module_NeedsForceUpdate("azure_rm_postgresqldatabase"),

            // PostgreSQL Database Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqldatabase_facts", "azure_rm_postgresqldatabase", null, null),

            // PostgreSQL Server Firewall Rule
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqlfirewallrule", "azure_rm_postgresqlserver", null, null),
            //new Tweak_Module_FlattenParametersDictionary("azure_rm_postgresqlfirewallrule"),

            // PostgreSQL Firewall Rule Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqlfirewallrule_facts", "azure_rm_postgresqlfirewallrule", null, null),

            // PostgreSQL Server Configuration
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqlconfiguration", "azure_rm_postgresqlserver", null, null),
            //new Tweak_Module_FlattenParametersDictionary("azure_rm_postgresqlconfiguration"),

            // PostgreSQL Configuration Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqlconfiguration_facts", "azure_rm_postgresqlconfiguration", null, null),

            // PostgreSQL Server Virtual Network Rule
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqlvirtualnetworkrule", "azure_rm_postgresqlserver", null, null),

            // PostgreSQL Virtual Network Rule Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_postgresqlvirtualnetworkrule_facts", "azure_rm_postgresqlvirtualnetworkrule", null, null),

            // Authorization
            //new Tweak_Module_FlattenParametersDictionary("azure_rm_authorizationroleassignment"),

            // Application Gateway
            new Tweak_Module_CannotTestUpdate("azure_rm_applicationgateway"),
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



            // Application Gateway Application Security Group Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_applicationgatewayapplicationsecuritygroup_facts", "azure_rm_applicationgatewayapplicationsecuritygroup", null, null),


            // Application Gateway Route Table Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_applicationgatewayroutetable_facts", "azure_rm_applicationgatewayroutetable", null, null),

            // Application Gateway Route
            new Tweak_Module_TestPrerequisitesModule("azure_rm_applicationgatewayroute", "azure_rm_applicationgatewayroutetable", null, null),
            new Tweak_Module_CannotTestUpdate("azure_rm_applicationgatewayroute"),

            // Application Gateway Route Facts
            new Tweak_Module_TestPrerequisitesModule("azure_rm_applicationgatewayroute_facts", "azure_rm_applicationgatewayroute", null, null),

            // Application Inbound NAT Rule

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
            //new Tweak_Module_FlattenParametersDictionary("azure_rm_containerregistryreplication"),

            new Tweak_Module_TestPrerequisitesModule("azure_rm_containerregistryreplication_facts", "azure_rm_containerregistryreplication", null, null),

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

            new Tweak_Module_TestPrerequisitesModule("azure_rm_containerregistrywebhook_facts", "azure_rm_containerregistrywebhook", null, null),
        };
    }
}
