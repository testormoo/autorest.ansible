
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
