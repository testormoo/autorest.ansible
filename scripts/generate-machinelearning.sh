cd /azure-rest-api-specs/specification/machinelearning/resource-manager
autorest --output-folder=/ansible-hatchery-tmp/ --python-sdks-folder=/ansible-hatchery-tmp/ --use=/autorest.ansible --python --tag=package-webservices-2017-01
cp -r /ansible-hatchery-tmp/azure-mgmt-rdbms/* /ansible-hatchery-tmpx/
