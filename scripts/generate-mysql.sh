cd /azure-rest-api-specs/specification/mysql/resource-manager
autorest --output-folder=/ansible-hatchery-tmp/ --python-sdks-folder=/ansible-hatchery-tmp/ --use=/autorest.ansible --python --tag=package-2017-04-preview
cp -r /ansible-hatchery-tmp/azure-mgmt-mysql/* /ansible-hatchery-tmpx/
