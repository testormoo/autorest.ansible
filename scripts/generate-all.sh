
# remove all the old files from the hatchery
rm -rf /ansible-hatchery/library/*
rm -rf /ansible-hatchery/tests/integration/targets/*

# generate all the modules API by API
echo "Generating SQL"
/autorest.ansible/scripts/generate-sql.sh
echo "Generating MySQL"
/autorest.ansible/scripts/generate-mysql.sh
echo "Generating PostgreSQL"
/autorest.ansible/scripts/generate-postgresql.sh
/autorest.ansible/scripts/generate-authorization.sh
/autorest.ansible/scripts/generate-web.sh
/autorest.ansible/scripts/generate-network.sh
/autorest.ansible/scripts/generate-containerinstance.sh
/autorest.ansible/scripts/generate-containerregistry.sh
/autorest.ansible/scripts/generate-keyvault.sh
/autorest.ansible/scripts/generate-batch.sh
/autorest.ansible/scripts/generate-batchai.sh
