cd /azure-rest-api-specs/specification/keyvault/resource-manager
autorest --output-folder=/ansible-hatchery-tmp/ --python-sdks-folder=/ansible-hatchery-tmp/ --use=/autorest.ansible --python --tag=package-2016-10
cp -r /azure-mgmt-keyvault/azure/mgmt/keyvault/v2016_10_01/* /ansible-hatchery-tmpx/
