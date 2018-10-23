cd /azure-rest-api-specs/specification/authorization/resource-manager
autorest --output-folder=/ansible-hatchery-tmp/ --python-sdks-folder=/ansible-hatchery-tmp/ --use=/autorest.ansible --python --tag=package-2015-07
cp -r /ansible-hatchery-tmp/* /ansible-hatchery-tmpx/
