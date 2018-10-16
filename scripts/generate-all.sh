
# remove all the old files from the hatchery
rm -rf /ansible-hatchery/library/*
rm -rf /ansible-hatchery/tests/integration/targets/*

# generate all the modules API by API
echo "----------------------------- Generating SQL"
/autorest.ansible/scripts/generate-sql.sh
echo "----------------------------- Generating MySQL"
/autorest.ansible/scripts/generate-mysql.sh
echo "----------------------------- Generating PostgreSQL"
/autorest.ansible/scripts/generate-postgresql.sh
echo "----------------------------- Generating Authorization"
/autorest.ansible/scripts/generate-authorization.sh
echo "----------------------------- Generating Web"
/autorest.ansible/scripts/generate-web.sh
echo "----------------------------- Generating Network"
/autorest.ansible/scripts/generate-network.sh
echo "----------------------------- Generating Container Instance"
/autorest.ansible/scripts/generate-containerinstance.sh
echo "----------------------------- Generating Container Registry"
/autorest.ansible/scripts/generate-containerregistry.sh
echo "----------------------------- Generating KeyVault"
/autorest.ansible/scripts/generate-keyvault.sh
echo "----------------------------- Generating Batch"
/autorest.ansible/scripts/generate-batch.sh
echo "----------------------------- Generating Batch AI"
/autorest.ansible/scripts/generate-batchai.sh
echo "----------------------------- Generating Cosmos"
/autorest.ansible/scripts/generate-cosmos.sh
