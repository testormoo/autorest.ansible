
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
echo "----------------------------- Generating FrontDoor"
rm -rf /ansible-hatchery-tmp/*
/autorest.ansible/scripts/generate-frontdoor.sh
echo "---"
cd /ansible-hatchery-tmp
find -name azure_rm*.py
