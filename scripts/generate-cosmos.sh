cd /azure-rest-api-specs/specification/cosmos-db/resource-manager
autorest --output-folder=/ansible-hatchery-tmp/ --python-sdks-folder=/ansible-hatchery-tmp/ --use=/autorest.ansible --python --tag=package-2015-04
cp -r /ansible-hatchery-tmp/azure-mgmt-cosmosdb/* /ansible-hatchery-tmpx
