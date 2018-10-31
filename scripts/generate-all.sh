
# remove all the old files from the hatchery
rm -rf /ansible-hatchery/library/*
rm -rf /ansible-hatchery/tests/integration/targets/*

# generate all the modules API by API
echo "----------------------------- Generating SQL"
rm -rf /ansible-hatchery-tmp/*
/autorest.ansible/scripts/generate-sql.sh
echo "---"
cd /ansible-hatchery-tmp
find -name azure_rm*.py
echo "----------------------------- Generating MySQL"
rm -rf /ansible-hatchery-tmp/*
/autorest.ansible/scripts/generate-mysql.sh
echo "---"
cd /ansible-hatchery-tmp
find -name azure_rm*.py
echo "----------------------------- Generating PostgreSQL"
rm -rf /ansible-hatchery-tmp/*
/autorest.ansible/scripts/generate-postgresql.sh
echo "---"
cd /ansible-hatchery-tmp
find -name azure_rm*.py
echo "----------------------------- Generating Authorization"
rm -rf /ansible-hatchery-tmp/*
/autorest.ansible/scripts/generate-authorization.sh
echo "---"
cd /ansible-hatchery-tmp
find -name azure_rm*.py
echo "----------------------------- Generating Web"
rm -rf /ansible-hatchery-tmp/*
/autorest.ansible/scripts/generate-web.sh
echo "---"
cd /ansible-hatchery-tmp
find -name azure_rm*.py
echo "----------------------------- Generating Network"
rm -rf /ansible-hatchery-tmp/*
/autorest.ansible/scripts/generate-network.sh
echo "---"
cd /ansible-hatchery-tmp
find -name azure_rm*.py
echo "----------------------------- Generating Container Instance"
rm -rf /ansible-hatchery-tmp/*
/autorest.ansible/scripts/generate-containerinstance.sh
echo "---"
cd /ansible-hatchery-tmp
find -name azure_rm*.py
echo "----------------------------- Generating Container Registry"
rm -rf /ansible-hatchery-tmp/*
/autorest.ansible/scripts/generate-containerregistry.sh
echo "---"
cd /ansible-hatchery-tmp
find -name azure_rm*.py
echo "----------------------------- Generating KeyVault"
rm -rf /ansible-hatchery-tmp/*
/autorest.ansible/scripts/generate-keyvault.sh
echo "---"
cd /ansible-hatchery-tmp
find -name azure_rm*.py
echo "----------------------------- Generating Batch"
rm -rf /ansible-hatchery-tmp/*
/autorest.ansible/scripts/generate-batch.sh
echo "---"
cd /ansible-hatchery-tmp
find -name azure_rm*.py
echo "----------------------------- Generating Batch AI"
rm -rf /ansible-hatchery-tmp/*
/autorest.ansible/scripts/generate-batchai.sh
echo "---"
cd /ansible-hatchery-tmp
find -name azure_rm*.py
echo "----------------------------- Generating Cosmos"
rm -rf /ansible-hatchery-tmp/*
/autorest.ansible/scripts/generate-cosmos.sh
echo "---"
cd /ansible-hatchery-tmp
find -name azure_rm*.py
echo "----------------------------- Generating Compute"
rm -rf /ansible-hatchery-tmp/*
/autorest.ansible/scripts/generate-compute.sh
echo "---"
cd /ansible-hatchery-tmp
find -name azure_rm*.py
echo "----------------------------- Generating HDInsight"
rm -rf /ansible-hatchery-tmp/*
/autorest.ansible/scripts/generate-hdinsight.sh
echo "---"
cd /ansible-hatchery-tmp
find -name azure_rm*.py
echo "----------------------------- Generating FrontDoor"
rm -rf /ansible-hatchery-tmp/*
/autorest.ansible/scripts/generate-frontdoor.sh
echo "---"
cd /ansible-hatchery-tmp
find -name azure_rm*.py
echo "----------------------------- Generating Machine Learning"
rm -rf /ansible-hatchery-tmp/*
/autorest.ansible/scripts/generate-machinelearning.sh
echo "---"
cd /ansible-hatchery-tmp
find -name azure_rm*.py
echo "----------------------------- Generating Machine Learning Compute"
rm -rf /ansible-hatchery-tmp/*
/autorest.ansible/scripts/generate-machinelearningcompute.sh
echo "---"
cd /ansible-hatchery-tmp
find -name azure_rm*.py
echo "----------------------------- Generating Machine Learning Experimentation"
rm -rf /ansible-hatchery-tmp/*
/autorest.ansible/scripts/generate-machinelearningexperimentation.sh
echo "---"
cd /ansible-hatchery-tmp
find -name azure_rm*.py
echo "----------------------------- Generating Machine Learning Services"
rm -rf /ansible-hatchery-tmp/*
/autorest.ansible/scripts/generate-machinelearningservices.sh
echo "---"
cd /ansible-hatchery-tmp
find -name azure_rm*.py
echo "----------------------------- Generating MariaDB"
rm -rf /ansible-hatchery-tmp/*
/autorest.ansible/scripts/generate-mariadb.sh
echo "---"
cd /ansible-hatchery-tmp
find -name azure_rm*.py

/autorest.ansible/scripts/generate-generic.sh addons package-2018-03 azure-mgmt-addons
/autorest.ansible/scripts/generate-generic.sh adhybridhealthservice package-2014-01 azure-mgmt-adhybridhealthservice
/autorest.ansible/scripts/generate-generic.sh advisor package-2017-04 azure-mgmt-advisor
/autorest.ansible/scripts/generate-generic.sh alertsmanagement package-2018-05 .
/autorest.ansible/scripts/generate-generic.sh apimanagement package-2018-01 azure-mgmt-apimanagement
/autorest.ansible/scripts/generate-generic.sh applicationinsights package-2015-05 azure-mgmt-applicationinsights
/autorest.ansible/scripts/generate-generic.sh automation package-2018-06-preview azure-mgmt-automation
/autorest.ansible/scripts/generate-generic.sh azure-kusto package-2018-09-07-preview azure-mgmt-kusto
/autorest.ansible/scripts/generate-generic.sh azurestack package-2017-06-01 .
/autorest.ansible/scripts/generate-generic.sh billing package-2018-03-preview .
/autorest.ansible/scripts/generate-generic.sh blueprint package-2017-11-preview .
/autorest.ansible/scripts/generate-generic.sh botservice package-2018-07-12 .
/autorest.ansible/scripts/generate-generic.sh cdn package-2017-10 .
/autorest.ansible/scripts/generate-generic.sh cognitiveservices package-2017-04 .
/autorest.ansible/scripts/generate-generic.sh commerce package-2015-06-preview .
/autorest.ansible/scripts/generate-generic.sh consumption package-2018-10 .
/autorest.ansible/scripts/generate-generic.sh containerservices package-2018-08-preview .
/autorest.ansible/scripts/generate-generic.sh cost-management package-2018-05 .
/autorest.ansible/scripts/generate-generic.sh customer-insights package-2017-04 .
/autorest.ansible/scripts/generate-generic.sh databox package-2018-01 .
/autorest.ansible/scripts/generate-generic.sh databricks package-2018-04-01 .
/autorest.ansible/scripts/generate-generic.sh datacatalog package-2016-03-30 .
/autorest.ansible/scripts/generate-generic.sh datafactory package-2018-06 .
/autorest.ansible/scripts/generate-generic.sh datalake-analytics package-2016-11 .
/autorest.ansible/scripts/generate-generic.sh datalake-store package-2016-11 .
/autorest.ansible/scripts/generate-generic.sh datamigration package-2018-07-15-preview .
/autorest.ansible/scripts/generate-generic.sh deploymentmanager package-2018-09-01-preview .
/autorest.ansible/scripts/generate-generic.sh deviceprovisioningservices package-2018-01 .
/autorest.ansible/scripts/generate-generic.sh devspaces package-2018-06-01-preview .
/autorest.ansible/scripts/generate-generic.sh devtestlabs package-2016-05 .
/autorest.ansible/scripts/generate-generic.sh dns package-2018-05 .
/autorest.ansible/scripts/generate-generic.sh domainservices package-2017-06 .
/autorest.ansible/scripts/generate-generic.sh eventgrid package-2018-09-preview .
/autorest.ansible/scripts/generate-generic.sh eventhub package-2017-04 .
#/autorest.ansible/scripts/generate-generic.sh graphrbac yyy .
/autorest.ansible/scripts/generate-generic.sh guestconfiguration package-2018-06-30-preview .
/autorest.ansible/scripts/generate-generic.sh hanaonazure package-2017-11 .
/autorest.ansible/scripts/generate-generic.sh hardwaresecuritymodules package-2018-10 .
/autorest.ansible/scripts/generate-generic.sh intune package-2015-01-preview .
/autorest.ansible/scripts/generate-generic.sh iotcentral package-2018-09-01 .
/autorest.ansible/scripts/generate-generic.sh iothub package-2018-04 .
/autorest.ansible/scripts/generate-generic.sh iotespaces package-2017-10-preview .
/autorest.ansible/scripts/generate-generic.sh logic package-2018-07-preview .
/autorest.ansible/scripts/generate-generic.sh managementgroups package-2018-03 .
/autorest.ansible/scripts/generate-generic.sh managementpartner package-2018-02 .
/autorest.ansible/scripts/generate-generic.sh maps package-2018-05 .
/autorest.ansible/scripts/generate-generic.sh marketpaceordering package-2015-06-01 .
/autorest.ansible/scripts/generate-generic.sh mediaservices package-2018-07 .
/autorest.ansible/scripts/generate-generic.sh migrate package-2018-02 .
/autorest.ansible/scripts/generate-generic.sh monitor package-2018-09 .
/autorest.ansible/scripts/generate-generic.sh msi package-2015-08-31-preview .
/autorest.ansible/scripts/generate-generic.sh netapp package-2017-08-15 .
/autorest.ansible/scripts/generate-generic.sh notificationhubs package-2017-04 .
/autorest.ansible/scripts/generate-generic.sh operationalinsights package-2015-11-preview .
/autorest.ansible/scripts/generate-generic.sh operationsmanagement package-2015-11-preview .
/autorest.ansible/scripts/generate-generic.sh policyinsights package-2018-07 .
/autorest.ansible/scripts/generate-generic.sh powerbidedicated package-2017-10-01 .
/autorest.ansible/scripts/generate-generic.sh powerbiembedded package-2016-01 .
/autorest.ansible/scripts/generate-generic.sh recoveryservices package-2016-06 .
/autorest.ansible/scripts/generate-generic.sh recoveryservicesbackup package-2017-07 .
/autorest.ansible/scripts/generate-generic.sh recoveryservuicessiterecovery package-2018-01 .
/autorest.ansible/scripts/generate-generic.sh redis package-2018-03 .
/autorest.ansible/scripts/generate-generic.sh relay package-2017-04 .
/autorest.ansible/scripts/generate-generic.sh reservations package-2018-06 .
/autorest.ansible/scripts/generate-generic.sh resourcegraph package-2018-09-preview .
/autorest.ansible/scripts/generate-generic.sh resourcehealth package-2017-07 .
/autorest.ansible/scripts/generate-generic.sh resources package-features-2015-12 .
/autorest.ansible/scripts/generate-generic.sh scheduler package-2016-03 .
/autorest.ansible/scripts/generate-generic.sh search package-2015-08 .
/autorest.ansible/scripts/generate-generic.sh security package-composite-v1 .
/autorest.ansible/scripts/generate-generic.sh serialconsole package-2018-05 .
/autorest.ansible/scripts/generate-generic.sh servicebus package-2017-04 .
/autorest.ansible/scripts/generate-generic.sh servicefabric package-2018-02 .
/autorest.ansible/scripts/generate-generic.sh servicefabricmesh package-2018-09-01-preview .
/autorest.ansible/scripts/generate-generic.sh service-map package-2015-11-preview .
/autorest.ansible/scripts/generate-generic.sh signalr package-2018-03-01-preview .
/autorest.ansible/scripts/generate-generic.sh storage package-2018-07 .
/autorest.ansible/scripts/generate-generic.sh storageimportexport package-2016-11 .
/autorest.ansible/scripts/generate-generic.sh storagesync package-2018-07-01 .
/autorest.ansible/scripts/generate-generic.sh storSimple1200Series package-2016-10 .
/autorest.ansible/scripts/generate-generic.sh storsimple8000Series package-2017-06 .
/autorest.ansible/scripts/generate-generic.sh streamanalytics package-2016-03 .
/autorest.ansible/scripts/generate-generic.sh subscription package-2018-03-preview .
/autorest.ansible/scripts/generate-generic.sh timeseriesinsights package-2017-11-15 .
/autorest.ansible/scripts/generate-generic.sh trafficmanager package-2018-04 .
/autorest.ansible/scripts/generate-generic.sh visualstudio package-2014-04-preview .
/autorest.ansible/scripts/generate-generic.sh windowsiot package-2018-02 .
/autorest.ansible/scripts/generate-generic.sh workloadmonitor package-2018-08-31-preview .
