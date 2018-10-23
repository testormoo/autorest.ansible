cd /azure-rest-api-specs/specification/batchai/resource-manager
autorest --output-folder=/ansible-hatchery-tmp/ --python-sdks-folder=/ansible-hatchery-tmp/ --use=/autorest.ansible --python --tag=package-2017-09-preview
cp -r /ansible-hatchery-tmp/azure-mgmt-batchai/* /ansible-hatchery-tmpx
