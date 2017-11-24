
# Prerequisites

To use this extenstion you have to install autorest. Just follow instructions here:

https://github.com/Azure/autorest

You also need Azure REST API specification, clone it from here:

https://github.com/Azure/azure-rest-api-specs

and you need to clone this repo as well.


# How to use?

To generate ansibel modules, go to selected directory in REST API spec, for example:

     cd ...\azure-rest-api-specs\specification\sql\resource-manager\

and execute following command:

     autorest --output-folder=[your output directory]\ --use=[your source directory]\autorest.ansible\ --python --tag=package-2017-03-preview

Note that you have to specify location **autorest.ansible** repo, and the plugin should be already built here, either using **npm** or **Visual Studio**.

Also note that **--tag** value comes from **readme.txt** file you can find in your curent directory.