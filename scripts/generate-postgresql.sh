cd /azure-rest-api-specs/specification/postgresql/resource-manager
autorest --output-folder=/ansible-hatchery-tmp/ --python-sdks-folder=/ansible-hatchery-tmp/ --use=/autorest.ansible --python --tag=package-2017-04-preview
cp -r /ansible-hatchery-tmp/azure-mgmt-postgresql/* /ansible-hatchery-tmpx/
