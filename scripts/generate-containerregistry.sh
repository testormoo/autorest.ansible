cd /azure-rest-api-specs/specification/containerregistry/resource-manager
autorest --output-folder=/ansible-hatchery-tmp/ --python-sdks-folder=/ansible-hatchery-tmp/ --use=/autorest.ansible --python --tag=package-2017-10
cp -r /azure-mgmt-containerregistry/azure/mgmt/containerregistry/v2017_10_01/* /ansible-hatchery-tmpx/
