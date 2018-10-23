cd /azure-rest-api-specs/specification/compute/resource-manager
autorest --output-folder=/ansible-hatchery-tmp --python-sdks-folder=/ansible-hatchery-tmp/ --use=/autorest.ansible --python --tag=package-2018-10-01
cp -r /ansible-hatchery-tmp/* /ansible-hatchery-tmpx
