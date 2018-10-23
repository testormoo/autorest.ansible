cd /azure-rest-api-specs/specification/containerinstance/resource-manager
autorest --output-folder=/ansible-hatchery-tmp/ --python-sdks-folder=/ansible-hatchery-tmp/ --use=/autorest.ansible --python --tag=package-2017-10-preview
cp -r  /ansible-hatchery-tmp/azure-mgmt-containerinstance/* /ansible-hatchery-tmp && rm -R /ansible-hatchery-tmp/azure-mgmt-containerinstance/*