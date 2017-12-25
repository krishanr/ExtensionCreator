<?php
// No direct access to this file
defined('_JEXEC') or die('Restricted access');
 
/**
 * Script file for Listing Component
 */
class com_ListingInstallerScript
{
	/**
	 * method to install the component
	 * $parent is the class calling this method
	 * @return void
	 */
	function install($parent) 
	{
		//$parent->getParent()->setRedirectURL('index.php?option=com_listing');
	}
 
	/**
	 * method to uninstall the component
	 * $parent is the class calling this method
	 * @return void
	 */
	function uninstall($parent) 
	{
	}
 
	/**
	 * method to update the component
	 * $parent is the class calling this method
	 * @return void
	 */
	function update($parent) 
	{
	}
 
	/**
	 * method to run before an install/update/uninstall method
	 * $type is the type of change (install, update or discover_install)
	 * $parent is the class calling this method
	 * @return void
	 */
	function preflight($type, $parent) 
	{
	}
 
	/**
	 * method to run after an install/update/uninstall method
	 * $type is the type of change (install, update or discover_install)
	 * $parent is the class calling this method
	 * @return void
	 */
	function postflight($type, $parent) 
	{
	}
}