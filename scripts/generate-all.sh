
# remove all the old files from the hatchery
rm -rf /ansible-hatchery/library/*
rm -rf /ansible-hatchery/tests/integration/targets/*
rm -rf /ansible-hatchery/examples/*

# generate all the modules API by API
echo "----------------------------- Generating Network"
rm -rf /ansible-hatchery-tmp/*
/autorest.ansible/scripts/generate-network.sh
echo "---"
cd /ansible-hatchery-tmp
find -name azure_rm*.py

/autorest.ansible/scripts/generate-generic.sh sql package-composite-v3 azure-mgmt-sql
/autorest.ansible/scripts/generate-generic.sh mysql package-2017-04-preview azure-mgmt-rdbms
/autorest.ansible/scripts/generate-generic.sh postgresql package-2017-04-preview azure-mgmt-rdbms
/autorest.ansible/scripts/generate-generic.sh authorization package-2015-07 .
/autorest.ansible/scripts/generate-generic.sh web package-2016-09 azure-mgmt-web
# namespace issue
#/autorest.ansible/scripts/generate-generic.sh network xxx .
/autorest.ansible/scripts/generate-generic.sh containerinstance package-2017-10-preview azure-mgmt-containerinstance
/autorest.ansible/scripts/generate-generic.sh containerregistry package-2017-10 azure-mgmt-containerregistry/azure/mgmt/containerregistry/v2017_10_01
/autorest.ansible/scripts/generate-generic.sh keyvault package-2016-10 azure-mgmt-keyvault/azure/mgmt/keyvault/v2016_10_01
/autorest.ansible/scripts/generate-generic.sh batch package-2017-09 azure-mgmt-batch
/autorest.ansible/scripts/generate-generic.sh batchai package-2017-09-preview azure-mgmt-batchai
/autorest.ansible/scripts/generate-generic.sh cosmos-db package-2015-04 azure-mgmt-cosmosdb
/autorest.ansible/scripts/generate-generic.sh compute package-2018-10-01 .
/autorest.ansible/scripts/generate-generic.sh hdinsight package-2015-03-preview azure-mgmt-hdinsight
/autorest.ansible/scripts/generate-generic.sh frontdoor package-2018-08-preview azure-mgmt-frontdoor
/autorest.ansible/scripts/generate-generic.sh machinelearning package-webservices-2017-01 .
/autorest.ansible/scripts/generate-generic.sh machinelearningcompute package-2017-08-preview azure-mgmt-machinelearningcompute
/autorest.ansible/scripts/generate-generic.sh machinelearningexperimentation package-2017-05-preview .
/autorest.ansible/scripts/generate-generic.sh machinelearningservices package-2018-03-preview azure-mgmt-machinelearningservices/all/modules
/autorest.ansible/scripts/generate-generic.sh mariadb package-2018-06-01-preview azure-mgmt-rdbms

/autorest.ansible/scripts/generate-generic.sh addons package-2018-03 azure-mgmt-addons
/autorest.ansible/scripts/generate-generic.sh adhybridhealthservice package-2014-01 azure-mgmt-adhybridhealthservice
/autorest.ansible/scripts/generate-generic.sh advisor package-2017-04 azure-mgmt-advisor
/autorest.ansible/scripts/generate-generic.sh alertsmanagement package-2018-05 .
/autorest.ansible/scripts/generate-generic.sh apimanagement package-2018-01 azure-mgmt-apimanagement
/autorest.ansible/scripts/generate-generic.sh applicationinsights package-2015-05 azure-mgmt-applicationinsights
/autorest.ansible/scripts/generate-generic.sh automation package-2018-06-preview azure-mgmt-automation
/autorest.ansible/scripts/generate-generic.sh azure-kusto package-2018-09-07-preview azure-mgmt-kusto
/autorest.ansible/scripts/generate-generic.sh azurestack package-2017-06-01 .
/autorest.ansible/scripts/generate-generic.sh billing package-2018-03-preview azure-mgmt-billing
/autorest.ansible/scripts/generate-generic.sh blueprint package-2017-11-preview azure-mgmt-blueprint
/autorest.ansible/scripts/generate-generic.sh botservice package-2018-07-12 azure-mgmt-botservice
/autorest.ansible/scripts/generate-generic.sh cdn package-2017-10 azure-mgmt-cdn
/autorest.ansible/scripts/generate-generic.sh cognitiveservices package-2017-04 azure-mgmt-cognitiveservices
/autorest.ansible/scripts/generate-generic.sh commerce package-2015-06-preview azure-mgmt-commerce
/autorest.ansible/scripts/generate-generic.sh consumption package-2018-10 azure-mgmt-consumption
/autorest.ansible/scripts/generate-generic.sh containerservices package-2018-08-preview .
/autorest.ansible/scripts/generate-generic.sh cost-management package-2018-05 azure-mgmt-costmanagement
/autorest.ansible/scripts/generate-generic.sh customer-insights package-2017-04 .
/autorest.ansible/scripts/generate-generic.sh databox package-2018-01 azure-mgmt-databox
/autorest.ansible/scripts/generate-generic.sh databricks package-2018-04-01 .
/autorest.ansible/scripts/generate-generic.sh datacatalog package-2016-03-30 .
/autorest.ansible/scripts/generate-generic.sh datafactory package-2018-06 azure-mgmt-datafactory
/autorest.ansible/scripts/generate-generic.sh datalake-analytics package-2016-11 azure-mgmt-datalake-analytics/azure/mgmt/datalake/analytics/account
/autorest.ansible/scripts/generate-generic.sh datalake-store package-2016-11 azure-mgmt-datalake-store
/autorest.ansible/scripts/generate-generic.sh datamigration package-2018-07-15-preview azure-mgmt-datamigration
/autorest.ansible/scripts/generate-generic.sh deploymentmanager package-2018-09-01-preview azure-mgmt-deploymentmanager
/autorest.ansible/scripts/generate-generic.sh deviceprovisioningservices package-2018-01 azure-mgmt-iothubprovisioningservices
/autorest.ansible/scripts/generate-generic.sh devspaces package-2018-06-01-preview azure-mgmt-devspaces
/autorest.ansible/scripts/generate-generic.sh devtestlabs package-2016-05 azure-mgmt-devtestlabs
/autorest.ansible/scripts/generate-generic.sh dns package-2018-05 azure-mgmt-dns/azure/mgmt/dns/v2018_05_01
/autorest.ansible/scripts/generate-generic.sh domainservices package-2017-06 .
/autorest.ansible/scripts/generate-generic.sh eventgrid package-2018-09-preview azure-mgmt-eventgrid
/autorest.ansible/scripts/generate-generic.sh eventhub package-2017-04 azure-mgmt-eventhub
#/autorest.ansible/scripts/generate-generic.sh graphrbac yyy .
/autorest.ansible/scripts/generate-generic.sh guestconfiguration package-2018-06-30-preview .
/autorest.ansible/scripts/generate-generic.sh hanaonazure package-2017-11 azure-mgmt-hanaonazure
/autorest.ansible/scripts/generate-generic.sh hardwaresecuritymodules package-2018-10 .
/autorest.ansible/scripts/generate-generic.sh intune package-2015-01-preview .
/autorest.ansible/scripts/generate-generic.sh iotcentral package-2018-09-01 azure-mgmt-iotcentral
/autorest.ansible/scripts/generate-generic.sh iothub package-2018-04 azure-mgmt-iothub
/autorest.ansible/scripts/generate-generic.sh iotespaces package-2017-10-preview .
/autorest.ansible/scripts/generate-generic.sh logic package-2018-07-preview azure-mgmt-logic
/autorest.ansible/scripts/generate-generic.sh managementgroups package-2018-03 azure-mgmt-managementgroups
/autorest.ansible/scripts/generate-generic.sh managementpartner package-2018-02 azure-mgmt-managementpartner
/autorest.ansible/scripts/generate-generic.sh maps package-2018-05 azure-mgmt-maps
/autorest.ansible/scripts/generate-generic.sh marketpaceordering package-2015-06-01 .
/autorest.ansible/scripts/generate-generic.sh mediaservices package-2018-07 azure-mgmt-media
/autorest.ansible/scripts/generate-generic.sh migrate package-2018-02 .
/autorest.ansible/scripts/generate-generic.sh monitor package-2018-09 azure-mgmt-monitor
/autorest.ansible/scripts/generate-generic.sh msi package-2015-08-31-preview azure-mgmt-msi
/autorest.ansible/scripts/generate-generic.sh netapp package-2017-08-15 azure-mgmt-netapp
/autorest.ansible/scripts/generate-generic.sh notificationhubs package-2017-04 azure-mgmt-notificationhubs
/autorest.ansible/scripts/generate-generic.sh operationalinsights package-2015-11-preview azure-mgmt-loganalytics
/autorest.ansible/scripts/generate-generic.sh operationsmanagement package-2015-11-preview .
/autorest.ansible/scripts/generate-generic.sh policyinsights package-2018-07 .
/autorest.ansible/scripts/generate-generic.sh powerbidedicated package-2017-10-01 .
/autorest.ansible/scripts/generate-generic.sh powerbiembedded package-2016-01 azure-mgmt-powerbiembedded
/autorest.ansible/scripts/generate-generic.sh recoveryservices package-2016-06 azure-mgmt-recoveryservices
/autorest.ansible/scripts/generate-generic.sh recoveryservicesbackup package-2017-07 azure-mgmt-recoveryservicesbackup
/autorest.ansible/scripts/generate-generic.sh recoveryservuicessiterecovery package-2018-01 .
/autorest.ansible/scripts/generate-generic.sh redis package-2018-03 azure-mgmt-redis
/autorest.ansible/scripts/generate-generic.sh relay package-2017-04 azure-mgmt-relay
/autorest.ansible/scripts/generate-generic.sh reservations package-2018-06 azure-mgmt-reservations
/autorest.ansible/scripts/generate-generic.sh resourcegraph package-2018-09-preview .
/autorest.ansible/scripts/generate-generic.sh resourcehealth package-2017-07 .
/autorest.ansible/scripts/generate-generic.sh resources package-features-2015-12 azure-mgmt-resource/azure/mgmt/resource/features/v2015_12_01
/autorest.ansible/scripts/generate-generic.sh scheduler package-2016-03 azure-mgmt-scheduler
/autorest.ansible/scripts/generate-generic.sh search package-2015-08 azure-mgmt-search
/autorest.ansible/scripts/generate-generic.sh security package-composite-v1 azure-mgmt-security
/autorest.ansible/scripts/generate-generic.sh serialconsole package-2018-05 .
/autorest.ansible/scripts/generate-generic.sh servicebus package-2017-04 azure-mgmt-servicebus
/autorest.ansible/scripts/generate-generic.sh servicefabric package-2018-02 .
/autorest.ansible/scripts/generate-generic.sh servicefabricmesh package-2018-09-01-preview azure-mgmt-servicefabricmesh
/autorest.ansible/scripts/generate-generic.sh service-map package-2015-11-preview .
/autorest.ansible/scripts/generate-generic.sh signalr package-2018-03-01-preview azure-mgmt-signalr
/autorest.ansible/scripts/generate-generic.sh storage package-2018-07 azure-mgmt-storage/azure/mgmt/storage/v2018_07_01
/autorest.ansible/scripts/generate-generic.sh storageimportexport package-2016-11 .
/autorest.ansible/scripts/generate-generic.sh storagesync package-2018-07-01 azure-mgmt-storagesync
/autorest.ansible/scripts/generate-generic.sh storSimple1200Series package-2016-10 .
/autorest.ansible/scripts/generate-generic.sh storsimple8000Series package-2017-06 .
/autorest.ansible/scripts/generate-generic.sh streamanalytics package-2016-03 .
/autorest.ansible/scripts/generate-generic.sh subscription package-2018-03-preview azure-mgmt-subscription
/autorest.ansible/scripts/generate-generic.sh timeseriesinsights package-2017-11-15 .
/autorest.ansible/scripts/generate-generic.sh trafficmanager package-2018-04 azure-mgmt-trafficmanager
/autorest.ansible/scripts/generate-generic.sh visualstudio package-2014-04-preview .
/autorest.ansible/scripts/generate-generic.sh windowsiot package-2018-02 .
/autorest.ansible/scripts/generate-generic.sh workloadmonitor package-2018-08-31-preview azure-mgmt-workloadmonitor
