cd /azure-rest-api-specs/specification/batch/resource-manager
autorest --output-folder=/ansible-hatchery-tmp/ --python-sdks-folder=/ansible-hatchery-tmp/ --use=/autorest.ansible --python --tag=package-2017-09
cp -r  /ansible-hatchery-tmp/azure-mgmt-batch/* /ansible-hatchery-tmp && rm -R /ansible-hatchery-tmp/azure-mgmt-batch/*