cd /azure-rest-api-specs/specification/network/resource-manager
autorest --output-folder=/ansible-hatchery-tmp/ --python-sdks-folder=/ansible-hatchery-tmp/ --use=/autorest.ansible --python --tag=package-2017-10 --namespace=network
cp -r /ansible-hatchery-tmp/azure-mgmt-network/azure/mgmt/network/v2017_10_01/* /ansible-hatchery-tmpx/
