Extension Creator
======

![Extension Creator](https://github.com/krishanr/ExtensionCreator/blob/master/ExtensionCreator.PNG "Extension Creator")

ExtensionCreator is a general purpose Windows application designed to help reduce redundancies in the development of software extensions. It wraps a template engine creating code files from code templates using parameters from an XML file. Strengths of the application include the following:
* Parameters are stored in XML files which can be reused for different extensions using XPATH queries.
* Code templates, which use the Visual Basic programming language, can call Visual Basic functions in addition to storing place-holders for parameters.
* The file structure of the generated code is put inside an XML file which can have place-holders for parameters.

This application was originally designed to create boiler plate code for Joomla components, modules and plugins, but was designed abstractly to be useful for similar problems. As a component creator for Joomla, this software has the following advantages:
* Creates a functioning Joomla component.
* Can create an editable administrator table, or form in the front end, from a MySql table.
* Existing code template files could be edited and new ones could be created based on custom requirements.

## Download
* [Version 0.5](https://github.com/krishanr/ExtensionCreator/releases/download/v0.5-alpha/ECInstaller.msi)

## Usage
After installing the application, the "Extension Creator" folder will be created in the "Documents" folder, and will contain the following files:

![EC Files](https://github.com/krishanr/ExtensionCreator/blob/master/EcFiles.PNG "Extension creator files")

The program will work with the above files by default, and put the generated files in the _Output_ folder. After a Joomla extension is run, an installable ZIP file will be created in the _Archives_ folder.

## License 
* see [LICENSE](https://github.com/krishanr/ExtensionCreator/blob/master/license.txt) file

## Version 
* Version 0.5

## Contact
#### Developer/Company
* Homepage: http://krishanr.nfshost.com
