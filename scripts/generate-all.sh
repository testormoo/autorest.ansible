
# remove all the old files from the hatchery
rm -rf /ansible-hatchery/library/*
rm -rf /ansible-hatchery/tests/integration/targets/*

# generate all the modules API by API
/ansible-hatchery/generate-sql.sh
/ansible-hatchery/generate-mysql.sh
/ansible-hatchery/generate-postgresql.sh
/ansible-hatchery/generate-authorization.sh
/ansible-hatchery/generate-web.sh
/ansible-hatchery/generate-network.sh
/ansible-hatchery/generate-containerinstance.sh
/ansible-hatchery/generate-containerregistry.sh
/ansible-hatchery/generate-keyvault.sh
/ansible-hatchery/generate-batch.sh
/ansible-hatchery/generate-batchai.sh
