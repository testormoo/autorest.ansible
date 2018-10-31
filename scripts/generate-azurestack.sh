cd /azure-rest-api-specs/specification/azurestack/resource-manager
autorest --output-folder=/ansible-hatchery-tmp/ --python-sdks-folder=/ansible-hatchery-tmp/ --use=/autorest.ansible --python --tag=package-2017-06-01
cp -r /ansible-hatchery-tmp/* /ansible-hatchery-tmpx/
