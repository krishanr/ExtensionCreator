<#@ template inherits="ETemplate" debug="False" language="VB" #>
<?php
/**
 * @version		<#= Value("Extension.version") #>
 * @package		Joomla.<#= Value("Extension.client") #>
 * @subpackage	mod_<#= Value("Extension.name") #>
 * @copyright	<#= Value("Extension.copyright") #>
 * @license		<#= Value("Extension.license") #>
 */

// no direct access
defined('_JEXEC') or die;

class mod<#= Replace(StrConv(Value("Extension.name"), VbStrConv.ProperCase), "_", "") #>
{
	/* Make it a static function so
	 * that you can access it without 
	 * initalizing the class.
	 */
	public static function SampleFunction()
	{
		//Access params by:
		//$params->get('KEYNAME', 'DEFAULT');
		//Access module stats (such as the module id) by:
		//echo $module->id;
	}
}
