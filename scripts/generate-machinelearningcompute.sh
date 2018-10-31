cd /azure-rest-api-specs/specification/machinelearningcompute/resource-manager
autorest --output-folder=/ansible-hatchery-tmp/ --python-sdks-folder=/ansible-hatchery-tmp/ --use=/autorest.ansible --python --tag=package-2017-08-preview
cp -r /ansible-hatchery-tmp/azure-mgmt-machinelearningcompute/* /ansible-hatchery-tmpx/
